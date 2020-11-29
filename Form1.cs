﻿using System;
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


        Dictionary<string, AppsData> SavedAppsData = new Dictionary<string, AppsData>(); // SavedAppsData[path-to-file] = { data... };
        public class AppsData
        {
            public int startingWidth = 0;
            public int startingHeight = 0;
            public int delayStartingResize = 0;
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

            clearNotExistsProfiles();

            getSettigns();
            
            updateProcessList();

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
                Process CurrentProcess = getForegroundProcess();
                
                if (String.IsNullOrEmpty(CurrentProcess.MainWindowTitle))
                    continue;
                if (CurrentProcess.HasExited)
                    continue;
                
                bool processProtected = false;
                string currProcFilename = "";
                try {
                    currProcFilename = CurrentProcess.MainModule.FileName;
                }
                catch { // Process is protected. Getting an error, if trying to recieve info about process's file path.
                    continue;
                }

                // Additional data only if not previously exists
                if (!ProcAdditionalDataList.ContainsKey(CurrentProcess.MainWindowHandle))
                    ProcAdditionalDataList.Add(CurrentProcess.MainWindowHandle, new ProcessAdditionalData());
                    
                // Change sizes for the new windows, which saved before as Profiles in INI file
                if (!ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted 
                    && SavedAppsData.ContainsKey(currProcFilename))
                {
                    if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft == -999) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft =
                            SavedAppsData[currProcFilename].delayStartingResize - 500; // Init Delay to First auto resize
                    }
                    else if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft > 0) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft -= 500; // Decrease delay
                    }
                    // Do nothing, if window minimized
                    else if (windowModeStatus(CurrentProcess.MainWindowHandle) != WndMode.Minimized)
                    {
                        // If time to Starting resize come
                        SetWindowSize(CurrentProcess.MainWindowHandle,
                            SavedAppsData[currProcFilename].startingWidth,
                            SavedAppsData[currProcFilename].startingHeight);

                        // Update sizes in programm also, if window currently selected
                        if (lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                            && !ProcList[lastSelectedWindowNode].HasExited
                            && ProcList[lastSelectedWindowNode].MainModule.FileName == currProcFilename)
                        {
                            Invoke(new Action(() => {
                                numericUpDown_ResolutionW.Value = SavedAppsData[currProcFilename].startingWidth;
                                numericUpDown_ResolutionH.Value = SavedAppsData[currProcFilename].startingHeight;
                            }));
                        }

                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted = true;
                    }
                }

                // Update window size info, if currently selected
                if (this.WindowState != FormWindowState.Minimized 
                        && lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                        && !ProcList[lastSelectedWindowNode].HasExited
                        && ProcList[lastSelectedWindowNode].MainModule.FileName == currProcFilename) {
                    WndSizes wndSizes = new WndSizes();
                    GetWndSizes(CurrentProcess.MainWindowHandle, ref wndSizes);
                    Invoke(new Action(() => {
                        label_SizeW.Text = wndSizes.Res_W.ToString();
                        label_SizeH.Text = wndSizes.Res_H.ToString();
                    }));
                }
            }
        }
        
        private void button_RefreshProcList_Click(object sender, EventArgs e)
        { updateProcessList(); }

        public void updateProcessList() {
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

        public Process getForegroundProcess() {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return Process.GetProcessById((int)pid);
        }

        public void clearNotExistsProfiles() {

            if (!File.Exists("applications.ini")) {
                File.WriteAllText("applications.ini", "");
                return;
            }

            string[] lines = File.ReadAllLines("applications.ini");

            // Write entire settings file
            File.WriteAllText("applications.ini", "");
            using (StreamWriter fileWriter = new StreamWriter("applications.ini"))
            {
                for (int i = 0; i < lines.Length; i++) {
                    Match match = new Regex(@"path: '(.+)';",
                            RegexOptions.IgnoreCase).Match(lines[i]);

                    // If Profile's game file exists...
                    if (match.Success && File.Exists(match.Groups[1].Value))
                        fileWriter.WriteLine(lines[i]);
                }
                fileWriter.Close();
            }
        }

        public void getSettigns() {

            if (!File.Exists("settings.ini"))
                File.WriteAllText("settings.ini", "delayStartingResize: 2;");

            // default params for entire programm
            if (File.Exists("settings.ini"))
            {
                Match match = new Regex(@"delayStartingResize: (\d+);", 
                    RegexOptions.IgnoreCase).Match(File.ReadAllLines("settings.ini")[0]);
                if (match.Success)
                {
                    defaultDelayAutoResize  = int.Parse(match.Groups[1].Value);
                    numericUpDown_DelayStartResize.Value = defaultDelayAutoResize;
                }
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
                        Match match = new Regex(@"path: '(.+)'; resolution: (\d+)x(\d+); delayStartingResize: (\d+);", 
                            RegexOptions.IgnoreCase).Match(line);
                        if (match.Success) {
                            SavedAppsData.Add(match.Groups[1].Value, new AppsData());
                            SavedAppsData[match.Groups[1].Value].startingWidth          = int.Parse(match.Groups[2].Value);
                            SavedAppsData[match.Groups[1].Value].startingHeight         = int.Parse(match.Groups[3].Value);
                            SavedAppsData[match.Groups[1].Value].delayStartingResize    = int.Parse(match.Groups[4].Value);
                        }
                    }
                    file.Close();
                }
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
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
        
        private void SetWindowSize(IntPtr window_handle, int resolution_W, int resolution_H) {
            // Current Rect info
            WndSizes wndSizes = new WndSizes();
            GetWndSizes(window_handle, ref wndSizes);
            MoveWindow(window_handle, wndSizes.X, wndSizes.Y, 
                (resolution_W + wndSizes.border_Left + wndSizes.border_Right),
                (resolution_H + wndSizes.border_Top + wndSizes.border_Bot), true);
        }
        
        

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.F5 && timer_scan_pixels.Enabled) // Apply selecting border data
                //borderSelectToggle();
        }
        
        private void listBox_Windows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Windows.SelectedIndex < 0)
                return;


            if (listBox_Windows.SelectedIndex >= ProcList.Length)
                return;
            
            if (ProcList[listBox_Windows.SelectedIndex].HasExited) {
                updateProcessList();
                return;
            }

            Process currProc = ProcList[listBox_Windows.SelectedIndex];
            
            bool processProtected = false;
            string proc_path = "";
            label_ProcTitle.Text = "Title: " + currProc.MainWindowTitle;
            try
            {
                proc_path = "Path:  " + (
                    currProc.MainModule.FileName.Length > 46
                        ? "..." + currProc.MainModule.FileName.Substring(currProc.MainModule.FileName.Length - 43)
                        : currProc.MainModule.FileName);
            }
            catch { // Process is protected. Getting an error, if trying to recieve info about process's file path.
                processProtected = true;
                proc_path = "   Process is protected !!! Can't use him =(   ";
                lastSelectedWindowNode = -1;
                lastSelectedWindowProcId = -1;
                label_ProcPath.BackColor = Color.FromArgb(255,255,88,88);
            }

            if (!processProtected) { // If we can use the process in another parts of program.
                lastSelectedWindowNode = listBox_Windows.SelectedIndex;
                lastSelectedWindowProcId = (int)currProc.MainWindowHandle;
                label_ProcPath.BackColor = Color.Transparent;
            }
            
            label_ProcPath.Text = proc_path;
            
            // Proc info to Form
            if (!processProtected && SavedAppsData.ContainsKey(currProc.MainModule.FileName )) {
                numericUpDown_DelayStartResize.Value    = (decimal)SavedAppsData[ currProc.MainModule.FileName ].delayStartingResize / 1000;
                label_HaveProfile.Visible = true;
                button_RemoveProfile.Visible = true;
            }
            else {
                numericUpDown_DelayStartResize.Value    = defaultDelayAutoResize;
                label_HaveProfile.Visible = false;
                button_RemoveProfile.Visible = false;
            }
                
            // Total Resolution
            WndSizes wndSizes = new WndSizes();
            GetWndSizes(currProc.MainWindowHandle, ref wndSizes);

            int total_W = wndSizes.Res_W;
            int total_H = wndSizes.Res_H;
            if (total_W < 0) total_W = 0;
            if (total_H < 0) total_H = 0;
                
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
            string profileFilePath =    ProcList[selIndex].MainModule.FileName;
            string resultProfileText = "path: '"+ profileFilePath + "'; resolution: "+ int.Parse(label_SizeW.Text) + "x"+ int.Parse(label_SizeH.Text) + 
                "; delayStartingResize: " + Math.Round(numericUpDown_DelayStartResize.Value * 1000) + ";";
            
            // Edit only data with current Profile
            if (File.Exists("applications.ini")) {
                string[] lines = File.ReadAllLines("applications.ini");
                bool savedBefore = false;

                for (int i = 0; i < lines.Length; i++) {
                    if (lines[i].IndexOf("path: '"+ profileFilePath + "'") > -1) {
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
                if (!SavedAppsData.ContainsKey(profileFilePath))
                    SavedAppsData.Add(profileFilePath, new AppsData());
                SavedAppsData[profileFilePath].startingWidth =          int.Parse(label_SizeW.Text);
                SavedAppsData[profileFilePath].startingHeight =         int.Parse(label_SizeH.Text);
                SavedAppsData[profileFilePath].delayStartingResize =    (int)(numericUpDown_DelayStartResize.Value * 1000);

                // No need in correcting size of selected window
                ProcAdditionalDataList[ProcList[selIndex].MainWindowHandle].alreadyStarted = true;
                label_HaveProfile.Visible = true;
                button_RemoveProfile.Visible = true;
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
            string profileFilePath = ProcList[selIndex].MainModule.FileName;

            // Edit only data with current Profile
            if (File.Exists("applications.ini"))
            {
                string[] lines = File.ReadAllLines("applications.ini");

                // Write entire settings file
                File.WriteAllText("applications.ini", "");
                using (StreamWriter fileWriter = new StreamWriter("applications.ini"))
                {
                    for (int i = 0; i < lines.Length; i++) {
                        if (!lines[i].Contains("'" + profileFilePath + "'"))
                            fileWriter.WriteLine(lines[i]);
                    }

                    fileWriter.Close();
                }

                // Save to the current memory also
                if (SavedAppsData.ContainsKey(profileFilePath))
                    SavedAppsData.Remove(profileFilePath);

                // No need in correcting size of selected window
                ProcAdditionalDataList[ProcList[selIndex].MainWindowHandle].alreadyStarted = false;
                label_HaveProfile.Visible = false;
                button_RemoveProfile.Visible = false;
                MessageBox.Show("Removed!", "Success");
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        { Tread_Update.Abort(); }


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

        private void button_set1050p_Click(object sender, EventArgs e)
        {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Process not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            // Calc result sizes
            int total_W = (int)Math.Round(decimal.Parse(label_SizeW.Text));
            int total_H = (int)Math.Round(decimal.Parse(label_SizeH.Text));

            for (double mult_i = 2.0; mult_i > 0.9; mult_i = mult_i - 0.1) {
                int test_H = (int)Math.Round(Convert.ToDouble(total_H) * mult_i);
                if (test_H > 1050)
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

    }
}
