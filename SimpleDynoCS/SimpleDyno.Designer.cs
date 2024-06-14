using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SimpleDyno
{
    public partial class SimpleDyno : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            SuspendLayout();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleDyno));
            SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            SelectFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            btnStartLoggingRaw = new Button();
            btnStartLoggingRaw.Click += btnStartLoggingRaw_Click;
            btnResetMaxima = new Button();
            btnResetMaxima.Click += btnResetMaxima_Click;
            btnStartPowerRun = new Button();
            btnStartPowerRun.Click += btnStartPowerRun_Click;
            btnCOM = new Button();
            btnCOM.Click += btnCOM_Click;
            btnDyno = new Button();
            btnDyno.Click += btnDyno_Click;
            btnAnalysis = new Button();
            btnAnalysis.Click += btnAnalysis_Click;
            txtThreshold2 = new TextBox();
            txtThreshold1 = new TextBox();
            btnClose = new Button();
            btnMultiYTime = new Button();
            btnLoad = new Button();
            btnSave = new Button();
            btnNewGauge = new Button();
            btnSaveAs = new Button();
            Label17 = new System.Windows.Forms.Label();
            btnNewLabel = new Button();
            btnHide = new Button();
            btnShow = new Button();
            txtPowerRunThreshold = new TextBox();
            txtZeroTimeDetect = new TextBox();
            lblZeroDetect = new System.Windows.Forms.Label();
            btnStartAcquisition = new Button();
            cmbAcquisition = new ComboBox();
            cmbSampleRate = new ComboBox();
            cmbChannels = new ComboBox();
            cmbBaudRate = new ComboBox();
            cmbCOMPorts = new ComboBox();
            lblCOMActive = new System.Windows.Forms.Label();
            OpenFileDialog1 = new OpenFileDialog();
            lblInterface = new System.Windows.Forms.Label();
            txtInterface = new TextBox();
            chkAdvancedProcessing = new CheckBox();
            cmbBufferSize = new ComboBox();
            btnPerformanceTest = new Button();
            Button1 = new Button();
            Button1.Click += Button1_Click;
            SessionTextBox = new TextBox();
            SessionLabel = new System.Windows.Forms.Label();
            PostfixLabel = new System.Windows.Forms.Label();
            pnlSignalWindow = new DoubleBufferPanel();
            txtPowerrunDir = new TextBox();
            PowerrunDirLabel = new System.Windows.Forms.Label();
            SetDirButton = new Button();
            SuspendLayout();
            // 
            // SaveFileDialog1
            // 
            SaveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            // 
            // btnStartLoggingRaw
            // 
            btnStartLoggingRaw.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnStartLoggingRaw.Location = new System.Drawing.Point(274, 1);
            btnStartLoggingRaw.Name = "btnStartLoggingRaw";
            btnStartLoggingRaw.Size = new System.Drawing.Size(68, 21);
            btnStartLoggingRaw.TabIndex = 42;
            btnStartLoggingRaw.Text = "Log Raw Data";
            // 
            // btnResetMaxima
            // 
            btnResetMaxima.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnResetMaxima.Location = new System.Drawing.Point(138, 1);
            btnResetMaxima.Name = "btnResetMaxima";
            btnResetMaxima.Size = new System.Drawing.Size(68, 21);
            btnResetMaxima.TabIndex = 41;
            btnResetMaxima.Text = "Reset";
            // 
            // btnStartPowerRun
            // 
            btnStartPowerRun.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnStartPowerRun.Location = new System.Drawing.Point(342, 1);
            btnStartPowerRun.Name = "btnStartPowerRun";
            btnStartPowerRun.Size = new System.Drawing.Size(68, 21);
            btnStartPowerRun.TabIndex = 43;
            btnStartPowerRun.Text = "Power Run";
            // 
            // btnCOM
            // 
            btnCOM.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnCOM.Location = new System.Drawing.Point(70, 1);
            btnCOM.Name = "btnCOM";
            btnCOM.Size = new System.Drawing.Size(68, 21);
            btnCOM.TabIndex = 172;
            btnCOM.Text = "COM";
            btnCOM.UseVisualStyleBackColor = true;
            // 
            // btnDyno
            // 
            btnDyno.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnDyno.Location = new System.Drawing.Point(2, 1);
            btnDyno.Name = "btnDyno";
            btnDyno.Size = new System.Drawing.Size(68, 21);
            btnDyno.TabIndex = 170;
            btnDyno.Text = "Dyno";
            btnDyno.UseVisualStyleBackColor = true;
            // 
            // btnAnalysis
            // 
            btnAnalysis.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnAnalysis.Location = new System.Drawing.Point(206, 1);
            btnAnalysis.Name = "btnAnalysis";
            btnAnalysis.Size = new System.Drawing.Size(68, 21);
            btnAnalysis.TabIndex = 171;
            btnAnalysis.Text = "Analysis";
            btnAnalysis.UseVisualStyleBackColor = true;
            // 
            // txtThreshold2
            // 
            txtThreshold2.CausesValidation = false;
            txtThreshold2.Enabled = false;
            txtThreshold2.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtThreshold2.Location = new System.Drawing.Point(429, 67);
            txtThreshold2.Name = "txtThreshold2";
            txtThreshold2.Size = new System.Drawing.Size(23, 21);
            txtThreshold2.TabIndex = 169;
            txtThreshold2.Tag = "";
            txtThreshold2.Text = "113";
            txtThreshold2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtThreshold2.Visible = false;
            // 
            // txtThreshold1
            // 
            txtThreshold1.CausesValidation = false;
            txtThreshold1.Enabled = false;
            txtThreshold1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtThreshold1.Location = new System.Drawing.Point(400, 67);
            txtThreshold1.Name = "txtThreshold1";
            txtThreshold1.Size = new System.Drawing.Size(23, 21);
            txtThreshold1.TabIndex = 168;
            txtThreshold1.Tag = "";
            txtThreshold1.Text = "143";
            txtThreshold1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtThreshold1.Visible = false;
            // 
            // btnClose
            // 
            btnClose.Enabled = false;
            btnClose.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnClose.Location = new System.Drawing.Point(206, 90);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(68, 21);
            btnClose.TabIndex = 86;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // btnMultiYTime
            // 
            btnMultiYTime.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnMultiYTime.Location = new System.Drawing.Point(138, 68);
            btnMultiYTime.Name = "btnMultiYTime";
            btnMultiYTime.Size = new System.Drawing.Size(68, 21);
            btnMultiYTime.TabIndex = 85;
            btnMultiYTime.Text = "Y vs Time";
            btnMultiYTime.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            btnLoad.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnLoad.Location = new System.Drawing.Point(2, 90);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new System.Drawing.Size(68, 21);
            btnLoad.TabIndex = 77;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnSave.Enabled = false;
            btnSave.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnSave.Location = new System.Drawing.Point(70, 90);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(68, 21);
            btnSave.TabIndex = 78;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnNewGauge
            // 
            btnNewGauge.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnNewGauge.Location = new System.Drawing.Point(70, 68);
            btnNewGauge.Name = "btnNewGauge";
            btnNewGauge.Size = new System.Drawing.Size(68, 21);
            btnNewGauge.TabIndex = 83;
            btnNewGauge.Text = "Gauge";
            btnNewGauge.UseVisualStyleBackColor = true;
            // 
            // btnSaveAs
            // 
            btnSaveAs.Enabled = false;
            btnSaveAs.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnSaveAs.Location = new System.Drawing.Point(138, 90);
            btnSaveAs.Name = "btnSaveAs";
            btnSaveAs.Size = new System.Drawing.Size(68, 21);
            btnSaveAs.TabIndex = 79;
            btnSaveAs.Text = "Save As";
            btnSaveAs.UseVisualStyleBackColor = true;
            // 
            // Label17
            // 
            Label17.AutoSize = true;
            Label17.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            Label17.Location = new System.Drawing.Point(344, 27);
            Label17.Name = "Label17";
            Label17.Size = new System.Drawing.Size(66, 13);
            Label17.TabIndex = 58;
            Label17.Text = "Run Start at";
            Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNewLabel
            // 
            btnNewLabel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnNewLabel.Location = new System.Drawing.Point(2, 68);
            btnNewLabel.Name = "btnNewLabel";
            btnNewLabel.Size = new System.Drawing.Size(68, 21);
            btnNewLabel.TabIndex = 82;
            btnNewLabel.Text = "Label";
            btnNewLabel.UseVisualStyleBackColor = true;
            // 
            // btnHide
            // 
            btnHide.Enabled = false;
            btnHide.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnHide.Location = new System.Drawing.Point(274, 90);
            btnHide.Name = "btnHide";
            btnHide.Size = new System.Drawing.Size(68, 21);
            btnHide.TabIndex = 80;
            btnHide.Text = "Hide";
            btnHide.UseVisualStyleBackColor = true;
            // 
            // btnShow
            // 
            btnShow.Enabled = false;
            btnShow.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnShow.Location = new System.Drawing.Point(342, 90);
            btnShow.Name = "btnShow";
            btnShow.Size = new System.Drawing.Size(68, 21);
            btnShow.TabIndex = 81;
            btnShow.Text = "Show";
            btnShow.UseVisualStyleBackColor = true;
            // 
            // txtPowerRunThreshold
            // 
            txtPowerRunThreshold.CausesValidation = false;
            txtPowerRunThreshold.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtPowerRunThreshold.Location = new System.Drawing.Point(412, 24);
            txtPowerRunThreshold.Name = "txtPowerRunThreshold";
            txtPowerRunThreshold.Size = new System.Drawing.Size(67, 21);
            txtPowerRunThreshold.TabIndex = 44;
            txtPowerRunThreshold.Tag = @"1\999999";
            txtPowerRunThreshold.Text = "0";
            txtPowerRunThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtZeroTimeDetect
            // 
            txtZeroTimeDetect.CausesValidation = false;
            txtZeroTimeDetect.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtZeroTimeDetect.Location = new System.Drawing.Point(553, 68);
            txtZeroTimeDetect.Name = "txtZeroTimeDetect";
            txtZeroTimeDetect.Size = new System.Drawing.Size(38, 21);
            txtZeroTimeDetect.TabIndex = 51;
            txtZeroTimeDetect.Tag = @"0.1\2";
            txtZeroTimeDetect.Text = "1";
            txtZeroTimeDetect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblZeroDetect
            // 
            lblZeroDetect.AutoSize = true;
            lblZeroDetect.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            lblZeroDetect.Location = new System.Drawing.Point(487, 72);
            lblZeroDetect.Name = "lblZeroDetect";
            lblZeroDetect.Size = new System.Drawing.Size(64, 13);
            lblZeroDetect.TabIndex = 32;
            lblZeroDetect.Text = "Zero Detect";
            lblZeroDetect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnStartAcquisition
            // 
            btnStartAcquisition.Location = new System.Drawing.Point(486, 90);
            btnStartAcquisition.Name = "btnStartAcquisition";
            btnStartAcquisition.Size = new System.Drawing.Size(105, 21);
            btnStartAcquisition.TabIndex = 163;
            btnStartAcquisition.Text = "Start";
            btnStartAcquisition.UseVisualStyleBackColor = true;
            // 
            // cmbAcquisition
            // 
            cmbAcquisition.FormattingEnabled = true;
            cmbAcquisition.Location = new System.Drawing.Point(487, 2);
            cmbAcquisition.Name = "cmbAcquisition";
            cmbAcquisition.Size = new System.Drawing.Size(171, 21);
            cmbAcquisition.TabIndex = 162;
            // 
            // cmbSampleRate
            // 
            cmbSampleRate.FormattingEnabled = true;
            cmbSampleRate.Location = new System.Drawing.Point(569, 24);
            cmbSampleRate.Name = "cmbSampleRate";
            cmbSampleRate.Size = new System.Drawing.Size(89, 21);
            cmbSampleRate.TabIndex = 161;
            // 
            // cmbChannels
            // 
            cmbChannels.FormattingEnabled = true;
            cmbChannels.Location = new System.Drawing.Point(487, 24);
            cmbChannels.Name = "cmbChannels";
            cmbChannels.Size = new System.Drawing.Size(81, 21);
            cmbChannels.TabIndex = 160;
            // 
            // cmbBaudRate
            // 
            cmbBaudRate.FormattingEnabled = true;
            cmbBaudRate.Location = new System.Drawing.Point(592, 46);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new System.Drawing.Size(66, 21);
            cmbBaudRate.TabIndex = 159;
            // 
            // cmbCOMPorts
            // 
            cmbCOMPorts.DropDownWidth = 300;
            cmbCOMPorts.FormattingEnabled = true;
            cmbCOMPorts.Location = new System.Drawing.Point(487, 46);
            cmbCOMPorts.Name = "cmbCOMPorts";
            cmbCOMPorts.Size = new System.Drawing.Size(104, 21);
            cmbCOMPorts.TabIndex = 158;
            // 
            // lblCOMActive
            // 
            lblCOMActive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblCOMActive.Location = new System.Drawing.Point(592, 91);
            lblCOMActive.Name = "lblCOMActive";
            lblCOMActive.Size = new System.Drawing.Size(66, 19);
            lblCOMActive.TabIndex = 157;
            lblCOMActive.Text = "COM Active";
            lblCOMActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // lblInterface
            // 
            lblInterface.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            lblInterface.Location = new System.Drawing.Point(1, 54);
            lblInterface.Name = "lblInterface";
            lblInterface.Size = new System.Drawing.Size(205, 13);
            lblInterface.TabIndex = 174;
            lblInterface.Text = "Currently using:";
            lblInterface.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtInterface
            // 
            txtInterface.CausesValidation = false;
            txtInterface.Enabled = false;
            txtInterface.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtInterface.Location = new System.Drawing.Point(342, 68);
            txtInterface.Name = "txtInterface";
            txtInterface.Size = new System.Drawing.Size(23, 21);
            txtInterface.TabIndex = 175;
            txtInterface.Tag = "";
            txtInterface.Text = @"C:\SimpleDyno\DefaultView.sdi";
            txtInterface.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtInterface.Visible = false;
            // 
            // chkAdvancedProcessing
            // 
            chkAdvancedProcessing.AutoSize = true;
            chkAdvancedProcessing.Location = new System.Drawing.Point(592, 71);
            chkAdvancedProcessing.Name = "chkAdvancedProcessing";
            chkAdvancedProcessing.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            chkAdvancedProcessing.Size = new System.Drawing.Size(45, 17);
            chkAdvancedProcessing.TabIndex = 182;
            chkAdvancedProcessing.Text = "Adv";
            chkAdvancedProcessing.UseVisualStyleBackColor = true;
            // 
            // cmbBufferSize
            // 
            cmbBufferSize.FormattingEnabled = true;
            cmbBufferSize.Location = new System.Drawing.Point(300, 68);
            cmbBufferSize.Name = "cmbBufferSize";
            cmbBufferSize.Size = new System.Drawing.Size(36, 21);
            cmbBufferSize.TabIndex = 183;
            cmbBufferSize.Visible = false;
            // 
            // btnPerformanceTest
            // 
            btnPerformanceTest.Enabled = false;
            btnPerformanceTest.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            btnPerformanceTest.Location = new System.Drawing.Point(214, 67);
            btnPerformanceTest.Name = "btnPerformanceTest";
            btnPerformanceTest.Size = new System.Drawing.Size(36, 22);
            btnPerformanceTest.TabIndex = 184;
            btnPerformanceTest.Text = "Perf";
            btnPerformanceTest.UseVisualStyleBackColor = true;
            btnPerformanceTest.Visible = false;
            // 
            // Button1
            // 
            Button1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            Button1.Location = new System.Drawing.Point(2, 23);
            Button1.Name = "Button1";
            Button1.Size = new System.Drawing.Size(68, 21);
            Button1.TabIndex = 185;
            Button1.Text = "Correction";
            // 
            // SessionTextBox
            // 
            SessionTextBox.Location = new System.Drawing.Point(155, 23);
            SessionTextBox.Name = "SessionTextBox";
            SessionTextBox.Size = new System.Drawing.Size(119, 21);
            SessionTextBox.TabIndex = 186;
            // 
            // SessionLabel
            // 
            SessionLabel.AutoSize = true;
            SessionLabel.Location = new System.Drawing.Point(77, 27);
            SessionLabel.Name = "SessionLabel";
            SessionLabel.Size = new System.Drawing.Size(72, 13);
            SessionLabel.TabIndex = 187;
            SessionLabel.Text = "Session name";
            // 
            // PostfixLabel
            // 
            PostfixLabel.AutoSize = true;
            PostfixLabel.Location = new System.Drawing.Point(281, 29);
            PostfixLabel.Name = "PostfixLabel";
            PostfixLabel.Size = new System.Drawing.Size(0, 13);
            PostfixLabel.TabIndex = 188;
            // 
            // pnlSignalWindow
            // 
            pnlSignalWindow.BackColor = System.Drawing.SystemColors.Control;
            pnlSignalWindow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            //pnlSignalWindow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlSignalWindow.Location = new System.Drawing.Point(660, 2);
            pnlSignalWindow.Name = "pnlSignalWindow";
            pnlSignalWindow.Size = new System.Drawing.Size(25, 108);
            pnlSignalWindow.TabIndex = 33;
            // 
            // txtPowerrunDir
            // 
            txtPowerrunDir.CausesValidation = false;
            txtPowerrunDir.Enabled = false;
            txtPowerrunDir.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            txtPowerrunDir.Location = new System.Drawing.Point(371, 68);
            txtPowerrunDir.Name = "txtPowerrunDir";
            txtPowerrunDir.Size = new System.Drawing.Size(23, 21);
            txtPowerrunDir.TabIndex = 189;
            txtPowerrunDir.Tag = "";
            txtPowerrunDir.Text = @"C:\SimpleDyno\DefaultView.sdi";
            txtPowerrunDir.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtPowerrunDir.Visible = false;
            // 
            // PowerrunDirLabel
            // 
            PowerrunDirLabel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            PowerrunDirLabel.Location = new System.Drawing.Point(247, 54);
            PowerrunDirLabel.Name = "PowerrunDirLabel";
            PowerrunDirLabel.Size = new System.Drawing.Size(205, 13);
            PowerrunDirLabel.TabIndex = 190;
            PowerrunDirLabel.Text = "Powerrun dir:";
            PowerrunDirLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SetDirButton
            // 
            SetDirButton.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            SetDirButton.Location = new System.Drawing.Point(411, 1);
            SetDirButton.Name = "SetDirButton";
            SetDirButton.Size = new System.Drawing.Size(68, 21);
            SetDirButton.TabIndex = 191;
            SetDirButton.Text = "Set dir";
            SetDirButton.UseVisualStyleBackColor = true;
            // 
            // SimpleDyno
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleBaseSize = new Size(5, 14);
            AutoScroll = true;
            CausesValidation = false;
            ClientSize = new Size(687, 112);
            Controls.Add(SetDirButton);
            Controls.Add(PowerrunDirLabel);
            Controls.Add(txtPowerrunDir);
            Controls.Add(PostfixLabel);
            Controls.Add(SessionLabel);
            Controls.Add(SessionTextBox);
            Controls.Add(txtThreshold1);
            Controls.Add(pnlSignalWindow);
            Controls.Add(txtThreshold2);
            Controls.Add(btnStartAcquisition);
            Controls.Add(lblCOMActive);
            Controls.Add(cmbCOMPorts);
            Controls.Add(cmbBaudRate);
            Controls.Add(cmbChannels);
            Controls.Add(cmbSampleRate);
            Controls.Add(cmbAcquisition);
            Controls.Add(lblZeroDetect);
            Controls.Add(txtZeroTimeDetect);
            Controls.Add(txtInterface);
            Controls.Add(lblInterface);
            Controls.Add(btnCOM);
            Controls.Add(btnDyno);
            Controls.Add(btnAnalysis);
            Controls.Add(btnResetMaxima);
            Controls.Add(btnClose);
            Controls.Add(btnMultiYTime);
            Controls.Add(btnLoad);
            Controls.Add(txtPowerRunThreshold);
            Controls.Add(btnSave);
            Controls.Add(btnShow);
            Controls.Add(btnStartPowerRun);
            Controls.Add(btnStartLoggingRaw);
            Controls.Add(btnNewGauge);
            Controls.Add(btnHide);
            Controls.Add(btnSaveAs);
            Controls.Add(Label17);
            Controls.Add(btnNewLabel);
            Controls.Add(chkAdvancedProcessing);
            Controls.Add(btnPerformanceTest);
            Controls.Add(cmbBufferSize);
            Controls.Add(Button1);
            Font = new Font("Tahoma", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "SimpleDyno";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SimpleDynoCS by Vmx17";
            Activated += SimpleDyno_Activated;
            FormClosed += SimpleDyno_FormClosed;
            Load += SimpleDyno_Load;
            Shown += SimpleDyno_Shown;
            ResumeLayout(false);
            PerformLayout();
        }
        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        internal TextBox txtPowerRunThreshold;
        internal System.Windows.Forms.Label Label17;
        internal OpenFileDialog OpenFileDialog1;
        internal Button btnStartLoggingRaw;
        internal Button btnResetMaxima;
        public Button btnStartPowerRun;
        internal SaveFileDialog SaveFileDialog1;
        internal FolderBrowserDialog SelectFolderDialog;
        internal TextBox txtZeroTimeDetect;
        internal System.Windows.Forms.Label lblZeroDetect;
        internal DoubleBufferPanel pnlSignalWindow;
        internal Button btnClose;
        internal Button btnMultiYTime;
        internal Button btnNewGauge;
        internal Button btnNewLabel;
        internal Button btnShow;
        internal Button btnHide;
        internal Button btnSaveAs;
        internal Button btnSave;
        internal Button btnLoad;
        internal Button btnStartAcquisition;
        internal ComboBox cmbAcquisition;
        internal ComboBox cmbSampleRate;
        internal ComboBox cmbChannels;
        internal ComboBox cmbBaudRate;
        internal ComboBox cmbCOMPorts;
        internal System.Windows.Forms.Label lblCOMActive;
        internal TextBox txtThreshold2;
        internal TextBox txtThreshold1;
        internal Button btnCOM;
        internal Button btnAnalysis;
        internal Button btnDyno;
        internal System.Windows.Forms.Label lblInterface;
        internal TextBox txtInterface;
        internal CheckBox chkAdvancedProcessing;
        internal ComboBox cmbBufferSize;
        internal Button btnPerformanceTest;
        private Button Button1;
        private TextBox SessionTextBox;
        private System.Windows.Forms.Label SessionLabel;
        private System.Windows.Forms.Label PostfixLabel;
        private TextBox txtPowerrunDir;
        private System.Windows.Forms.Label PowerrunDirLabel;
        private Button SetDirButton;
        #endregion
    }
}