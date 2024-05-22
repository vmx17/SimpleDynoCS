using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleDyno
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class Analysis:Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Analysis));
            btnAddOverlayFile = new Button();
            btnAddOverlayFile.Click += new EventHandler(btnAddOverlayFile_Click_1);
            btnClearOverlay = new Button();
            btnClearOverlay.Click += new EventHandler(btnClearOverlay_Click_1);
            btnSaveOverlay = new Button();
            btnSaveOverlay.Click += new EventHandler(btnSaveOverlay_Click_1);
            cmbOverlayUnitsY2 = new ComboBox();
            cmbOverlayUnitsY2.SelectedIndexChanged += new EventHandler(cmbOverlayY2Units_SelectedIndexChanged);
            cmbOverlayUnitsY1 = new ComboBox();
            cmbOverlayUnitsY1.SelectedIndexChanged += new EventHandler(cmbOverlayY1Units_SelectedIndexChanged);
            cmbOverlayUnitsX = new ComboBox();
            cmbOverlayUnitsX.SelectedIndexChanged += new EventHandler(cmbOverlayXUnits_SelectedIndexChanged);
            Label48 = new Label();
            Label47 = new Label();
            Label46 = new Label();
            cmbOverlayDataY2 = new ComboBox();
            cmbOverlayDataY2.SelectedIndexChanged += new EventHandler(cmbOverlayY2_SelectedIndexChanged);
            cmbOverlayDataY1 = new ComboBox();
            cmbOverlayDataY1.SelectedIndexChanged += new EventHandler(cmbOverlayY1_SelectedIndexChanged);
            cmbOverlayDataX = new ComboBox();
            cmbOverlayDataX.SelectedIndexChanged += new EventHandler(cmbOverlayX_SelectedIndexChanged);
            Label51 = new Label();
            cmbOverlayCorrectedSpeedUnits = new ComboBox();
            cmbOverlayCorrectedSpeedUnits.SelectedIndexChanged += new EventHandler(cmbOverlayCorrectedSpeedUnits_SelectedIndexChanged);
            cmbOverlayUnitsY4 = new ComboBox();
            cmbOverlayUnitsY4.SelectedIndexChanged += new EventHandler(cmbOverlayY4Units_SelectedIndexChanged);
            cmbOverlayUnitsY3 = new ComboBox();
            cmbOverlayUnitsY3.SelectedIndexChanged += new EventHandler(cmbOverlayY3Units_SelectedIndexChanged);
            Label50 = new Label();
            Label49 = new Label();
            cmbOverlayDataY4 = new ComboBox();
            cmbOverlayDataY4.SelectedIndexChanged += new EventHandler(cmbOverlayY4_SelectedIndexChanged);
            cmbOverlayDataY3 = new ComboBox();
            cmbOverlayDataY3.SelectedIndexChanged += new EventHandler(cmbOverlayY3_SelectedIndexChanged);
            lblCurrentXValue = new Label();
            Label9 = new Label();
            OpenFileDialog1 = new OpenFileDialog();
            SaveFileDialog1 = new SaveFileDialog();
            pnlDataOverlay = new DoubleBufferPanel();
            pnlDataOverlay.Click += new EventHandler(pnlDataOverlay_Click);
            pnlDataOverlay.MouseMove += new MouseEventHandler(pnlDataOverlay_MouseMove);
            Label1 = new Label();
            Label2 = new Label();
            Label3 = new Label();
            Label4 = new Label();
            Label5 = new Label();
            TextBox_XEnd = new TextBox();
            TextBox_XEnd.TextChanged += new EventHandler(TextBox_XEnd_Changed);
            TextBox_XStart = new TextBox();
            TextBox_XStart.TextChanged += new EventHandler(TextBox_XStart_changed);
            RangeLabel = new Label();
            SuspendLayout();
            // 
            // btnAddOverlayFile
            // 
            btnAddOverlayFile.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAddOverlayFile.Location = new Point(3, 3);
            btnAddOverlayFile.Name = "btnAddOverlayFile";
            btnAddOverlayFile.Size = new Size(154, 29);
            btnAddOverlayFile.TabIndex = 65;
            btnAddOverlayFile.Text = "Add...";
            // 
            // btnClearOverlay
            // 
            btnClearOverlay.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnClearOverlay.Location = new Point(3, 34);
            btnClearOverlay.Name = "btnClearOverlay";
            btnClearOverlay.Size = new Size(154, 29);
            btnClearOverlay.TabIndex = 66;
            btnClearOverlay.Text = "Clear";
            // 
            // btnSaveOverlay
            // 
            btnSaveOverlay.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSaveOverlay.Location = new Point(3, 65);
            btnSaveOverlay.Name = "btnSaveOverlay";
            btnSaveOverlay.Size = new Size(154, 29);
            btnSaveOverlay.TabIndex = 67;
            btnSaveOverlay.Text = "Save";
            // 
            // cmbOverlayUnitsY2
            // 
            cmbOverlayUnitsY2.FormattingEnabled = true;
            cmbOverlayUnitsY2.Location = new Point(109, 289);
            cmbOverlayUnitsY2.Name = "cmbOverlayUnitsY2";
            cmbOverlayUnitsY2.Size = new Size(48, 21);
            cmbOverlayUnitsY2.TabIndex = 92;
            // 
            // cmbOverlayUnitsY1
            // 
            cmbOverlayUnitsY1.FormattingEnabled = true;
            cmbOverlayUnitsY1.Location = new Point(109, 235);
            cmbOverlayUnitsY1.Name = "cmbOverlayUnitsY1";
            cmbOverlayUnitsY1.Size = new Size(48, 21);
            cmbOverlayUnitsY1.TabIndex = 91;
            // 
            // cmbOverlayUnitsX
            // 
            cmbOverlayUnitsX.FormattingEnabled = true;
            cmbOverlayUnitsX.Location = new Point(62, 138);
            cmbOverlayUnitsX.Name = "cmbOverlayUnitsX";
            cmbOverlayUnitsX.Size = new Size(95, 21);
            cmbOverlayUnitsX.TabIndex = 90;
            // 
            // Label48
            // 
            Label48.AutoSize = true;
            Label48.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label48.Location = new Point(0, 252);
            Label48.Name = "Label48";
            Label48.Size = new Size(42, 13);
            Label48.TabIndex = 89;
            Label48.Text = "Y2 Axis";
            // 
            // Label47
            // 
            Label47.AutoSize = true;
            Label47.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label47.Location = new Point(0, 198);
            Label47.Name = "Label47";
            Label47.Size = new Size(42, 13);
            Label47.TabIndex = 88;
            Label47.Text = "Y1 Axis";
            // 
            // Label46
            // 
            Label46.AutoSize = true;
            Label46.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label46.Location = new Point(0, 101);
            Label46.Name = "Label46";
            Label46.Size = new Size(36, 13);
            Label46.TabIndex = 87;
            Label46.Text = "X Axis";
            // 
            // cmbOverlayDataY2
            // 
            cmbOverlayDataY2.DropDownWidth = 150;
            cmbOverlayDataY2.FormattingEnabled = true;
            cmbOverlayDataY2.Location = new Point(3, 268);
            cmbOverlayDataY2.Name = "cmbOverlayDataY2";
            cmbOverlayDataY2.Size = new Size(154, 21);
            cmbOverlayDataY2.TabIndex = 86;
            // 
            // cmbOverlayDataY1
            // 
            cmbOverlayDataY1.DropDownWidth = 150;
            cmbOverlayDataY1.FormattingEnabled = true;
            cmbOverlayDataY1.Location = new Point(3, 214);
            cmbOverlayDataY1.Name = "cmbOverlayDataY1";
            cmbOverlayDataY1.Size = new Size(154, 21);
            cmbOverlayDataY1.TabIndex = 85;
            // 
            // cmbOverlayDataX
            // 
            cmbOverlayDataX.DropDownWidth = 150;
            cmbOverlayDataX.FormattingEnabled = true;
            cmbOverlayDataX.Location = new Point(3, 117);
            cmbOverlayDataX.Name = "cmbOverlayDataX";
            cmbOverlayDataX.Size = new Size(154, 21);
            cmbOverlayDataX.TabIndex = 84;
            // 
            // Label51
            // 
            Label51.AutoSize = true;
            Label51.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label51.Location = new Point(65, 430);
            Label51.Name = "Label51";
            Label51.Size = new Size(92, 13);
            Label51.TabIndex = 100;
            Label51.Text = "Corr. Speed Units";
            // 
            // cmbOverlayCorrectedSpeedUnits
            // 
            cmbOverlayCorrectedSpeedUnits.FormattingEnabled = true;
            cmbOverlayCorrectedSpeedUnits.Location = new Point(109, 446);
            cmbOverlayCorrectedSpeedUnits.Name = "cmbOverlayCorrectedSpeedUnits";
            cmbOverlayCorrectedSpeedUnits.Size = new Size(48, 21);
            cmbOverlayCorrectedSpeedUnits.TabIndex = 99;
            // 
            // cmbOverlayUnitsY4
            // 
            cmbOverlayUnitsY4.FormattingEnabled = true;
            cmbOverlayUnitsY4.Location = new Point(109, 397);
            cmbOverlayUnitsY4.Name = "cmbOverlayUnitsY4";
            cmbOverlayUnitsY4.Size = new Size(48, 21);
            cmbOverlayUnitsY4.TabIndex = 98;
            // 
            // cmbOverlayUnitsY3
            // 
            cmbOverlayUnitsY3.FormattingEnabled = true;
            cmbOverlayUnitsY3.Location = new Point(109, 343);
            cmbOverlayUnitsY3.Name = "cmbOverlayUnitsY3";
            cmbOverlayUnitsY3.Size = new Size(48, 21);
            cmbOverlayUnitsY3.TabIndex = 97;
            // 
            // Label50
            // 
            Label50.AutoSize = true;
            Label50.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label50.Location = new Point(0, 360);
            Label50.Name = "Label50";
            Label50.Size = new Size(42, 13);
            Label50.TabIndex = 96;
            Label50.Text = "Y4 Axis";
            // 
            // Label49
            // 
            Label49.AutoSize = true;
            Label49.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label49.Location = new Point(0, 306);
            Label49.Name = "Label49";
            Label49.Size = new Size(42, 13);
            Label49.TabIndex = 95;
            Label49.Text = "Y3 Axis";
            // 
            // cmbOverlayDataY4
            // 
            cmbOverlayDataY4.DropDownWidth = 150;
            cmbOverlayDataY4.FormattingEnabled = true;
            cmbOverlayDataY4.Location = new Point(3, 376);
            cmbOverlayDataY4.Name = "cmbOverlayDataY4";
            cmbOverlayDataY4.Size = new Size(154, 21);
            cmbOverlayDataY4.TabIndex = 94;
            // 
            // cmbOverlayDataY3
            // 
            cmbOverlayDataY3.DropDownWidth = 150;
            cmbOverlayDataY3.FormattingEnabled = true;
            cmbOverlayDataY3.Location = new Point(3, 322);
            cmbOverlayDataY3.Name = "cmbOverlayDataY3";
            cmbOverlayDataY3.Size = new Size(154, 21);
            cmbOverlayDataY3.TabIndex = 93;
            // 
            // lblCurrentXValue
            // 
            lblCurrentXValue.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentXValue.Location = new Point(85, 488);
            lblCurrentXValue.Name = "lblCurrentXValue";
            lblCurrentXValue.Size = new Size(72, 13);
            lblCurrentXValue.TabIndex = 102;
            lblCurrentXValue.Text = "0";
            // 
            // Label9
            // 
            Label9.AutoSize = true;
            Label9.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label9.Location = new Point(0, 488);
            Label9.Name = "Label9";
            Label9.Size = new Size(85, 13);
            Label9.TabIndex = 101;
            Label9.Text = "Current X value ";
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // pnlDataOverlay
            // 
            pnlDataOverlay.BackColor = SystemColors.ActiveCaptionText;
            pnlDataOverlay.BackgroundImageLayout = ImageLayout.None;
            pnlDataOverlay.BorderStyle = BorderStyle.FixedSingle;
            pnlDataOverlay.Location = new Point(160, 3);
            pnlDataOverlay.Margin = new Padding(0);
            pnlDataOverlay.Name = "pnlDataOverlay";
            pnlDataOverlay.Size = new Size(406, 294);
            pnlDataOverlay.TabIndex = 103;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(17, 141);
            Label1.Name = "Label1";
            Label1.Size = new Size(31, 13);
            Label1.TabIndex = 104;
            Label1.Text = "Units";
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(72, 238);
            Label2.Name = "Label2";
            Label2.Size = new Size(31, 13);
            Label2.TabIndex = 105;
            Label2.Text = "Units";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(72, 292);
            Label3.Name = "Label3";
            Label3.Size = new Size(31, 13);
            Label3.TabIndex = 106;
            Label3.Text = "Units";
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Location = new Point(72, 346);
            Label4.Name = "Label4";
            Label4.Size = new Size(31, 13);
            Label4.TabIndex = 107;
            Label4.Text = "Units";
            // 
            // Label5
            // 
            Label5.AutoSize = true;
            Label5.Location = new Point(72, 400);
            Label5.Name = "Label5";
            Label5.Size = new Size(31, 13);
            Label5.TabIndex = 108;
            Label5.Text = "Units";
            // 
            // TextBox_XEnd
            // 
            TextBox_XEnd.Location = new Point(110, 159);
            TextBox_XEnd.Name = "TextBox_XEnd";
            TextBox_XEnd.Size = new Size(48, 20);
            TextBox_XEnd.TabIndex = 109;
            // 
            // TextBox_XStart
            // 
            TextBox_XStart.Location = new Point(62, 159);
            TextBox_XStart.Name = "TextBox_XStart";
            TextBox_XStart.Size = new Size(42, 20);
            TextBox_XStart.TabIndex = 110;
            // 
            // RangeLabel
            // 
            RangeLabel.AutoSize = true;
            RangeLabel.Location = new Point(17, 162);
            RangeLabel.Name = "RangeLabel";
            RangeLabel.Size = new Size(39, 13);
            RangeLabel.TabIndex = 111;
            RangeLabel.Text = "Range";
            // 
            // Analysis
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(944, 555);
            Controls.Add(RangeLabel);
            Controls.Add(TextBox_XStart);
            Controls.Add(TextBox_XEnd);
            Controls.Add(Label5);
            Controls.Add(Label4);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Controls.Add(pnlDataOverlay);
            Controls.Add(lblCurrentXValue);
            Controls.Add(Label9);
            Controls.Add(Label51);
            Controls.Add(cmbOverlayCorrectedSpeedUnits);
            Controls.Add(cmbOverlayUnitsY4);
            Controls.Add(cmbOverlayUnitsY3);
            Controls.Add(Label50);
            Controls.Add(Label49);
            Controls.Add(cmbOverlayDataY4);
            Controls.Add(cmbOverlayDataY3);
            Controls.Add(cmbOverlayUnitsY2);
            Controls.Add(cmbOverlayUnitsY1);
            Controls.Add(cmbOverlayUnitsX);
            Controls.Add(Label48);
            Controls.Add(Label47);
            Controls.Add(Label46);
            Controls.Add(cmbOverlayDataY2);
            Controls.Add(cmbOverlayDataY1);
            Controls.Add(cmbOverlayDataX);
            Controls.Add(btnAddOverlayFile);
            Controls.Add(btnClearOverlay);
            Controls.Add(btnSaveOverlay);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Analysis";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Data Analysis";
            FormClosing += new FormClosingEventHandler(Analysis_FormClosing);
            ResumeLayout(false);
            PerformLayout();

        }
        internal Button btnAddOverlayFile;
        internal Button btnClearOverlay;
        internal Button btnSaveOverlay;
        internal ComboBox cmbOverlayUnitsY2;
        internal ComboBox cmbOverlayUnitsY1;
        internal ComboBox cmbOverlayUnitsX;
        internal Label Label48;
        internal Label Label47;
        internal Label Label46;
        internal ComboBox cmbOverlayDataY2;
        internal ComboBox cmbOverlayDataY1;
        internal ComboBox cmbOverlayDataX;
        internal Label Label51;
        internal ComboBox cmbOverlayCorrectedSpeedUnits;
        internal ComboBox cmbOverlayUnitsY4;
        internal ComboBox cmbOverlayUnitsY3;
        internal Label Label50;
        internal Label Label49;
        internal ComboBox cmbOverlayDataY4;
        internal ComboBox cmbOverlayDataY3;
        internal Label lblCurrentXValue;
        internal Label Label9;
        internal DoubleBufferPanel pnlDataOverlay;
        internal OpenFileDialog OpenFileDialog1;
        internal SaveFileDialog SaveFileDialog1;
        internal Label Label1;
        internal Label Label2;
        internal Label Label3;
        internal Label Label4;
        internal Label Label5;
        internal TextBox TextBox_XEnd;
        internal TextBox TextBox_XStart;
        internal Label RangeLabel;
        #endregion
    }
}