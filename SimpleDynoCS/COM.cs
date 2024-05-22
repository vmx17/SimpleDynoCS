using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SimpleDyno
{
    public partial class COM : Form
    {
        // Global Temporary Double for Checking the numeric input of the textboxes
        private double TempDouble;
        // Calibration timer specific
        //private Button CalibrationButton;
        //private System.Windows.Forms.Timer Smalltmr = new();
        internal bool Calibrating = false;
        internal double[] CalibrationValues = new double[7];
        internal long NumberOfCalibrationValues;

        public COM()
        {
            InitializeComponent();
        }
        internal void COM_Setup()
        {
            SetupCalibrationButtons();
            Program.MainI.COMPortCalibration();
        }
        private void COM_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevents form from actually closing, rather it hides
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                this.Hide();
                e.Cancel = true;
                Program.MainI.btnShow_Click(this, EventArgs.Empty);
            }
        }
        internal void SetupCalibrationButtons()
        {
            // Adds handler to all of the calibration buttons
            Smalltmr.Stop();
            this.Smalltmr.Tick += CalibrationCounter;
            Smalltmr.Interval = 1000;
            foreach (Control c in Controls)
            {
                if (c is Button && Strings.InStr(c.Name, "Calibrate") != 0)
                {
                    c.Click += btnCalibrate_Click;
                }
            }
        }
        private void btnCalibrate_Click(object? sender, EventArgs e) // Handles btnCalibrateV1.Click
        {
            // User enters known value into the "Input" Box.  Sample those readings for 1 sec and enter the average value into the Voltage at Ax box
            // This sub just sets the id of the button and starts the timer
            CalibBtn = sender as Button;
            CalibrationValues = new double[7];
            Smalltmr.Start();
            Calibrating = true;
        }
        private void CalibrationCounter(object? sender, EventArgs e)
        {
            Calibrating = false;
            Smalltmr.Stop();
            var tempeventargs = new EventArgs();
            for (int temp = 0; temp <= 5; temp++)
                CalibrationValues[temp] = CalibrationValues[temp] / NumberOfCalibrationValues / SimpleDyno.BitsToVoltage;
            {
                var withBlock = Program.MainI;
                switch (CalibBtn.Name.ToString() ?? "")
                {
                    case var @case when @case == "btnCalibrateV1":
                    {
                        withBlock.SetControlText_Threadsafe(txtA0Voltage1, withBlock.NewCustomFormat(CalibrationValues[0]));
                        txtA0Voltage1_Leave(txtA0Voltage1, tempeventargs);
                        break;
                    }
                    case var case1 when case1 == "btnCalibrateV2":
                    {
                        withBlock.SetControlText_Threadsafe(txtA0Voltage2, withBlock.NewCustomFormat(CalibrationValues[0]));
                        txtA0Voltage2_Leave(txtA0Voltage2, tempeventargs);
                        break;
                    }
                    // Note the current calibration needs to use the resistance
                    case var case2 when case2 == "btnCalibrateI1":
                    {
                        // use resistance and voltage to calibrate
                        if (SimpleDyno.Resistance1 != 0d)
                        {
                            withBlock.SetControlText_Threadsafe(txtA1Voltage1, withBlock.NewCustomFormat(CalibrationValues[1]));
                            withBlock.SetControlText_Threadsafe(txtInputCurrent1, withBlock.NewCustomFormat(CalibrationValues[0] / SimpleDyno.Resistance1));

                            txtA1Voltage1_Leave(txtA1Voltage1, tempeventargs);
                            txtInputCurrent1_Leave(txtInputCurrent1, tempeventargs);
                        }
                        else
                        {
                            withBlock.SetControlText_Threadsafe(txtA1Voltage1, withBlock.NewCustomFormat(CalibrationValues[1]));
                            txtA1Voltage1_Leave(txtA1Voltage1, tempeventargs);
                        }

                        break;
                    }
                    case var case3 when case3 == "btnCalibrateI2":
                    {
                        if (SimpleDyno.Resistance2 != 0)
                        {
                            withBlock.SetControlText_Threadsafe(txtA1Voltage2, withBlock.NewCustomFormat(CalibrationValues[1]));
                            withBlock.SetControlText_Threadsafe(txtInputCurrent2, withBlock.NewCustomFormat(SimpleDyno.VoltageIntercept + SimpleDyno.VoltageSlope * CalibrationValues[0] * SimpleDyno.BitsToVoltage / SimpleDyno.Resistance2));

                            txtA1Voltage2_Leave(txtA1Voltage2, tempeventargs);
                            txtInputCurrent2_Leave(txtInputCurrent2, tempeventargs);
                        }

                        else
                        {
                            withBlock.SetControlText_Threadsafe(txtA1Voltage2, withBlock.NewCustomFormat(CalibrationValues[1]));
                            txtA1Voltage2_Leave(txtA1Voltage2, tempeventargs);
                        }

                        break;
                    }
                    case var case4 when case4 == "btnCalibrateTemp1T1":
                    {
                        withBlock.SetControlText_Threadsafe(txtA2Voltage1, withBlock.NewCustomFormat(CalibrationValues[2]));
                        txtA2Voltage1_Leave(txtA2Voltage1, tempeventargs);
                        break;
                    }
                    case var case5 when case5 == "btnCalibrateTemp1T2":
                    {
                        withBlock.SetControlText_Threadsafe(txtA2Voltage2, withBlock.NewCustomFormat(CalibrationValues[2]));
                        txtA2Voltage2_Leave(txtA2Voltage2, tempeventargs);
                        break;
                    }
                    case var case6 when case6 == "btnCalibrateTemp2T1":
                    {
                        withBlock.SetControlText_Threadsafe(txtA3Voltage1, withBlock.NewCustomFormat(CalibrationValues[3]));
                        txtA3Voltage1_Leave(txtA3Voltage1, tempeventargs);
                        break;
                    }
                    case var case7 when case7 == "btnCalibrateTemp2T2":
                    {
                        withBlock.SetControlText_Threadsafe(txtA3Voltage2, withBlock.NewCustomFormat(CalibrationValues[3]));
                        txtA3Voltage2_Leave(txtA3Voltage2, tempeventargs);
                        break;
                    }
                    case var case8 when case8 == "btnCalibratePinA4V1":
                    {
                        withBlock.SetControlText_Threadsafe(txtA4Voltage1, withBlock.NewCustomFormat(CalibrationValues[4]));
                        txtA4Voltage1_Leave(txtA4Voltage1, tempeventargs);
                        break;
                    }
                    case var case9 when case9 == "btnCalibratePinA4V2":
                    {
                        withBlock.SetControlText_Threadsafe(txtA4Voltage2, withBlock.NewCustomFormat(CalibrationValues[4]));
                        txtA4Voltage1_Leave(txtA4Voltage2, tempeventargs);
                        break;
                    }
                    case var case10 when case10 == "btnCalibratePinA5V1":
                    {
                        withBlock.SetControlText_Threadsafe(txtA5Voltage1, withBlock.NewCustomFormat(CalibrationValues[5]));
                        txtA4Voltage1_Leave(txtA5Voltage1, tempeventargs);
                        break;
                    }
                    case var case11 when case11 == "btnCalibratePinA5V2":
                    {
                        withBlock.SetControlText_Threadsafe(txtA5Voltage2, withBlock.NewCustomFormat(CalibrationValues[5]));
                        txtA4Voltage1_Leave(txtA5Voltage2, tempeventargs);
                        break;
                    }
                }
            }
            NumberOfCalibrationValues = 0L;
        }

        private void txtA0Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A0Voltage2)
                {
                    // need to check that it is not the same as A1Voltage
                    SimpleDyno.A0Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A0Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A0Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A0Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputVoltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Voltage2)
                {
                    SimpleDyno.Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA0Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A0Voltage1)
                {
                    SimpleDyno.A0Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A0Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A0Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A0Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputVoltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Voltage1)
                {
                    SimpleDyno.Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA1Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A1Voltage2)
                {
                    SimpleDyno.A1Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A1Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A1Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A1Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputCurrent1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Current2)
                {
                    SimpleDyno.Current1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Current2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Current1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Current1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA1Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A1Voltage1)
                {
                    SimpleDyno.A1Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A1Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A1Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A1Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputCurrent2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Current1)
                {
                    SimpleDyno.Current2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Current1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Current2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Current2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA2Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A2Voltage2)
                {
                    SimpleDyno.A2Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A2Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A2Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A2Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputTemp1Temperature1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Temp1Temperature2)
                {
                    SimpleDyno.Temp1Temperature1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Temp1Temperature2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Temp1Temperature1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Temp1Temperature1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA2Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A2Voltage1)
                {
                    SimpleDyno.A2Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A2Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A2Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A2Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputTemp1Temperature2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Temp1Temperature1)
                {
                    SimpleDyno.Temp1Temperature2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Temp1Temperature1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Temp1Temperature2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Temp1Temperature2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA3Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A3Voltage2)
                {
                    SimpleDyno.A3Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A3Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A3Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A3Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputTemp2Temperature1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Temp2Temperature2)
                {
                    SimpleDyno.Temp2Temperature1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Temp2Temperature2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + "Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Temp2Temperature1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Temp2Temperature1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA3Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A3Voltage1)
                {
                    SimpleDyno.A3Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A3Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A3Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A3Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtInputTemp2Temperature2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.Temp2Temperature1)
                {
                    SimpleDyno.Temp2Temperature2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.Temp2Temperature1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.Temp2Temperature2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.Temp2Temperature2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtResistance1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                SimpleDyno.Resistance1 = TempDouble;
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = SimpleDyno.Resistance1.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtResistance2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                SimpleDyno.Resistance2 = TempDouble;
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = SimpleDyno.Resistance2.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtA4Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A4Voltage2)
                {
                    SimpleDyno.A4Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A4Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A4Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A4Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA4Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A4Voltage1)
                {
                    SimpleDyno.A4Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A4Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A4Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A4Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtPin4Value1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A4Value2)
                {
                    SimpleDyno.A4Value1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A4Value2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A4Value1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A4Value1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtPin4Value2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A4Value1)
                {
                    SimpleDyno.A4Value2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A4Value1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A4Value2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A4Value2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA5Voltage1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A5Voltage2)
                {
                    SimpleDyno.A5Voltage1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A5Voltage2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A5Voltage1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A5Voltage1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtA5Voltage2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 5d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A5Voltage1)
                {
                    SimpleDyno.A5Voltage2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A5Voltage1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A5Voltage2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A5Voltage2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtPin5Value1_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A5Value2)
                {
                    SimpleDyno.A5Value1 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A5Value2)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A5Value1.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A5Value1 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }
        private void txtPin5Value2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            // if mainform loaded, need to validate the entry and check that values 1 and 2 in the calibration curve are different
            if (SimpleDyno.Formloaded)
            {
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) && TempDouble != SimpleDyno.A5Value1)
                {
                    SimpleDyno.A5Value2 = TempDouble;
                    Program.MainI.COMPortCalibration();
                }
                else
                {
                    if (TempDouble == SimpleDyno.A5Value1)
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " Values 1 and 2 must be different for calibration", MsgBoxStyle.Exclamation);
                    }
                    else
                    {
                        Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    }
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = SimpleDyno.A5Value2.ToString();
                        withBlock.Focus();
                    }
                }
            }
            else
            {
                double.TryParse(((TextBox)sender).Text, out TempDouble);
                SimpleDyno.A5Value2 = TempDouble;
                Program.MainI.COMPortCalibration();
            }
        }

        private void A5PowerRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SimpleDyno.A5PowerRunControl = A5PowerRunCheckBox.Checked;
        }
    }
}
