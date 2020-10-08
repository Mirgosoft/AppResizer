﻿namespace AppResizer
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label_ProcPath = new System.Windows.Forms.Label();
            this.button_RefreshProcList = new System.Windows.Forms.Button();
            this.button_SetBorder = new System.Windows.Forms.Button();
            this.listBox_Windows = new System.Windows.Forms.ListBox();
            this.label_ProcTitle = new System.Windows.Forms.Label();
            this.button_SaveProfile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_BorderTop = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_BorderRight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_BorderBot = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_BorderLeft = new System.Windows.Forms.NumericUpDown();
            this.label_Size = new System.Windows.Forms.Label();
            this.button_ScalePlus = new System.Windows.Forms.Button();
            this.numericUpDown_Scale = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ScaleMinus = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_ResolutionW = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_ResolutionH = new System.Windows.Forms.NumericUpDown();
            this.label_SizeW = new System.Windows.Forms.Label();
            this.label_SizeH = new System.Windows.Forms.Label();
            this.button_SetCustomSizes = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.picture_BorderVisualizer = new System.Windows.Forms.TextBox();
            this.timer_scan_pixels = new System.Windows.Forms.Timer(this.components);
            this.label_CursorPos = new System.Windows.Forms.Label();
            this.label_SetBorderTips = new System.Windows.Forms.Label();
            this.button_HelpInfoRU = new System.Windows.Forms.Button();
            this.button_HelpInfoEN = new System.Windows.Forms.Button();
            this.numericUpDown_DelayStartResize = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.label_HaveProfile = new System.Windows.Forms.Label();
            this.contextMenuStrip_Tray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.button_RemoveProfile = new System.Windows.Forms.Button();
            this.button_set1050p = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderBot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Scale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ResolutionW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ResolutionH)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DelayStartResize)).BeginInit();
            this.contextMenuStrip_Tray.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_ProcPath
            // 
            this.label_ProcPath.AutoSize = true;
            this.label_ProcPath.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ProcPath.Location = new System.Drawing.Point(313, 44);
            this.label_ProcPath.Name = "label_ProcPath";
            this.label_ProcPath.Size = new System.Drawing.Size(81, 19);
            this.label_ProcPath.TabIndex = 2;
            this.label_ProcPath.Text = "Path:  -";
            this.label_ProcPath.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_RefreshProcList
            // 
            this.button_RefreshProcList.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RefreshProcList.Location = new System.Drawing.Point(108, 5);
            this.button_RefreshProcList.Name = "button_RefreshProcList";
            this.button_RefreshProcList.Size = new System.Drawing.Size(97, 34);
            this.button_RefreshProcList.TabIndex = 1;
            this.button_RefreshProcList.Text = "Refresh";
            this.button_RefreshProcList.UseVisualStyleBackColor = true;
            this.button_RefreshProcList.Click += new System.EventHandler(this.button_RefreshProcList_Click);
            // 
            // button_SetBorder
            // 
            this.button_SetBorder.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SetBorder.Location = new System.Drawing.Point(368, 7);
            this.button_SetBorder.Name = "button_SetBorder";
            this.button_SetBorder.Size = new System.Drawing.Size(116, 48);
            this.button_SetBorder.TabIndex = 3;
            this.button_SetBorder.Text = "Calibrate Border Size";
            this.button_SetBorder.UseVisualStyleBackColor = true;
            this.button_SetBorder.Click += new System.EventHandler(this.button_SetBorder_Click);
            // 
            // listBox_Windows
            // 
            this.listBox_Windows.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_Windows.FormattingEnabled = true;
            this.listBox_Windows.ItemHeight = 18;
            this.listBox_Windows.Items.AddRange(new object[] {
            "Windows list..."});
            this.listBox_Windows.Location = new System.Drawing.Point(8, 44);
            this.listBox_Windows.Name = "listBox_Windows";
            this.listBox_Windows.Size = new System.Drawing.Size(299, 166);
            this.listBox_Windows.TabIndex = 5;
            this.listBox_Windows.SelectedIndexChanged += new System.EventHandler(this.listBox_Windows_SelectedIndexChanged);
            // 
            // label_ProcTitle
            // 
            this.label_ProcTitle.AutoSize = true;
            this.label_ProcTitle.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ProcTitle.Location = new System.Drawing.Point(313, 23);
            this.label_ProcTitle.Name = "label_ProcTitle";
            this.label_ProcTitle.Size = new System.Drawing.Size(81, 19);
            this.label_ProcTitle.TabIndex = 6;
            this.label_ProcTitle.Text = "Title: -";
            this.label_ProcTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_SaveProfile
            // 
            this.button_SaveProfile.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SaveProfile.Location = new System.Drawing.Point(322, 172);
            this.button_SaveProfile.Name = "button_SaveProfile";
            this.button_SaveProfile.Size = new System.Drawing.Size(163, 37);
            this.button_SaveProfile.TabIndex = 7;
            this.button_SaveProfile.Text = "Save Profile";
            this.button_SaveProfile.UseVisualStyleBackColor = true;
            this.button_SaveProfile.Click += new System.EventHandler(this.button_SaveProfile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 19);
            this.label2.TabIndex = 8;
            this.label2.Text = "Top:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(86, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 19);
            this.label3.TabIndex = 9;
            this.label3.Text = "Right:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(189, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 19);
            this.label4.TabIndex = 10;
            this.label4.Text = "Bot:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(273, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 19);
            this.label5.TabIndex = 11;
            this.label5.Text = "Left:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // numericUpDown_BorderTop
            // 
            this.numericUpDown_BorderTop.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_BorderTop.Location = new System.Drawing.Point(43, 17);
            this.numericUpDown_BorderTop.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_BorderTop.Name = "numericUpDown_BorderTop";
            this.numericUpDown_BorderTop.Size = new System.Drawing.Size(38, 26);
            this.numericUpDown_BorderTop.TabIndex = 12;
            this.numericUpDown_BorderTop.ValueChanged += new System.EventHandler(this.numericUpDown_BorderTop_ValueChanged);
            // 
            // numericUpDown_BorderRight
            // 
            this.numericUpDown_BorderRight.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_BorderRight.Location = new System.Drawing.Point(146, 17);
            this.numericUpDown_BorderRight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_BorderRight.Name = "numericUpDown_BorderRight";
            this.numericUpDown_BorderRight.Size = new System.Drawing.Size(38, 26);
            this.numericUpDown_BorderRight.TabIndex = 13;
            this.numericUpDown_BorderRight.ValueChanged += new System.EventHandler(this.numericUpDown_BorderRight_ValueChanged);
            // 
            // numericUpDown_BorderBot
            // 
            this.numericUpDown_BorderBot.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_BorderBot.Location = new System.Drawing.Point(230, 17);
            this.numericUpDown_BorderBot.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_BorderBot.Name = "numericUpDown_BorderBot";
            this.numericUpDown_BorderBot.Size = new System.Drawing.Size(38, 26);
            this.numericUpDown_BorderBot.TabIndex = 14;
            this.numericUpDown_BorderBot.ValueChanged += new System.EventHandler(this.numericUpDown_BorderBot_ValueChanged);
            // 
            // numericUpDown_BorderLeft
            // 
            this.numericUpDown_BorderLeft.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_BorderLeft.Location = new System.Drawing.Point(324, 18);
            this.numericUpDown_BorderLeft.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_BorderLeft.Name = "numericUpDown_BorderLeft";
            this.numericUpDown_BorderLeft.Size = new System.Drawing.Size(38, 26);
            this.numericUpDown_BorderLeft.TabIndex = 15;
            this.numericUpDown_BorderLeft.ValueChanged += new System.EventHandler(this.numericUpDown_BorderLeft_ValueChanged);
            // 
            // label_Size
            // 
            this.label_Size.AutoSize = true;
            this.label_Size.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Size.Location = new System.Drawing.Point(1, 8);
            this.label_Size.Name = "label_Size";
            this.label_Size.Size = new System.Drawing.Size(126, 19);
            this.label_Size.TabIndex = 16;
            this.label_Size.Text = "Size:       x";
            this.label_Size.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_ScalePlus
            // 
            this.button_ScalePlus.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScalePlus.Location = new System.Drawing.Point(228, 35);
            this.button_ScalePlus.Name = "button_ScalePlus";
            this.button_ScalePlus.Size = new System.Drawing.Size(32, 32);
            this.button_ScalePlus.TabIndex = 17;
            this.button_ScalePlus.Text = "+";
            this.button_ScalePlus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_ScalePlus.UseVisualStyleBackColor = true;
            this.button_ScalePlus.Click += new System.EventHandler(this.button_ScalePlus_Click);
            // 
            // numericUpDown_Scale
            // 
            this.numericUpDown_Scale.DecimalPlaces = 2;
            this.numericUpDown_Scale.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_Scale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown_Scale.Location = new System.Drawing.Point(232, 6);
            this.numericUpDown_Scale.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_Scale.Name = "numericUpDown_Scale";
            this.numericUpDown_Scale.Size = new System.Drawing.Size(65, 26);
            this.numericUpDown_Scale.TabIndex = 18;
            this.numericUpDown_Scale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_Scale.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(214, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 19);
            this.label1.TabIndex = 19;
            this.label1.Text = "x";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_ScaleMinus
            // 
            this.button_ScaleMinus.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScaleMinus.Location = new System.Drawing.Point(266, 35);
            this.button_ScaleMinus.Name = "button_ScaleMinus";
            this.button_ScaleMinus.Size = new System.Drawing.Size(32, 32);
            this.button_ScaleMinus.TabIndex = 20;
            this.button_ScaleMinus.Text = "-";
            this.button_ScaleMinus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_ScaleMinus.UseVisualStyleBackColor = true;
            this.button_ScaleMinus.Click += new System.EventHandler(this.button_ScaleMinus_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(162, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 19);
            this.label6.TabIndex = 21;
            this.label6.Text = "Scale";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // numericUpDown_ResolutionW
            // 
            this.numericUpDown_ResolutionW.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_ResolutionW.Location = new System.Drawing.Point(6, 6);
            this.numericUpDown_ResolutionW.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_ResolutionW.Name = "numericUpDown_ResolutionW";
            this.numericUpDown_ResolutionW.Size = new System.Drawing.Size(62, 26);
            this.numericUpDown_ResolutionW.TabIndex = 22;
            this.numericUpDown_ResolutionW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_ResolutionW.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(68, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 19);
            this.label8.TabIndex = 23;
            this.label8.Text = "x";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // numericUpDown_ResolutionH
            // 
            this.numericUpDown_ResolutionH.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_ResolutionH.Location = new System.Drawing.Point(85, 6);
            this.numericUpDown_ResolutionH.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_ResolutionH.Name = "numericUpDown_ResolutionH";
            this.numericUpDown_ResolutionH.Size = new System.Drawing.Size(62, 26);
            this.numericUpDown_ResolutionH.TabIndex = 24;
            this.numericUpDown_ResolutionH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_ResolutionH.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // label_SizeW
            // 
            this.label_SizeW.AutoSize = true;
            this.label_SizeW.BackColor = System.Drawing.Color.Transparent;
            this.label_SizeW.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SizeW.Location = new System.Drawing.Point(69, 8);
            this.label_SizeW.Name = "label_SizeW";
            this.label_SizeW.Size = new System.Drawing.Size(45, 19);
            this.label_SizeW.TabIndex = 25;
            this.label_SizeW.Text = "9999";
            this.label_SizeW.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label_SizeH
            // 
            this.label_SizeH.AutoSize = true;
            this.label_SizeH.BackColor = System.Drawing.Color.Transparent;
            this.label_SizeH.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SizeH.Location = new System.Drawing.Point(122, 8);
            this.label_SizeH.Name = "label_SizeH";
            this.label_SizeH.Size = new System.Drawing.Size(45, 19);
            this.label_SizeH.TabIndex = 26;
            this.label_SizeH.Text = "9999";
            // 
            // button_SetCustomSizes
            // 
            this.button_SetCustomSizes.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SetCustomSizes.Location = new System.Drawing.Point(24, 35);
            this.button_SetCustomSizes.Name = "button_SetCustomSizes";
            this.button_SetCustomSizes.Size = new System.Drawing.Size(105, 27);
            this.button_SetCustomSizes.TabIndex = 27;
            this.button_SetCustomSizes.Text = "Set Sizes";
            this.button_SetCustomSizes.UseVisualStyleBackColor = true;
            this.button_SetCustomSizes.Click += new System.EventHandler(this.button_SetCustomSizes_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.panel1.Controls.Add(this.numericUpDown_BorderLeft);
            this.panel1.Controls.Add(this.numericUpDown_BorderBot);
            this.panel1.Controls.Add(this.numericUpDown_BorderRight);
            this.panel1.Controls.Add(this.numericUpDown_BorderTop);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.button_SetBorder);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(311, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(488, 59);
            this.panel1.TabIndex = 28;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(255)))), ((int)(((byte)(244)))));
            this.panel2.Controls.Add(this.label_SizeW);
            this.panel2.Controls.Add(this.label_Size);
            this.panel2.Controls.Add(this.label_SizeH);
            this.panel2.Location = new System.Drawing.Point(311, 134);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(195, 36);
            this.panel2.TabIndex = 29;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(255)))), ((int)(((byte)(244)))));
            this.panel3.Controls.Add(this.button_set1050p);
            this.panel3.Controls.Add(this.numericUpDown_ResolutionH);
            this.panel3.Controls.Add(this.numericUpDown_ResolutionW);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.numericUpDown_Scale);
            this.panel3.Controls.Add(this.button_SetCustomSizes);
            this.panel3.Controls.Add(this.button_ScaleMinus);
            this.panel3.Controls.Add(this.button_ScalePlus);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(496, 134);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(303, 72);
            this.panel3.TabIndex = 30;
            // 
            // picture_BorderVisualizer
            // 
            this.picture_BorderVisualizer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.picture_BorderVisualizer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.picture_BorderVisualizer.Location = new System.Drawing.Point(151, 44);
            this.picture_BorderVisualizer.Multiline = true;
            this.picture_BorderVisualizer.Name = "picture_BorderVisualizer";
            this.picture_BorderVisualizer.Size = new System.Drawing.Size(160, 160);
            this.picture_BorderVisualizer.TabIndex = 28;
            this.picture_BorderVisualizer.Visible = false;
            // 
            // timer_scan_pixels
            // 
            this.timer_scan_pixels.Interval = 50;
            this.timer_scan_pixels.Tick += new System.EventHandler(this.timer_scan_pixels_Tick);
            // 
            // label_CursorPos
            // 
            this.label_CursorPos.AutoSize = true;
            this.label_CursorPos.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_CursorPos.Location = new System.Drawing.Point(453, 53);
            this.label_CursorPos.Name = "label_CursorPos";
            this.label_CursorPos.Size = new System.Drawing.Size(108, 19);
            this.label_CursorPos.TabIndex = 31;
            this.label_CursorPos.Text = "Cursor pos:";
            this.label_CursorPos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label_CursorPos.Visible = false;
            // 
            // label_SetBorderTips
            // 
            this.label_SetBorderTips.AutoSize = true;
            this.label_SetBorderTips.BackColor = System.Drawing.SystemColors.Control;
            this.label_SetBorderTips.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SetBorderTips.ForeColor = System.Drawing.Color.Red;
            this.label_SetBorderTips.Location = new System.Drawing.Point(678, 50);
            this.label_SetBorderTips.Name = "label_SetBorderTips";
            this.label_SetBorderTips.Size = new System.Drawing.Size(120, 22);
            this.label_SetBorderTips.TabIndex = 32;
            this.label_SetBorderTips.Text = "F5 to Apply";
            this.label_SetBorderTips.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label_SetBorderTips.Visible = false;
            // 
            // button_HelpInfoRU
            // 
            this.button_HelpInfoRU.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_HelpInfoRU.Location = new System.Drawing.Point(7, 22);
            this.button_HelpInfoRU.Name = "button_HelpInfoRU";
            this.button_HelpInfoRU.Size = new System.Drawing.Size(46, 20);
            this.button_HelpInfoRU.TabIndex = 33;
            this.button_HelpInfoRU.Text = "? RU";
            this.button_HelpInfoRU.UseVisualStyleBackColor = true;
            this.button_HelpInfoRU.Click += new System.EventHandler(this.button_HelpInfoRU_Click);
            // 
            // button_HelpInfoEN
            // 
            this.button_HelpInfoEN.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_HelpInfoEN.Location = new System.Drawing.Point(7, 3);
            this.button_HelpInfoEN.Name = "button_HelpInfoEN";
            this.button_HelpInfoEN.Size = new System.Drawing.Size(46, 20);
            this.button_HelpInfoEN.TabIndex = 34;
            this.button_HelpInfoEN.Text = "? EN";
            this.button_HelpInfoEN.UseVisualStyleBackColor = true;
            this.button_HelpInfoEN.Click += new System.EventHandler(this.button_HelpInfoEN_Click);
            // 
            // numericUpDown_DelayStartResize
            // 
            this.numericUpDown_DelayStartResize.DecimalPlaces = 1;
            this.numericUpDown_DelayStartResize.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_DelayStartResize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_DelayStartResize.Location = new System.Drawing.Point(693, 3);
            this.numericUpDown_DelayStartResize.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_DelayStartResize.Name = "numericUpDown_DelayStartResize";
            this.numericUpDown_DelayStartResize.Size = new System.Drawing.Size(68, 26);
            this.numericUpDown_DelayStartResize.TabIndex = 16;
            this.numericUpDown_DelayStartResize.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(762, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 19);
            this.label7.TabIndex = 35;
            this.label7.Text = "sec";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(450, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(243, 19);
            this.label9.TabIndex = 36;
            this.label9.Text = "Auto Resize delay on start";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipTitle = "AppResizer";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Show Application Resizer";
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // label_HaveProfile
            // 
            this.label_HaveProfile.AutoSize = true;
            this.label_HaveProfile.BackColor = System.Drawing.Color.Yellow;
            this.label_HaveProfile.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_HaveProfile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(22)))));
            this.label_HaveProfile.Location = new System.Drawing.Point(328, 161);
            this.label_HaveProfile.Name = "label_HaveProfile";
            this.label_HaveProfile.Size = new System.Drawing.Size(135, 19);
            this.label_HaveProfile.TabIndex = 37;
            this.label_HaveProfile.Text = "* Have Profile";
            this.label_HaveProfile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label_HaveProfile.Visible = false;
            // 
            // contextMenuStrip_Tray
            // 
            this.contextMenuStrip_Tray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Exit});
            this.contextMenuStrip_Tray.Name = "contextMenuStrip_Tray";
            this.contextMenuStrip_Tray.Size = new System.Drawing.Size(94, 26);
            // 
            // toolStripMenuItem_Exit
            // 
            this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
            this.toolStripMenuItem_Exit.Size = new System.Drawing.Size(93, 22);
            this.toolStripMenuItem_Exit.Text = "Exit";
            this.toolStripMenuItem_Exit.Click += new System.EventHandler(this.toolStripMenuItem_Exit_Click);
            // 
            // button_RemoveProfile
            // 
            this.button_RemoveProfile.BackColor = System.Drawing.Color.Red;
            this.button_RemoveProfile.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_RemoveProfile.Location = new System.Drawing.Point(461, 158);
            this.button_RemoveProfile.Name = "button_RemoveProfile";
            this.button_RemoveProfile.Size = new System.Drawing.Size(20, 24);
            this.button_RemoveProfile.TabIndex = 38;
            this.button_RemoveProfile.Text = "X";
            this.button_RemoveProfile.UseVisualStyleBackColor = false;
            this.button_RemoveProfile.Visible = false;
            this.button_RemoveProfile.Click += new System.EventHandler(this.button_RemoveProfile_Click);
            // 
            // button_set1050p
            // 
            this.button_set1050p.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_set1050p.Location = new System.Drawing.Point(153, 37);
            this.button_set1050p.Name = "button_set1050p";
            this.button_set1050p.Size = new System.Drawing.Size(63, 27);
            this.button_set1050p.TabIndex = 28;
            this.button_set1050p.Text = "1050p";
            this.button_set1050p.UseVisualStyleBackColor = true;
            this.button_set1050p.Click += new System.EventHandler(this.button_set1050p_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 217);
            this.Controls.Add(this.button_RemoveProfile);
            this.Controls.Add(this.label_HaveProfile);
            this.Controls.Add(this.label_CursorPos);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDown_DelayStartResize);
            this.Controls.Add(this.label_SetBorderTips);
            this.Controls.Add(this.label_ProcPath);
            this.Controls.Add(this.label_ProcTitle);
            this.Controls.Add(this.button_HelpInfoEN);
            this.Controls.Add(this.button_HelpInfoRU);
            this.Controls.Add(this.picture_BorderVisualizer);
            this.Controls.Add(this.listBox_Windows);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_SaveProfile);
            this.Controls.Add(this.button_RefreshProcList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Application Resizer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderBot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BorderLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Scale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ResolutionW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ResolutionH)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DelayStartResize)).EndInit();
            this.contextMenuStrip_Tray.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_ProcPath;
        private System.Windows.Forms.Button button_RefreshProcList;
        private System.Windows.Forms.Button button_SetBorder;
        private System.Windows.Forms.ListBox listBox_Windows;
        private System.Windows.Forms.Label label_ProcTitle;
        private System.Windows.Forms.Button button_SaveProfile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown_BorderTop;
        private System.Windows.Forms.NumericUpDown numericUpDown_BorderRight;
        private System.Windows.Forms.NumericUpDown numericUpDown_BorderBot;
        private System.Windows.Forms.NumericUpDown numericUpDown_BorderLeft;
        private System.Windows.Forms.Label label_Size;
        private System.Windows.Forms.Button button_ScalePlus;
        private System.Windows.Forms.NumericUpDown numericUpDown_Scale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ScaleMinus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_ResolutionW;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown_ResolutionH;
        private System.Windows.Forms.Label label_SizeW;
        private System.Windows.Forms.Label label_SizeH;
        private System.Windows.Forms.Button button_SetCustomSizes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox picture_BorderVisualizer;
        private System.Windows.Forms.Timer timer_scan_pixels;
        private System.Windows.Forms.Label label_CursorPos;
        private System.Windows.Forms.Label label_SetBorderTips;
        private System.Windows.Forms.Button button_HelpInfoRU;
        private System.Windows.Forms.Button button_HelpInfoEN;
        private System.Windows.Forms.NumericUpDown numericUpDown_DelayStartResize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label label_HaveProfile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Tray;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
        private System.Windows.Forms.Button button_RemoveProfile;
        private System.Windows.Forms.Button button_set1050p;
    }
}

