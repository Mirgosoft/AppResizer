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
        public int defaultWndBorder_Top = 0;
        public int defaultWndBorder_Right = 0;
        public int defaultWndBorder_Bot = 0;
        public int defaultWndBorder_Left = 0;
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
            public int Width = 0;
            public int Height = 0;
        }
        
        Dictionary<string, AppsData> SavedAppsData = new Dictionary<string, AppsData>(); // SavedAppsData[path-to-file] = { data... };
        public class AppsData
        {
            public int borderTop = 0;
            public int borderRight = 0;
            public int borderBottom = 0;
            public int borderLeft = 0;
            public int startingWidth = 0;
            public int startingHeight = 0;
            public int delayStartingResize = 0;
        }

        Thread Tread_Update;
        

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern long GetWindowRect(int hWnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

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

        // For recieving pixels
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("gdi32", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDC);
        [DllImport("gdi32", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hDC, int x, int y);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);



        // Рисование прямоугольника
        public void draw_rect(byte r, byte g, byte b, int X, int Y, int W, int H)
        {
            // Рисование прямоугольника
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(r, g, b));
            System.Drawing.Graphics formGraphics;
            formGraphics = picture_BorderVisualizer.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(X, Y, W, H));
            myBrush.Dispose();
            formGraphics.Dispose();
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
                
                if (String.IsNullOrEmpty(CurrentProcess.MainWindowTitle) || CurrentProcess.ProcessName == "Taskmgr")
                    continue;
                if (CurrentProcess.HasExited)
                    continue;

                // Additional data only if not previously exists
                if (!ProcAdditionalDataList.ContainsKey(CurrentProcess.MainWindowHandle))
                    ProcAdditionalDataList.Add(CurrentProcess.MainWindowHandle, new ProcessAdditionalData());
                    
                // Change sizes for the new windows, which saved before as Profiles in INI file
                if (!ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted 
                    && SavedAppsData.ContainsKey(CurrentProcess.MainModule.FileName))
                {
                    if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft == -999) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft =
                            SavedAppsData[CurrentProcess.MainModule.FileName].delayStartingResize - 500; // Init Delay to First auto resize
                    }
                    else if (ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft > 0) {
                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].delayStartingResizeLeft -= 500; // Decrease delay
                    }
                    // Do nothing, if window not minimized
                    else if (windowModeStatus(CurrentProcess.MainWindowHandle) != WndMode.Minimized)
                    {
                        // If time to Starting resize come
                        SetWindowSize(CurrentProcess.MainWindowHandle,
                            SavedAppsData[CurrentProcess.MainModule.FileName].startingWidth + SavedAppsData[CurrentProcess.MainModule.FileName].borderLeft
                                + SavedAppsData[CurrentProcess.MainModule.FileName].borderRight,
                            SavedAppsData[CurrentProcess.MainModule.FileName].startingHeight + SavedAppsData[CurrentProcess.MainModule.FileName].borderTop
                                + SavedAppsData[CurrentProcess.MainModule.FileName].borderBottom);

                        // Update sizes in programm also, if window currently selected
                        if (lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                            && !ProcList[lastSelectedWindowNode].HasExited
                            && ProcList[lastSelectedWindowNode].MainModule.FileName == CurrentProcess.MainModule.FileName)
                        {
                            Invoke(new Action(() => {
                                numericUpDown_BorderTop.Value = SavedAppsData[CurrentProcess.MainModule.FileName].borderTop;
                                numericUpDown_BorderRight.Value = SavedAppsData[CurrentProcess.MainModule.FileName].borderRight;
                                numericUpDown_BorderBot.Value = SavedAppsData[CurrentProcess.MainModule.FileName].borderBottom;
                                numericUpDown_BorderLeft.Value = SavedAppsData[CurrentProcess.MainModule.FileName].borderLeft;
                                numericUpDown_ResolutionW.Value = SavedAppsData[CurrentProcess.MainModule.FileName].startingWidth;
                                numericUpDown_ResolutionH.Value = SavedAppsData[CurrentProcess.MainModule.FileName].startingHeight;
                            }));
                        }

                        ProcAdditionalDataList[CurrentProcess.MainWindowHandle].alreadyStarted = true;
                    }
                }

                // Update window size info, if currently selected
                if (this.WindowState != FormWindowState.Minimized 
                        && lastSelectedWindowNode > -1 && lastSelectedWindowNode < ProcList.Length
                        && !ProcList[lastSelectedWindowNode].HasExited
                        && ProcList[lastSelectedWindowNode].MainModule.FileName == CurrentProcess.MainModule.FileName) {
                    WndSizes wndSizes = new WndSizes();
                    getWndSizes(CurrentProcess.MainWindowHandle, ref wndSizes);
                    Invoke(new Action(() => {
                        label_SizeW.Text = (wndSizes.Width - numericUpDown_BorderRight.Value - numericUpDown_BorderLeft.Value).ToString();
                        label_SizeH.Text = (wndSizes.Height - numericUpDown_BorderTop.Value - numericUpDown_BorderBot.Value).ToString();
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

        public void getSettigns() {

            if (!File.Exists("settings.ini"))
                File.WriteAllText("settings.ini", "border_top: 26; border_right: 3; border_bot: 3; border_left: 3; delayStartingResize: 1;");

            // default params for entire programm
            if (File.Exists("settings.ini"))
            {
                Match match = new Regex(@"border_top: (\d+); border_right: (\d+); border_bot: (\d+); border_left: (\d+); delayStartingResize: (\d+);", 
                    RegexOptions.IgnoreCase).Match(File.ReadAllLines("settings.ini")[0]);
                if (match.Success)
                {
                    defaultWndBorder_Top    = int.Parse(match.Groups[1].Value);
                    defaultWndBorder_Right  = int.Parse(match.Groups[2].Value);
                    defaultWndBorder_Bot    = int.Parse(match.Groups[3].Value);
                    defaultWndBorder_Left   = int.Parse(match.Groups[4].Value);
                    defaultDelayAutoResize  = int.Parse(match.Groups[5].Value);
                    numericUpDown_BorderTop.Value           = defaultWndBorder_Top;
                    numericUpDown_BorderRight.Value         = defaultWndBorder_Right;
                    numericUpDown_BorderBot.Value           = defaultWndBorder_Bot;
                    numericUpDown_BorderLeft.Value          = defaultWndBorder_Left;
                    numericUpDown_DelayStartResize.Value    = defaultDelayAutoResize;
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
                        Match match = new Regex(@"path: '(.+)'; border_top: -?(\d+); border_right: -?(\d+); border_bot: -?(\d+); border_left: -?(\d+); resolution: (\d+)x(\d+); delayStartingResize: (\d+);", 
                            RegexOptions.IgnoreCase).Match(line);
                        if (match.Success) {
                            SavedAppsData.Add(match.Groups[1].Value, new AppsData());
                            SavedAppsData[match.Groups[1].Value].borderTop              = int.Parse(match.Groups[2].Value);
                            SavedAppsData[match.Groups[1].Value].borderRight            = int.Parse(match.Groups[3].Value);
                            SavedAppsData[match.Groups[1].Value].borderBottom           = int.Parse(match.Groups[4].Value);
                            SavedAppsData[match.Groups[1].Value].borderLeft             = int.Parse(match.Groups[5].Value);
                            SavedAppsData[match.Groups[1].Value].startingWidth          = int.Parse(match.Groups[6].Value);
                            SavedAppsData[match.Groups[1].Value].startingHeight         = int.Parse(match.Groups[7].Value);
                            SavedAppsData[match.Groups[1].Value].delayStartingResize    = int.Parse(match.Groups[8].Value);
                        }
                    }
                    file.Close();
                }
            }
            else
                MessageBox.Show("applications.ini not found", "Error");
        }


        public void getWndSizes(IntPtr wnd_handle, ref WndSizes wnd_sizes) {
            Rectangle wndRect = new Rectangle();
            GetWindowRect((int)wnd_handle, ref wndRect);
            wnd_sizes.X = wndRect.X;
            wnd_sizes.Y = wndRect.Y;
            wnd_sizes.Width = wndRect.Width - wndRect.X;
            wnd_sizes.Height = wndRect.Height - wndRect.Y;
        }
        
        private void SetWindowSize(IntPtr window_handle, int width, int height) {
            // Current Rect info
            WndSizes wndSizes = new WndSizes();
            getWndSizes(window_handle, ref wndSizes);
            MoveWindow(window_handle, wndSizes.X, wndSizes.Y, width, height, true);
        }
        

        private void button_SetBorder_Click(object sender, EventArgs e)
        { borderSelectToggle(false); }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && timer_scan_pixels.Enabled) // Apply selecting border data
                borderSelectToggle();
        }

        public void borderSelectToggle(bool needApplyResult = true) {
            // Errors check
            if (!timer_scan_pixels.Enabled)
            {
                if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                    MessageBox.Show("Window not selected", "Error"); return;
                }
                else if (ProcList[lastSelectedWindowNode].HasExited) {
                    MessageBox.Show("Proccess not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
                }
            }
            
            timer_scan_pixels.Enabled = !timer_scan_pixels.Enabled;
            
            // if border setup mode activated
            if (timer_scan_pixels.Enabled)
            {
                picture_BorderVisualizer.Visible = true;
                label_CursorPos.Visible = true;
                label_SetBorderTips.Visible = true;
                button_SetBorder.Text = "Cancel";
                backupWndBorder_Top = (int)numericUpDown_BorderTop.Value;
                backupWndBorder_Right = (int)numericUpDown_BorderRight.Value;
                backupWndBorder_Bottom = (int)numericUpDown_BorderBot.Value;
                backupWndBorder_Left = (int)numericUpDown_BorderLeft.Value;
            }
            else
            {
                picture_BorderVisualizer.Visible = false;
                label_SetBorderTips.Visible = false;
                label_CursorPos.Visible = false;
                button_SetBorder.Text = "Calibrate Border Size";

                if (!needApplyResult) {  // Restore backup border data, if no need in Applying data
                    numericUpDown_BorderTop.Value   = backupWndBorder_Top;
                    numericUpDown_BorderRight.Value = backupWndBorder_Right;
                    numericUpDown_BorderBot.Value   = backupWndBorder_Bottom;
                    numericUpDown_BorderLeft.Value  = backupWndBorder_Left;
                }
            }
        }

        private void listBox_Windows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Windows.SelectedIndex < 0)
                return;

            lastSelectedWindowNode = listBox_Windows.SelectedIndex;

            if (listBox_Windows.SelectedIndex < ProcList.Length)
            {
                if (ProcList[lastSelectedWindowNode].HasExited) {
                    updateProcessList();
                    return;
                }

                Process currProc = ProcList[listBox_Windows.SelectedIndex];
                lastSelectedWindowProcId = (int)currProc.MainWindowHandle;

                label_ProcTitle.Text = "Title: " + currProc.MainWindowTitle;
                label_ProcPath.Text = "Path:  " + (
                        currProc.MainModule.FileName.Length > 46 
                            ? "..." + currProc.MainModule.FileName.Substring(currProc.MainModule.FileName.Length - 43)
                            : currProc.MainModule.FileName
                    );

                // Border info
                if (SavedAppsData.ContainsKey(currProc.MainModule.FileName )) {
                    numericUpDown_BorderTop.Value           = SavedAppsData[ currProc.MainModule.FileName ].borderTop;
                    numericUpDown_BorderRight.Value         = SavedAppsData[ currProc.MainModule.FileName ].borderRight;
                    numericUpDown_BorderBot.Value           = SavedAppsData[ currProc.MainModule.FileName ].borderBottom;
                    numericUpDown_BorderLeft.Value          = SavedAppsData[ currProc.MainModule.FileName ].borderLeft;
                    numericUpDown_DelayStartResize.Value    = (decimal)SavedAppsData[ currProc.MainModule.FileName ].delayStartingResize / 1000;
                    label_HaveProfile.Visible = true;
                }
                else {
                    numericUpDown_BorderTop.Value           = defaultWndBorder_Top;
                    numericUpDown_BorderRight.Value         = defaultWndBorder_Right;
                    numericUpDown_BorderBot.Value           = defaultWndBorder_Bot;
                    numericUpDown_BorderLeft.Value          = defaultWndBorder_Left;
                    numericUpDown_DelayStartResize.Value    = defaultDelayAutoResize;
                    label_HaveProfile.Visible = false;
                }
                
                // Total Resolution
                WndSizes wndSizes = new WndSizes();
                getWndSizes(currProc.MainWindowHandle, ref wndSizes);

                int total_W = wndSizes.Width - (int)numericUpDown_BorderRight.Value - (int)numericUpDown_BorderLeft.Value;
                int total_H = wndSizes.Height - (int)numericUpDown_BorderTop.Value - (int)numericUpDown_BorderBot.Value;
                if (total_W < 0) total_W = 0;
                if (total_H < 0) total_H = 0;
                
                label_SizeW.Text = total_W.ToString();
                label_SizeH.Text = total_H.ToString();
                numericUpDown_ResolutionW.Value = total_W;
                numericUpDown_ResolutionH.Value = total_H;
            }
        }

        private void button_SaveProfile_Click(object sender, EventArgs e)
        { saveCurrProfile(); }
        public void saveCurrProfile()
        {
            int selIndex = listBox_Windows.SelectedIndex;
            if (selIndex == -1) {
                MessageBox.Show("Select Proccess", "Error"); return;
            }
            if (selIndex >= ProcList.Length) {
                MessageBox.Show("Please, Refresh the Proccess list", "Error"); return;
            }
            if (ProcList[selIndex].HasExited) {
                MessageBox.Show("Proccess not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            if (!File.Exists("applications.ini"))
                File.WriteAllText("applications.ini", "");

            // Gather data to save
            string profileFilePath =    ProcList[selIndex].MainModule.FileName;
            string resultProfileText = "path: '"+ profileFilePath + "'; border_top: "+ numericUpDown_BorderTop.Value + "; border_right: " +
                numericUpDown_BorderRight.Value + "; border_bot: " + numericUpDown_BorderBot.Value + "; border_left: " +
                numericUpDown_BorderLeft.Value + "; resolution: "+ int.Parse(label_SizeW.Text) + "x"+ int.Parse(label_SizeH.Text) + 
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
                SavedAppsData[profileFilePath].borderTop =              (int)numericUpDown_BorderTop.Value;
                SavedAppsData[profileFilePath].borderRight =            (int)numericUpDown_BorderRight.Value;
                SavedAppsData[profileFilePath].borderBottom =           (int)numericUpDown_BorderBot.Value;
                SavedAppsData[profileFilePath].borderLeft =             (int)numericUpDown_BorderLeft.Value;
                SavedAppsData[profileFilePath].startingWidth =          int.Parse(label_SizeW.Text);
                SavedAppsData[profileFilePath].startingHeight =         int.Parse(label_SizeH.Text);
                SavedAppsData[profileFilePath].delayStartingResize =    (int)(numericUpDown_DelayStartResize.Value * 1000);

                // No need in correcting size of selected window
                ProcAdditionalDataList[ProcList[selIndex].MainWindowHandle].alreadyStarted = true;
                label_HaveProfile.Visible = true;
                MessageBox.Show("Saved!", "Success");
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
                MessageBox.Show("Proccess not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            // calc result sizes
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

            total_W += (int)(numericUpDown_BorderLeft.Value + numericUpDown_BorderRight.Value);
            total_H += (int)(numericUpDown_BorderTop.Value + numericUpDown_BorderBot.Value);
            
            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, total_W, total_H);
        }

        private void button_SetCustomSizes_Click(object sender, EventArgs e)
        {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length) {
                MessageBox.Show("Window not selected", "Error"); return;
            }
            if (ProcList[lastSelectedWindowNode].HasExited) {
                MessageBox.Show("Proccess not exists anymore!\n\rRefresh wondow's list and try again", "Error"); return;
            }

            // result sizes
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

            total_W += (int)(numericUpDown_BorderLeft.Value + numericUpDown_BorderRight.Value);
            total_H += (int)(numericUpDown_BorderTop.Value + numericUpDown_BorderBot.Value);

            SetWindowSize(ProcList[lastSelectedWindowNode].MainWindowHandle, total_W, total_H);
        }

        private void timer_scan_pixels_Tick(object sender, EventArgs e)
        {
            if (!timer_scan_pixels.Enabled)
                return;
            
            // 1) Draw Scaled area of cursor pos for selecting Top-Left Border point

            // Clear previous picture from memory for sure
            if (selected_scanned_px_area != null)
                selected_scanned_px_area.Dispose();
            
            selected_area_wndX = Cursor.Position.X;
            selected_area_wndY = Cursor.Position.Y;

            // Recieve necessery part of screen
            selected_scanned_px_area = scan_pixels_area_from_wnd(Cursor.Position.X - (scan_area_W/2), Cursor.Position.Y - (scan_area_W / 2), scan_area_W, scan_area_H);
            
            // Visuaaly whow selected area in programm
            Graphics gForm = Graphics.FromHwnd(picture_BorderVisualizer.Handle);
            gForm.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gForm.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gForm.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            gForm.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            gForm.DrawImage(selected_scanned_px_area, 6, 6, 150, 150);

            draw_rect(255, 33, 33, 79, 79, 20, 5); // Cursor pos
            draw_rect(255, 33, 33, 79, 79, 5, 20);

            gForm.Dispose();

            label_CursorPos.Text = "Cursor Pos: "+Cursor.Position.X + "x"+ Cursor.Position.Y;

            // 2) Calculate the border sizes
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length || ProcList[lastSelectedWindowNode].HasExited)
                return;

            // Current Window sizes (with Border)
            WndSizes wndSizes = new WndSizes();
            getWndSizes(ProcList[lastSelectedWindowNode].MainWindowHandle, ref wndSizes);

            int border_TitleHeight = Cursor.Position.Y - wndSizes.Y;
            int border_RightBotomLeftWidth = Cursor.Position.X - wndSizes.X;

            if (border_TitleHeight < -99)
                border_TitleHeight = -99;
            else if (border_TitleHeight > 99)
                border_TitleHeight = 99;

            if (border_RightBotomLeftWidth < -99)
                border_RightBotomLeftWidth = -99;
            else if (border_RightBotomLeftWidth > 99)
                border_RightBotomLeftWidth = 99;


            numericUpDown_BorderTop.Value   = border_TitleHeight;
            numericUpDown_BorderRight.Value = border_RightBotomLeftWidth;
            numericUpDown_BorderBot.Value   = border_RightBotomLeftWidth;
            numericUpDown_BorderLeft.Value  = border_RightBotomLeftWidth;
            
            // (window sizes calculating automaticaly (numericUpDown change event)) 
        }

        public Bitmap scan_pixels_area_from_wnd(int X, int Y, int Width, int Height)
        {
            Bitmap scanned_px_img = null;

            IntPtr hdcTo = IntPtr.Zero;
            IntPtr hdcFrom = IntPtr.Zero;
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                //Bitmap scanned_px_img = null;

                bool IsClientWnd = true;    // true or false
                                            // get device context of the window...
                hdcFrom = IsClientWnd ? GetDC(IntPtr.Zero) : GetWindowDC(IntPtr.Zero);

                // create dc that we can draw to...
                hdcTo = CreateCompatibleDC(hdcFrom);
                hBitmap = CreateCompatibleBitmap(hdcFrom, Width, Height);

                //  validate
                if (hBitmap != IntPtr.Zero)
                {
                    // adjust and copy
                    IntPtr hLocalBitmap = SelectObject(hdcTo, hBitmap);
                    BitBlt(hdcTo, 0, 0, Width, Height, hdcFrom, X, Y, 13369376);
                    SelectObject(hdcTo, hLocalBitmap);
                    //  create bitmap for window image...
                    scanned_px_img = System.Drawing.Image.FromHbitmap(hBitmap);
                }
            }
            finally
            {
                //  release the unmanaged resources
                if (hdcFrom != IntPtr.Zero) ReleaseDC(IntPtr.Zero, hdcFrom);
                if (hdcTo != IntPtr.Zero) DeleteDC(hdcTo);
                if (hBitmap != IntPtr.Zero) DeleteObject(hBitmap);
            }
            return scanned_px_img;
            //*/
        }

        private void numericUpDown_BorderTop_ValueChanged(object sender, EventArgs e)
        { calcWndInnerSizes(); }
        private void numericUpDown_BorderRight_ValueChanged(object sender, EventArgs e)
        { calcWndInnerSizes(); }

        private void numericUpDown_BorderBot_ValueChanged(object sender, EventArgs e)
        { calcWndInnerSizes(); }

        private void numericUpDown_BorderLeft_ValueChanged(object sender, EventArgs e)
        { calcWndInnerSizes(); }

        public void calcWndInnerSizes() {
            if (lastSelectedWindowNode < 0 || lastSelectedWindowNode >= ProcList.Length
               || ProcList[lastSelectedWindowNode].HasExited)
                return;

            WndSizes wndSizes = new WndSizes();
            getWndSizes(ProcList[lastSelectedWindowNode].MainWindowHandle, ref wndSizes);
            label_SizeW.Text = (wndSizes.Width - numericUpDown_BorderRight.Value - numericUpDown_BorderLeft.Value).ToString();
            label_SizeH.Text = (wndSizes.Height - numericUpDown_BorderTop.Value - numericUpDown_BorderBot.Value).ToString();
        }

        private void button_HelpInfoRU_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Калибровка размеров Рамки окна:\n\r" +
                "    1) Выбрать нужное окно из списка в левой \n\r" +
                "        части программы.\n\r\n\r" +
                "    2) Задать значения толщины Рамки (Top, Right, ...) \n\r" +
                "        вручную или нажав кнопку 'Calibrate Border Size':\n\r" +
                "           Если нажали кнопку 'Calibrate Border Size', \n\r" +
                "           тогда также нужно ещё:\n\r" +
                "           2.1) Передвинуть курсор в левый верхний угол\n\r" +
                "                 целевого окна (курсор НЕ должен быть\n\r" +
                "                 над частями Рамки, а также при этом НЕ\n\r" +
                "                 нажимайте никакие клавиши/кнопки мыши!)\n\r\n\r" +
                "    3) В программе наблюдайте за 'Size:' параметром.\n\r" +
                "        Его значения должны соотвествовать реальным\n\r" +
                "        размерам области окна (без учёта рамки, конечно).\n\r\n\r" +
                "    4) Примените изменения, нажав на F5, если до этого\n\r" +
                "        вы выполняли калибровку размеров Рамки при помощи\n\r" +
                "        кнопки 'Calibrate Border Size'.\n\r\n\r\n\r" +
                "Вы также можете Сохранить параметры целевого окна\n\r" +
                "нажатием кнопки 'Save Profile'.\n\r" +
                "В этом случае окна данной программы/игры\n\r" +
                "автоматически изменят свой размер при запуске.\n\r" +
                "Задержку автоматического изменения размера новых окон\n\r" +
                "сохраненных профилей можно настроить в поле\n\r" +
                "'Auto Resize delay on start'."
                ,
                "Краткое Руководство");
        }

        private void button_HelpInfoEN_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "How to Calibrate Border sizes of necessery window:\n\r" +
                "    1) Select the neccesery window from \n\r" +
                "        the List on the left programm's side.\n\r\n\r" +
                "    2) Set the window's Border values (Top, Right, ...) \n\r" +
                "        manually or do this by pressing \n\r" +
                "        'Calibrate Border Size' button:\n\r" +
                "           If you pressed the 'Calibrate Border Size' \n\r" +
                "           button, then you need also:\n\r" +
                "           2.1) Move your cursor at the Top-Left\n\r" +
                "                 corner of you necessery window (you should NOT\n\r" +
                "                 hover cursor over border part and not pressing\n\r" +
                "                 keys/mouse buttons!).\n\r\n\r" +
                "    3) Look at the 'Size:' param in programm.\n\r" +
                "        Those values should match with real size of\n\r" +
                "        the window (without border sizes, of course).\n\r\n\r" +
                "    4) Apply changes by pressing F5, if you want to\n\r" +
                "        finish calibrating window's Border sizes by using\n\r" +
                "        'Calibrate Border Size' button.\n\r\n\r\n\r" +
                "You also can Save params for the necessery window by pressing\n\r" +
                "'Save Profile' button.\n\r" +
                "In this case windows of necessery programm/game should\n\r" +
                "automaticaly change their sizes on starting.\n\r" +
                "The automatic resizing delay for new windows\n\r" +
                "of the saved profiles can be configured by\n\r" +
                "setting up the 'Auto Resize delay on start' field."
                ,
                "Info (How to)");
        }

        // Minimize in Tray
        private void Form1_Resize(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Minimized)
            {
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
