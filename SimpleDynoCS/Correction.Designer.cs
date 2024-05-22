using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleDyno
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    partial class Correction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        [DebuggerNonUserCode()]
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Correction));
            chkUseRunDown = new CheckBox();
            chkUseRunDown.CheckedChanged += new EventHandler(chkUseRunDown_CheckedChanged);
            grpRunDown = new GroupBox();
            rdoRollerAndWheel = new RadioButton();
            chkUseCoastDownFile = new CheckBox();
            chkUseCoastDownFile.CheckedChanged += new EventHandler(chkUseCoastDownFile_CheckedChanged);
            lblCoastDownFile = new Label();
            btnLoadCoastDown = new Button();
            btnLoadCoastDown.Click += new EventHandler(btnLoadCoastDown_Click);
            rdoRollerAndDrivetrain = new RadioButton();
            rdoFreeRoller = new RadioButton();
            GroupBox1 = new GroupBox();
            RadioButton1 = new RadioButton();
            RadioButton2 = new RadioButton();
            RadioButton3 = new RadioButton();
            CheckBox1 = new CheckBox();
            grpRunDown.SuspendLayout();
            GroupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // chkUseRunDown
            // 
            chkUseRunDown.AutoSize = true;
            chkUseRunDown.Location = new Point(15, 12);
            chkUseRunDown.Name = "chkUseRunDown";
            chkUseRunDown.Size = new Size(106, 17);
            chkUseRunDown.TabIndex = 0;
            chkUseRunDown.Text = "Use Coast Down";
            chkUseRunDown.UseVisualStyleBackColor = true;
            // 
            // grpRunDown
            // 
            grpRunDown.Controls.Add(rdoRollerAndWheel);
            grpRunDown.Controls.Add(chkUseCoastDownFile);
            grpRunDown.Controls.Add(lblCoastDownFile);
            grpRunDown.Controls.Add(btnLoadCoastDown);
            grpRunDown.Controls.Add(rdoRollerAndDrivetrain);
            grpRunDown.Controls.Add(rdoFreeRoller);
            grpRunDown.Enabled = false;
            grpRunDown.Location = new Point(12, 35);
            grpRunDown.Name = "grpRunDown";
            grpRunDown.Size = new Size(200, 153);
            grpRunDown.TabIndex = 1;
            grpRunDown.TabStop = false;
            grpRunDown.Text = "Coast Down Options";
            // 
            // rdoRollerAndWheel
            // 
            rdoRollerAndWheel.AutoSize = true;
            rdoRollerAndWheel.Location = new Point(13, 40);
            rdoRollerAndWheel.Name = "rdoRollerAndWheel";
            rdoRollerAndWheel.Size = new Size(95, 17);
            rdoRollerAndWheel.TabIndex = 5;
            rdoRollerAndWheel.Text = "Roller + Wheel";
            rdoRollerAndWheel.UseVisualStyleBackColor = true;
            // 
            // chkUseCoastDownFile
            // 
            chkUseCoastDownFile.AutoSize = true;
            chkUseCoastDownFile.Location = new Point(13, 95);
            chkUseCoastDownFile.Name = "chkUseCoastDownFile";
            chkUseCoastDownFile.Size = new Size(95, 17);
            chkUseCoastDownFile.TabIndex = 4;
            chkUseCoastDownFile.Text = "Use saved run";
            chkUseCoastDownFile.UseVisualStyleBackColor = true;
            // 
            // lblCoastDownFile
            // 
            lblCoastDownFile.AutoSize = true;
            lblCoastDownFile.Location = new Point(16, 120);
            lblCoastDownFile.Name = "lblCoastDownFile";
            lblCoastDownFile.Size = new Size(79, 13);
            lblCoastDownFile.TabIndex = 3;
            lblCoastDownFile.Text = "No File Loaded";
            // 
            // btnLoadCoastDown
            // 
            btnLoadCoastDown.Enabled = false;
            btnLoadCoastDown.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnLoadCoastDown.Location = new Point(120, 92);
            btnLoadCoastDown.Name = "btnLoadCoastDown";
            btnLoadCoastDown.Size = new Size(74, 21);
            btnLoadCoastDown.TabIndex = 2;
            btnLoadCoastDown.Text = "Load";
            btnLoadCoastDown.UseVisualStyleBackColor = true;
            // 
            // rdoRollerAndDrivetrain
            // 
            rdoRollerAndDrivetrain.AutoSize = true;
            rdoRollerAndDrivetrain.Location = new Point(13, 63);
            rdoRollerAndDrivetrain.Name = "rdoRollerAndDrivetrain";
            rdoRollerAndDrivetrain.Size = new Size(109, 17);
            rdoRollerAndDrivetrain.TabIndex = 1;
            rdoRollerAndDrivetrain.Text = "Roller + Drivetrain";
            rdoRollerAndDrivetrain.UseVisualStyleBackColor = true;
            // 
            // rdoFreeRoller
            // 
            rdoFreeRoller.AutoSize = true;
            rdoFreeRoller.Checked = true;
            rdoFreeRoller.Location = new Point(13, 19);
            rdoFreeRoller.Name = "rdoFreeRoller";
            rdoFreeRoller.Size = new Size(76, 17);
            rdoFreeRoller.TabIndex = 0;
            rdoFreeRoller.TabStop = true;
            rdoFreeRoller.Text = "Free Roller";
            rdoFreeRoller.UseVisualStyleBackColor = true;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(RadioButton1);
            GroupBox1.Controls.Add(RadioButton2);
            GroupBox1.Controls.Add(RadioButton3);
            GroupBox1.Enabled = false;
            GroupBox1.Location = new Point(256, 35);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new Size(200, 100);
            GroupBox1.TabIndex = 3;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "STD/SAE Correction Options";
            // 
            // RadioButton1
            // 
            RadioButton1.AutoSize = true;
            RadioButton1.Location = new Point(13, 40);
            RadioButton1.Name = "RadioButton1";
            RadioButton1.Size = new Size(114, 17);
            RadioButton1.TabIndex = 2;
            RadioButton1.Text = "SAE J1349 JUN90";
            RadioButton1.UseVisualStyleBackColor = true;
            // 
            // RadioButton2
            // 
            RadioButton2.AutoSize = true;
            RadioButton2.Location = new Point(13, 63);
            RadioButton2.Name = "RadioButton2";
            RadioButton2.Size = new Size(128, 17);
            RadioButton2.TabIndex = 1;
            RadioButton2.Text = "SAE J1349 AUG2004";
            RadioButton2.UseVisualStyleBackColor = true;
            // 
            // RadioButton3
            // 
            RadioButton3.AutoSize = true;
            RadioButton3.Checked = true;
            RadioButton3.Location = new Point(13, 19);
            RadioButton3.Name = "RadioButton3";
            RadioButton3.Size = new Size(47, 17);
            RadioButton3.TabIndex = 0;
            RadioButton3.TabStop = true;
            RadioButton3.Text = "STD";
            RadioButton3.UseVisualStyleBackColor = true;
            // 
            // CheckBox1
            // 
            CheckBox1.AutoSize = true;
            CheckBox1.Enabled = false;
            CheckBox1.Location = new Point(259, 12);
            CheckBox1.Name = "CheckBox1";
            CheckBox1.Size = new Size(147, 17);
            CheckBox1.TabIndex = 2;
            CheckBox1.Text = "Use STD/SAE Correction";
            CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Correction
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 218);
            Controls.Add(GroupBox1);
            Controls.Add(CheckBox1);
            Controls.Add(grpRunDown);
            Controls.Add(chkUseRunDown);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Correction";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dyno Correction Factors";
            grpRunDown.ResumeLayout(false);
            grpRunDown.PerformLayout();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            FormClosing += new FormClosingEventHandler(Correction_FormClosing);
            ResumeLayout(false);
            PerformLayout();

        }
        internal CheckBox chkUseRunDown;
        internal GroupBox grpRunDown;
        internal RadioButton rdoRollerAndDrivetrain;
        internal RadioButton rdoFreeRoller;
        internal GroupBox GroupBox1;
        internal RadioButton RadioButton1;
        internal RadioButton RadioButton2;
        internal RadioButton RadioButton3;
        internal CheckBox CheckBox1;
        internal Button btnLoadCoastDown;
        internal Label lblCoastDownFile;
        internal CheckBox chkUseCoastDownFile;
        internal RadioButton rdoRollerAndWheel;
        #endregion
    }
}