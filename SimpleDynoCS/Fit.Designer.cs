using System.Diagnostics;
using System.Resources;

namespace SimpleDyno
{
    partial class Fit
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Fit));
            cmbWhichFit = new ComboBox();
            cmbWhichFit.SelectedIndexChanged += new EventHandler(cmbWhichFit_SelectedIndexChanged);
            Label1 = new Label();
            txtFitStart = new TextBox();
            scrlStartFit = new VScrollBar();
            scrlStartFit.Scroll += new ScrollEventHandler(scrlStartFit_Scroll);
            Label7 = new Label();
            lblPowerRunSpikeLevel = new Label();
            txtPowerRunSpikeLevel = new TextBox();
            txtPowerRunSpikeLevel.Leave += new EventHandler(txtPowerRunSpikeLevel_Leave);
            txtVoltageSmooth = new TextBox();
            scrlVoltageSmooth = new VScrollBar();
            scrlVoltageSmooth.Scroll += new ScrollEventHandler(scrlVoltageSmooth_Scroll);
            txtCurrentSmooth = new TextBox();
            scrlCurrentSmooth = new VScrollBar();
            scrlCurrentSmooth.Scroll += new ScrollEventHandler(scrlCurrentSmooth_Scroll);
            rdoVoltage = new RadioButton();
            rdoVoltage.CheckedChanged += new EventHandler(rdoVoltage_CheckedChanged);
            rdoCurrent = new RadioButton();
            rdoCurrent.CheckedChanged += new EventHandler(rdoCurrent_CheckedChanged);
            rdoRPM1 = new RadioButton();
            rdoRPM1.CheckedChanged += new EventHandler(rdoRPM1_CheckedChanged);
            btnAddAnalysis = new Button();
            btnAddAnalysis.Click += new EventHandler(btnDone_Click);
            chkAddOrNew = new CheckBox();
            prgFit = new ProgressBar();
            lblProgress = new Label();
            cmbWhichRDFit = new ComboBox();
            cmbWhichRDFit.SelectedIndexChanged += new EventHandler(cmbWhichRDFit_SelectedIndexChanged);
            rdoRunDown = new RadioButton();
            rdoRunDown.CheckedChanged += new EventHandler(rdoRunDown_CheckedChanged);
            lblUsingRunDownFile = new Label();
            txtRPM1Smooth = new TextBox();
            scrlRPM1Smooth = new VScrollBar();
            scrlRPM1Smooth.Scroll += new ScrollEventHandler(scrlRPM1Smooth_Scroll);
            Label2 = new Label();
            btnStopFitting = new Button();
            btnStopFitting.Click += new EventHandler(Button1_Click);
            Label3 = new Label();
            txtCoastDownSmooth = new TextBox();
            scrlCoastDownSmooth = new VScrollBar();
            scrlCoastDownSmooth.Scroll += new ScrollEventHandler(scrlCoastDownSmooth_Scroll);
            pnlDataWindow = new DoubleBufferPanel();
            pnlDataWindow.MouseMove += new MouseEventHandler(pnlDataWindow_MouseMove);
            SuspendLayout();
            // 
            // cmbWhichFit
            // 
            cmbWhichFit.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbWhichFit.FormattingEnabled = true;
            cmbWhichFit.Location = new Point(95, 10);
            cmbWhichFit.Name = "cmbWhichFit";
            cmbWhichFit.Size = new Size(105, 21);
            cmbWhichFit.TabIndex = 74;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label1.Location = new Point(27, 89);
            Label1.Name = "Label1";
            Label1.Size = new Size(86, 13);
            Label1.TabIndex = 2;
            Label1.Text = "Start Fit @ Point";
            Label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtFitStart
            // 
            txtFitStart.BackColor = Color.White;
            txtFitStart.CausesValidation = false;
            txtFitStart.Enabled = false;
            txtFitStart.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFitStart.Location = new Point(119, 86);
            txtFitStart.Name = "txtFitStart";
            txtFitStart.ReadOnly = true;
            txtFitStart.Size = new Size(46, 21);
            txtFitStart.TabIndex = 3;
            txtFitStart.Tag = "";
            txtFitStart.Text = "1";
            txtFitStart.TextAlign = HorizontalAlignment.Center;
            // 
            // scrlStartFit
            // 
            scrlStartFit.LargeChange = 1;
            scrlStartFit.Location = new Point(166, 86);
            scrlStartFit.Maximum = 9;
            scrlStartFit.Name = "scrlStartFit";
            scrlStartFit.Size = new Size(34, 21);
            scrlStartFit.TabIndex = 4;
            scrlStartFit.Value = 9;
            // 
            // Label7
            // 
            Label7.AutoSize = true;
            Label7.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label7.Location = new Point(31, 64);
            Label7.Name = "Label7";
            Label7.Size = new Size(82, 13);
            Label7.TabIndex = 74;
            Label7.Text = "Spike Threshold";
            Label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblPowerRunSpikeLevel
            // 
            lblPowerRunSpikeLevel.AutoSize = true;
            lblPowerRunSpikeLevel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPowerRunSpikeLevel.Location = new Point(166, 64);
            lblPowerRunSpikeLevel.Name = "lblPowerRunSpikeLevel";
            lblPowerRunSpikeLevel.Size = new Size(34, 13);
            lblPowerRunSpikeLevel.TabIndex = 73;
            lblPowerRunSpikeLevel.Text = "RPM1";
            lblPowerRunSpikeLevel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtPowerRunSpikeLevel
            // 
            txtPowerRunSpikeLevel.CausesValidation = false;
            txtPowerRunSpikeLevel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPowerRunSpikeLevel.Location = new Point(119, 61);
            txtPowerRunSpikeLevel.Name = "txtPowerRunSpikeLevel";
            txtPowerRunSpikeLevel.Size = new Size(46, 21);
            txtPowerRunSpikeLevel.TabIndex = 72;
            txtPowerRunSpikeLevel.Tag = @"1\999999";
            txtPowerRunSpikeLevel.Text = "10000";
            txtPowerRunSpikeLevel.TextAlign = HorizontalAlignment.Center;
            // 
            // txtVoltageSmooth
            // 
            txtVoltageSmooth.BackColor = Color.White;
            txtVoltageSmooth.CausesValidation = false;
            txtVoltageSmooth.Enabled = false;
            txtVoltageSmooth.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtVoltageSmooth.Location = new Point(119, 245);
            txtVoltageSmooth.Name = "txtVoltageSmooth";
            txtVoltageSmooth.ReadOnly = true;
            txtVoltageSmooth.Size = new Size(46, 21);
            txtVoltageSmooth.TabIndex = 77;
            txtVoltageSmooth.Tag = "";
            txtVoltageSmooth.Text = "0.5";
            txtVoltageSmooth.TextAlign = HorizontalAlignment.Center;
            // 
            // scrlVoltageSmooth
            // 
            scrlVoltageSmooth.Enabled = false;
            scrlVoltageSmooth.LargeChange = 1;
            scrlVoltageSmooth.Location = new Point(166, 245);
            scrlVoltageSmooth.Maximum = 19;
            scrlVoltageSmooth.Name = "scrlVoltageSmooth";
            scrlVoltageSmooth.Size = new Size(34, 22);
            scrlVoltageSmooth.TabIndex = 78;
            scrlVoltageSmooth.Value = 19;
            // 
            // txtCurrentSmooth
            // 
            txtCurrentSmooth.BackColor = Color.White;
            txtCurrentSmooth.CausesValidation = false;
            txtCurrentSmooth.Enabled = false;
            txtCurrentSmooth.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCurrentSmooth.Location = new Point(119, 211);
            txtCurrentSmooth.Name = "txtCurrentSmooth";
            txtCurrentSmooth.ReadOnly = true;
            txtCurrentSmooth.Size = new Size(46, 21);
            txtCurrentSmooth.TabIndex = 75;
            txtCurrentSmooth.Tag = "";
            txtCurrentSmooth.Text = "0.5";
            txtCurrentSmooth.TextAlign = HorizontalAlignment.Center;
            // 
            // scrlCurrentSmooth
            // 
            scrlCurrentSmooth.Enabled = false;
            scrlCurrentSmooth.LargeChange = 1;
            scrlCurrentSmooth.Location = new Point(166, 211);
            scrlCurrentSmooth.Maximum = 19;
            scrlCurrentSmooth.Name = "scrlCurrentSmooth";
            scrlCurrentSmooth.Size = new Size(34, 22);
            scrlCurrentSmooth.TabIndex = 76;
            scrlCurrentSmooth.Value = 19;
            // 
            // rdoVoltage
            // 
            rdoVoltage.AutoSize = true;
            rdoVoltage.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            rdoVoltage.Location = new Point(52, 246);
            rdoVoltage.Name = "rdoVoltage";
            rdoVoltage.Size = new Size(61, 17);
            rdoVoltage.TabIndex = 8;
            rdoVoltage.Text = "Voltage";
            rdoVoltage.UseVisualStyleBackColor = true;
            // 
            // rdoCurrent
            // 
            rdoCurrent.AutoSize = true;
            rdoCurrent.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            rdoCurrent.Location = new Point(51, 212);
            rdoCurrent.Name = "rdoCurrent";
            rdoCurrent.Size = new Size(62, 17);
            rdoCurrent.TabIndex = 5;
            rdoCurrent.Text = "Current";
            rdoCurrent.UseVisualStyleBackColor = true;
            // 
            // rdoRPM1
            // 
            rdoRPM1.AutoSize = true;
            rdoRPM1.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            rdoRPM1.Location = new Point(12, 11);
            rdoRPM1.Name = "rdoRPM1";
            rdoRPM1.Size = new Size(52, 17);
            rdoRPM1.TabIndex = 0;
            rdoRPM1.Text = "RPM1";
            rdoRPM1.UseVisualStyleBackColor = true;
            // 
            // btnAddAnalysis
            // 
            btnAddAnalysis.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAddAnalysis.Location = new Point(2, 349);
            btnAddAnalysis.Name = "btnAddAnalysis";
            btnAddAnalysis.Size = new Size(192, 21);
            btnAddAnalysis.TabIndex = 78;
            btnAddAnalysis.Text = "Go to Analysis";
            btnAddAnalysis.UseVisualStyleBackColor = true;
            // 
            // chkAddOrNew
            // 
            chkAddOrNew.AutoSize = true;
            chkAddOrNew.Location = new Point(6, 377);
            chkAddOrNew.Name = "chkAddOrNew";
            chkAddOrNew.Size = new Size(119, 17);
            chkAddOrNew.TabIndex = 79;
            chkAddOrNew.Text = "Add to existing data";
            chkAddOrNew.UseVisualStyleBackColor = true;
            // 
            // prgFit
            // 
            prgFit.Location = new Point(2, 300);
            prgFit.Maximum = 1250;
            prgFit.Name = "prgFit";
            prgFit.Size = new Size(192, 16);
            prgFit.Step = 1;
            prgFit.TabIndex = 80;
            // 
            // lblProgress
            // 
            lblProgress.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblProgress.Location = new Point(3, 282);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(166, 15);
            lblProgress.TabIndex = 81;
            lblProgress.Text = "Progress";
            lblProgress.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbWhichRDFit
            // 
            cmbWhichRDFit.Enabled = false;
            cmbWhichRDFit.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbWhichRDFit.FormattingEnabled = true;
            cmbWhichRDFit.Location = new Point(95, 125);
            cmbWhichRDFit.Name = "cmbWhichRDFit";
            cmbWhichRDFit.Size = new Size(105, 21);
            cmbWhichRDFit.TabIndex = 83;
            // 
            // rdoRunDown
            // 
            rdoRunDown.AutoSize = true;
            rdoRunDown.Enabled = false;
            rdoRunDown.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            rdoRunDown.Location = new Point(12, 126);
            rdoRunDown.Name = "rdoRunDown";
            rdoRunDown.Size = new Size(83, 17);
            rdoRunDown.TabIndex = 82;
            rdoRunDown.Text = "Coast Down";
            rdoRunDown.UseVisualStyleBackColor = true;
            // 
            // lblUsingRunDownFile
            // 
            lblUsingRunDownFile.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsingRunDownFile.Location = new Point(6, 176);
            lblUsingRunDownFile.Name = "lblUsingRunDownFile";
            lblUsingRunDownFile.Size = new Size(166, 15);
            lblUsingRunDownFile.TabIndex = 84;
            lblUsingRunDownFile.Text = "Coast Down File";
            lblUsingRunDownFile.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtRPM1Smooth
            // 
            txtRPM1Smooth.BackColor = Color.White;
            txtRPM1Smooth.CausesValidation = false;
            txtRPM1Smooth.Enabled = false;
            txtRPM1Smooth.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtRPM1Smooth.Location = new Point(119, 34);
            txtRPM1Smooth.Name = "txtRPM1Smooth";
            txtRPM1Smooth.ReadOnly = true;
            txtRPM1Smooth.Size = new Size(46, 21);
            txtRPM1Smooth.TabIndex = 85;
            txtRPM1Smooth.Tag = "";
            txtRPM1Smooth.Text = "0.5";
            txtRPM1Smooth.TextAlign = HorizontalAlignment.Center;
            // 
            // scrlRPM1Smooth
            // 
            scrlRPM1Smooth.LargeChange = 1;
            scrlRPM1Smooth.Location = new Point(166, 34);
            scrlRPM1Smooth.Maximum = 39;
            scrlRPM1Smooth.Name = "scrlRPM1Smooth";
            scrlRPM1Smooth.Size = new Size(34, 22);
            scrlRPM1Smooth.TabIndex = 86;
            scrlRPM1Smooth.Value = 39;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(42, 37);
            Label2.Name = "Label2";
            Label2.Size = new Size(71, 13);
            Label2.TabIndex = 87;
            Label2.Text = "Smooth Level";
            Label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnStopFitting
            // 
            btnStopFitting.Location = new Point(2, 322);
            btnStopFitting.Name = "btnStopFitting";
            btnStopFitting.Size = new Size(192, 21);
            btnStopFitting.TabIndex = 88;
            btnStopFitting.Text = "Stop Fitting";
            btnStopFitting.UseVisualStyleBackColor = true;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label3.Location = new Point(42, 152);
            Label3.Name = "Label3";
            Label3.Size = new Size(71, 13);
            Label3.TabIndex = 91;
            Label3.Text = "Smooth Level";
            Label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCoastDownSmooth
            // 
            txtCoastDownSmooth.BackColor = Color.White;
            txtCoastDownSmooth.CausesValidation = false;
            txtCoastDownSmooth.Enabled = false;
            txtCoastDownSmooth.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCoastDownSmooth.Location = new Point(119, 149);
            txtCoastDownSmooth.Name = "txtCoastDownSmooth";
            txtCoastDownSmooth.ReadOnly = true;
            txtCoastDownSmooth.Size = new Size(46, 21);
            txtCoastDownSmooth.TabIndex = 89;
            txtCoastDownSmooth.Tag = "";
            txtCoastDownSmooth.Text = "0.5";
            txtCoastDownSmooth.TextAlign = HorizontalAlignment.Center;
            // 
            // scrlCoastDownSmooth
            // 
            scrlCoastDownSmooth.Enabled = false;
            scrlCoastDownSmooth.LargeChange = 1;
            scrlCoastDownSmooth.Location = new Point(166, 149);
            scrlCoastDownSmooth.Maximum = 39;
            scrlCoastDownSmooth.Name = "scrlCoastDownSmooth";
            scrlCoastDownSmooth.Size = new Size(34, 22);
            scrlCoastDownSmooth.TabIndex = 90;
            scrlCoastDownSmooth.Value = 39;
            // 
            // pnlDataWindow
            // 
            pnlDataWindow.Location = new Point(206, 10);
            pnlDataWindow.Name = "pnlDataWindow";
            pnlDataWindow.Size = new Size(358, 237);
            pnlDataWindow.TabIndex = 76;
            // 
            // Fit
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(944, 555);
            ControlBox = false;
            Controls.Add(Label3);
            Controls.Add(txtCoastDownSmooth);
            Controls.Add(scrlCoastDownSmooth);
            Controls.Add(btnStopFitting);
            Controls.Add(Label2);
            Controls.Add(txtRPM1Smooth);
            Controls.Add(scrlRPM1Smooth);
            Controls.Add(lblUsingRunDownFile);
            Controls.Add(cmbWhichRDFit);
            Controls.Add(rdoRunDown);
            Controls.Add(lblProgress);
            Controls.Add(prgFit);
            Controls.Add(chkAddOrNew);
            Controls.Add(txtVoltageSmooth);
            Controls.Add(btnAddAnalysis);
            Controls.Add(scrlVoltageSmooth);
            Controls.Add(txtCurrentSmooth);
            Controls.Add(pnlDataWindow);
            Controls.Add(scrlCurrentSmooth);
            Controls.Add(cmbWhichFit);
            Controls.Add(rdoRPM1);
            Controls.Add(Label1);
            Controls.Add(rdoCurrent);
            Controls.Add(Label7);
            Controls.Add(txtPowerRunSpikeLevel);
            Controls.Add(txtFitStart);
            Controls.Add(lblPowerRunSpikeLevel);
            Controls.Add(scrlStartFit);
            Controls.Add(rdoVoltage);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Fit";
            StartPosition = FormStartPosition.Manual;
            Text = "Power Run Curve Fitting";
            FormClosing += new FormClosingEventHandler(Fit_FormClosing);
            MouseMove += new MouseEventHandler(Fit_MouseMove);
            ResumeLayout(false);
            PerformLayout();
        }

        internal ComboBox cmbWhichFit;
        internal Label Label1;
        internal TextBox txtFitStart;
        internal VScrollBar scrlStartFit;
        internal Label Label7;
        internal Label lblPowerRunSpikeLevel;
        internal TextBox txtPowerRunSpikeLevel;
        internal RadioButton rdoVoltage;
        internal RadioButton rdoCurrent;
        internal RadioButton rdoRPM1;
        internal DoubleBufferPanel pnlDataWindow;
        internal TextBox txtVoltageSmooth;
        internal VScrollBar scrlVoltageSmooth;
        internal TextBox txtCurrentSmooth;
        internal VScrollBar scrlCurrentSmooth;
        internal Button btnAddAnalysis;
        internal CheckBox chkAddOrNew;
        internal ProgressBar prgFit;
        internal Label lblProgress;
        internal ComboBox cmbWhichRDFit;
        internal RadioButton rdoRunDown;
        internal Label lblUsingRunDownFile;
        internal TextBox txtRPM1Smooth;
        internal VScrollBar scrlRPM1Smooth;
        internal Label Label2;
        internal Button btnStopFitting;
        internal Label Label3;
        internal TextBox txtCoastDownSmooth;
        internal VScrollBar scrlCoastDownSmooth;
        #endregion
    }
}