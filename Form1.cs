using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace AppResizer
{
    public partial class Form1 : Form
    {
        Process[] ProcList;
        public int lastSelectedWindowNode = -1;
        public int lastSelectedWindowProcId = -1;
        public int defaultDelayAutoResize = 0;
        public int backupWndBorder_Top = 0;
        public int backupWndBorder_Right = 0;
        public int backupWndBorder_Bottom = 0;
        public int backupWndBorder_Left = 0;


        public Bitmap selected_scanned_px_area = null;      // Selected fragment for better accuracy Border Setup
        public int selected_area_wndX = 0;
        public int selected_area_wndY = 0;
        public int scan_area_W = 30;
        public int scan_area_H = 30;


        Dictionary<IntPtr, ProcessAdditionalData> ProcAdditionalDataList = 
            new Dictionary<IntPtr, ProcessAdditionalData>(); // key == IntPrt (handle) of processes
        public class ProcessAdditionalData
        {
            public bool alreadyStarted = false;
            public int delayStartingResizeLeft = -999;
        }

        public class WndSizes {
            public int X = 0;
            public int Y = 0;
            public int Frame_W = 0; // Width and Height of window (together with frame)
            public int Frame_H = 0;
            public int Res_W = 0;   // (without frame)
            public int Res_H = 0;
            public int border_Top = 0;
            public int border_Bot = 0;
            public int border_Left = 0;
            public int border_Right = 0;
        }
        public class WndResolutions {
            public int Width = 0;
            public int Height = 0;
        }


        // SavedAppsData["path: 'путь-до-файла-если-есть', procName: 'имя-процесса', wndTitle: 'заголовок-окна'"] = { data... };
        Dictionary<string, AppsData> SavedAppsData = new Dictionary<string, AppsData>();
        public class AppsData
        {
            public int startingPosX = 0;
            public int startingPosY = 0;
            public int startingWidth = 0;
            public int startingHeight = 0;
            public int delayStartingResize = 0;
        }

        public static class LastAppData {
            public static int posX = 0;
            public static int posY = 0;
            public static int width = 0;
            public static int height = 0;
        }
        
        Thread Tread_Update;
        

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern long GetWindowRect(int hWnd, ref Rectangle lpRect);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public enum WndMode {
            Normal = 1,
            Minimized, // 2
            Maximized  // 3
        }

        public WndMode windowModeStatus(IntPtr WndHandle) {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(WndHandle, ref placement);
            switch (placement.showCmd)
            {
                case 1:
                    return WndMode.Normal;
                    break;
                case 2:
                    return WndMode.Minimized;
                    break;
                case 3:
                    return WndMode.Maximized;
                    break;
            }
            return WndMode.Normal;
        }

        public Form1() { InitializeComponent(); }
        private void Form1_Shown(object sender, EventArgs e)
        {
            notifyIcon.ContextMenuStrip = contextMenuStrip_Tray;

            ClearNotExistsProfiles();

            GetSettigns();
            
            UpdateProcessList();

            // Start Thread
            Tread_Update = new Thread(Tread_Update_func);
            Tread_Update.Start();
        }
        public void Tread_Update_func()
        {
            while (true)
            {
                Thread.Sleep(500);

                // Get the current process.
                Process CurrentProcess = GetForegroundProcess();
                
                if (String.IsNullOrEmpty(CurrentProcess.MainWindowTitle))
                    continue;
                if (CurrentProcess.HasExited)
                    continue;
                
                bool processProtected = false;
                string currProcInfo = GetProcInfo(CurrentProcess);

                // Additional data only if not previously exists
                if (!ProcAdditionalDataList.ContainsKey(CurrentProcess.MainWindowHandle))
                    ProcAdditionalDataList.Add(CurrentProcess.MainWindowHandle, new ProcessAdditionalData());
                    
                // Change sizes for the new windows, which saved before as Profiles in INI file
                if (!ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted 
                    && SavedAppsData.ContainsKey(currProcInfo))
                {
                    if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft == -999) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft =
                            SavedAppsData[currProcInfo].delayStartingResize - 500; // Init Delay to First auto resize
                    }
                    else if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft > 0) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft -= 500; // Decrease delay
                    }
                    // Do nothing, if window minimized
                    else if (windowModeStatus(CurrentProcess.MainWindowHandle) != WndMode.Minimized)
                    {
                        // If time to Starting resize come
                        SetWindowSize(CurrentProcess.MainWindowHandle,
                            SavedAppsData[currProcInfo].startingWidth,
                            SavedAppsData[currProcInfo].startingHeight,
                            SavedAppsData[currProcInfo].startingPosX,
                            SavedAppsData[currProcInfo].startingPosY);
                        
                        // Update sizes in programm also, if window currently selected
                        if (lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                            && !ProcList[lastSelectedWindowNode].HasExited
                            && GetProcInfo(ProcList[lastSelectedWindowNode]) == currProcInfo)
                        {
                            Invoke(new Action(() => {
                                label_PosX.Text = SavedAppsData[currProcInfo].startingPosX.ToString();
                                label_PosY.Text = SavedAppsData[currProcInfo].startingPosY.ToString();
                                label_SizeW.Text = SavedAppsData[currProcInfo].startingWidth.ToString();
                                label_SizeH.Text = SavedAppsData[currProcInfo].startingHeight.ToString();
                                numericUpDown_ResolutionW.Value = SavedAppsData[currProcInfo].startingWidth;
                                numericUpDown_ResolutionH.Value = SavedAppsData[currProcInfo].startingHeight;
                            }));
                        }

                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted = true;
                    }
                }

                // Update window size info, if currently selected
                if (this.WindowState != FormWindowState.Minimized 
                        && lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                        && !ProcList[lastSelectedWindowNode].HasExited
                        && GetProcInfo(ProcList[lastSelectedWindowNode]) == currProcInfo) {
                    WndSizes wndSizes = new WndSizes();
                    GetWndSizes(CurrentProcess.MainWindowHandle, ref wndSizes);
                    // Update visually only if pos/sizes changed.
                    if (LastAppData.posX != wndSizes.X || LastAppData.posY != wndSizes.Y
                        || LastAppData.width != wndSizes.Res_W || LastAppData.height != wndSizes.Res_H
                        )
                        Invoke(new Action(() => {
                            label_PosX.Text = wndSizes.X.ToString();
                            label_PosY.Text = wndSizes.Y.ToString();
                            label_SizeW.Text = wndSizes.Res_W.ToString();
                            label_SizeH.Text = wndSizes.Res_H.ToString();
                        }));
                }
            }
        }
        
        private void button_RefreshProcList_Click(object sender, EventArgs e)
        { UpdateProcessList(); }

        public void UpdateProcessList() {
            // Get the current process.
            ProcList = getProcessList();
            
            listBox_Windows.Items.Clear();
            for (int proc_i = 0; proc_i < ProcList.Length; proc_i++)
            {
                if (String.IsNullOrEmpty(ProcList[proc_i].MainWindowTitle))
                    continue;

                listBox_Windows.Items.Add(ProcList[proc_i].MainWindowTitle.Length > 33 ? ProcList[proc_i].MainWindowTitle.Substring(0, 30) + "..." 
                    : ProcList[proc_i].MainWindowTitle);

                // Additional data only if not previously exists
                if (!ProcAdditionalDataList.ContainsKey(ProcList[proc_i].MainWindowHandle))
                    ProcAdditionalDataList.Add(ProcList[proc_i].MainWindowHandle, new ProcessAdditionalData());

                if (lastSelectedWindowProcId == (int)ProcList[proc_i].MainWindowHandle)
                    listBox_Windows.SelectedIndex = proc_i;
            }
        }

        public Process[] getProcessList(){
            // Get all processes running on the local computer.
            Process[] localAll = Process.GetProcesses();
            Process[] localAll2 = Array.FindAll(localAll, proc => (!String.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName != "Taskmgr"));
            try {
                return localAll2.OrderByDescending(proc => proc.StartTime).ToArray();
            }
            catch (InvalidOperationException e) {
                return new Process[0];
            }
        }

        public Process GetForegroundProcess() {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return Process.GetProcessById((int)pid);
        }

        public void ClearNotExistsProfiles() {

            if (!File.Exists("applications.ini")) {
                File.WriteAllText("applications.ini", "");
                return;
            }

            string[] lines = File.ReadAllLines("applications.ini");

            List<string> resultLines_List = new List<string>();
            
            for (int i = 0; i < lines.Length; i++) {
                string path = GetParamFromLineINI(lines[i], "path");

                // Verify, if Profile's game file exists...
                if (path != null && (path == "" || File.Exists(path)))
                    resultLines_List.Add(lines[i]);
                // If file not exists - not add his data line.
            }

            if (lines.Length != resultLines_List.Count)
                File.WriteAllLines("applications.ini", resultLines_List.ToArray());
        }

        public void GetSettigns() {

            // Если файла не было или он не по шаблону...
            if (!File.Exists("settings.ini") || 
                !Regex.Match(File.ReadAllText("settings.ini"), @"iDelayStartingResize: \d+;\r\nbLaunchInTray: [^;]+;\r\niAutoP: \d+;").Success
                )
                File.WriteAllText("settings.ini", "iDelayStartingResize: 2;\nbLaunchInTray: false;\niAutoP: 1050;");

            // default params for entire programm
            if (File.Exists("settings.ini"))
            {
                string settings_text = File.ReadAllText("settings.ini");
                Match match = new Regex(@"iDelayStartingResize: (\d+);").Match(settings_text);
                if (match.Success) {
                    defaultDelayAutoResize  = int.Parse(match.Groups[1].Value);
                    numericUpDown_DelayStartResize.Value = defaultDelayAutoResize;
                }

                match = new Regex(@"bLaunchInTray: ([^;]+);").Match(settings_text);
                if (match.Success && match.Groups[1].Value == "true") {
                    this.WindowState = FormWindowState.Minimized;
                    checkBox_LaunchInTray.Checked = true;
                }
                match = new Regex(@"iAutoP: (\d+);").Match(settings_text);
                if (match.Success)
                    numericUpDown_AutoXXXX_p.Value = Convert.ToInt32(match.Groups[1].Value);
            }
            else
                MessageBox.Show("settings.ini not found", "Error");

            if (!File.Exists("applications.ini"))
                File.WriteAllText("applications.ini", "");

            // Applications params
            if (File.Exists("applications.ini"))
            {
                using (StreamReader file = new StreamReader("applications.ini"))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        string path = GetParamFromLineINI(line, "path");
                        string procName = GetParamFromLineINI(line, "procName");
                        string wndTitle = GetParamFromLineINI(line, "wndTitle");
                        string startingPosX = Regex.Match(GetParamFromLineINI(line, "pos"), @"(-?\d+)x").Groups[1].Value;
                        string startingPosY = Regex.Match(GetParamFromLineINI(line, "pos"), @"x(-?\d+)").Groups[1].Value;
                        string startingWidth = Regex.Match(GetParamFromLineINI(line, "resolution"), @"(-?\d+)x").Groups[1].Value;
                        string startingHeight = Regex.Match(GetParamFromLineINI(line, "resolution"), @"x(-?\d+)").Groups[1].Value;
                        string delayStartingResize = GetParamFromLineINI(line, "delayStartingResize");
                        if (path != null && procName != null && wndTitle != null) {
                            string procInfo = "path: '" + path + "', procName: '" + procName + "', wndTitle: '" + wndTitle + "'";
                            if (SavedAppsData.ContainsKey(procInfo))
                                SavedAppsData[procInfo] = new AppsData();
                            else
                                SavedAppsData.Add(procInfo, new AppsData());
                            SavedAppsData[procInfo].startingPosX            = int.Parse(startingPosX);
                            SavedAppsData[procInfo].startingPosY            = int.Parse(startingPosY);
                            SavedAppsData[procInfo].startingWidth           = int.Parse(startingWidth);
                            SavedAppsData[procInfo].startingHeight          = int.Parse(startingHeight);
                            SavedAppsData[procInfo].delayStartingResize     = int.Parse(delayStartingResize);
                        }
                    }
                    file.Close();
                }
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
        }

        public void SaveSettings() {
            string total_text = "";
            string[] lines = File.ReadAllLines("settings.ini");
            
            for (int line_i = 0; line_i < lines.Length; line_i++) {
                if (lines[line_i].Contains("bLaunchInTray"))
                    lines[line_i] = "bLaunchInTray: " + (checkBox_LaunchInTray.Checked ? "true" : "false") +";";
                else if (lines[line_i].Contains("iAutoP"))
                    lines[line_i] = "iAutoP: " + ((int)numericUpDown_AutoXXXX_p.Value) + ";";
            }

            File.WriteAllLines("settings.ini", lines);
        }


        public void GetWndSizes(IntPtr wnd_handle, ref WndSizes wnd_sizes) {
            Rectangle wndRect = new Rectangle();
            GetWindowRect((int)wnd_handle, ref wndRect);
            wnd_sizes.X = wndRect.X;
            wnd_sizes.Y = wndRect.Y;
            wnd_sizes.Frame_W = wndRect.Width - wndRect.X;
            wnd_sizes.Frame_H = wndRect.Height - wndRect.Y;
            
            RECT test_RECT = new RECT();                // Internal part (without frame).
            GetClientRect(wnd_handle, out test_RECT);
            wnd_sizes.Res_W = test_RECT.right - test_RECT.left;
            wnd_sizes.Res_H = test_RECT.bottom - test_RECT.top;
            wnd_sizes.border_Left = (wnd_sizes.Frame_W - wnd_sizes.Res_W) / 2;
            wnd_sizes.border_Right = wnd_sizes.border_Left;
            wnd_sizes.border_Bot = wnd_sizes.border_Left;
            wnd_sizes.border_Top = wnd_sizes.Frame_H - (wnd_sizes.Res_H + wnd_sizes.border_Bot);
        }
        
        private void SetWindowSize(IntPtr window_handle, int resolution_W = -1, int resolution_H = -1, int pos_X = -999, int pos_Y = -999) {
            // Current Rect info
            WndSizes wndSizes = new WndSizes();
            GetWndSizes(window_handle, ref wndSizes);
            pos_X = pos_X == -999 ? wndSizes.X : pos_X;
            pos_Y = pos_Y == -999 ? wndSizes.Y : pos_Y;
            resolution_W = resolution_W < 1 ? wndSizes.Res_W : resolution_W;
            resolution_H = resolution_H < 1 ? wndSizes.Res_H : resolution_H;
            resolution_W += wndSizes.border_Left + wndSizes.border_Right;
            resolution_H += wndSizes.border_Top + wndSizes.border_Bot;
            MoveWindow(window_handle, pos_X, pos_Y, resolution_W, resolution_H, true);
            LastAppData.posX = pos_X;
            LastAppData.posY = pos_Y;
            LastAppData.width = resolution_W;
            LastAppData.height = resolution_H;
        }
        
        
        private void listBox_Windows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Windows.SelectedIndex < 0)
                return;
            
            if (listBox_Windows.SelectedIndex >= ProcList.Length)
                return;
            
            if (ProcList[listBox_Windows.SelectedIndex].HasExited) {
                UpdateProcessList();
                return;
            }

            Process currProc = ProcList[listBox_Windows.SelectedIndex];
            
            string proc_path = "";
            string currProcInfo = GetProcInfo(currProc);
            label_ProcTitle.Text = "Title: " + currProc.MainWindowTitle;
            try
            {
                proc_path = "Path:  " + (
                    currProc.MainModule.FileName.Length > 46
                        ? "..." + currProc.MainModule.FileName.Substring(currProc.MainModule.FileName.Length - 43)
                        : currProc.MainModule.FileName);
                label_ProcPath.BackColor = Color.Transparent;
            }
            catch { // Process is protected. Getting an error, if trying to recieve info about process's file path.
                proc_path = "  (Process is protected. His path isn't available)   ";
                label_ProcPath.BackColor = Color.FromArgb(255,255,88,88);
            }
            
            label_ProcPath.Text = proc_path;
            lastSelectedWindowNode = listBox_Windows.SelectedIndex;
            lastSelectedWindowProcId = (int)currProc.MainWindowHandle;

            // Proc info to Form
            if (SavedAppsData.ContainsKey(currProcInfo)) {
                numericUpDown_DelayStartResize.Value    = (decimal)SavedAppsData[currProcInfo].delayStartingResize / 1000;
                label_HaveProfile.Visible = true;
                button_RemoveProfile.Visible = true;
                button_LoadProfile.Visible = true;
            }
            else {
                numericUpDown_DelayStartResize.Value    = defaultDelayAutoResize;
                label_HaveProfile.Visible = false;
                button_RemoveProfile.Visible = false;
                button_LoadProfile.Visible = false;
            }
                
            // Total Resolution
            WndSizes wndSizes = new WndSizes();
            GetWndSizes(currProc.MainWindowHandle, ref wndSizes);

            int total_X = wndSizes.X;
            int total_Y = wndSizes.Y;
            int total_W = wndSizes.Res_W;
            int total_H = wndSizes.Res_H;
            if (total_W < 0) total_W = 0;
            if (total_H < 0) total_H = 0;

            label_PosX.Text = total_X.ToString();
            label_PosY.Text = total_Y.ToString();
            label_SizeW.Text = total_W.ToString();
            label_SizeH.Text = total_H.ToString();
            numericUpDown_ResolutionW.Value = total_W;
            numericUpDown_ResolutionH.Value = total_H;
        }

        private void button_SaveProfile_Click(object sender, EventArgs e)
        { saveCurrProfile(); }
        public void saveCurrProfile()
        {
            int selIndex = lastSelectedWindowNode;
            if (selIndex == -1) {
                MessageBox.Show("Select correct Process", "Error"); return;
            }
            if (selIndex >= ProcList.Length) {
                MessageBox.Show("Please, Refresh the Process list", "Error"); return;
            }
            if (ProcList[selIndex].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            if (!File.Exists("applications.ini"))
                File.WriteAllText("applications.ini", "");

            // Gather data to save
            string profileFileInfo = GetProcInfo(ProcList[selIndex]);
            string proc_path = "";
            try {
                proc_path = ProcList[selIndex].MainModule.FileName;
            }
            catch {
                proc_path = "";
            }

            string resultProfileText = "♿ path: " + proc_path + " ♿ procName: " + ProcList[selIndex].ProcessName + " ♿ wndTitle: " + ProcList[selIndex].MainWindowTitle +
                " ♿ pos: " + int.Parse(label_PosX.Text) + "x" + int.Parse(label_PosY.Text) +
                " ♿ resolution: " + int.Parse(label_SizeW.Text) + "x"+ int.Parse(label_SizeH.Text) +
                " ♿ delayStartingResize: " + Math.Round(numericUpDown_DelayStartResize.Value * 1000) + " ♿";
            
            // Edit only data with current Profile
            if (File.Exists("applications.ini")) {
                string[] lines = File.ReadAllLines("applications.ini");
                bool savedBefore = false;

                for (int i = 0; i < lines.Length; i++) {
                    if (lines[i].Contains("♿ path: " + proc_path + " ♿") && 
                        lines[i].Contains("♿ procName: " + ProcList[selIndex].ProcessName + " ♿") &&
                        lines[i].Contains("♿ wndTitle: " + ProcList[selIndex].MainWindowTitle + " ♿"))
                    {
                        lines[i] = resultProfileText;
                        savedBefore = true;
                        break;
                    }
                }

                // Write entire settings file
                File.WriteAllText("applications.ini", "");
                using (StreamWriter fileWriter = new StreamWriter("applications.ini"))
                {
                    for (int i = 0; i < lines.Length; i++) {
                        fileWriter.WriteLine( lines[i] );
                    }

                    if (!savedBefore)
                        fileWriter.WriteLine(resultProfileText);

                    fileWriter.Close();
                }

                // Save to the current memory also
                if (!SavedAppsData.ContainsKey(profileFileInfo))
                    SavedAppsData.Add(profileFileInfo, new AppsData());
                SavedAppsData[profileFileInfo].startingPosX =           int.Parse(label_PosX.Text);
                SavedAppsData[profileFileInfo].startingPosY =           int.Parse(label_PosY.Text);
                SavedAppsData[profileFileInfo].startingWidth =          int.Parse(label_SizeW.Text);
                SavedAppsData[profileFileInfo].startingHeight =         int.Parse(label_SizeH.Text);
                SavedAppsData[profileFileInfo].delayStartingResize =    (int)(numericUpDown_DelayStartResize.Value * 1000);

                // No need in correcting size of selected window
                ProcAdditionalDataList[ProcList[selIndex].MainWindowHandle].alreadyStarted = true;
                label_HaveProfile.Visible = true;
                button_RemoveProfile.Visible = true;
                button_LoadProfile.Visible = true;
                MessageBox.Show("Saved!", "Success");
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
        }
        
        private void button_RemoveProfile_Click(object sender, EventArgs e)
        { removeCurrProfile(); }
        public void removeCurrProfile()
        {
            int selIndex = listBox_Windows.SelectedIndex;
            if (selIndex == -1) {
                MessageBox.Show("Select correct Process", "Error"); return;
            }
            if (selIndex >= ProcList.Length) {
                MessageBox.Show("Please, Refresh the Process list", "Error"); return;
            }
            if (ProcList[selIndex].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            if (!File.Exists("applications.ini"))
                File.WriteAllText("applications.ini", "");

            // Gather data to save
            string profileFileInfo = GetProcInfo(ProcList[selIndex]);
            string proc_path = "";
            try {
                proc_path = ProcList[selIndex].MainModule.FileName;
            }
            catch {
                proc_path = "";
            }

            // Edit only data with current Profile
            if (File.Exists("applications.ini"))
            {
                string[] lines = File.ReadAllLines("applications.ini");

                // Write entire settings file
                File.WriteAllText("applications.ini", "");
                using (StreamWriter fileWriter = new StreamWriter("applications.ini"))
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (!lines[i].Contains(proc_path) &&
                            !lines[i].Contains("procName: " + ProcList[selIndex].ProcessName) &&
                            !lines[i].Contains("wndTitle: " + ProcList[selIndex].MainWindowTitle)
                            )
                            fileWriter.WriteLine(lines[i]);
                    }

                    fileWriter.Close();
                }

                // Save to the current memory also
                if (SavedAppsData.ContainsKey(profileFileInfo))
                    SavedAppsData.Remove(profileFileInfo);

                // No need in correcting size of selected window
                ProcAdditionalDataList[ProcList[selIndex].MainWindowHandle].alreadyStarted = true;
                label_HaveProfile.Visible = false;
                button_RemoveProfile.Visible = false;
                button_LoadProfile.Visible = false;
                MessageBox.Show("Removed!", "Success");
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Tread_Update != null)
                Tread_Update.Abort();
            SaveSettings();
        }


        private void button_ScalePlus_Click(object sender, EventArgs e)
        { ScaleWindow(true); }

        private void button_ScaleMinus_Click(object sender, EventArgs e)
        { ScaleWindow(false); }

        public void ScaleWindow(bool isMultiply) {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            // Calc result sizes
            int total_W = 0;
            int total_H = 0;

            if (isMultiply) {
                total_W = (int)Math.Round(decimal.Parse(label_SizeW.Text) * numericUpDown_Scale.Value);
                total_H = (int)Math.Round(decimal.Parse(label_SizeH.Text) * numericUpDown_Scale.Value);
            }
            else {
                total_W = (int)Math.Round(decimal.Parse(label_SizeW.Text) / numericUpDown_Scale.Value);
                total_H = (int)Math.Round(decimal.Parse(label_SizeH.Text) / numericUpDown_Scale.Value);
            }

            if (total_W < 200) {
                MessageBox.Show("Total Width must not to be below 200 px in result!", "Error"); return;
            }
            if (total_H < 30) {
                MessageBox.Show("Total Width must not to be below 30 px in result!\n\r(maybe target window is minimized?)", "Error"); return;
            }

            numericUpDown_ResolutionW.Value = total_W;
            numericUpDown_ResolutionH.Value = total_H;
            label_SizeW.Text = total_W.ToString();
            label_SizeH.Text = total_H.ToString();
            
            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, total_W, total_H);
        }

        // Возвращаемое значение должно быть по шаблону   "path: 'путь-до-файла-если-есть', procName: 'имя-процесса', wndTitle: 'заголовок-окна'"
        public string GetProcInfo(Process proc)
        {
            string result = "";
            string currProcFilename = "";
            try {
                currProcFilename = proc.MainModule.FileName;
            }
            catch { }
            if (currProcFilename != "")
                result = "path: '" + currProcFilename + "', procName: '" + proc.ProcessName + "', wndTitle: '" + proc.MainWindowTitle + "'";
            else        // Если процесс защищен, записываем только имя процесса и заголовок окна
                result = "path: '', procName: '" + proc.ProcessName + "', wndTitle: '" + proc.MainWindowTitle + "'";

            return result;
        }

        private void button_SetCustomSizes_Click(object sender, EventArgs e)
        {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            // Result sizes
            int total_W = (int)numericUpDown_ResolutionW.Value;
            int total_H = (int)numericUpDown_ResolutionH.Value;

            if (total_W < 200) {
                MessageBox.Show("Total Width must not to be below 200 px in result!", "Error"); return;
            }
            if (total_H < 200) {
                MessageBox.Show("Total Height must not to be below 200 px in result!", "Error"); return;
            }

            label_SizeW.Text = total_W.ToString();
            label_SizeH.Text = total_H.ToString();

            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, total_W, total_H);
        }

        private void button_setXXXXp_Click(object sender, EventArgs e)
        {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length)
            {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited)
            {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            int maxP = (int)numericUpDown_AutoXXXX_p.Value;

            // Calc result sizes
            int total_W = (int)Math.Round(decimal.Parse(label_SizeW.Text));
            int total_H = (int)Math.Round(decimal.Parse(label_SizeH.Text));

            for (double mult_i = 2.0; mult_i > 0.1; mult_i = mult_i - 0.1)
            {
                int test_H = (int)Math.Round(Convert.ToDouble(total_H) * mult_i);
                if (test_H > maxP)
                    continue;

                // If sizes OK.

                total_W = (int)Math.Round(Convert.ToDouble(total_W) * mult_i);
                total_H = (int)Math.Round(Convert.ToDouble(total_H) * mult_i);

                numericUpDown_ResolutionW.Value = total_W;
                numericUpDown_ResolutionH.Value = total_H;
                label_SizeW.Text = total_W.ToString();
                label_SizeH.Text = total_H.ToString();

                SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, total_W, total_H);

                // MessageBox.Show("result size:" + total_W + "x" + total_H + " (multiply = " + mult_i + ")!", "Saccess.");

                break;
            }
        }

        // Minimize in Tray
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) {
                Hide();
                notifyIcon.Visible = true;
            }
        }
        
        
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { // Maximize from Tray
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }
        }

        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void button_LoadProfile_Click(object sender, EventArgs e)
        {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }
            string currProcInfo = GetProcInfo(ProcList[lastSelectedWindowNode]);

            if (!SavedAppsData.ContainsKey(currProcInfo)) {
                MessageBox.Show("Process doesn't have a Saved Profile!", "Error"); return;
            }

            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle,
                            SavedAppsData[currProcInfo].startingWidth,
                            SavedAppsData[currProcInfo].startingHeight,
                            SavedAppsData[currProcInfo].startingPosX,
                            SavedAppsData[currProcInfo].startingPosY);
            
            label_PosX.Text = SavedAppsData[currProcInfo].startingPosX.ToString();
            label_PosY.Text = SavedAppsData[currProcInfo].startingPosY.ToString();
            label_SizeW.Text = SavedAppsData[currProcInfo].startingWidth.ToString();
            label_SizeH.Text = SavedAppsData[currProcInfo].startingHeight.ToString();
        }

        private void Form1_Activated(object sender, EventArgs e)
        { UpdateProcessList(); }

        private void button_SetPosTo_0x1_Click(object sender, EventArgs e) {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }
            string currProcInfo = GetProcInfo(ProcList[lastSelectedWindowNode]);
            
            // Calculate PosX and PosY without borders
            WndSizes wndSizes = new WndSizes();
            GetWndSizes(ProcList[lastSelectedWindowNode].MainWindowHandle, ref wndSizes);
            int result_PosX = -wndSizes.border_Left;
            int result_PosY = 1 - wndSizes.border_Top;

            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, -1, -1,
                result_PosX, result_PosY);

            label_PosX.Text = result_PosX.ToString();
            label_PosY.Text = result_PosY.ToString();
        }
    }
}
