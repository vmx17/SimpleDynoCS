using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{
    public partial class Fit : Form
    {
        // CHECK - This needs to be reset to 0 for release versions
        /* TODO ERROR: Skipped DefineDirectiveTrivia */
//      #define LoadOldPowerRunData
        private string[] AvailableFits = new[] { "Four Parameter", "2nd Order Poly", "3rd Order Poly", "4th Order Poly", "5th Order Poly", "MA Smooth", "MLSQ Linfit", "None" }; // "Test"} ', "Simple Smoothing"}
        private int FitStartPoint = 1;
        private double CurrentSmooth;
        private double VoltageSmooth;
        private double RPM1Smooth;
        private double CoastDownSmooth;
        private int MaxPosition;
        private int MinAllowableDataPoints = 10;
        public static double PowerRunSpikeLevel;
        private double TempDouble;
        private double MaxRunDownRPM;
        // Dim CopyRPM1Data(Program.MainI.MAXDATAPOINTS) As Double
        private double[] x = new double[1]; // Program.MainI.MAXDATAPOINTS) As Double
        private double[] y = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double
        private double[] fy = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double

        private double[] Vx = new double[1]; // Program.MainI.MAXDATAPOINTS) As Double
        private double[] Vy = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double
        private double[] Vfy = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double

        private double[] Ix = new double[1]; // Program.MainI.MAXDATAPOINTS) As Double
        private double[] Iy = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double
        private double[] Ify = new double[1]; // (Program.MainI.MAXDATAPOINTS) As Double


        private double[] CoastDownX = new double[1];
        private double[] CoastDownY = new double[1];
        private double[] CoastDownFY = new double[1];
        private double[] CoastDownP = new double[1];
        private double[] CoastDownT = new double[1];
        // Dim fx(Program.MainI.MAXDATAPOINTS) As Double
        private double[] c = new double[1];
        public static bool blnfit = false;
        private bool blnRPMFit = false;
        private bool blnCoastDownDownFit = false;
        private bool blnVoltageFit = false;
        private bool blnCurrentFit = false;
        private Font LabelFont = new Font("Arial", 10);
        private int WhichFitData = 0;
        private int RPM = 0;
        private int RUNDOWN = 1;
        private int CURRENT = 2;
        private int VOLTAGE = 3;
        private double[,] FitData = new double[SimpleDyno.LAST + 1, SimpleDyno.MAXDATAPOINTS + 1]; // CHECK adding one additional primary dimension to hold residuals
        private StreamReader inputfile;
        private OpenFileDialog inputdialog = new OpenFileDialog();
        private int interruptAutoCloseEventCounter = 0;
        private const int EVENTS_TO_INTERRUPT_AUTOCLOSE = 50;

        /// <summary>
        /// Constructors
        /// </summary>
        public Fit()
        {
            InitializeComponent();
        }

        internal void Fit_Setup()
        {
            cmbWhichFit.Items.AddRange(AvailableFits);
            cmbWhichFit.SelectedIndex = 1; // Second Order - fastest initial
            cmbWhichRDFit.Items.AddRange(AvailableFits);
            cmbWhichRDFit.SelectedIndex = 1; // Second Order - fastest initial
            VoltageSmooth = (scrlVoltageSmooth.Maximum + 1 - scrlVoltageSmooth.Value) / 2d;
            CurrentSmooth = (scrlCurrentSmooth.Maximum + 1 - scrlCurrentSmooth.Value) / 2d;
            RPM1Smooth = (scrlRPM1Smooth.Maximum + 1 - scrlRPM1Smooth.Value) / 2d;
            CoastDownSmooth = (scrlCoastDownSmooth.Maximum + 1 - scrlCoastDownSmooth.Value) / 2d;
        }
        private void Fit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                Hide();
                e.Cancel = true;
                Program.MainI.RestartForms();
                Program.MainI.btnShow_Click(this, EventArgs.Empty);
            }
        }
        private void scrlStartFit_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                FitStartPoint = scrlStartFit.Maximum + 1 - scrlStartFit.Value;
                txtFitStart.Text = FitStartPoint.ToString();
                rdoRPM1.Checked = true;
                ProcessData();
            }
        }
        private void scrlCurrentSmooth_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                CurrentSmooth = (scrlCurrentSmooth.Maximum + 1 - scrlCurrentSmooth.Value) / 2d;
                txtCurrentSmooth.Text = CurrentSmooth.ToString();
                rdoCurrent.Checked = true;
                FitCurrent();
            }
        }
        private void scrlVoltageSmooth_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                VoltageSmooth = (scrlVoltageSmooth.Maximum + 1 - scrlVoltageSmooth.Value) / 2d;
                txtVoltageSmooth.Text = VoltageSmooth.ToString();
                rdoVoltage.Checked = true;
                FitVoltage();
            }
        }
        private void scrlRPM1Smooth_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                RPM1Smooth = (scrlRPM1Smooth.Maximum + 1 - scrlRPM1Smooth.Value) / 2d;
                txtRPM1Smooth.Text = RPM1Smooth.ToString();
                FitRPMData();
            }
        }
        private void scrlCoastDownSmooth_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                CoastDownSmooth = (scrlCoastDownSmooth.Maximum + 1 - scrlCoastDownSmooth.Value) / 2d;
                txtCoastDownSmooth.Text = CoastDownSmooth.ToString();
                FitRPMRunDownData();
            }
        }
        private void txtPowerRunSpikeLevel_Leave(object sender, EventArgs e)
        {
            if (SimpleDyno.Formloaded)
            {
                double LocalMin = 1d;
                double LocalMax = 999999d;
                if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
                {
                    PowerRunSpikeLevel = TempDouble;
                    rdoRPM1.Checked = true;
                    ProcessData();
                }
                else
                {
                    Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                    {
                        var withBlock = (TextBox)sender;
                        withBlock.Text = PowerRunSpikeLevel.ToString();
                        withBlock.Focus();
                    }
                }
            }
        }
        #region Plotting Section
        private void pnlDataWindowSetup()
        {
            // Need to resize the graphics panel to suit the screen size
            Size = Screen.PrimaryScreen.WorkingArea.Size;
            int PicDataWidth;
            int PicDataHeight;

            {
                var withBlock = pnlDataWindow;
                withBlock.Width = ClientSize.Width - withBlock.Left - 10;
                withBlock.Height = ClientSize.Height - withBlock.Top - 10;
                PicDataHeight = withBlock.Height;
                PicDataWidth = withBlock.Width;
            }

            var DataBitMap = new Bitmap(PicDataWidth, PicDataHeight);
            var DataWindowBMP = Graphics.FromImage(DataBitMap);

            double XLeft = 0.1d * PicDataWidth;
            double XRight = 0.9d * PicDataWidth;
            double Ytop = 0.1d * PicDataHeight;
            double YBottom = 0.9d * PicDataHeight;

            var AxesPen = new Pen(Color.Black);
            var AxisBrush = new SolidBrush(AxesPen.Color);
            var RawDataPen = new Pen(Color.Green, 2f);
            var CopyDataPen = new Pen(Color.Black, 2f);
            var RawDataBrush = new SolidBrush(RawDataPen.Color);
            var FitDataPen = new Pen(Color.Red, 2f);
            var FitDataBrush = new SolidBrush(FitDataPen.Color);
            var TempTorqueDataPen = new Pen(Color.LightBlue, 2f);
            var TempPowerDataPen = new Pen(Color.LightGray, 2f);
            var TempTorqueDataBrush = new SolidBrush(TempTorqueDataPen.Color);
            var TempPowerDataBrush = new SolidBrush(TempPowerDataPen.Color);


            double RawDataMax = 0d;
            double RawDataMin = 999999d;
            double RawPDataMax = 0d;
            double RawPDataMin = 999999d;
            double RawTDataMax = 0d;
            double RawTDataMin = 999999d;
            double FitDataMax = 0d;
            double FitDataMin = 999999d;
            var WhichDimension = default(int);
            string YAxisLabel = "";

            var XTimeMax = default(double); // = x(UBound(x)) 'FitData(Main.SESSIONTIME, UBound(FitData, 2))
            var XTimeMin = default(double); // = x(1)

            switch (WhichFitData)
            {
                case var @case when @case == RPM:
                    {
                        XTimeMax = x[Information.UBound(x)];
                        XTimeMin = x[1];
                        // Debug.Print("xmax " & XTimeMax.ToString & "  xmin " & XTimeMin.ToString)
                        WhichDimension = SimpleDyno.RPM1_ROLLER;
                        YAxisLabel = "RPM";
                        for (int Count = 1, loopTo = Information.UBound(y); Count <= loopTo; Count++)
                        {
                            if (y[Count] > RawDataMax)
                                RawDataMax = y[Count];
                            if (y[Count] < RawDataMin)
                                RawDataMin = y[Count];
                            if (FitData[SimpleDyno.TORQUE_ROLLER, Count] > RawTDataMax)
                                RawTDataMax = FitData[SimpleDyno.TORQUE_ROLLER, Count];
                            if (FitData[SimpleDyno.TORQUE_ROLLER, Count] < RawTDataMin)
                                RawTDataMin = FitData[SimpleDyno.TORQUE_ROLLER, Count];
                            if (FitData[SimpleDyno.POWER, Count] > RawPDataMax)
                                RawPDataMax = FitData[SimpleDyno.POWER, Count];
                            if (FitData[SimpleDyno.POWER, Count] < RawPDataMin)
                                RawPDataMin = FitData[SimpleDyno.POWER, Count];
                        }

                        break;
                    }
                // Debug.Print("Tdatmax " & RawTDataMax.ToString & " : Tdatamin " & RawTDataMin.ToString)
                case var case1 when case1 == RUNDOWN:
                    {
                        XTimeMax = CoastDownX[Information.UBound(CoastDownX)];
                        XTimeMin = CoastDownX[1];
                        WhichDimension = SimpleDyno.RPM1_ROLLER;
                        YAxisLabel = "RPM";
                        for (int Count = 1, loopTo1 = Information.UBound(CoastDownY); Count <= loopTo1; Count++)
                        {
                            if (CoastDownY[Count] > RawDataMax)
                                RawDataMax = CoastDownY[Count];
                            if (CoastDownY[Count] < RawDataMin)
                                RawDataMin = CoastDownY[Count];
                            if (CoastDownT[Count] > RawTDataMax)
                                RawTDataMax = CoastDownT[Count];
                            if (CoastDownT[Count] < RawTDataMin)
                                RawTDataMin = CoastDownT[Count];
                            if (CoastDownP[Count] > RawPDataMax)
                                RawPDataMax = CoastDownP[Count];
                            if (CoastDownP[Count] < RawPDataMin)
                                RawPDataMin = CoastDownP[Count];
                        }

                        break;
                    }
                case var case2 when case2 == CURRENT:
                    {
                        XTimeMax = x[Information.UBound(x)];
                        XTimeMin = x[1];
                        // XTimeMax = Ix(UBound(Ix))
                        // XTimeMin = Ix(1)
                        WhichDimension = SimpleDyno.AMPS;
                        YAxisLabel = "Amps";
                        for (int Count = 1, loopTo2 = Information.UBound(FitData, 2); Count <= loopTo2; Count++)
                        {
                            if (SimpleDyno.CollectedData[WhichDimension, Count] > RawDataMax)
                                RawDataMax = SimpleDyno.CollectedData[WhichDimension, Count];
                            if (SimpleDyno.CollectedData[WhichDimension, Count] < RawDataMin)
                                RawDataMin = SimpleDyno.CollectedData[WhichDimension, Count];
                        }

                        break;
                    }
                case var case3 when case3 == VOLTAGE:
                    {
                        XTimeMax = x[Information.UBound(x)];
                        XTimeMin = x[1];
                        // XTimeMax = Vx(UBound(Vx))
                        // XTimeMin = Vx(1)
                        WhichDimension = SimpleDyno.VOLTS;
                        YAxisLabel = "Volts";
                        for (int Count = 1, loopTo3 = Information.UBound(FitData, 2); Count <= loopTo3; Count++)
                        {
                            if (SimpleDyno.CollectedData[WhichDimension, Count] > RawDataMax)
                                RawDataMax = SimpleDyno.CollectedData[WhichDimension, Count];
                            if (SimpleDyno.CollectedData[WhichDimension, Count] < RawDataMin)
                                RawDataMin = SimpleDyno.CollectedData[WhichDimension, Count];
                        }

                        break;
                    }
            }


            int TickLength = 5;
            string TickString = "";
            var GraphicsFont = new Font("Arial", 10f);
            double XTickInterval = 0d;
            double YTickInterval = 0d;

            DataWindowBMP.Clear(Color.White);
            DataWindowBMP.DrawLine(AxesPen, (int)Math.Round(XLeft), (int)Math.Round(Ytop), (int)Math.Round(XLeft), (int)Math.Round(YBottom)); // Draw Left Y Axis
            DataWindowBMP.DrawLine(AxesPen, (int)Math.Round(XLeft), (int)Math.Round(YBottom), (int)Math.Round(XRight), (int)Math.Round(YBottom)); // Draw Bottom X Axis  
            XTickInterval = (XRight - XLeft) / 5d;
            YTickInterval = (YBottom - Ytop) / 5d;
            for (int Count = 0; Count <= 4; Count++)
            {
                // Left Y ticks 
                DataWindowBMP.DrawLine(AxesPen, (int)Math.Round(XLeft - TickLength), (int)Math.Round(Ytop + YTickInterval * Count), (int)Math.Round(XLeft), (int)Math.Round(Ytop + YTickInterval * Count));
                TickString = Program.MainI.NewCustomFormat(((RawDataMax - RawDataMin) / 5d * (5 - Count) + RawDataMin) * SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1]); // .ToString(Main.CustomFormat((YRollerRPMAxis / 5) * (5 - Count)))
                DataWindowBMP.DrawString(TickString, GraphicsFont, AxisBrush, (int)Math.Round(XLeft - (double)DataWindowBMP.MeasureString(TickString, GraphicsFont).Width - TickLength), (int)Math.Round(Ytop + YTickInterval * Count - (double)(DataWindowBMP.MeasureString(TickString, GraphicsFont).Height / 2f)));
                // Bottom X Ticks
                DataWindowBMP.DrawLine(AxesPen, (int)Math.Round(XLeft + XTickInterval * (Count + 1)), (int)Math.Round(YBottom), (int)Math.Round(XLeft + XTickInterval * (Count + 1)), (int)Math.Round(YBottom + TickLength));
                TickString = Program.MainI.NewCustomFormat((XTimeMax - XTimeMin) / 5d * (Count + 1) + XTimeMin); // .ToString(Main.CustomFormat(XTimeAxis / 5 * (Count + 1)))
                DataWindowBMP.DrawString(TickString, GraphicsFont, AxisBrush, (int)Math.Round(XLeft + XTickInterval * (Count + 1)) - DataWindowBMP.MeasureString(TickString, GraphicsFont).Width / 2f, (int)Math.Round(YBottom) + TickLength);
            }
            // X-Axis Title
            DataWindowBMP.DrawString("Seconds", GraphicsFont, AxisBrush, (int)Math.Round(PicDataWidth / 2d) - DataWindowBMP.MeasureString("Seconds", GraphicsFont).Width / 2f, (int)Math.Round(YBottom) + TickLength * 2);
            // y-axis Title
            DataWindowBMP.DrawString(YAxisLabel, GraphicsFont, AxisBrush, (int)Math.Round(XLeft) - DataWindowBMP.MeasureString(YAxisLabel, GraphicsFont).Width / 2f, (int)Math.Round(Ytop) - (int)Math.Round((double)DataWindowBMP.MeasureString(YAxisLabel, GraphicsFont).Height * 1.5d));

            DataWindowBMP.DrawString("RAW Data", GraphicsFont, Brushes.Green, 10f, 5f);
            DataWindowBMP.DrawString("FIT Data", GraphicsFont, Brushes.Red, 90f, 5f);
            DataWindowBMP.DrawString("Torque Curve (Max = " + Program.MainI.NewCustomFormat(RawTDataMax) + ")", GraphicsFont, TempTorqueDataBrush, 170f, 5f);
            DataWindowBMP.DrawString("Power Curve (Max = " + Program.MainI.NewCustomFormat(RawPDataMax) + ")", GraphicsFont, TempPowerDataBrush, 350f, 5f);


            // Draw Raw data
            if (SimpleDyno.DataPoints > 1) // And blnfit Then
            {
                switch (WhichFitData)
                {
                    case var case4 when case4 == RPM:
                    {
                        for (int Count = 1, loopTo4 = Information.UBound(y) - 1; Count <= loopTo4; Count++)
                        {
                            DataWindowBMP.DrawLine(RawDataPen, (int)Math.Round(XLeft + (x[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (y[Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (x[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (y[Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            if (blnRPMFit)
                            {
                                DataWindowBMP.DrawLine(FitDataPen, (int)Math.Round(XLeft + (x[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (fy[Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (x[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (fy[Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                                // CHECK - get rid of this try catch block when done
                                try
                                {
                                    DataWindowBMP.DrawLine(TempTorqueDataPen, (int)Math.Round(XLeft + (x[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[SimpleDyno.TORQUE_ROLLER, Count] - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (x[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[SimpleDyno.TORQUE_ROLLER, Count + 1] - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)));
                                }
                                catch (Exception ex)
                                {
                                    Debugger.Break();
                                }
                                DataWindowBMP.DrawLine(TempPowerDataPen, (int)Math.Round(XLeft + (x[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[SimpleDyno.POWER, Count] - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (x[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[SimpleDyno.POWER, Count + 1] - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)));
                            }
                        }

                        break;
                    }
                    case var case5 when case5 == RUNDOWN:
                    {
                        for (int Count = 1, loopTo5 = Information.UBound(CoastDownY) - 1; Count <= loopTo5; Count++)
                        {
                            DataWindowBMP.DrawLine(RawDataPen, (int)Math.Round(XLeft + (CoastDownX[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownY[Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (CoastDownX[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownY[Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            if (blnCoastDownDownFit)
                            {
                                DataWindowBMP.DrawLine(FitDataPen, (int)Math.Round(XLeft + (CoastDownX[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownFY[Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (CoastDownX[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownFY[Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                                DataWindowBMP.DrawLine(TempTorqueDataPen, (int)Math.Round(XLeft + (CoastDownX[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownT[Count] - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (CoastDownX[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownT[Count + 1] - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)));
                                DataWindowBMP.DrawLine(TempPowerDataPen, (int)Math.Round(XLeft + (CoastDownX[Count] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownP[Count] - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + (CoastDownX[Count + 1] - XTimeMin) / (XTimeMax - XTimeMin) * (XRight - XLeft)), (int)Math.Round(YBottom - (CoastDownP[Count + 1] - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)));
                            }
                        }

                        break;
                    }
                    case var case6 when case6 == VOLTAGE:
                    {
                        for (int Count = 1, loopTo6 = Information.UBound(y) - 1; Count <= loopTo6; Count++)
                        {
                            DataWindowBMP.DrawLine(RawDataPen, (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (SimpleDyno.CollectedData[WhichDimension, Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count + 1] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (SimpleDyno.CollectedData[WhichDimension, Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            if (blnVoltageFit)
                            {
                                DataWindowBMP.DrawLine(FitDataPen, (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[WhichDimension, Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count + 1] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[WhichDimension, Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            }
                        }

                        break;
                    }
                    case var case7 when case7 == CURRENT:
                    {
                        for (int Count = 1, loopTo7 = Information.UBound(y) - 1; Count <= loopTo7; Count++)
                        {
                            DataWindowBMP.DrawLine(RawDataPen, (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (SimpleDyno.CollectedData[WhichDimension, Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count + 1] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (SimpleDyno.CollectedData[WhichDimension, Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            if (blnCurrentFit)
                            {
                                DataWindowBMP.DrawLine(FitDataPen, (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[WhichDimension, Count] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), (int)Math.Round(XLeft + SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count + 1] / XTimeMax * (XRight - XLeft)), (int)Math.Round(YBottom - (FitData[WhichDimension, Count + 1] * SimpleDyno.DataUnits[WhichDimension, 0] - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)));
                            }
                        }

                        break;
                    }
                }


                // If Main.DataPoints > 1 And blnfit Then
                // 'For Count As Integer = 1 To UBound(fy) - 1 'UBound(FitData, 2) - 1
                // Select Case WhichFitData
                // Case Is = RPM
                // For Count As Integer = 1 To UBound(y) - 1
                // .DrawLine(FitDataPen, CInt(XLeft + ((x(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (fy(Count) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), CInt(XLeft + ((x(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (fy(Count + 1) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)))
                // .DrawLine(TempTorqueDataPen, CInt(XLeft + ((x(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (FitData(Main.TORQUE_ROLLER, Count) - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)), CInt(XLeft + ((x(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (FitData(Main.TORQUE_ROLLER, Count + 1) - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)))
                // .DrawLine(TempPowerDataPen, CInt(XLeft + ((x(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (FitData(Main.POWER, Count) - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)), CInt(XLeft + ((x(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (FitData(Main.POWER, Count + 1) - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)))
                // Next
                // Case Is = RUNDOWN
                // For Count As Integer = 1 To UBound(CoastDownY) - 1
                // .DrawLine(FitDataPen, CInt(XLeft + ((CoastDownX(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (CoastDownFY(Count) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), CInt(XLeft + ((CoastDownX(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (CoastDownFY(Count + 1) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)))
                // .DrawLine(TempTorqueDataPen, CInt(XLeft + ((CoastDownX(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (CoastDownT(Count) - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)), CInt(XLeft + ((CoastDownX(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (CoastDownT(Count + 1) - RawTDataMin) / (RawTDataMax - RawTDataMin) * (YBottom - Ytop)))
                // .DrawLine(TempPowerDataPen, CInt(XLeft + ((CoastDownX(Count) - XTimeMin) / (XTimeMax - XTimeMin)) * (XRight - XLeft)), CInt(YBottom - (CoastDownP(Count) - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)), CInt(XLeft + ((CoastDownX(Count + 1) - XTimeMin)) / (XTimeMax - XTimeMin) * (XRight - XLeft)), CInt(YBottom - (CoastDownP(Count + 1) - RawPDataMin) / (RawPDataMax - RawPDataMin) * (YBottom - Ytop)))
                // Next
                // Case Is = VOLTAGE, CURRENT
                // For Count As Integer = 1 To UBound(y) - 1
                // .DrawLine(FitDataPen, CInt(XLeft + ((Main.CollectedData(Main.SESSIONTIME, Count)) / XTimeMax) * (XRight - XLeft)), CInt(YBottom - (FitData(WhichDimension, Count) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)), CInt(XLeft + ((Main.CollectedData(Main.SESSIONTIME, Count + 1)) / XTimeMax) * (XRight - XLeft)), CInt(YBottom - (FitData(WhichDimension, Count + 1) * Main.DataUnits(WhichDimension, 0) - RawDataMin) / (RawDataMax - RawDataMin) * (YBottom - Ytop)))
                // Next
                // End Select
                // End If

            }

            pnlDataWindow.BackgroundImage = DataBitMap;
            pnlDataWindow.Invalidate();

        }
        #endregion
        #region Mathematical Curve Fitting Section
        internal void ProcessData()
        {
            try
            {
                do // Loop until the RPM data falls below the Power Run Threshold at which time WhichDataMode will go to LIVE
                    Application.DoEvents();
                // Debug.Print(Main.WhichDataMode.ToString)
                while (SimpleDyno.WhichDataMode != SimpleDyno.LIVE);

                if (SimpleDyno.StopFitting == false)
                {

                    blnfit = false;
                    blnRPMFit = false;
                    blnCoastDownDownFit = false;
                    blnVoltageFit = false;
                    blnCurrentFit = false;

                    Program.MainI.btnHide_Click(this, EventArgs.Empty);
                    Show();


                    // //////////////////////////////////////////////
                    // This section allows you to run a dummy fit if
                    // if LoadPowerRunData Const is 1
                    // //////////////////////////////////////////////
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If LoadOldPowerRunData = 1 Then
                    *//* TODO ERROR: Skipped DisabledTextTrivia
                                    'If Main.GearRatio = 999.0 AndAlso inputdialog.FileName = "" Then
                                    setxyc() 'reads an existing data file for the raw RPM numbers
                                    'End If
                    *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */                // Copy all collected data to fit data
                    FitData = new double[37, SimpleDyno.DataPoints + 1];
                    int t;
                    int count;
                    for (t = 0; t <= SimpleDyno.LAST - 1; t++)
                    {
                        var loopTo = SimpleDyno.DataPoints;
                        for (count = 1; count <= loopTo; count++)  // - FitStartPoint + 1
                            FitData[t, count] = SimpleDyno.CollectedData[t, count + FitStartPoint - 1];
                    }

                    // Always Fit the RPM1 Data
                    FitRPMData();

                    if (SimpleDyno.frmCorrection.chkUseRunDown.Checked == true)
                    {
                        FitRPMRunDownData();
                    }
                    else
                    {
                        blnCoastDownDownFit = true;
                    }

                    if (rdoVoltage.Enabled == true)
                    {
                        FitVoltage();
                    }
                    else
                    {
                        blnVoltageFit = true;
                    }

                    if (rdoCurrent.Enabled == true)
                    {
                        FitCurrent();
                    }
                    else
                    {
                        blnCurrentFit = true;
                    }

                    rdoRPM1.Checked = true;
                    rdoRPM1_CheckedChanged(this, EventArgs.Empty);

                    Application.DoEvents();
                    string originalTitle = Text;
                    interruptAutoCloseEventCounter = 0;
                    for (t = 0; t <= 50; t++)
                    {
                        Text = originalTitle + " - Auto-exiting in " + ((int)Math.Round((50 - t) / 10d)).ToString() + " seconds.. Interrupt by moving the mouse";
                        System.Threading.Thread.Sleep(100);
                        Application.DoEvents();
                        if (interruptAutoCloseEventCounter > EVENTS_TO_INTERRUPT_AUTOCLOSE)
                            break;
                    }
                    Text = originalTitle;

                    if (interruptAutoCloseEventCounter <= EVENTS_TO_INTERRUPT_AUTOCLOSE)
                    {
                        btnDone_Click(null, null);
                    }

                }
                Program.MainI.RestartForms();
            }
            catch (Exception e)
            {
                Interaction.MsgBox("ProcessData Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
        }
        public void FitRPMData()
        {

            double yoffset;
            double xoffset;
            int count;
            var count2 = default(int);
            var RawRPM1Max = default(double);

            y = new double[SimpleDyno.DataPoints + 1];
            x = new double[SimpleDyno.DataPoints + 1];

            // Flag to the code and to the user that we are fitting the data
            SimpleDyno.ProcessingData = true;
            Program.MainI.Cursor = Cursors.WaitCursor;
            Cursor = Cursors.WaitCursor;

            // x(0) = Main.CollectedData(Main.SESSIONTIME, FitStartPoint - 1)
            // y(0) = Main.CollectedData(Main.RPM1_ROLLER, FitStartPoint - 1)

            PowerRunSpikeLevel = Conversions.ToDouble(txtPowerRunSpikeLevel.Text);
            lblProgress.Text = "RPM1 Spike removal...";
            prgFit.Maximum = SimpleDyno.DataPoints;
            var loopTo = SimpleDyno.DataPoints;
            for (count = FitStartPoint; count <= loopTo; count++)
            {
                prgFit.Value = count;
                count2 += 1;
                y[count2] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, count];
                x[count2] = SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, count];
                if (Math.Abs(y[count2] - y[count2 - 1]) * SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1] > PowerRunSpikeLevel)
                {
                    y[count2] = y[count2 - 1]; // Main.CollectedData(Main.RPM1_ROLLER, count - 1)
                }
                else if (y[count2] > RawRPM1Max)
                {
                    MaxPosition = count2;
                    RawRPM1Max = y[count2];
                }
            }
            prgFit.Value = prgFit.Maximum;
            lblProgress.Text = "Done";
            // Then reset FitData to that size and copy all of the Collected data to that array.

            var oldFitData = FitData;
            FitData = new double[37, MaxPosition + 1];
            if (oldFitData is not null)
                for (var i = 0; i <= oldFitData.Length / oldFitData.GetLength(1) - 1; ++i)
                    Array.Copy(oldFitData, i * oldFitData.GetLength(1), FitData, i * FitData.GetLength(1), Math.Min(oldFitData.GetLength(1), FitData.GetLength(1))); // - FitStartPoint + 1)
            Array.Resize(ref y, MaxPosition + 1); // - FitStartPoint + 1)
            Array.Resize(ref x, MaxPosition + 1);
            fy = new double[MaxPosition + 1];
            // ReDim fx(MaxPosition)

            // the x and y value arrays sent to NonlinFit are working copies of the raw data
            // To fit using NonLin, x and y arrays need to start at the origin (0,0) -CHECK THIS DOES NOT SEEM TO BE TRUE FOR RUNDOWN
            // This means subtracting the first x and y values from all pairs
            // we need to remember the yoffset to add it back later.  We can apply this to all fits for simplicity
            yoffset = y[1]; // FitData(Main.RPM1_ROLLER, 1)
                            // xoffset = x(1) 'the first time point should now always be zero so no need for offset removal'FitData(Main.SESSIONTIME, 1)

            var loopTo1 = Information.UBound(y);
            for (count = 1; count <= loopTo1; count++) // UBound(FitData, 2)
                                                       // x(count) = x(count) - xoffset 'FitData(Main.SESSIONTIME, Count) - xoffset 'FitData(Main.SESSIONTIME, 1)
                y[count] = y[count] - yoffset; // CopyRPM1Data(Count) - yoffset 'FitData(Main.RPM1_ROLLER, count) - yoffset

            blnfit = false;
            blnRPMFit = false;
            WhichFitData = RPM;
            pnlDataWindowSetup(); // Shows the user the data we are fitting.
            lblProgress.Text = "Fitting RPM1...";
            // DC 21JAN16 - Old code based on VBA source from Optimiz.xla
            // Note that the old code returns the function values for the fitted Y parameter
            // Call NonLin, sending the local copies of the data by ref
            NonLin_fitting(ref x, ref y, ref fy, cmbWhichFit.SelectedIndex + 1, ref blnRPMFit);

            // DC 21JAN16 - New code based on the work of Rod Stephens - Used with permission of the Author
            // BestCoeffs is a generic list of type double and holds the coefficients of the polynomial solution
            // Points is a generic list of System.Drawing.PointF and is the raw X and Y data to be fit
            // Degree is an integer and represents the degree of the fit.
            // Original line is:
            // BestCoeffs = FindPolynomialLeastSquaresFit(Points, degree)
            // blnRPMFit = FindPolynomialLeastSquaresFit_NEW(x, y, fy, cmbWhichFit.SelectedIndex + 1)

            if (blnRPMFit)
            {

                // fy() now contains the fit data. Copy it to FitData adding back the offsets
                // FitData(Main.RPM1_ROLLER, 1) = fy(1) + yoffset
                // FitData(Main.SESSIONTIME, 1) = x(1) + xoffset

                var loopTo2 = Information.UBound(y);
                for (count = 1; count <= loopTo2; count++) // UBound(FitData, 2)
                {
                    // x(count) = x(count) + xoffset
                    y[count] = y[count] + yoffset;
                    // fx(count) = fx(count) + xoffset
                    fy[count] = fy[count] + yoffset;
                    FitData[SimpleDyno.SESSIONTIME, count] = x[count];
                    FitData[SimpleDyno.RPM1_ROLLER, count] = fy[count];
                    // Setup power and torque temporarily
                    FitData[SimpleDyno.TORQUE_ROLLER, count] = (FitData[SimpleDyno.RPM1_ROLLER, count] - FitData[SimpleDyno.RPM1_ROLLER, count - 1]) / (FitData[SimpleDyno.SESSIONTIME, count] - FitData[SimpleDyno.SESSIONTIME, count - 1]) * SimpleDyno.DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                    FitData[SimpleDyno.POWER, count] = FitData[SimpleDyno.TORQUE_ROLLER, count] * FitData[SimpleDyno.RPM1_ROLLER, count]; // + FitData(Main.RPM1_ROLLER, count - 1)) / 2)
                }

                FitData[SimpleDyno.TORQUE_ROLLER, 1] = 0d; // fy(1) + yoffset
                FitData[SimpleDyno.POWER, 1] = 0d; // x(1) + xoffset

                if (SimpleDyno.frmCorrection.chkUseRunDown.Checked && blnCoastDownDownFit)
                    CreateCoastDownData();
                prgFit.Value = prgFit.Maximum;
                lblProgress.Text = "Done";
                Cursor = Cursors.Default;
                SimpleDyno.ProcessingData = false;
                Program.MainI.Cursor = Cursors.Default;
                blnfit = true;
                pnlDataWindowSetup();
            }
            else
            {
                // What are we going to do if the fit was not completed?
            }
        }
        public void FitRPMRunDownData()
        {

            int Count;
            int Count2;
            double RawRPM1Max;

            if (Correction.blnUsingLoadedRunDownFile == false)
            {

                lblUsingRunDownFile.Text = "No Coast Down File Loaded";

                SimpleDyno.ProcessingData = true;
                Program.MainI.Cursor = Cursors.WaitCursor;
                Cursor = Cursors.WaitCursor;

                CoastDownX = new double[SimpleDyno.DataPoints + 1];
                CoastDownY = new double[SimpleDyno.DataPoints + 1];

                // CoastDownY(0) = Main.CollectedData(Main.RPM1_ROLLER, Main.DataPoints)
                // CoastDownX(0) = Main.CollectedData(Main.SESSIONTIME, Main.DataPoints)
                PowerRunSpikeLevel = Conversions.ToDouble(txtPowerRunSpikeLevel.Text);
                Count2 = 0;
                RawRPM1Max = 0d;
                lblProgress.Text = "Coast Down Spike removal...";
                prgFit.Maximum = SimpleDyno.DataPoints;
                for (Count = SimpleDyno.DataPoints; Count >= 1; Count -= 1)
                {
                    prgFit.Value = Count2; // 
                    Count2 += 1;
                    CoastDownY[Count2] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count];
                    CoastDownX[Count2] = SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count];
                    if (Math.Abs(CoastDownY[Count2] - CoastDownY[Count2 - 1]) * SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1] > PowerRunSpikeLevel)
                    {
                        CoastDownY[Count2] = CoastDownY[Count2 - 1];
                    }
                    else if (CoastDownY[Count2] > RawRPM1Max)
                    {
                        MaxPosition = Count2;
                        RawRPM1Max = CoastDownY[Count2];
                    }
                }
                prgFit.Value = prgFit.Maximum;
                lblProgress.Text = "Done";
                // Then reset FitData to that size and copy all of the Collected data to that array.
                // ReDim FitData(Main.LAST, MaxPosition) '- FitStartPoint + 1) 'CHECK - DON'T MODIFY THIS AGAIN
                Array.Resize(ref CoastDownY, MaxPosition + 1 + 1); // - FitStartPoint + 1)
                Array.Resize(ref CoastDownX, MaxPosition + 1 + 1);
                CoastDownY[MaxPosition + 1] = 0d; // ensures that the 0 position is 0 when reverse
                CoastDownX[MaxPosition + 1] = 0d;
                CoastDownFY = new double[MaxPosition + 1];
                CoastDownP = new double[MaxPosition + 1];
                CoastDownT = new double[MaxPosition + 1];
                // ReDim fx(MaxPosition)

                // CHECK - This routine does not seem to need the offsets
                // Dim yoffset As Double, xoffset As Double, tempcopy(UBound(y)) As Double

                // Flag to the code and to the user that we are fitting the data

                lblProgress.Text = "Fitting Coast Down...";

                // x and y need to have their order reversed as they were pouplated from the end of the run to the top of the run down
                Array.Reverse(CoastDownX); // , ', 1, UBound(CoastDownX))
                Array.Reverse(CoastDownY); // , 1, UBound(CoastDownY))

                Array.Resize(ref CoastDownY, MaxPosition + 1); // - FitStartPoint + 1)
                Array.Resize(ref CoastDownX, MaxPosition + 1);

                blnfit = false;
                blnCoastDownDownFit = false;
                WhichFitData = RUNDOWN;
                pnlDataWindowSetup(); // Shows the user the data we are fitting.

                // Call NonLin, sending the local copies of the data by ref
                NonLin_fitting(ref CoastDownX, ref CoastDownY, ref CoastDownFY, cmbWhichRDFit.SelectedIndex + 1, ref blnCoastDownDownFit);
                if (blnCoastDownDownFit)
                {
                    // Calc the rundown torque and power
                    // Note these are based on RPM and not time which means we have to find the closest RPM in the power up section
                    // This is going to be a pain.
                    prgFit.Value = prgFit.Maximum;
                    lblProgress.Text = "Done";
                    Cursor = Cursors.Default;
                    SimpleDyno.ProcessingData = false;
                    Program.MainI.Cursor = Cursors.Default;
                    FitData[SimpleDyno.RPM1_ROLLER, 0] = FitData[SimpleDyno.RPM1_ROLLER, 1];
                    CoastDownFY[0] = CoastDownFY[1];
                    // Dim Difference As Double, MinDifference As Double, MinDifferenceIndex As Integer
                    // Need to find the closest value
                    // First need to find the highestt run down fit poin
                    // Dim MaxRunDownRPM As Double
                    var loopTo = Information.UBound(CoastDownFY);
                    for (Count = 1; Count <= loopTo; Count++)
                    {
                        if (CoastDownFY[Count] > MaxRunDownRPM)
                            MaxRunDownRPM = CoastDownFY[Count];
                        CoastDownT[Count] = -1 * (CoastDownFY[Count] - CoastDownFY[Count - 1]) / (CoastDownX[Count] - CoastDownX[Count - 1]) * SimpleDyno.DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                        CoastDownP[Count] = CoastDownT[Count] * CoastDownFY[Count]; // + CoastDownFY(Count - 1)) / 2)
                                                                                    // Debug.Print(CoastDownX(Count) & " " & CoastDownFY(Count) & " " & CoastDownT(Count) & " " & CoastDownP(Count))
                    }
                    CreateCoastDownData();
                    // MsgBox(MaxRunDownRPM.ToString, vbOKOnly)
                    // This section should be in its own routine as it needs to be called if the RPM1 data is refit.
                    // Also, if the spike threshold is changed, both RPM1 and Coast Down need to be refit.
                    // For Count = 1 To UBound(FitData, 2)
                    // If FitData(Main.RPM1_ROLLER, Count) >= MaxRunDownRPM Then
                    // FitData(Main.TORQUE_COASTDOWN, Count) = -1 * (CoastDownFY(2) - CoastDownFY(1)) / (CoastDownX(2) - CoastDownX(1)) * Main.DynoMomentOfInertia 'this is the roller torque, should calc the wheel and motor at this point also
                    // Else
                    // MinDifference = 999999
                    // For Count2 = 1 To UBound(CoastDownFY)
                    // Difference = Math.Abs(CoastDownFY(Count2) - FitData(Main.RPM1_ROLLER, Count))
                    // If Difference < MinDifference Then
                    // MinDifference = Difference
                    // MinDifferenceIndex = Count2
                    // End If
                    // Next
                    // FitData(Main.TORQUE_COASTDOWN, Count) = -1 * (CoastDownFY(MinDifferenceIndex) - CoastDownFY(MinDifferenceIndex + 1)) / (CoastDownX(MinDifferenceIndex) - CoastDownX(MinDifferenceIndex + 1)) * Main.DynoMomentOfInertia 'this is the roller torque, should calc the wheel and motor at this point also
                    // End If
                    // FitData(Main.POWER_COASTDOWN, Count) = FitData(Main.TORQUE_COASTDOWN, Count) * ((FitData(Main.RPM1_ROLLER, Count) + FitData(Main.RPM1_ROLLER, Count - 1)) / 2)
                    // Next
                    blnfit = true;
                    pnlDataWindowSetup();
                }
                else
                {
                    // what are we doing if the fit didn;t complete
                    Interaction.MsgBox("Skipped Fitdata", Constants.vbOKOnly);
                }
            }

            else
            {
                // read in the selected coast down file
                StreamReader CoastDownInputStream;
                string? temp;
                string[] tempsplit;
                int NumberOfPoints;
                CoastDownInputStream = new StreamReader(Correction.RunDownOpenFileDialog.FileName);
                lblUsingRunDownFile.Text = Correction.RunDownOpenFileDialog.FileName.Substring(Correction.RunDownOpenFileDialog.FileName.LastIndexOf(@"\") + 1);
                do
                    temp = CoastDownInputStream.ReadLine();
                while (!temp.StartsWith("NUMBER_OF_COAST_DOWN_POINTS_FIT"));
                NumberOfPoints = Conversions.ToInteger(temp.Substring(temp.LastIndexOf(" ")));
                CoastDownX = new double[NumberOfPoints + 1];
                CoastDownFY = new double[NumberOfPoints + 1];
                temp = CoastDownInputStream.ReadLine(); // titles
                var loopTo1 = NumberOfPoints;
                for (Count = 1; Count <= loopTo1; Count++)
                {
                    temp = CoastDownInputStream.ReadLine();
                    tempsplit = Strings.Split(temp, " ");
                    CoastDownX[Count] = Conversions.ToDouble(tempsplit[0]);
                    CoastDownFY[Count] = Conversions.ToDouble(tempsplit[1]);
                }
                FitData[SimpleDyno.RPM1_ROLLER, 0] = FitData[SimpleDyno.RPM1_ROLLER, 1];
                // Dim Difference As Double, MinDifference As Double, MinDifferenceIndex As Integer
                // Need to find the closest value
                // First need to find the highestt run down fit poin
                // Dim MaxRunDownRPM As Double
                var loopTo2 = Information.UBound(CoastDownFY);
                for (Count = 1; Count <= loopTo2; Count++)
                {
                    if (CoastDownFY[Count] > MaxRunDownRPM)
                        MaxRunDownRPM = CoastDownFY[Count];
                }
                CreateCoastDownData();
                // For Count = 1 To UBound(FitData, 2)
                // If FitData(Main.RPM1_ROLLER, Count) >= MaxRunDownRPM Then
                // FitData(Main.TORQUE_COASTDOWN, Count) = -1 * (CoastDownFY(2) - CoastDownFY(1)) / (CoastDownX(2) - CoastDownX(1)) * Main.DynoMomentOfInertia 'this is the roller torque, should calc the wheel and motor at this point also
                // Else
                // MinDifference = 999999
                // For Count2 = 1 To UBound(CoastDownFY)
                // Difference = Math.Abs(CoastDownFY(Count2) - FitData(Main.RPM1_ROLLER, Count))
                // If Difference < MinDifference Then
                // MinDifference = Difference
                // MinDifferenceIndex = Count2
                // End If
                // Next
                // FitData(Main.TORQUE_COASTDOWN, Count) = -1 * (CoastDownFY(MinDifferenceIndex) - CoastDownFY(MinDifferenceIndex + 1)) / (CoastDownX(MinDifferenceIndex) - CoastDownX(MinDifferenceIndex + 1)) * Main.DynoMomentOfInertia 'this is the roller torque, should calc the wheel and motor at this point also
                // End If
                // FitData(Main.POWER_COASTDOWN, Count) = FitData(Main.TORQUE_COASTDOWN, Count) * ((FitData(Main.RPM1_ROLLER, Count) + FitData(Main.RPM1_ROLLER, Count - 1)) / 2)
                // Next
                blnCoastDownDownFit = true;
            }



        }
        public void CreateCoastDownData()
        {
            // this section needs to be independent as it changes if either RPM1 or Coast Down fits change
            int Count;
            int Count2;
            double MinDifference;
            double difference;
            var MinDifferenceIndex = default(int);
            var loopTo = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo; Count++)
            {
                if (FitData[SimpleDyno.RPM1_ROLLER, Count] >= MaxRunDownRPM)
                {
                    FitData[SimpleDyno.TORQUE_COASTDOWN, Count] = -1 * (CoastDownFY[2] - CoastDownFY[1]) / (CoastDownX[2] - CoastDownX[1]) * SimpleDyno.DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                }
                else
                {
                    MinDifference = 999999d;
                    var loopTo1 = Information.UBound(CoastDownFY) - 1;
                    for (Count2 = 1; Count2 <= loopTo1; Count2++)
                    {
                        difference = Math.Abs(CoastDownFY[Count2] - FitData[SimpleDyno.RPM1_ROLLER, Count]);
                        if (difference < MinDifference)
                        {
                            MinDifference = difference;
                            MinDifferenceIndex = Count2;
                        }
                    }
                    try
                    {
                        FitData[SimpleDyno.TORQUE_COASTDOWN, Count] = -1 * (CoastDownFY[MinDifferenceIndex] - CoastDownFY[MinDifferenceIndex + 1]) / (CoastDownX[MinDifferenceIndex] - CoastDownX[MinDifferenceIndex + 1]) * SimpleDyno.DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                    }

                }
                FitData[SimpleDyno.POWER_COASTDOWN, Count] = FitData[SimpleDyno.TORQUE_COASTDOWN, Count] * FitData[SimpleDyno.RPM1_ROLLER, Count]; // + FitData(Main.RPM1_ROLLER, Count - 1)) / 2)
            }
        }
        public void FitVoltage()
        {
            SimpleDyno.ProcessingData = true;
            Program.MainI.Cursor = Cursors.WaitCursor;
            Cursor = Cursors.WaitCursor;
            lblProgress.Text = "Smoothing voltage...";
            VoltageSmooth = (scrlVoltageSmooth.Maximum + 1 - scrlVoltageSmooth.Value) / 2d;
            int Count;
            Vx = new double[Information.UBound(FitData, 2) + 1];
            Vy = new double[Information.UBound(FitData, 2) + 1];
            Vfy = new double[Information.UBound(FitData, 2) + 1];
            // Voltage
            var loopTo = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo; Count++)
                Vy[Count] = SimpleDyno.CollectedData[SimpleDyno.VOLTS, Count + FitStartPoint - 1];
            blnVoltageFit = false;
            MovingAverageSmooth(ref Vy, ref Vfy, VoltageSmooth); // Last Number is the smoothing window in %
            blnVoltageFit = true;

            var loopTo1 = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo1; Count++)
                FitData[SimpleDyno.VOLTS, Count] = Vfy[Count];

            // WritePowerFile()

            Cursor = Cursors.Default;
            SimpleDyno.ProcessingData = false;
            Program.MainI.Cursor = Cursors.Default;
            lblProgress.Text = "Done";
            blnfit = true;
            pnlDataWindowSetup();
        }
        public void FitCurrent()
        {
            SimpleDyno.ProcessingData = true;
            Program.MainI.Cursor = Cursors.WaitCursor;
            Cursor = Cursors.WaitCursor;
            lblProgress.Text = "Smoothing current...";
            CurrentSmooth = (scrlCurrentSmooth.Maximum + 1 - scrlCurrentSmooth.Value) / 2d;
            int Count;
            Ix = new double[Information.UBound(FitData, 2) + 1];
            Iy = new double[Information.UBound(FitData, 2) + 1];
            Ify = new double[Information.UBound(FitData, 2) + 1];
            // Current
            var loopTo = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo; Count++)
                Iy[Count] = SimpleDyno.CollectedData[SimpleDyno.AMPS, Count + FitStartPoint - 1];
            blnCurrentFit = false;
            MovingAverageSmooth(ref Iy, ref Ify, CurrentSmooth); // Last Number is the smoothing window in %
            blnCurrentFit = true;
            var loopTo1 = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo1; Count++)
                FitData[SimpleDyno.AMPS, Count] = Ify[Count];

            // WritePowerFile()

            Cursor = Cursors.Default;
            SimpleDyno.ProcessingData = false;
            Program.MainI.Cursor = Cursors.Default;
            lblProgress.Text = "Done";
            blnfit = true;
            pnlDataWindowSetup();
        }
        public void WritePowerFile()
        {
            var DataOutputFile = new StreamWriter(SimpleDyno.LogPowerRunDataFileName);
            int Count;

            // NOTE: The data files are space delimited
            // Write out the header information - CHECK - this needs to align with the V5 / V6 conversion and read in by overlay section
            // CHECK - this needs to be updated per version
            DataOutputFile.WriteLine(SimpleDyno.PowerRunVersion); // Confirms power curve and version
            DataOutputFile.WriteLine(SimpleDyno.LogPowerRunDataFileName + Constants.vbCrLf + Conversions.ToString(DateTime.Today) + Constants.vbCrLf);
            DataOutputFile.WriteLine("Acquisition: " + Program.MainI.cmbAcquisition.SelectedItem.ToString());
            DataOutputFile.WriteLine("Number_of_Channels: " + SimpleDyno.NUMBER_OF_CHANNELS.ToString());
            DataOutputFile.WriteLine("Sampling_Rate " + SimpleDyno.SAMPLE_RATE.ToString());
            if (Program.MainI.cmbCOMPorts.SelectedItem is not null)
            {
                DataOutputFile.WriteLine("COM_Port: " + Program.MainI.cmbCOMPorts.SelectedItem.ToString());
            }
            else
            {
                DataOutputFile.WriteLine("No_COM_Port_Selected");
            }
            if (Program.MainI.cmbBaudRate.SelectedItem is not null)
            {
                DataOutputFile.WriteLine("Baud_Rate: " + Program.MainI.cmbBaudRate.SelectedItem.ToString());
            }
            else
            {
                DataOutputFile.WriteLine("No_Baud_Rate_Selected");
            }
            DataOutputFile.WriteLine("Car_Mass: " + SimpleDyno.frmDyno.CarMass.ToString() + " grams");
            DataOutputFile.WriteLine("Frontal_Area: " + SimpleDyno.frmDyno.FrontalArea.ToString() + " mm2");
            DataOutputFile.WriteLine("Drag_Coefficient: " + SimpleDyno.frmDyno.DragCoefficient.ToString());
            DataOutputFile.WriteLine("Gear_Ratio: " + SimpleDyno.GearRatio.ToString());
            DataOutputFile.WriteLine("Wheel_Diameter: " + SimpleDyno.frmDyno.WheelDiameter.ToString() + " mm");
            DataOutputFile.WriteLine("Roller_Diameter: " + SimpleDyno.frmDyno.RollerDiameter.ToString() + " mm");
            DataOutputFile.WriteLine("Roller_Wall_Thickness: " + SimpleDyno.frmDyno.RollerWallThickness.ToString() + " mm");
            DataOutputFile.WriteLine("Roller_Mass: " + SimpleDyno.frmDyno.RollerMass.ToString() + " grams");
            DataOutputFile.WriteLine("Axle_Diameter: " + SimpleDyno.frmDyno.AxleDiameter.ToString() + " mm");
            DataOutputFile.WriteLine("Axle_Mass: " + SimpleDyno.frmDyno.AxleMass.ToString() + " grams");
            DataOutputFile.WriteLine("End_Cap_Mass: " + SimpleDyno.frmDyno.EndCapMass.ToString() + " grams");
            DataOutputFile.WriteLine("Extra_Diameter: " + SimpleDyno.frmDyno.ExtraDiameter.ToString() + " mm");
            DataOutputFile.WriteLine("Extra_Wall_Thickness: " + SimpleDyno.frmDyno.ExtraWallThickness.ToString() + " mm");
            DataOutputFile.WriteLine("Extra_Mass: " + SimpleDyno.frmDyno.ExtraMass.ToString() + " grams");
            DataOutputFile.WriteLine("Target_MOI: " + SimpleDyno.IdealMomentOfInertia.ToString() + " kg/m2");
            DataOutputFile.WriteLine("Actual_MOI: " + SimpleDyno.DynoMomentOfInertia.ToString() + " kg/m2");
            DataOutputFile.WriteLine("Target_Roller_Mass: " + SimpleDyno.IdealRollerMass.ToString() + " grams");
            DataOutputFile.WriteLine("Signals_Per_RPM1: " + SimpleDyno.frmDyno.SignalsPerRPM.ToString());
            DataOutputFile.WriteLine("Signals_Per_RPM2: " + SimpleDyno.frmDyno.SignalsPerRPM2.ToString());
            DataOutputFile.WriteLine("Channel_1_Threshold: " + SimpleDyno.HighSignalThreshold.ToString());
            DataOutputFile.WriteLine("Channel_2_Threshold: " + SimpleDyno.HighSignalThreshold2.ToString());
            DataOutputFile.WriteLine("Run_RPM_Threshold: " + SimpleDyno.PowerRunThreshold.ToString());
            DataOutputFile.WriteLine("Run_Spike_Removal_Threshold: " + PowerRunSpikeLevel.ToString());
            DataOutputFile.WriteLine("Curve_Fit_Model: " + cmbWhichFit.SelectedItem.ToString());
            DataOutputFile.WriteLine("Coast_Down?_Roller?_Wheel?_Drivetrain?: " + rdoRunDown.Enabled.ToString() + " " + SimpleDyno.frmCorrection.rdoFreeRoller.Checked.ToString() + " " + SimpleDyno.frmCorrection.rdoRollerAndWheel.Checked.ToString() + " " + SimpleDyno.frmCorrection.rdoRollerAndDrivetrain.Checked.ToString());
            if (Correction.blnUsingLoadedRunDownFile == true)
            {
                DataOutputFile.WriteLine("Coast_Down_File_Loaded: " + Correction.RunDownOpenFileDialog.FileName);
            }
            else
            {
                DataOutputFile.WriteLine("Coast_Down_Fit_Model: " + cmbWhichRDFit?.SelectedItem?.ToString());
            }

            DataOutputFile.WriteLine("Voltage_Smoothing_%: " + txtVoltageSmooth.Text);
            DataOutputFile.WriteLine("Current_Smoothing_%: " + txtCurrentSmooth.Text);
            DataOutputFile.WriteLine(Constants.vbCrLf);
            DataOutputFile.WriteLine("PRIMARY_CHANNEL_CURVE_FIT_DATA");
            DataOutputFile.WriteLine("NUMBER_OF_POINTS_FIT" + " " + Information.UBound(FitData, 2).ToString());
            DataOutputFile.WriteLine("STARTING_POINT" + " " + FitStartPoint.ToString());

            // Create the column headings string based on the Data structure 
            // Only Primary SI units of the values are written
            string tempstring = "";
            string[] tempsplit;
            int paramcount;
            for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++)
            {
                tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " ");
                tempstring = tempstring + SimpleDyno.DataTags[paramcount].Replace(" ", "_") + "_(" + tempsplit[0] + ") ";
            }
            // Write the column headings
            DataOutputFile.WriteLine(tempstring);
            // Need to set the zeroth value to support using the count and count-1 approach to torque and power calculations
            FitData[SimpleDyno.RPM1_ROLLER, 0] = FitData[SimpleDyno.RPM1_ROLLER, 1];
            // Reset Maxima
            // YMax = 0 ': Pmax = 0 : Tmax = 0

            // Process the results for the fit data
            var loopTo = Information.UBound(FitData, 2);
            for (Count = 1; Count <= loopTo; Count++)
            {
                // Keep track of the max value for plotting purposes
                FitData[SimpleDyno.LAST, Count] = FitData[SimpleDyno.RPM1_ROLLER, Count] - SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count];
                // If FitData(Main.RPM1_ROLLER, count) > YMax Then YMax = FitData(Main.RPM1_ROLLER, count) 'for scaling the axis when we show the fit
                // update wheel and motor RPM
                FitData[SimpleDyno.RPM1_WHEEL, Count] = FitData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRPMtoWheelRPM;
                FitData[SimpleDyno.RPM1_MOTOR, Count] = FitData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRPMtoMotorRPM;
                // update speed and drag
                FitData[SimpleDyno.SPEED, Count] = FitData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRadsPerSecToMetersPerSec;
                FitData[SimpleDyno.DRAG, Count] = Math.Pow(FitData[SimpleDyno.SPEED, Count], 3d) * SimpleDyno.ForceAir;
                // re-calc roller torque and power using the Fit data
                // FitData(Main.TORQUE_ROLLER, Count) = (FitData(Main.RPM1_ROLLER, Count) - FitData(Main.RPM1_ROLLER, Count - 1)) / (FitData(Main.SESSIONTIME, Count) - FitData(Main.SESSIONTIME, Count - 1)) * Main.DynoMomentOfInertia 'this is the roller torque, should calc the wheel and motor at this point also
                // NOTE - new power calculation uses (new-old) / 2  - REMOVED 06DEC13
                // FitData(Main.POWER, Count) = FitData(Main.TORQUE_ROLLER, Count) * ((FitData(Main.RPM1_ROLLER, Count) + FitData(Main.RPM1_ROLLER, Count - 1)) / 2)
                FitData[SimpleDyno.TORQUE_WHEEL, Count] = FitData[SimpleDyno.POWER, Count] / FitData[SimpleDyno.RPM1_WHEEL, Count];
                FitData[SimpleDyno.TORQUE_MOTOR, Count] = FitData[SimpleDyno.POWER, Count] / FitData[SimpleDyno.RPM1_MOTOR, Count];
                // Calculated Corrected values for power and torque if use rundown is selected
                if (SimpleDyno.frmCorrection.chkUseRunDown.Checked)
                {
                    // Select how the coast down values are to be applied
                    // If its a freeroller rundown, just add torque to the roller torque and continue as usual.
                    if (SimpleDyno.frmCorrection.rdoFreeRoller.Checked)
                    {
                        FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] = FitData[SimpleDyno.TORQUE_ROLLER, Count] + FitData[SimpleDyno.TORQUE_COASTDOWN, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] = FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] * ((FitData[SimpleDyno.RPM1_ROLLER, Count] + FitData[SimpleDyno.RPM1_ROLLER, Count - 1]) / 2d);
                        FitData[SimpleDyno.CORRECTED_POWER_WHEEL, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_TORQUE_WHEEL, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] / FitData[SimpleDyno.RPM1_WHEEL, Count];
                        FitData[SimpleDyno.CORRECTED_TORQUE_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] / FitData[SimpleDyno.RPM1_MOTOR, Count];
                    }
                    // if its a roller plus wheel leave roller alone, adjust wheel and back calc to motor
                    if (SimpleDyno.frmCorrection.rdoRollerAndWheel.Checked)
                    {
                        // Add corrected torque to wheel
                        FitData[SimpleDyno.CORRECTED_TORQUE_WHEEL, Count] = FitData[SimpleDyno.TORQUE_ROLLER, Count] + FitData[SimpleDyno.TORQUE_COASTDOWN, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_WHEEL, Count] = FitData[SimpleDyno.CORRECTED_TORQUE_WHEEL, Count] * ((FitData[SimpleDyno.RPM1_ROLLER, Count] + FitData[SimpleDyno.RPM1_ROLLER, Count - 1]) / 2d);
                        // Now set the motor torque and power
                        FitData[SimpleDyno.CORRECTED_POWER_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_WHEEL, Count];
                        FitData[SimpleDyno.CORRECTED_TORQUE_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_WHEEL, Count] / FitData[SimpleDyno.RPM1_MOTOR, Count];
                        // Now reset the wheel and roller torques to uncorrected
                        FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] = FitData[SimpleDyno.TORQUE_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] = FitData[SimpleDyno.POWER, Count];
                    }
                    // if its a roller plus drive train leave roller and wheel alone and apply only to motor
                    if (SimpleDyno.frmCorrection.rdoRollerAndDrivetrain.Checked)
                    {
                        // temporarily add torque to roller
                        FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] = FitData[SimpleDyno.TORQUE_ROLLER, Count] + FitData[SimpleDyno.TORQUE_COASTDOWN, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] = FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] * ((FitData[SimpleDyno.RPM1_ROLLER, Count] + FitData[SimpleDyno.RPM1_ROLLER, Count - 1]) / 2d);
                        // Now set the motor torque and power
                        FitData[SimpleDyno.CORRECTED_POWER_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_TORQUE_MOTOR, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] / FitData[SimpleDyno.RPM1_MOTOR, Count];
                        // Now reset the wheel and roller torques to uncorrected
                        FitData[SimpleDyno.CORRECTED_TORQUE_ROLLER, Count] = FitData[SimpleDyno.TORQUE_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] = FitData[SimpleDyno.POWER, Count];
                        FitData[SimpleDyno.CORRECTED_POWER_WHEEL, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count];
                        FitData[SimpleDyno.CORRECTED_TORQUE_WHEEL, Count] = FitData[SimpleDyno.CORRECTED_POWER_ROLLER, Count] / FitData[SimpleDyno.RPM1_WHEEL, Count];
                    }
                }

                // Update other parameters requiring calculations
                // RPM2 will be already there but the ratio and rollout need to be calculated
                if (FitData[SimpleDyno.RPM2, Count] != 0d)
                {
                    FitData[SimpleDyno.RPM2_RATIO, Count] = FitData[SimpleDyno.RPM2, Count] / FitData[SimpleDyno.RPM1_WHEEL, Count];
                    FitData[SimpleDyno.RPM2_ROLLOUT, Count] = SimpleDyno.WheelCircumference / FitData[SimpleDyno.RPM2_RATIO, Count];
                }
                else
                {
                    FitData[SimpleDyno.RPM2_RATIO, Count] = 0d;
                    FitData[SimpleDyno.RPM2_ROLLOUT, Count] = 0d;
                }
                // Volts and Amps will already be there but watts in and efficiency need to be added
                FitData[SimpleDyno.WATTS_IN, Count] = FitData[SimpleDyno.VOLTS, Count] * FitData[SimpleDyno.AMPS, Count];
                if (FitData[SimpleDyno.WATTS_IN, Count] != 0d)
                {
                    FitData[SimpleDyno.EFFICIENCY, Count] = FitData[SimpleDyno.POWER, Count] / FitData[SimpleDyno.WATTS_IN, Count] * 100d;
                    FitData[SimpleDyno.CORRECTED_EFFICIENCY, Count] = FitData[SimpleDyno.CORRECTED_POWER_MOTOR, Count] / FitData[SimpleDyno.WATTS_IN, Count] * 100d;
                }
                else
                {
                    FitData[SimpleDyno.EFFICIENCY, Count] = 0d;
                    FitData[SimpleDyno.CORRECTED_EFFICIENCY, Count] = 0d;
                }
                // Write the data file based on the FitData structure.
                // Build the results string...
                tempstring = ""; // count.ToString & " "
                for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++)
                {
                    tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " "); // How many units are there
                    tempstring = tempstring + FitData[paramcount, Count] * SimpleDyno.DataUnits[paramcount, 0] + " "; // DataTags(paramcount).Replace(" ", "_") & "(" & tempsplit(unitcount) & ") "
                }
                // ...and write it
                DataOutputFile.WriteLine(tempstring);

            }
            // Add the coast down data if it was recorded
            DataOutputFile.WriteLine(Constants.vbCrLf + "FULL_SET_COAST_DOWN_FIT_DATA");
            DataOutputFile.WriteLine("NUMBER_OF_COAST_DOWN_POINTS_FIT " + Information.UBound(CoastDownFY));
            DataOutputFile.WriteLine("Time_(Sec) RPM1_Roller_(rad/s)");
            if (Information.UBound(CoastDownFY) > 1)
            {
                var loopTo1 = Information.UBound(CoastDownFY);
                for (Count = 1; Count <= loopTo1; Count++)
                    DataOutputFile.WriteLine(CoastDownX[Count] + " " + CoastDownFY[Count]);
            }

            // Add the raw data.  In V6 we are also calculating the raw torques, powers etc. This makes the file larger but will make Excel work easier
            DataOutputFile.WriteLine(Constants.vbCrLf + "PRIMARY_CHANNEL_RAW_DATA");
            DataOutputFile.WriteLine("NUMBER_OF_POINTS_COLLECTED" + " " + SimpleDyno.DataPoints.ToString());
            // Again, create the header row
            tempstring = "";
            for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++) // CHECK - time is now the last column which will mess up the overlay routine .
            {
                tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " ");
                tempstring = tempstring + SimpleDyno.DataTags[paramcount].Replace(" ", "_") + "(" + tempsplit[0] + ") ";
            }
            // Write the column headings
            DataOutputFile.WriteLine(tempstring);
            // Need to set the zeroth value to support using the count and count-1 approach to torque and power calculations
            SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, 0] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, 1];
            var loopTo2 = SimpleDyno.DataPoints;
            for (Count = 1; Count <= loopTo2; Count++)
            {
                // re-calc speed, wheel and motor RPMs based on collected data
                SimpleDyno.CollectedData[SimpleDyno.SPEED, Count] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRadsPerSecToMetersPerSec;
                SimpleDyno.CollectedData[SimpleDyno.RPM1_WHEEL, Count] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRPMtoWheelRPM;
                SimpleDyno.CollectedData[SimpleDyno.RPM1_MOTOR, Count] = SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count] * SimpleDyno.RollerRPMtoMotorRPM;
                // re-calc roller torque and power useing the collected data
                SimpleDyno.CollectedData[SimpleDyno.TORQUE_ROLLER, Count] = (SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count] - SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count - 1]) / (SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count] - SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Count - 1]) * SimpleDyno.DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                // NOTE - new power calculation uses (new-old) / 2
                SimpleDyno.CollectedData[SimpleDyno.POWER, Count] = SimpleDyno.CollectedData[SimpleDyno.TORQUE_ROLLER, Count] * ((SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count] + SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Count - 1]) / 2d);
                // now re-calc wheel and motor torque based on Power
                SimpleDyno.CollectedData[SimpleDyno.TORQUE_WHEEL, Count] = SimpleDyno.CollectedData[SimpleDyno.POWER, Count] / SimpleDyno.CollectedData[SimpleDyno.RPM1_WHEEL, Count];
                SimpleDyno.CollectedData[SimpleDyno.TORQUE_MOTOR, Count] = SimpleDyno.CollectedData[SimpleDyno.POWER, Count] / SimpleDyno.CollectedData[SimpleDyno.RPM1_MOTOR, Count];
                // recalc Drag and set a max speed based on it
                SimpleDyno.CollectedData[SimpleDyno.DRAG, Count] = Math.Pow(SimpleDyno.CollectedData[SimpleDyno.SPEED, Count], 3d) * SimpleDyno.ForceAir;
                // Update other parameters requiring calculations
                // Main.RPM2 will be already there but the ratio and rollout need to be calculated
                if (SimpleDyno.CollectedData[SimpleDyno.RPM2, Count] != 0d)
                {
                    SimpleDyno.CollectedData[SimpleDyno.RPM2_RATIO, Count] = SimpleDyno.CollectedData[SimpleDyno.RPM2, Count] / SimpleDyno.CollectedData[SimpleDyno.RPM1_WHEEL, Count];
                    SimpleDyno.CollectedData[SimpleDyno.RPM2_ROLLOUT, Count] = SimpleDyno.WheelCircumference / SimpleDyno.CollectedData[SimpleDyno.RPM2_RATIO, Count];
                }
                else
                {
                    SimpleDyno.CollectedData[SimpleDyno.RPM2_RATIO, Count] = 0d;
                    SimpleDyno.CollectedData[SimpleDyno.RPM2_ROLLOUT, Count] = 0d;
                }
                // Volts and Amps will already be there but watts in and efficiency need to be added
                SimpleDyno.CollectedData[SimpleDyno.WATTS_IN, Count] = SimpleDyno.CollectedData[SimpleDyno.VOLTS, Count] * SimpleDyno.CollectedData[SimpleDyno.AMPS, Count];
                if (SimpleDyno.CollectedData[SimpleDyno.WATTS_IN, Count] != 0d)
                {
                    SimpleDyno.CollectedData[SimpleDyno.EFFICIENCY, Count] = SimpleDyno.CollectedData[SimpleDyno.POWER, Count] / SimpleDyno.CollectedData[SimpleDyno.WATTS_IN, Count] * 100d;
                }
                else
                {
                    SimpleDyno.CollectedData[SimpleDyno.EFFICIENCY, Count] = 0d;
                }
                // Build the results string...
                tempstring = "";
                for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++) // CHECK - time is now the last column which will mess up the overlay routine .
                {
                    tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " "); // How many units are there
                    tempstring = tempstring + SimpleDyno.CollectedData[paramcount, Count] * SimpleDyno.DataUnits[paramcount, 0] + " "; // DataTags(paramcount).Replace(" ", "_") & "(" & tempsplit(unitcount) & ") "
                }
                // ...and write it
                DataOutputFile.WriteLine(tempstring);


            }
            // Save the file
            DataOutputFile.Close();
            Program.MainI.updateSessionPostfix();
        }
        public void MovingAverageSmooth(ref double[] SentY, ref double[] SentFY, double WindowPercent)
        {
            long n;
            int w;
            long t;
            long s;
            var temp = default(double);
            n = 0L; // reset window width
            w = (int)Math.Round(Conversion.Int(Information.UBound(SentY) / 100d * WindowPercent));
            var loopTo = (long)Information.UBound(SentY);
            for (t = 1L; t <= loopTo; t++)  // top to bottom of data
            {
                prgFit.Value = (int)Math.Round(prgFit.Maximum / (double)Information.UBound(SentY) * t);
                var loopTo1 = t + n;
                for (s = t - n; s <= loopTo1; s++)
                    temp = temp + SentY[(int)s];
                SentFY[(int)t] = temp / (n * 2L + 1L);
                temp = 0d;
                if (n != w & Information.UBound(SentY) - t > w)
                {
                    n = n + 1L;
                }
                else if (Information.UBound(SentY) - t <= w)
                {
                    n = n - 1L;
                }
            }
        }
        public void MovingLSQLinfitSmooth(ref double[] SentX, ref double[] SentY, ref double[] SentFY, double WindowPercent)
        {
            long n;
            int w;
            long t;
            long s;
            double SumXY;
            double SumX;
            double SumY;
            double SumXsq;
            double k;
            double b;
            long m;

            n = 0L; // reset window width
            w = (int)Math.Round(Conversion.Int(Information.UBound(SentY) / 100d * WindowPercent));
            var loopTo = (long)Information.UBound(SentY);
            for (t = 1L; t <= loopTo; t++)  // top to bottom of data
            {
                prgFit.Value = (int)Math.Round(prgFit.Maximum / (double)Information.UBound(SentY) * t);
                SumX = 0d;
                SumY = 0d;
                SumXY = 0d;
                SumXsq = 0d;
                var loopTo1 = t + n;
                for (s = t - n; s <= loopTo1; s++)
                {
                    SumX += SentX[(int)s];
                    SumY += SentY[(int)s];
                    SumXY += SentX[(int)s] * SentY[(int)s];
                    SumXsq += SentX[(int)s] * SentX[(int)s];
                }
                m = n * 2L + 1L;
                if (m > 1L)
                {
                    k = (m * SumXY - SumX * SumY) / (m * SumXsq - SumX * SumX);
                    b = (SumY * SumXsq - SumX * SumXY) / (m * SumXsq - SumX * SumX);
                    SentFY[(int)t] = k * SentX[(int)t] + b;
                }
                else
                {
                    SentFY[(int)t] = SentY[(int)t];
                }
                if (n != w & Information.UBound(SentY) - t > w)
                {
                    n = n + 1L;
                }
                else if (Information.UBound(SentY) - t <= w)
                {
                    n = n - 1L;
                }
            }
        }

        public void NoSmooth(ref double[] SentY, ref double[] SentFY)
        {
            long t;
            var loopTo = (long)Information.UBound(SentY);
            for (t = 0L; t <= loopTo; t++)  // top to bottom of data
                SentFY[(int)t] = SentY[(int)t];
        }
        public void NonLin_fitting(ref double[] SentX, ref double[] SentY, ref double[] SentFY, int SentCurveChoice, ref bool blnFitFinished)
        {

            int ParameterSetSize;
            double NewC;

            switch (SentCurveChoice)
            {
                case var @case when @case == 1: // Four parameter fit
                    {
                        // Set the number of parameters to be fit 
                        ParameterSetSize = 4;
                        c = new double[ParameterSetSize + 1];
                        NewC = 1d;
                        // Resize and initialize the equation parameters

                        c[1] = 1d;
                        c[2] = NewC;
                        c[3] = SentY[Information.UBound(SentY)];
                        c[4] = NewC;
                        do
                        {
                            Application.DoEvents();
                            // Perform the NL-Rregressio with LM algorithm
                            LMNoLinearFit(ref SentX, ref SentY, ref SentFY, ref c, 1);
                            // Check that we have valid results.  
                            // If not, change initial conditions and re-try
                            if (double.IsNaN(c[1]) == true)
                            {
                                blnfit = false;
                                if (NewC > 10000d) // CHECK - why is this here, if NewC > 10000 then this simply resets all parameters and starts again?
                                {
                                    NewC = 1d;
                                    c[1] = 1d;
                                    c[2] = NewC;
                                    c[3] = SentY[Information.UBound(SentY)];
                                    c[4] = NewC;
                                }
                                else
                                {
                                    NewC = NewC * 5d;
                                    c[1] = 1d;
                                    c[2] = NewC;
                                    c[3] = SentY[Information.UBound(SentY)];
                                    c[4] = NewC;
                                }
                            }
                            else
                            {
                                // Flag that the fit has worked
                                blnfit = true;
                                blnFitFinished = true;
                            }
                        }
                        while (!(blnfit | blnFitFinished | SimpleDyno.StopFitting));
                        break;
                    }
                case var case1 when case1 == 2:
                case 3:
                case 4:
                case 5:
                    {
                        // Resize and initialize the equation parameters based on sentcurve choice
                        c = new double[SentCurveChoice + 1 + 1];
                        Application.DoEvents();
                        // Perform the NL-Rregressio with LM algorithm
                        LMNoLinearFit(ref SentX, ref SentY, ref SentFY, ref c, SentCurveChoice + 1);
                        // Check that we have valid results.  
                        blnfit = true;
                        blnFitFinished = true;
                        break;
                    }
                case var case2 when case2 == 6: // Test algorithm(s)
                    {
                        if (rdoRPM1.Checked)
                        {
                            lblProgress.Text = "Smoothing RPM1...";
                            RPM1Smooth = (scrlRPM1Smooth.Maximum + 1 - scrlRPM1Smooth.Value) / 2d;
                            MovingAverageSmooth(ref SentY, ref SentFY, RPM1Smooth);
                            blnfit = true;
                            blnFitFinished = true;
                        }
                        if (rdoRunDown.Checked)
                        {
                            lblProgress.Text = "Smoothing Coast Down...";
                            CoastDownSmooth = (scrlCoastDownSmooth.Maximum + 1 - scrlCoastDownSmooth.Value) / 2d;
                            MovingAverageSmooth(ref SentY, ref SentFY, CoastDownSmooth);
                            blnfit = true;
                            blnFitFinished = true;
                        }

                        break;
                    }
                case var case3 when case3 == 7: // Moving least squares linfit
                    {
                        if (rdoRPM1.Checked)
                        {
                            lblProgress.Text = "Smoothing RPM1...";
                            RPM1Smooth = (scrlRPM1Smooth.Maximum + 1 - scrlRPM1Smooth.Value) / 2d;
                            MovingLSQLinfitSmooth(ref SentX, ref SentY, ref SentFY, RPM1Smooth);
                            blnfit = true;
                            blnFitFinished = true;
                        }
                        if (rdoRunDown.Checked)
                        {
                            lblProgress.Text = "Smoothing Coast Down...";
                            CoastDownSmooth = (scrlCoastDownSmooth.Maximum + 1 - scrlCoastDownSmooth.Value) / 2d;
                            MovingLSQLinfitSmooth(ref SentX, ref SentY, ref SentFY, RPM1Smooth);
                            blnfit = true;
                            blnFitFinished = true;
                        }

                        break;
                    }
                case var case4 when case4 == 8: // No smoothing
                    {
                        if (rdoRPM1.Checked)
                        {
                            NoSmooth(ref SentY, ref SentFY);
                            blnfit = true;
                            blnFitFinished = true;
                        }
                        if (rdoRunDown.Checked)
                        {
                            NoSmooth(ref SentY, ref SentFY);
                            blnfit = true;
                            blnFitFinished = true;
                        }

                        break;
                    }
            }
        }
        public void LMNoLinearFit(ref double[] x, ref double[] y, ref double[] fy, ref double[] c, int WhichCurve)
        {
            // ver. 14.04.2006 by Luis Isaac Ramos Garcia and Foxes Team

            int i;
            int k;
            int kmax;
            var Mcoef = default(double[,]);
            var det = default(double);
            double[] tpc;
            double[] Diffc;       // Vector de diferecias entre antiguos c y nuevos
            double delta;         // Valor delta de algoritmo Levenberg-Marquardt
            double tdelta;        // relative increment
            var mu = default(double);            // reduction/amplification quadratic error
            double ch20;          // quadratic error previous step
            var delta0 = default(double);        // increment previous step
            double tol;           // machine tolerance
            double fact;          // step factor
            int itmax;
            int niter;
            double ch2;

            Diffc = new double[Information.UBound(c) + 1];
            tpc = new double[Information.UBound(c) + 1]; // was tpc(LBound(c) To UBound(c))
            delta = 0.05d;  // 0.1%
            k = 0;
            niter = 0;
            itmax = 1000;
            fact = 10d;
            tol = 2d * Math.Pow(10d, -16);
            kmax = (int)Math.Round(itmax / 4d);
            ch20 = chi2(ref x, ref y, ref c, WhichCurve);

            // Dim temp As Integer, tempstring As String

            prgFit.Maximum = kmax + itmax;
            while (k < kmax & niter < itmax & SimpleDyno.StopFitting == false) // VL. 30/10/2004
            {
                prgFit.Value = k + niter;
                Application.DoEvents();
                CalculateMcoef(ref x, ref y, ref c, ref Mcoef, delta, WhichCurve);
                SolveLS(ref Mcoef, Diffc, det);
                var loopTo = Information.UBound(c);
                for (i = 1; i <= loopTo; i++)
                    // If tpc(i) < 0 Then tpc(i) = 1
                    tpc[i] = c[i] + Diffc[i];
                ch2 = chi2(ref x, ref y, ref tpc, WhichCurve);

                if (ch2 > tol)
                    mu = ch2 / ch20;

                tdelta = ch2 - ch20;
                if (ch2 > tol)
                    tdelta = tdelta / ch2; // VL. 30/10/2004

                if (mu > 10d)
                {
                    delta = delta * fact;
                }
                else
                {
                    if (Math.Abs(tdelta) < 0.001d)
                        k = k + 1;
                    delta = delta / fact;
                    var loopTo1 = Information.UBound(c);
                    for (i = 1; i <= loopTo1; i++)
                        c[i] = tpc[i];
                    ch20 = ch2;
                }

                // check delta oscillation
                if (niter % 4 == 0)
                {
                    if (delta0 == delta)
                    {
                        fact = 1d; // 2 relaxed step
                    }
                    else         // Both of these were set to 1 (redundant really) because XP and Vista were giving different results
                    {
                        fact = 1d;
                    } // 10 fast step
                    delta0 = delta;
                }

                niter = niter + 1;
            }

            ch2 = chi2(ref x, ref y, ref c, WhichCurve);

            FunParm(ref fy, c, x, WhichCurve);

        }
        public void FunParm(ref double[] f, double[] c, double[] x, int WhichCurve)
        {
            int i;
            int n;
            int t; // CHECK i and n were long
            n = Information.UBound(x);
            switch (WhichCurve)
            {
                case var @case when @case == 1:
                    {
                        var loopTo = n;
                        for (i = 1; i <= loopTo; i++)
                            f[i] = -c[3] / Math.Pow(1d + Math.Pow(x[i] / c[2], c[1]), c[4]) + c[3]; // this is the four parameter logistic fit
                        break;
                    }
                case var case1 when case1 >= 2:
                    {
                        var loopTo1 = n;
                        for (i = 1; i <= loopTo1; i++)
                        {
                            f[i] = c[1];
                            var loopTo2 = WhichCurve - 1;
                            for (t = 1; t <= loopTo2; t++)
                                f[i] = f[i] + Math.Pow(x[i], t) * c[t + 1];
                        }

                        break;
                    }
            }
        }
        public void DFunParm(ref double[,] df, double[] c, double[] x, int WhichCurve)
        {
            double[] ctemp;
            int n;
            int m;
            int i;
            int j;
            double[] f0;
            double[] f1;
            double h;
            int t;
            n = Information.UBound(x);
            m = Information.UBound(c);
            ctemp = new double[m + 1];
            // approximate derivative with finite differences
            h = Math.Pow(10d, -4);
            var loopTo = m;
            for (j = 1; j <= loopTo; j++)
            {
                // step forward
                var loopTo1 = m;
                for (i = 1; i <= loopTo1; i++)
                {
                    if (i == j)
                    {
                        ctemp[i] = c[i] + h / 2d;
                    }
                    else
                    {
                        ctemp[i] = c[i];
                    }
                    // If ctemp(i) < 0 Then ctemp(i) = 1
                }
                f1 = new double[n + 1];
                switch (WhichCurve)
                {
                    case var @case when @case == 1:
                        {
                            var loopTo2 = n;
                            for (i = 1; i <= loopTo2; i++)
                                f1[i] = -ctemp[3] / Math.Pow(1d + Math.Pow(x[i] / ctemp[2], ctemp[1]), ctemp[4]) + ctemp[3]; // four parameter logistic fit
                            break;
                        }
                    case var case1 when case1 >= 2:
                        {
                            var loopTo3 = n;
                            for (i = 1; i <= loopTo3; i++)
                            {
                                f1[i] = ctemp[1];
                                var loopTo4 = WhichCurve - 1;
                                for (t = 1; t <= loopTo4; t++)
                                    f1[i] = f1[i] + Math.Pow(x[i], t) * ctemp[t + 1]; // Linear Fit
                            }

                            break;
                        }
                }
                // step back
                var loopTo5 = m;
                for (i = 1; i <= loopTo5; i++)
                {
                    if (i == j)
                    {
                        ctemp[i] = c[i] - h / 2d;
                    }
                    else
                    {
                        ctemp[i] = c[i];
                    }
                    // If ctemp(i) < 0 Then ctemp(i) = 1
                }
                f0 = new double[n + 1];
                switch (WhichCurve)
                {
                    case var case2 when case2 == 1:
                        {
                            var loopTo6 = n;
                            for (i = 1; i <= loopTo6; i++)
                                f0[i] = -ctemp[3] / Math.Pow(1d + Math.Pow(x[i] / ctemp[2], ctemp[1]), ctemp[4]) + ctemp[3]; // four parameter logistic fit
                            break;
                        }
                    case var case3 when case3 >= 2:
                        {
                            var loopTo7 = n;
                            for (i = 1; i <= loopTo7; i++)
                            {
                                f0[i] = ctemp[1];
                                var loopTo8 = WhichCurve - 1;
                                for (t = 1; t <= loopTo8; t++)
                                    f0[i] = f0[i] + Math.Pow(x[i], t) * ctemp[t + 1];
                            }

                            break;
                        }
                }
                // finite difference
                var loopTo9 = n;
                for (i = 1; i <= loopTo9; i++)
                    df[i, j] = (f1[i] - f0[i]) / h;
            }
        }
        private double chi2(ref double[] x, ref double[] y, ref double[] c, int WhichCurve)
        {
            double chi2Ret = default;

            int i;
            var f = new double[Information.UBound(x) + 1];

            FunParm(ref f, c, x, WhichCurve);
            chi2Ret = 0d;
            var loopTo = Information.UBound(x);
            for (i = 1; i <= loopTo; i++) // was LBound(x) To UBound(x)
                chi2Ret = chi2Ret + Math.Pow(y[i] - f[i], 2d);
            return chi2Ret;

        }
        public void CalculateMcoef(ref double[] x, ref double[] y, ref double[] c, ref double[,] Mcoef, double delta, int WhichCurve)
        {

            int i;
            int l;
            int k;
            var dv = new double[Information.UBound(x) + 1, Information.UBound(c) + 1 + 1];
            // Dim temp As Double
            var f = new double[Information.UBound(x) + 1];
            Mcoef = new double[Information.UBound(c) + 1, Information.UBound(c) + 1 + 1];
            FunParm(ref f, c, x, WhichCurve);
            DFunParm(ref dv, c, x, WhichCurve);
            var loopTo = Information.UBound(x);
            for (i = 1; i <= loopTo; i++) // - LBound(x) + 1
            {
                var loopTo1 = Information.UBound(c);
                for (k = 1; k <= loopTo1; k++) // - LBound(c) + 1
                {
                    Mcoef[k, Information.UBound(c) + 1] = (y[i] - f[i]) * dv[i, k] + Mcoef[k, Information.UBound(c) + 1];
                    var loopTo2 = Information.UBound(c);
                    for (l = k; l <= loopTo2; l++) // - LBound(c) + 1
                        Mcoef[k, l] = dv[i, k] * dv[i, l] + Mcoef[k, l];
                }
            }
            var loopTo3 = Information.UBound(c);
            for (k = 2; k <= loopTo3; k++) // - LBound(c) + 1
            {
                var loopTo4 = k - 1;
                for (l = 1; l <= loopTo4; l++)
                    Mcoef[k, l] = Mcoef[l, k];
            }
            var loopTo5 = Information.UBound(c);
            for (i = 1; i <= loopTo5; i++) // - LBound(c) + 1              'Algoritmo Marquart
                Mcoef[i, i] = Mcoef[i, i] * (1d + delta);

        }
        // ------------------------------------------------------------------------------------------
        // =====================================================================================
        // Linear System routine by Luis Isaac Ramos Garcia
        // v. 13.10.2004
        // Sub SolveLS  = Solving Linear System with scaled pivot
        // Sub TM       = Triangularize a square matrix with scaled pivot
        // sub solveTS  = Solving a triangular system with backsubstitution
        // Last mod. 2.11.2004 by Foxes Team
        // =====================================================================================
        public void SolveLS(ref double[,] Mc, double[] sol, double det)
        {
            // Rutina para resolver un sistema de ecucacines lineales de la forma:
            // MC(1,1)*sol(1)+ MC(1,2)*sol(2)+MC(1,3)*sol(3)+...+MC(1,N)*sol(N) = MC(1,N+1)
            // MC(2,1)*sol(1)+ MC(2,2)*sol(2)+MC(2,3)*sol(3)+...+MC(2,N)*sol(N) = MC(2,N+1)
            // MC(3,1)*sol(1)+ MC(2,2)*sol(2)+MC(3,3)*sol(3)+...+MC(3,N)*sol(N) = MC(3,N+1)
            // .............................................................................
            // MC(N,1)*sol(1)+ MC(N,2)*sol(2)+MC(N,3)*sol(3)+...+MC(N,N)*sol(N) = MC(N,N+1)
            // n numero de incognitas
            // MC matriz de coeficientes extendida
            // MC(1,1), MC(1,2),MC(1,3),...,MC(1,N) |MC(1,N+1)
            // MC(2,1), MC(2,2),MC(2,3),...,MC(2,N) |MC(2,N+1)
            // MC(3,1), MC(3,2),MC(3,3),...,MC(3,N) |MC(3,N+1)
            // ..............................................
            // MC(N,1), MC(N,2),MC(N,3),...,MC(N,N) |MC(N,N+1)
            // Sol vector solucion
            // det derminante de la matriz de coeficientes

            // Bibliografia
            // Numerical Recipies in Fortran77; W.H. Press, et al.; Cambridge U. Press
            // Metodos numericos con Matlab; J. M Mathewss et al.; Prentice Hall
            int n;
            n = Information.UBound(sol); // - LBound(sol) + 1

            tm(n, ref Mc);                // Trigonalizamos la matriz con pivoteo escalado para eviar errores
            solveTS(Mc, sol, det);      // Solucion del sistema trigonal
        }
        public void tm(int n, ref double[,] Mc)
        {
            // Rutina para tragonalizar una matriz extendida de un sistema
            // de ecuaciones lineales en la forma:
            // 
            // MC(1,1), MC(1,2),MC(1,3),...,MC(1,N) |MC(1,N+1)
            // MC(2,1), MC(2,2),MC(2,3),...,MC(2,N) |MC(2,N+1)
            // MC(3,1), MC(3,2),MC(3,3),...,MC(3,N) |MC(3,N+1)
            // ..............................................
            // MC(N,1), MC(N,2),MC(N,3),...,MC(N,N) |MC(N,N+1)
            // 
            // n numero de incognitas
            // MC matriz de coeficientes extendida
            int q;
            int r;
            int c;
            double[,] m;
            m = new double[n + 1, n + 1];
            var loopTo = n;
            for (q = 1; q <= loopTo; q++)
            {
                Pivot(q, n, ref Mc);                     // Pivotamos la matriz para evitar errores de redondeo
                var loopTo1 = n;
                for (r = q + 1; r <= loopTo1; r++)
                {
                    if (Mc[q, q] == 0d)
                        return;
                    m[r, q] = Mc[r, q] / Mc[q, q];
                    Mc[r, q] = 0d;
                    var loopTo2 = n + 1;
                    for (c = q + 1; c <= loopTo2; c++)
                        Mc[r, c] = Mc[r, c] - m[r, q] * Mc[q, c];
                }
            }
        }
        private void Pivot(int rin, int n, ref double[,] Mc)
        {
            // Rutina para hacer un pivoteo paracial ecalado de una matriz extendida
            // para un sistema de ecuaciones lineales en la forma:
            // MC(1,1), MC(1,2),MC(1,3),...,MC(1,N) |MC(1,N+1)
            // MC(2,1), MC(2,2),MC(2,3),...,MC(2,N) |MC(2,N+1)
            // MC(3,1), MC(3,2),MC(3,3),...,MC(3,N) |MC(3,N+1)
            // ..............................................
            // MC(N,1), MC(N,2),MC(N,3),...,MC(N,N) |MC(N,N+1)
            // 
            // rin la fila dede la que comenzamos el pivoteo
            // n numero de incognitas
            // MC matriz de coeficientes extendida
            // 
            // mod. ver. 2-11-04 VL
            // 
            // Dim q As Integer '///This removed - flagged by VB Express as unused
            int r;
            int c;
            double[] m;                               // m() guarda el maximo falor de cada fila
            m = new double[n + 1]; // was ReDim m(rin To n)
            double max;
            // Dim temp() As Double
            // ReDim temp(rin To N + 1)
            double temp;
            var loopTo = n;
            for (r = rin; r <= loopTo; r++)
            {
                m[r] = Math.Abs(Mc[r, rin]);
                // For c = rin + 1 To N + 1
                var loopTo1 = n;
                for (c = rin + 1; c <= loopTo1; c++)
                {
                    if (Math.Abs(Mc[r, c]) > m[r])
                    {
                        m[r] = Math.Abs(Mc[r, c]);
                    }
                }
            }

            var loopTo2 = n;
            for (r = rin; r <= loopTo2; r++)
            {
                if (m[r] != 0d)  // VL 25-10-2004
                {
                    m[r] = Math.Abs(Mc[r, rin]) / m[r];
                }
            }

            max = m[rin];
            if (max == 0d)
                return; // VL 25-10-2004
                        // rows swap routine 'VL 2-11-2004
            var loopTo3 = n;
            for (r = rin + 1; r <= loopTo3; r++)
            {
                if (max < m[r])
                {
                    var loopTo4 = n + 1;
                    for (c = rin; c <= loopTo4; c++)
                    {
                        temp = Mc[rin, c];
                        Mc[rin, c] = Mc[r, c];
                        Mc[r, c] = temp;
                    }
                    max = m[r];
                }
            }
        }
        private void solveTS(double[,] Mc, double[] sol, double det)
        {
            // Rutina para resolver un sisitema trigonal de ecucacines lineales de la forma:
            // MC(1,1)*sol(1)+ MC(1,2)*sol(2)+MC(1,3)*sol(3)+...+MC(1,N)*sol(N) = MC(1,N+1)
            // MC(2,2)*sol(2)+MC(2,3)*sol(3)+...+MC(2,N)*sol(N) = MC(2,N+1)
            // MC(3,3)*sol(3)+...+MC(3,N)*sol(N) = MC(3,N+1)
            // .............................................................................
            // MC(N,N)*sol(N) = MC(N,N+1)
            // n numero de incognitas
            // MC matriz de coeficientes extendida
            // MC(1,1), MC(1,2),MC(1,3),...,MC(1,N) |MC(1,N+1)
            // MC(2,1), MC(2,2),MC(2,3),...,MC(2,N) |MC(2,N+1)
            // MC(3,1), MC(3,2),MC(3,3),...,MC(3,N) |MC(3,N+1)
            // ..............................................
            // MC(N,1), MC(N,2),MC(N,3),...,MC(N,N) |MC(N,N+1)
            // Sol vector solucion
            int n;
            int k;
            int i;

            n = Information.UBound(sol); // - LBound(sol) + 1
                                         // Aseguramos que el sistema tiene solucion
            det = 1d;
            var loopTo = n;
            for (k = 1; k <= loopTo; k++)
                det = Mc[k, k] * det;

            for (k = n; k >= 1; k -= 1)
                sol[k] = Mc[k, n + 1] / Mc[k, k];
            for (k = n - 1; k >= 1; k -= 1)
            {
                var loopTo1 = n;
                for (i = k + 1; i <= loopTo1; i++)
                    sol[k] = sol[k] - Mc[k, i] / Mc[k, k] * sol[i];
            }
        }
        public void setxyc()
        {

            // Reads raw RPM data from an existing data file.  This is used to test the curve fit procedure
            // and to re-fit data sent by other folks

            {
                ref var withBlock = ref inputdialog;
                withBlock.Reset();
                withBlock.Filter = "All files (*.*)|*.*";
                withBlock.ShowDialog();
            }

            if (!string.IsNullOrEmpty(inputdialog.FileName))
            {
                inputfile = new StreamReader(inputdialog.FileName);
                switch (inputfile.ReadLine() ?? "")
                {
                    case var @case when @case == SimpleDyno.PowerRunVersion:
                    case "POWER_RUN_6_3":
                        {
                            string temp = "";
                            string[] tempsplit;
                            while (!temp.StartsWith("Actual_MOI:"))
                                temp = inputfile.ReadLine();
                            tempsplit = Strings.Split(temp, " ");
                            SimpleDyno.DynoMomentOfInertia = Conversions.ToDouble(tempsplit[1]);
                            while (inputfile.ReadLine() != "PRIMARY_CHANNEL_RAW_DATA")
                            {
                            }
                            SimpleDyno.DataPoints = Conversions.ToInteger(Strings.Split(inputfile.ReadLine(), " ")[1]); // Number of points
                            inputfile.ReadLine(); // Blank line with headings
                            int Counter;
                            string DataLine;
                            var loopTo = SimpleDyno.DataPoints;
                            for (Counter = 1; Counter <= loopTo; Counter++)
                            {
                                DataLine = inputfile.ReadLine();
                                // CHECK A LOOP SIMILAR TO THE ONE USED TO WRITE THE FILE WOULD WORK WELL HERE
                                SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Counter] = Conversions.ToDouble(Strings.Split(DataLine, " ")[0]); // Time
                                SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Counter] = Conversions.ToDouble(Strings.Split(DataLine, " ")[1]); // rads/s
                            }

                            break;
                        }
                    case var case1 when case1 == "POWER_CURVE": // the string for version 5.5 
                    {
                        while (inputfile.ReadLine() != "PRIMARY_CHANNEL_RAW_DATA")
                        {
                        }
                        SimpleDyno.DataPoints = Conversions.ToInteger(Strings.Split(inputfile.ReadLine(), " ")[1]); // Number of points
                        inputfile.ReadLine(); // Blank line with headings
                        int Counter;
                        string DataLine;
                        var loopTo1 = SimpleDyno.DataPoints;
                        for (Counter = 1; Counter <= loopTo1; Counter++)
                        {
                            DataLine = inputfile.ReadLine();
                            // CHECK A LOOP SIMILAR TO THE ONE USED TO WRITE THE FILE WOULD WORK WELL HERE
                            SimpleDyno.CollectedData[SimpleDyno.SESSIONTIME, Counter] = Conversions.ToDouble(Strings.Split(DataLine, " ")[1]); // Time
                            SimpleDyno.CollectedData[SimpleDyno.RPM1_ROLLER, Counter] = Conversions.ToDouble(Strings.Split(DataLine, " ")[2]) / SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1]; // rads/s
                        }
                        break;
                    }
                }
            }
        }
        #endregion
        private void rdoRPM1_CheckedChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                if (rdoRPM1.Checked)
                {
                    cmbWhichFit.Enabled = true;
                    scrlRPM1Smooth.Enabled = true;
                    scrlStartFit.Enabled = true;
                    txtPowerRunSpikeLevel.Enabled = true;
                    WhichFitData = RPM;
                    pnlDataWindowSetup();
                }
                else
                {
                    cmbWhichFit.Enabled = false;
                    scrlRPM1Smooth.Enabled = false;
                    scrlStartFit.Enabled = false;
                    txtPowerRunSpikeLevel.Enabled = false;
                }
            }
        }
        private void rdoRunDown_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoRunDown.Checked)
            {
                cmbWhichRDFit.Enabled = true;
                if (cmbWhichRDFit.SelectedItem.ToString() == "MA Smooth" | cmbWhichRDFit.SelectedItem.ToString() == "MLSQ Linfit")
                {
                    scrlCoastDownSmooth.Enabled = true;
                }
                else
                {
                    scrlCoastDownSmooth.Enabled = false;
                }
                WhichFitData = RUNDOWN;
                pnlDataWindowSetup();
            }
            else
            {
                cmbWhichRDFit.Enabled = false;
            }
        }
        private void rdoCurrent_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCurrent.Checked)
            {
                scrlCurrentSmooth.Enabled = true;
                WhichFitData = CURRENT;
                pnlDataWindowSetup();
            }
            else
            {
                scrlCurrentSmooth.Enabled = false;
            }
        }
        private void rdoVoltage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoVoltage.Checked)
            {
                scrlVoltageSmooth.Enabled = true;
                WhichFitData = VOLTAGE;
                pnlDataWindowSetup();
            }
            else
            {
                scrlVoltageSmooth.Enabled = false;
            }
        }
        private void cmbWhichFit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                // CHECK - THIS WILL CHANEG THE INDEX CANT CHANGE ULNESS THE RDO IS CHECKED
                if (rdoRPM1.Checked == true)
                {
                    WhichFitData = RPM;
                    SimpleDyno.StopFitting = false;
                    if (cmbWhichFit.SelectedItem.ToString() == "MA Smooth" | cmbWhichFit.SelectedItem.ToString() == "MLSQ Linfit")
                    {
                        scrlRPM1Smooth.Enabled = true;
                    }
                    else
                    {
                        scrlRPM1Smooth.Enabled = false;
                    }
                    FitRPMData();
                }
                else
                {
                    rdoRPM1.Checked = true;
                }
            }
        }
        private void cmbWhichRDFit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (rdoRunDown.Checked == true)
                {
                    WhichFitData = RUNDOWN;
                    SimpleDyno.StopFitting = false;
                    if (cmbWhichRDFit.SelectedItem.ToString() == "MA Smooth" | cmbWhichRDFit.SelectedItem.ToString() == "MLSQ Linfit")
                    {
                        scrlCoastDownSmooth.Enabled = true;
                    }
                    else
                    {
                        scrlCoastDownSmooth.Enabled = false;
                    }
                    FitRPMRunDownData();
                }
                else
                {
                    rdoRunDown.Checked = true;
                }
            }
        }
        private void btnDone_Click(object sender, EventArgs e)
        {
            inputdialog.FileName = ""; // reset for the setxyc hack
            Hide();
            Program.MainI.btnStartLoggingRaw.Enabled = true;
            // If blnfit Then
            if (blnRPMFit && blnCoastDownDownFit && blnVoltageFit && blnCurrentFit)
            {
                WritePowerFile();
                if (interruptAutoCloseEventCounter > EVENTS_TO_INTERRUPT_AUTOCLOSE)
                {
                    if (chkAddOrNew.Checked == false)
                    {
                        SimpleDyno.frmAnalysis.btnClearOverlay_Click_1(this, EventArgs.Empty);
                    }
                    SimpleDyno.frmAnalysis.WindowState = FormWindowState.Normal;
                    SimpleDyno.frmAnalysis.OpenFileDialog1.FileName = SimpleDyno.LogPowerRunDataFileName;
                    SimpleDyno.frmAnalysis.btnAddOverlayFile_Click_1(this, EventArgs.Empty);
                    SimpleDyno.frmAnalysis.ShowDialog();
                }
            }
            else
            {
                // Main.StopFitting = True
                Interaction.MsgBox("All of the expected curve fits were not completed. Please make sure all data are fit appropriately", Constants.vbOKOnly);
                Show();
            } // Main.btnShow_Click(Me, System.EventArgs.Empty)
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SimpleDyno.StopFitting = true;
            blnfit = false;
            Cursor = Cursors.Default;
            // Main.ProcessingData = False
            Program.MainI.Cursor = Cursors.Default;
        }

        private void Fit_MouseMove(object sender, MouseEventArgs e)
        {
            interruptAutoCloseEventCounter = interruptAutoCloseEventCounter + 1;
        }

        private void pnlDataWindow_MouseMove(object sender, MouseEventArgs e)
        {
            interruptAutoCloseEventCounter = interruptAutoCloseEventCounter + 1;
        }
    }
}
