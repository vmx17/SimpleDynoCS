using System.Diagnostics;

namespace SimpleDyno
{
    partial class COM
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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(COM));
            Label72 = new Label();
            Label68 = new Label();
            Label69 = new Label();
            Label66 = new Label();
            Label67 = new Label();
            txtResistance2 = new TextBox();
            txtResistance1 = new TextBox();
            btnCalibrateI2 = new Button();
            btnCalibrateI1 = new Button();
            btnCalibrateV2 = new Button();
            btnCalibrateV1 = new Button();
            Label63 = new Label();
            Label62 = new Label();
            txtInputCurrent2 = new TextBox();
            txtInputCurrent1 = new TextBox();
            Label37 = new Label();
            txtInputVoltage2 = new TextBox();
            txtInputVoltage1 = new TextBox();
            Label18 = new Label();
            lblCurrentAmps = new Label();
            lblCurrentVolts = new Label();
            Label29 = new Label();
            txtA1Voltage2 = new TextBox();
            txtA1Voltage1 = new TextBox();
            txtA0Voltage2 = new TextBox();
            txtA0Voltage1 = new TextBox();
            Label71 = new Label();
            btnCalibrateTemp2T2 = new Button();
            btnCalibrateTemp2T1 = new Button();
            btnCalibrateTemp1T2 = new Button();
            btnCalibrateTemp1T1 = new Button();
            Label64 = new Label();
            txtInputTemp2Temperature2 = new TextBox();
            txtInputTemp2Temperature1 = new TextBox();
            Label55 = new Label();
            lblCurrentTemperature2 = new Label();
            txtA3Voltage2 = new TextBox();
            txtA3Voltage1 = new TextBox();
            txtInputTemp1Temperature2 = new TextBox();
            txtInputTemp1Temperature1 = new TextBox();
            Label10 = new Label();
            lblCurrentTemperature1 = new Label();
            txtA2Voltage2 = new TextBox();
            txtA2Voltage1 = new TextBox();
            Label1 = new Label();
            btnCalibratePinA5V2 = new Button();
            btnCalibratePinA5V1 = new Button();
            btnCalibratePinA4V2 = new Button();
            btnCalibratePinA4V1 = new Button();
            Label2 = new Label();
            txtPin5Value2 = new TextBox();
            txtPin5Value1 = new TextBox();
            lblCurrentPinA5 = new Label();
            txtA5Voltage2 = new TextBox();
            txtA5Voltage1 = new TextBox();
            txtPin4Value2 = new TextBox();
            txtPin4Value1 = new TextBox();
            lblCurrentPinA4 = new Label();
            txtA4Voltage2 = new TextBox();
            txtA4Voltage1 = new TextBox();
            Label4 = new Label();
            Label5 = new Label();
            Label3 = new Label();
            Label6 = new Label();
            Label7 = new Label();
            Label8 = new Label();
            Label9 = new Label();
            Label11 = new Label();
            Label12 = new Label();
            Label13 = new Label();
            Label14 = new Label();
            Label15 = new Label();
            Label16 = new Label();
            Label17 = new Label();
            Label19 = new Label();
            Label20 = new Label();
            Label21 = new Label();
            Label22 = new Label();
            Label23 = new Label();
            Label24 = new Label();
            Label25 = new Label();
            Label26 = new Label();
            A5PowerRunCheckBox = new CheckBox();
            Smalltmr = new System.Windows.Forms.Timer(components);
            CalibBtn = new Button();
            SuspendLayout();
            // 
            // Label72
            // 
            Label72.AutoSize = true;
            Label72.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label72.Location = new Point(328, 49);
            Label72.Name = "Label72";
            Label72.Size = new Size(49, 23);
            Label72.TabIndex = 185;
            Label72.Text = "Volts";
            // 
            // Label68
            // 
            Label68.AutoSize = true;
            Label68.Location = new Point(860, 124);
            Label68.Name = "Label68";
            Label68.Size = new Size(37, 13);
            Label68.TabIndex = 184;
            Label68.Text = "Ohms)";
            // 
            // Label69
            // 
            Label69.AutoSize = true;
            Label69.Location = new Point(860, 98);
            Label69.Name = "Label69";
            Label69.Size = new Size(37, 13);
            Label69.TabIndex = 183;
            Label69.Text = "Ohms)";
            // 
            // Label66
            // 
            Label66.AutoSize = true;
            Label66.Location = new Point(774, 124);
            Label66.Name = "Label66";
            Label66.Size = new Size(37, 13);
            Label66.TabIndex = 182;
            Label66.Text = "(Using";
            // 
            // Label67
            // 
            Label67.AutoSize = true;
            Label67.Location = new Point(774, 98);
            Label67.Name = "Label67";
            Label67.Size = new Size(37, 13);
            Label67.TabIndex = 181;
            Label67.Text = "(Using";
            // 
            // txtResistance2
            // 
            txtResistance2.Location = new Point(812, 121);
            txtResistance2.Name = "txtResistance2";
            txtResistance2.Size = new Size(42, 20);
            txtResistance2.TabIndex = 180;
            txtResistance2.Tag = "";
            txtResistance2.Text = "0";
            txtResistance2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtResistance1
            // 
            txtResistance1.Location = new Point(812, 95);
            txtResistance1.Name = "txtResistance1";
            txtResistance1.Size = new Size(42, 20);
            txtResistance1.TabIndex = 179;
            txtResistance1.Tag = "";
            txtResistance1.Text = "0";
            txtResistance1.TextAlign = HorizontalAlignment.Center;
            // 
            // btnCalibrateI2
            // 
            btnCalibrateI2.Enabled = false;
            btnCalibrateI2.Location = new Point(684, 119);
            btnCalibrateI2.Name = "btnCalibrateI2";
            btnCalibrateI2.Size = new Size(56, 21);
            btnCalibrateI2.TabIndex = 178;
            btnCalibrateI2.Text = "Calibrate";
            btnCalibrateI2.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateI1
            // 
            btnCalibrateI1.Enabled = false;
            btnCalibrateI1.Location = new Point(684, 93);
            btnCalibrateI1.Name = "btnCalibrateI1";
            btnCalibrateI1.Size = new Size(56, 21);
            btnCalibrateI1.TabIndex = 177;
            btnCalibrateI1.Text = "Calibrate";
            btnCalibrateI1.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateV2
            // 
            btnCalibrateV2.Enabled = false;
            btnCalibrateV2.Location = new Point(292, 118);
            btnCalibrateV2.Name = "btnCalibrateV2";
            btnCalibrateV2.Size = new Size(56, 21);
            btnCalibrateV2.TabIndex = 176;
            btnCalibrateV2.Text = "Calibrate";
            btnCalibrateV2.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateV1
            // 
            btnCalibrateV1.Enabled = false;
            btnCalibrateV1.Location = new Point(292, 92);
            btnCalibrateV1.Name = "btnCalibrateV1";
            btnCalibrateV1.Size = new Size(56, 21);
            btnCalibrateV1.TabIndex = 175;
            btnCalibrateV1.Text = "Calibrate";
            btnCalibrateV1.UseVisualStyleBackColor = true;
            // 
            // Label63
            // 
            Label63.AutoSize = true;
            Label63.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label63.Location = new Point(442, 49);
            Label63.Name = "Label63";
            Label63.Size = new Size(73, 23);
            Label63.TabIndex = 174;
            Label63.Text = "Current";
            // 
            // Label62
            // 
            Label62.AutoSize = true;
            Label62.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label62.Location = new Point(50, 49);
            Label62.Name = "Label62";
            Label62.Size = new Size(72, 23);
            Label62.TabIndex = 173;
            Label62.Text = "Voltage";
            // 
            // txtInputCurrent2
            // 
            txtInputCurrent2.Location = new Point(635, 120);
            txtInputCurrent2.Name = "txtInputCurrent2";
            txtInputCurrent2.Size = new Size(43, 20);
            txtInputCurrent2.TabIndex = 168;
            txtInputCurrent2.Tag = "";
            txtInputCurrent2.Text = "5";
            txtInputCurrent2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtInputCurrent1
            // 
            txtInputCurrent1.Location = new Point(635, 94);
            txtInputCurrent1.Name = "txtInputCurrent1";
            txtInputCurrent1.Size = new Size(43, 20);
            txtInputCurrent1.TabIndex = 167;
            txtInputCurrent1.Tag = "";
            txtInputCurrent1.Text = "0";
            txtInputCurrent1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label37
            // 
            Label37.AutoSize = true;
            Label37.Location = new Point(166, 96);
            Label37.Name = "Label37";
            Label37.Size = new Size(71, 13);
            Label37.TabIndex = 164;
            Label37.Text = "Voltage (V) = ";
            Label37.TextAlign = ContentAlignment.TopRight;
            // 
            // txtInputVoltage2
            // 
            txtInputVoltage2.Location = new Point(243, 119);
            txtInputVoltage2.Name = "txtInputVoltage2";
            txtInputVoltage2.Size = new Size(43, 20);
            txtInputVoltage2.TabIndex = 163;
            txtInputVoltage2.Tag = "";
            txtInputVoltage2.Text = "5";
            txtInputVoltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtInputVoltage1
            // 
            txtInputVoltage1.Location = new Point(243, 93);
            txtInputVoltage1.Name = "txtInputVoltage1";
            txtInputVoltage1.Size = new Size(43, 20);
            txtInputVoltage1.TabIndex = 162;
            txtInputVoltage1.Tag = "";
            txtInputVoltage1.Text = "0";
            txtInputVoltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label18
            // 
            Label18.AutoSize = true;
            Label18.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label18.Location = new Point(720, 49);
            Label18.Name = "Label18";
            Label18.Size = new Size(56, 23);
            Label18.TabIndex = 161;
            Label18.Text = "Amps";
            // 
            // lblCurrentAmps
            // 
            lblCurrentAmps.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentAmps.Location = new Point(613, 50);
            lblCurrentAmps.Name = "lblCurrentAmps";
            lblCurrentAmps.Size = new Size(113, 21);
            lblCurrentAmps.TabIndex = 160;
            lblCurrentAmps.Text = "999999";
            lblCurrentAmps.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblCurrentVolts
            // 
            lblCurrentVolts.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentVolts.Location = new Point(221, 49);
            lblCurrentVolts.Name = "lblCurrentVolts";
            lblCurrentVolts.Size = new Size(113, 23);
            lblCurrentVolts.TabIndex = 159;
            lblCurrentVolts.Text = "999999";
            lblCurrentVolts.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label29
            // 
            Label29.AutoSize = true;
            Label29.Location = new Point(32, 96);
            Label29.Name = "Label29";
            Label29.Size = new Size(82, 13);
            Label29.TabIndex = 157;
            Label29.Text = "When Pin A0 = ";
            // 
            // txtA1Voltage2
            // 
            txtA1Voltage2.Location = new Point(505, 119);
            txtA1Voltage2.Name = "txtA1Voltage2";
            txtA1Voltage2.Size = new Size(43, 20);
            txtA1Voltage2.TabIndex = 152;
            txtA1Voltage2.Tag = "";
            txtA1Voltage2.Text = "5";
            txtA1Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA1Voltage1
            // 
            txtA1Voltage1.Location = new Point(505, 93);
            txtA1Voltage1.Name = "txtA1Voltage1";
            txtA1Voltage1.Size = new Size(43, 20);
            txtA1Voltage1.TabIndex = 151;
            txtA1Voltage1.Tag = "";
            txtA1Voltage1.Text = "0";
            txtA1Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA0Voltage2
            // 
            txtA0Voltage2.Location = new Point(117, 119);
            txtA0Voltage2.Name = "txtA0Voltage2";
            txtA0Voltage2.Size = new Size(43, 20);
            txtA0Voltage2.TabIndex = 150;
            txtA0Voltage2.Tag = "";
            txtA0Voltage2.Text = "5";
            txtA0Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA0Voltage1
            // 
            txtA0Voltage1.Location = new Point(117, 93);
            txtA0Voltage1.Name = "txtA0Voltage1";
            txtA0Voltage1.Size = new Size(43, 20);
            txtA0Voltage1.TabIndex = 149;
            txtA0Voltage1.Tag = "";
            txtA0Voltage1.Text = "0";
            txtA0Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label71
            // 
            Label71.AutoSize = true;
            Label71.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label71.Location = new Point(443, 228);
            Label71.Name = "Label71";
            Label71.Size = new Size(129, 23);
            Label71.TabIndex = 215;
            Label71.Text = "Temperature2";
            // 
            // btnCalibrateTemp2T2
            // 
            btnCalibrateTemp2T2.Enabled = false;
            btnCalibrateTemp2T2.Location = new Point(684, 300);
            btnCalibrateTemp2T2.Name = "btnCalibrateTemp2T2";
            btnCalibrateTemp2T2.Size = new Size(56, 21);
            btnCalibrateTemp2T2.TabIndex = 214;
            btnCalibrateTemp2T2.Text = "Calibrate";
            btnCalibrateTemp2T2.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateTemp2T1
            // 
            btnCalibrateTemp2T1.Enabled = false;
            btnCalibrateTemp2T1.Location = new Point(684, 274);
            btnCalibrateTemp2T1.Name = "btnCalibrateTemp2T1";
            btnCalibrateTemp2T1.Size = new Size(56, 21);
            btnCalibrateTemp2T1.TabIndex = 213;
            btnCalibrateTemp2T1.Text = "Calibrate";
            btnCalibrateTemp2T1.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateTemp1T2
            // 
            btnCalibrateTemp1T2.Enabled = false;
            btnCalibrateTemp1T2.Location = new Point(293, 299);
            btnCalibrateTemp1T2.Name = "btnCalibrateTemp1T2";
            btnCalibrateTemp1T2.Size = new Size(56, 21);
            btnCalibrateTemp1T2.TabIndex = 212;
            btnCalibrateTemp1T2.Text = "Calibrate";
            btnCalibrateTemp1T2.UseVisualStyleBackColor = true;
            // 
            // btnCalibrateTemp1T1
            // 
            btnCalibrateTemp1T1.Enabled = false;
            btnCalibrateTemp1T1.Location = new Point(293, 274);
            btnCalibrateTemp1T1.Name = "btnCalibrateTemp1T1";
            btnCalibrateTemp1T1.Size = new Size(56, 21);
            btnCalibrateTemp1T1.TabIndex = 211;
            btnCalibrateTemp1T1.Text = "Calibrate";
            btnCalibrateTemp1T1.UseVisualStyleBackColor = true;
            // 
            // Label64
            // 
            Label64.AutoSize = true;
            Label64.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label64.Location = new Point(52, 230);
            Label64.Name = "Label64";
            Label64.Size = new Size(129, 23);
            Label64.TabIndex = 210;
            Label64.Text = "Temperature1";
            // 
            // txtInputTemp2Temperature2
            // 
            txtInputTemp2Temperature2.Location = new Point(635, 301);
            txtInputTemp2Temperature2.Name = "txtInputTemp2Temperature2";
            txtInputTemp2Temperature2.Size = new Size(43, 20);
            txtInputTemp2Temperature2.TabIndex = 206;
            txtInputTemp2Temperature2.Tag = "0\\999999";
            txtInputTemp2Temperature2.Text = "30";
            txtInputTemp2Temperature2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtInputTemp2Temperature1
            // 
            txtInputTemp2Temperature1.Location = new Point(635, 275);
            txtInputTemp2Temperature1.Name = "txtInputTemp2Temperature1";
            txtInputTemp2Temperature1.Size = new Size(43, 20);
            txtInputTemp2Temperature1.TabIndex = 205;
            txtInputTemp2Temperature1.Tag = "";
            txtInputTemp2Temperature1.Text = "15";
            txtInputTemp2Temperature1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label55
            // 
            Label55.AutoSize = true;
            Label55.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label55.Location = new Point(737, 230);
            Label55.Name = "Label55";
            Label55.Size = new Size(30, 23);
            Label55.TabIndex = 204;
            Label55.Text = "°C";
            // 
            // lblCurrentTemperature2
            // 
            lblCurrentTemperature2.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentTemperature2.Location = new Point(614, 229);
            lblCurrentTemperature2.Name = "lblCurrentTemperature2";
            lblCurrentTemperature2.Size = new Size(113, 21);
            lblCurrentTemperature2.TabIndex = 203;
            lblCurrentTemperature2.Text = "999999";
            lblCurrentTemperature2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtA3Voltage2
            // 
            txtA3Voltage2.Location = new Point(505, 300);
            txtA3Voltage2.Name = "txtA3Voltage2";
            txtA3Voltage2.Size = new Size(43, 20);
            txtA3Voltage2.TabIndex = 199;
            txtA3Voltage2.Tag = "";
            txtA3Voltage2.Text = "5";
            txtA3Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA3Voltage1
            // 
            txtA3Voltage1.Location = new Point(505, 274);
            txtA3Voltage1.Name = "txtA3Voltage1";
            txtA3Voltage1.Size = new Size(43, 20);
            txtA3Voltage1.TabIndex = 198;
            txtA3Voltage1.Tag = "";
            txtA3Voltage1.Text = "0";
            txtA3Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // txtInputTemp1Temperature2
            // 
            txtInputTemp1Temperature2.Location = new Point(244, 301);
            txtInputTemp1Temperature2.Name = "txtInputTemp1Temperature2";
            txtInputTemp1Temperature2.Size = new Size(43, 20);
            txtInputTemp1Temperature2.TabIndex = 194;
            txtInputTemp1Temperature2.Tag = "";
            txtInputTemp1Temperature2.Text = "30";
            txtInputTemp1Temperature2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtInputTemp1Temperature1
            // 
            txtInputTemp1Temperature1.Location = new Point(244, 275);
            txtInputTemp1Temperature1.Name = "txtInputTemp1Temperature1";
            txtInputTemp1Temperature1.Size = new Size(43, 20);
            txtInputTemp1Temperature1.TabIndex = 193;
            txtInputTemp1Temperature1.Tag = "";
            txtInputTemp1Temperature1.Text = "15";
            txtInputTemp1Temperature1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label10
            // 
            Label10.AutoSize = true;
            Label10.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label10.Location = new Point(348, 230);
            Label10.Name = "Label10";
            Label10.Size = new Size(30, 23);
            Label10.TabIndex = 192;
            Label10.Text = "°C";
            // 
            // lblCurrentTemperature1
            // 
            lblCurrentTemperature1.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentTemperature1.Location = new Point(223, 230);
            lblCurrentTemperature1.Name = "lblCurrentTemperature1";
            lblCurrentTemperature1.Size = new Size(113, 20);
            lblCurrentTemperature1.TabIndex = 191;
            lblCurrentTemperature1.Text = "999999";
            lblCurrentTemperature1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtA2Voltage2
            // 
            txtA2Voltage2.Location = new Point(117, 301);
            txtA2Voltage2.Name = "txtA2Voltage2";
            txtA2Voltage2.Size = new Size(43, 20);
            txtA2Voltage2.TabIndex = 187;
            txtA2Voltage2.Tag = "";
            txtA2Voltage2.Text = "5";
            txtA2Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA2Voltage1
            // 
            txtA2Voltage1.Location = new Point(117, 275);
            txtA2Voltage1.Name = "txtA2Voltage1";
            txtA2Voltage1.Size = new Size(43, 20);
            txtA2Voltage1.TabIndex = 186;
            txtA2Voltage1.Tag = "";
            txtA2Voltage1.Text = "0";
            txtA2Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label1.Location = new Point(443, 413);
            Label1.Name = "Label1";
            Label1.Size = new Size(62, 23);
            Label1.TabIndex = 245;
            Label1.Text = "Pin A5";
            // 
            // btnCalibratePinA5V2
            // 
            btnCalibratePinA5V2.Enabled = false;
            btnCalibratePinA5V2.Location = new Point(684, 485);
            btnCalibratePinA5V2.Name = "btnCalibratePinA5V2";
            btnCalibratePinA5V2.Size = new Size(56, 21);
            btnCalibratePinA5V2.TabIndex = 244;
            btnCalibratePinA5V2.Text = "Calibrate";
            btnCalibratePinA5V2.UseVisualStyleBackColor = true;
            // 
            // btnCalibratePinA5V1
            // 
            btnCalibratePinA5V1.Enabled = false;
            btnCalibratePinA5V1.Location = new Point(684, 459);
            btnCalibratePinA5V1.Name = "btnCalibratePinA5V1";
            btnCalibratePinA5V1.Size = new Size(56, 21);
            btnCalibratePinA5V1.TabIndex = 243;
            btnCalibratePinA5V1.Text = "Calibrate";
            btnCalibratePinA5V1.UseVisualStyleBackColor = true;
            // 
            // btnCalibratePinA4V2
            // 
            btnCalibratePinA4V2.Enabled = false;
            btnCalibratePinA4V2.Location = new Point(293, 484);
            btnCalibratePinA4V2.Name = "btnCalibratePinA4V2";
            btnCalibratePinA4V2.Size = new Size(56, 21);
            btnCalibratePinA4V2.TabIndex = 242;
            btnCalibratePinA4V2.Text = "Calibrate";
            btnCalibratePinA4V2.UseVisualStyleBackColor = true;
            // 
            // btnCalibratePinA4V1
            // 
            btnCalibratePinA4V1.Enabled = false;
            btnCalibratePinA4V1.Location = new Point(293, 459);
            btnCalibratePinA4V1.Name = "btnCalibratePinA4V1";
            btnCalibratePinA4V1.Size = new Size(56, 21);
            btnCalibratePinA4V1.TabIndex = 241;
            btnCalibratePinA4V1.Text = "Calibrate";
            btnCalibratePinA4V1.UseVisualStyleBackColor = true;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(52, 415);
            Label2.Name = "Label2";
            Label2.Size = new Size(62, 23);
            Label2.TabIndex = 240;
            Label2.Text = "Pin A4";
            // 
            // txtPin5Value2
            // 
            txtPin5Value2.Location = new Point(635, 486);
            txtPin5Value2.Name = "txtPin5Value2";
            txtPin5Value2.Size = new Size(43, 20);
            txtPin5Value2.TabIndex = 236;
            txtPin5Value2.Tag = "0\\999999";
            txtPin5Value2.Text = "30";
            txtPin5Value2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtPin5Value1
            // 
            txtPin5Value1.Location = new Point(635, 460);
            txtPin5Value1.Name = "txtPin5Value1";
            txtPin5Value1.Size = new Size(43, 20);
            txtPin5Value1.TabIndex = 235;
            txtPin5Value1.Tag = "";
            txtPin5Value1.Text = "15";
            txtPin5Value1.TextAlign = HorizontalAlignment.Center;
            // 
            // lblCurrentPinA5
            // 
            lblCurrentPinA5.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentPinA5.Location = new Point(614, 414);
            lblCurrentPinA5.Name = "lblCurrentPinA5";
            lblCurrentPinA5.Size = new Size(113, 21);
            lblCurrentPinA5.TabIndex = 233;
            lblCurrentPinA5.Text = "999999";
            lblCurrentPinA5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtA5Voltage2
            // 
            txtA5Voltage2.Location = new Point(505, 485);
            txtA5Voltage2.Name = "txtA5Voltage2";
            txtA5Voltage2.Size = new Size(43, 20);
            txtA5Voltage2.TabIndex = 229;
            txtA5Voltage2.Tag = "";
            txtA5Voltage2.Text = "5";
            txtA5Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA5Voltage1
            // 
            txtA5Voltage1.Location = new Point(505, 459);
            txtA5Voltage1.Name = "txtA5Voltage1";
            txtA5Voltage1.Size = new Size(43, 20);
            txtA5Voltage1.TabIndex = 228;
            txtA5Voltage1.Tag = "";
            txtA5Voltage1.Text = "0";
            txtA5Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // txtPin4Value2
            // 
            txtPin4Value2.Location = new Point(244, 486);
            txtPin4Value2.Name = "txtPin4Value2";
            txtPin4Value2.Size = new Size(43, 20);
            txtPin4Value2.TabIndex = 224;
            txtPin4Value2.Tag = "";
            txtPin4Value2.Text = "30";
            txtPin4Value2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtPin4Value1
            // 
            txtPin4Value1.Location = new Point(244, 460);
            txtPin4Value1.Name = "txtPin4Value1";
            txtPin4Value1.Size = new Size(43, 20);
            txtPin4Value1.TabIndex = 223;
            txtPin4Value1.Tag = "";
            txtPin4Value1.Text = "15";
            txtPin4Value1.TextAlign = HorizontalAlignment.Center;
            // 
            // lblCurrentPinA4
            // 
            lblCurrentPinA4.Font = new Font("Tahoma", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentPinA4.Location = new Point(223, 415);
            lblCurrentPinA4.Name = "lblCurrentPinA4";
            lblCurrentPinA4.Size = new Size(113, 20);
            lblCurrentPinA4.TabIndex = 221;
            lblCurrentPinA4.Text = "999999";
            lblCurrentPinA4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtA4Voltage2
            // 
            txtA4Voltage2.Location = new Point(117, 486);
            txtA4Voltage2.Name = "txtA4Voltage2";
            txtA4Voltage2.Size = new Size(43, 20);
            txtA4Voltage2.TabIndex = 217;
            txtA4Voltage2.Tag = "";
            txtA4Voltage2.Text = "5";
            txtA4Voltage2.TextAlign = HorizontalAlignment.Center;
            // 
            // txtA4Voltage1
            // 
            txtA4Voltage1.Location = new Point(117, 460);
            txtA4Voltage1.Name = "txtA4Voltage1";
            txtA4Voltage1.Size = new Size(43, 20);
            txtA4Voltage1.TabIndex = 216;
            txtA4Voltage1.Tag = "";
            txtA4Voltage1.Text = "0";
            txtA4Voltage1.TextAlign = HorizontalAlignment.Center;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Location = new Point(166, 122);
            Label4.Name = "Label4";
            Label4.Size = new Size(71, 13);
            Label4.TabIndex = 247;
            Label4.Text = "Voltage (V) = ";
            Label4.TextAlign = ContentAlignment.TopRight;
            // 
            // Label5
            // 
            Label5.AutoSize = true;
            Label5.Location = new Point(32, 122);
            Label5.Name = "Label5";
            Label5.Size = new Size(82, 13);
            Label5.TabIndex = 246;
            Label5.Text = "When Pin A0 = ";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(564, 122);
            Label3.Name = "Label3";
            Label3.Size = new Size(65, 13);
            Label3.TabIndex = 251;
            Label3.Text = "Current (I) = ";
            // 
            // Label6
            // 
            Label6.AutoSize = true;
            Label6.Location = new Point(423, 122);
            Label6.Name = "Label6";
            Label6.Size = new Size(82, 13);
            Label6.TabIndex = 250;
            Label6.Text = "When Pin A1 = ";
            // 
            // Label7
            // 
            Label7.AutoSize = true;
            Label7.Location = new Point(564, 96);
            Label7.Name = "Label7";
            Label7.Size = new Size(65, 13);
            Label7.TabIndex = 249;
            Label7.Text = "Current (I) = ";
            // 
            // Label8
            // 
            Label8.AutoSize = true;
            Label8.Location = new Point(423, 96);
            Label8.Name = "Label8";
            Label8.Size = new Size(82, 13);
            Label8.TabIndex = 248;
            Label8.Text = "When Pin A1 = ";
            // 
            // Label9
            // 
            Label9.AutoSize = true;
            Label9.Location = new Point(166, 304);
            Label9.Name = "Label9";
            Label9.Size = new Size(72, 13);
            Label9.TabIndex = 255;
            Label9.Text = "Temp1 (°C) = ";
            Label9.TextAlign = ContentAlignment.TopRight;
            // 
            // Label11
            // 
            Label11.AutoSize = true;
            Label11.Location = new Point(32, 304);
            Label11.Name = "Label11";
            Label11.Size = new Size(82, 13);
            Label11.TabIndex = 254;
            Label11.Text = "When Pin A2 = ";
            // 
            // Label12
            // 
            Label12.AutoSize = true;
            Label12.Location = new Point(166, 278);
            Label12.Name = "Label12";
            Label12.Size = new Size(72, 13);
            Label12.TabIndex = 253;
            Label12.Text = "Temp1 (°C) = ";
            Label12.TextAlign = ContentAlignment.TopRight;
            // 
            // Label13
            // 
            Label13.AutoSize = true;
            Label13.Location = new Point(32, 278);
            Label13.Name = "Label13";
            Label13.Size = new Size(82, 13);
            Label13.TabIndex = 252;
            Label13.Text = "When Pin A2 = ";
            // 
            // Label14
            // 
            Label14.AutoSize = true;
            Label14.Location = new Point(557, 303);
            Label14.Name = "Label14";
            Label14.Size = new Size(72, 13);
            Label14.TabIndex = 259;
            Label14.Text = "Temp2 (°C) = ";
            // 
            // Label15
            // 
            Label15.AutoSize = true;
            Label15.Location = new Point(423, 303);
            Label15.Name = "Label15";
            Label15.Size = new Size(82, 13);
            Label15.TabIndex = 258;
            Label15.Text = "When Pin A3 = ";
            // 
            // Label16
            // 
            Label16.AutoSize = true;
            Label16.Location = new Point(557, 277);
            Label16.Name = "Label16";
            Label16.Size = new Size(72, 13);
            Label16.TabIndex = 257;
            Label16.Text = "Temp2 (°C) = ";
            // 
            // Label17
            // 
            Label17.AutoSize = true;
            Label17.Location = new Point(423, 277);
            Label17.Name = "Label17";
            Label17.Size = new Size(82, 13);
            Label17.TabIndex = 256;
            Label17.Text = "When Pin A3 = ";
            // 
            // Label19
            // 
            Label19.AutoSize = true;
            Label19.Location = new Point(176, 489);
            Label19.Name = "Label19";
            Label19.Size = new Size(62, 13);
            Label19.TabIndex = 263;
            Label19.Text = "A4 Value = ";
            Label19.TextAlign = ContentAlignment.TopRight;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(32, 489);
            Label20.Name = "Label20";
            Label20.Size = new Size(82, 13);
            Label20.TabIndex = 262;
            Label20.Text = "When Pin A4 = ";
            // 
            // Label21
            // 
            Label21.AutoSize = true;
            Label21.Location = new Point(176, 463);
            Label21.Name = "Label21";
            Label21.Size = new Size(62, 13);
            Label21.TabIndex = 261;
            Label21.Text = "A4 Value = ";
            Label21.TextAlign = ContentAlignment.TopRight;
            // 
            // Label22
            // 
            Label22.AutoSize = true;
            Label22.Location = new Point(32, 463);
            Label22.Name = "Label22";
            Label22.Size = new Size(82, 13);
            Label22.TabIndex = 260;
            Label22.Text = "When Pin A4 = ";
            // 
            // Label23
            // 
            Label23.AutoSize = true;
            Label23.Location = new Point(567, 489);
            Label23.Name = "Label23";
            Label23.Size = new Size(62, 13);
            Label23.TabIndex = 267;
            Label23.Text = "A5 Value = ";
            // 
            // Label24
            // 
            Label24.AutoSize = true;
            Label24.Location = new Point(423, 489);
            Label24.Name = "Label24";
            Label24.Size = new Size(82, 13);
            Label24.TabIndex = 266;
            Label24.Text = "When Pin A5 = ";
            // 
            // Label25
            // 
            Label25.AutoSize = true;
            Label25.Location = new Point(567, 463);
            Label25.Name = "Label25";
            Label25.Size = new Size(62, 13);
            Label25.TabIndex = 265;
            Label25.Text = "A5 Value = ";
            // 
            // Label26
            // 
            Label26.AutoSize = true;
            Label26.Location = new Point(423, 463);
            Label26.Name = "Label26";
            Label26.Size = new Size(82, 13);
            Label26.TabIndex = 264;
            Label26.Text = "When Pin A5 = ";
            // 
            // A5PowerRunCheckBox
            // 
            A5PowerRunCheckBox.AutoSize = true;
            A5PowerRunCheckBox.Location = new Point(426, 516);
            A5PowerRunCheckBox.Name = "A5PowerRunCheckBox";
            A5PowerRunCheckBox.Size = new Size(258, 17);
            A5PowerRunCheckBox.TabIndex = 268;
            A5PowerRunCheckBox.Text = "Use A5 as binary signal for controlling Power Run";
            A5PowerRunCheckBox.UseVisualStyleBackColor = true;
            // 
            // CalibBtn
            // 
            CalibBtn.Location = new Point(838, 516);
            CalibBtn.Name = "CalibBtn";
            CalibBtn.Size = new Size(75, 23);
            CalibBtn.TabIndex = 269;
            CalibBtn.Text = "Calibrate";
            CalibBtn.UseVisualStyleBackColor = true;
            CalibBtn.Visible = false;
            CalibBtn.Click += btnCalibrate_Click;
            // 
            // COM
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(944, 555);
            Controls.Add(CalibBtn);
            Controls.Add(A5PowerRunCheckBox);
            Controls.Add(Label23);
            Controls.Add(Label24);
            Controls.Add(Label25);
            Controls.Add(Label26);
            Controls.Add(Label19);
            Controls.Add(Label20);
            Controls.Add(Label21);
            Controls.Add(Label22);
            Controls.Add(Label14);
            Controls.Add(Label15);
            Controls.Add(Label16);
            Controls.Add(Label17);
            Controls.Add(Label9);
            Controls.Add(Label11);
            Controls.Add(Label12);
            Controls.Add(Label13);
            Controls.Add(Label3);
            Controls.Add(Label6);
            Controls.Add(Label7);
            Controls.Add(Label8);
            Controls.Add(Label4);
            Controls.Add(Label5);
            Controls.Add(Label1);
            Controls.Add(btnCalibratePinA5V2);
            Controls.Add(btnCalibratePinA5V1);
            Controls.Add(btnCalibratePinA4V2);
            Controls.Add(btnCalibratePinA4V1);
            Controls.Add(Label2);
            Controls.Add(txtPin5Value2);
            Controls.Add(txtPin5Value1);
            Controls.Add(lblCurrentPinA5);
            Controls.Add(txtA5Voltage2);
            Controls.Add(txtA5Voltage1);
            Controls.Add(txtPin4Value2);
            Controls.Add(txtPin4Value1);
            Controls.Add(lblCurrentPinA4);
            Controls.Add(txtA4Voltage2);
            Controls.Add(txtA4Voltage1);
            Controls.Add(Label71);
            Controls.Add(btnCalibrateTemp2T2);
            Controls.Add(btnCalibrateTemp2T1);
            Controls.Add(btnCalibrateTemp1T2);
            Controls.Add(btnCalibrateTemp1T1);
            Controls.Add(Label64);
            Controls.Add(txtInputTemp2Temperature2);
            Controls.Add(txtInputTemp2Temperature1);
            Controls.Add(Label55);
            Controls.Add(lblCurrentTemperature2);
            Controls.Add(txtA3Voltage2);
            Controls.Add(txtA3Voltage1);
            Controls.Add(txtInputTemp1Temperature2);
            Controls.Add(txtInputTemp1Temperature1);
            Controls.Add(Label10);
            Controls.Add(lblCurrentTemperature1);
            Controls.Add(txtA2Voltage2);
            Controls.Add(txtA2Voltage1);
            Controls.Add(Label72);
            Controls.Add(Label68);
            Controls.Add(Label69);
            Controls.Add(Label66);
            Controls.Add(Label67);
            Controls.Add(txtResistance2);
            Controls.Add(txtResistance1);
            Controls.Add(btnCalibrateI2);
            Controls.Add(btnCalibrateI1);
            Controls.Add(btnCalibrateV2);
            Controls.Add(btnCalibrateV1);
            Controls.Add(Label63);
            Controls.Add(Label62);
            Controls.Add(txtInputCurrent2);
            Controls.Add(txtInputCurrent1);
            Controls.Add(Label37);
            Controls.Add(txtInputVoltage2);
            Controls.Add(txtInputVoltage1);
            Controls.Add(Label18);
            Controls.Add(lblCurrentAmps);
            Controls.Add(lblCurrentVolts);
            Controls.Add(Label29);
            Controls.Add(txtA1Voltage2);
            Controls.Add(txtA1Voltage1);
            Controls.Add(txtA0Voltage2);
            Controls.Add(txtA0Voltage1);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "COM";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "COM Port Calibration";
            FormClosing += COM_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        internal Label Label72;
        internal Label Label68;
        internal Label Label69;
        internal Label Label66;
        internal Label Label67;
        internal TextBox txtResistance2;
        internal TextBox txtResistance1;
        internal Button btnCalibrateI2;
        internal Button btnCalibrateI1;
        internal Button btnCalibrateV2;
        internal Button btnCalibrateV1;
        internal Label Label63;
        internal Label Label62;
        internal TextBox txtInputCurrent2;
        internal TextBox txtInputCurrent1;
        internal Label Label37;
        internal TextBox txtInputVoltage2;
        internal TextBox txtInputVoltage1;
        internal Label Label18;
        internal Label lblCurrentAmps;
        internal Label lblCurrentVolts;
        internal Label Label29;
        internal TextBox txtA1Voltage2;
        internal TextBox txtA1Voltage1;
        internal TextBox txtA0Voltage2;
        internal TextBox txtA0Voltage1;
        internal Label Label71;
        internal Button btnCalibrateTemp2T2;
        internal Button btnCalibrateTemp2T1;
        internal Button btnCalibrateTemp1T2;
        internal Button btnCalibrateTemp1T1;
        internal Label Label64;
        internal TextBox txtInputTemp2Temperature2;
        internal TextBox txtInputTemp2Temperature1;
        internal Label Label55;
        internal Label lblCurrentTemperature2;
        internal TextBox txtA3Voltage2;
        internal TextBox txtA3Voltage1;
        internal TextBox txtInputTemp1Temperature2;
        internal TextBox txtInputTemp1Temperature1;
        internal Label Label10;
        internal Label lblCurrentTemperature1;
        internal TextBox txtA2Voltage2;
        internal TextBox txtA2Voltage1;
        internal Label Label1;
        internal Button btnCalibratePinA5V2;
        internal Button btnCalibratePinA5V1;
        internal Button btnCalibratePinA4V2;
        internal Button btnCalibratePinA4V1;
        internal Label Label2;
        internal TextBox txtPin5Value2;
        internal TextBox txtPin5Value1;
        internal Label lblCurrentPinA5;
        internal TextBox txtA5Voltage2;
        internal TextBox txtA5Voltage1;
        internal TextBox txtPin4Value2;
        internal TextBox txtPin4Value1;
        internal Label lblCurrentPinA4;
        internal TextBox txtA4Voltage2;
        internal TextBox txtA4Voltage1;
        internal Label Label4;
        internal Label Label5;
        internal Label Label3;
        internal Label Label6;
        internal Label Label7;
        internal Label Label8;
        internal Label Label9;
        internal Label Label11;
        internal Label Label12;
        internal Label Label13;
        internal Label Label14;
        internal Label Label15;
        internal Label Label16;
        internal Label Label17;
        internal Label Label19;
        internal Label Label20;
        internal Label Label21;
        internal Label Label22;
        internal Label Label23;
        internal Label Label24;
        internal Label Label25;
        internal Label Label26;
        internal CheckBox A5PowerRunCheckBox;
        #endregion

        private System.Windows.Forms.Timer Smalltmr;
        private Button CalibBtn;
    }
}