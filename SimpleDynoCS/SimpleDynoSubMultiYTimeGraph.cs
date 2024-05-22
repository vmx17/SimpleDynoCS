using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{
    public partial class SimpleDynoSubMultiYTimeGraph : SimpleDynoSubForm
    {
        private Rectangle myGraphSurface;
        private Rectangle myDataRectangle;

        private Point[] MajorXTickOuter = new Point[5];
        private Point[] MajorXTickInner = new Point[5];
        private Point[] MinorXTickOuter = new Point[5];
        private Point[] MinorXTickInner = new Point[5];

        private Point[,] MajorYTickOuter;
        private Point[,] MajorYTickInner;
        private Point[,] MinorYTickOuter;
        private Point[,] MinorYTickInner;

        private Point[] XTickLabelPositions = new Point[5];
        private Point[,] YTickLabelPositions;
        private Point XOptionPosition;
        private Point XUnitPosition;
        private Point[] YOptionPosition = new Point[5];
        private Point[] YUnitPosition = new Point[5];

        private int NumberOfMajorTicks;
        private int NumberOfMinorTicks;

        private int OldX;
        private int[] OldY = new int[5];
        private int NewX;
        private int[] NewY = new int[5];
        private double X_Time;

        private string[] XTickLabels;
        private string[,] YTickLabels;
        private string XAxisLabel;
        private string[] YAxisLabel = new string[5];

        private double NewSessionTime;
        private double OldSessionTime;
        public SimpleDynoSubMultiYTimeGraph()
        {
            InitializeComponent();
        }
        public override void AddControlSpecificOptionItems()
        {
            ToolStripMenuItem TestStrip;
            string str1;
            string[] str2;
            string[] str3;

            str1 = "Configuration";
            str2 = new[] { "Lines", "Points" };
            str3 = Array.Empty<string>();

            TestStrip = this.CreateAToolStripMenuItem("O", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

            str1 = "Y Range";
            str2 = new[] { "Minimum", "Maximum" };
            str3 = new[] { "TXT" };

            TestStrip = this.CreateAToolStripMenuItem("Y", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

            str1 = "Time Range";
            str2 = new[] { "Maximum" };
            str3 = new[] { "TXT" };

            TestStrip = this.CreateAToolStripMenuItem("T", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

            str1 = "Remove Plot";
            str2 = Array.Empty<string>();
            str3 = Array.Empty<string>();

            TestStrip = this.CreateAToolStripMenuItem("X", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);
        }

        public override void ControlSpecficCreateFromSerializedData(string[] Sent)
        {

        }

        public override void ControlSpecificInitialization()
        {
            int Counter;

            this.myType = "MultiYTimeGraph";
            this.Y_Number_Allowed = 4;
            this.myConfiguration = "Lines";

            var loopTo = this.Y_Number_Allowed;
            for (Counter = 1; Counter <= loopTo; Counter++)
                this.IsThisYSelected[Counter] = false;

            this.IsThisYSelected[1] = true;

            this.X_PrimaryPointer = 0; // UBound(CopyOfDataNames) 'always points to the session timer
            this.X_MinCurMaxPointer = 1; // always points to the "current" time
            this.X_PrimaryLabel = "Time";
            this.X_UnitsLabel = "Seconds";
            NumberOfMajorTicks = 5;
            NumberOfMinorTicks = 21;
            this.X_Maximum = 10d;
            this.X_Minimum = 0d;

            MajorXTickOuter = new Point[NumberOfMajorTicks + 1];
            MajorXTickInner = new Point[NumberOfMajorTicks + 1];
            MinorXTickOuter = new Point[NumberOfMinorTicks + 1];
            MinorXTickInner = new Point[NumberOfMinorTicks + 1];
            MajorYTickOuter = new Point[5, NumberOfMajorTicks + 1];
            MajorYTickInner = new Point[5, NumberOfMajorTicks + 1];
            MinorYTickOuter = new Point[5, NumberOfMinorTicks + 1];
            MinorYTickInner = new Point[5, NumberOfMinorTicks + 1];
            XTickLabelPositions = new Point[NumberOfMajorTicks + 1];
            YTickLabelPositions = new Point[5, NumberOfMajorTicks + 1];
            XTickLabels = new string[NumberOfMajorTicks + 1];
            YTickLabels = new string[5, NumberOfMajorTicks + 1];
        }

        public override void ControlSpecificOptionSelection(string Sent)
        {
            switch (Sent ?? "")
            {
                case var @case when @case == "O_0":
                    {
                        this.myConfiguration = "Lines";
                        break;
                    }
                case var case1 when case1 == "O_1":
                    {
                        this.myConfiguration = "Points";
                        break;
                    }
                case var case2 when case2 == "X":
                    {
                        this.IsThisYSelected[this.XY_Selected] = false;
                        this.Y_PrimaryLabel[this.XY_Selected] = "Parameter";
                        this.Y_UnitsLabel[this.XY_Selected] = "Units";
                        break;
                    }

                default:
                    {
                        string[] Temp;
                        Temp = Strings.Split(Sent, " ");
                        // If Temp(0) = "T_0_0" Then X_Minimum = CDbl(Temp(1)) REMOVING Time Minimum - Will need to be put back in for Y vs X plot
                        if (Temp[0] == "T_0_0")
                            this.X_Maximum = Conversions.ToDouble(Temp[1]);
                        if (Temp[0] == "Y_0_0")
                            this.Y_Minimum[this.XY_Selected] = Conversions.ToDouble(Temp[1]);
                        if (Temp[0] == "Y_1_0")
                            this.Y_Maximum[this.XY_Selected] = Conversions.ToDouble(Temp[1]);
                        break;
                    }
            }
        }

        public override void ControlSpecificResize()
        {
            double Width;
            double Height;
            int Count;
            double MajorTickLength;
            double MinorTickLength;
            double Increment;

            Increment = 0.1d;

            {
                ref var withBlock = ref myGraphSurface;
                withBlock.Width = this.ClientSize.Width - 10; // padding 5 each side
                withBlock.Height = this.ClientSize.Height - 10; // padding 5 each side
                withBlock.X = 5;
                withBlock.Y = 5;
                MajorTickLength = withBlock.Height / 25d;
                MinorTickLength = MajorTickLength / 2d;
            }

            for (Count = 1; Count <= 4; Count++)
                this.Y_DataPen[Count].Width = (float)(int)Math.Round(MinorTickLength / 3d);


            {
                ref var withBlock1 = ref myDataRectangle;
                withBlock1.Height = (int)Math.Round(myGraphSurface.Height * 0.7d);
                withBlock1.Width = (int)Math.Round(myGraphSurface.Width * 0.7d);
                withBlock1.X = (int)Math.Round((double)this.ClientSize.Width / 2d - withBlock1.Width / 2d);
                withBlock1.Y = (int)Math.Round((double)this.ClientSize.Height / 2d - withBlock1.Height / 2d);

                var loopTo = NumberOfMajorTicks;
                for (Count = 1; Count <= loopTo; Count++)
                {
                    MajorXTickOuter[Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    MajorXTickOuter[Count].Y = (int)Math.Round(withBlock1.Y + withBlock1.Height + MajorTickLength);
                    MajorXTickInner[Count].X = MajorXTickOuter[Count].X;
                    MajorXTickInner[Count].Y = withBlock1.Y + withBlock1.Height;
                    XTickLabels[Count] = (this.X_Minimum + (this.X_Maximum - this.X_Minimum) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1)).ToString();

                    // Y1 - left side - outside
                    MajorYTickOuter[1, Count].X = (int)Math.Round(withBlock1.X - MajorTickLength);
                    MajorYTickOuter[1, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    MajorYTickInner[1, Count].X = withBlock1.X;
                    MajorYTickInner[1, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    YTickLabels[1, Count] = (this.Y_Minimum[1] + (this.Y_Maximum[1] - this.Y_Minimum[1]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1)).ToString();

                    // Y2 - left side - inside
                    MajorYTickOuter[2, Count].X = (int)Math.Round(withBlock1.X + MajorTickLength);
                    MajorYTickOuter[2, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    MajorYTickInner[2, Count].X = withBlock1.X;
                    MajorYTickInner[2, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    YTickLabels[2, Count] = (this.Y_Minimum[2] + (this.Y_Maximum[2] - this.Y_Minimum[2]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1)).ToString();

                    // Y3 - right side - inside
                    MajorYTickOuter[3, Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width - MajorTickLength);
                    MajorYTickOuter[3, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    MajorYTickInner[3, Count].X = withBlock1.X + withBlock1.Width;
                    MajorYTickInner[3, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    YTickLabels[3, Count] = (this.Y_Minimum[3] + (this.Y_Maximum[3] - this.Y_Minimum[3]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1)).ToString();

                    // Y4 - right side - outside
                    MajorYTickOuter[4, Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width + MajorTickLength);
                    MajorYTickOuter[4, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    MajorYTickInner[4, Count].X = withBlock1.X + withBlock1.Width;
                    MajorYTickInner[4, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMajorTicks - 1) * (Count - 1));
                    YTickLabels[4, Count] = (this.Y_Minimum[4] + (this.Y_Maximum[4] - this.Y_Minimum[4]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1)).ToString();


                }
                var loopTo1 = NumberOfMinorTicks;
                for (Count = 1; Count <= loopTo1; Count++)
                {
                    MinorXTickOuter[Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                    MinorXTickOuter[Count].Y = (int)Math.Round(withBlock1.Y + withBlock1.Height + MinorTickLength);
                    MinorXTickInner[Count].X = MinorXTickOuter[Count].X;
                    MinorXTickInner[Count].Y = withBlock1.Y + withBlock1.Height;

                    // Y1
                    MinorYTickOuter[1, Count].X = (int)Math.Round(withBlock1.X - MinorTickLength);
                    MinorYTickOuter[1, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                    MinorYTickInner[1, Count].X = withBlock1.X;
                    MinorYTickInner[1, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));

                    // Y2
                    MinorYTickOuter[2, Count].X = (int)Math.Round(withBlock1.X + MinorTickLength);
                    MinorYTickOuter[2, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                    MinorYTickInner[2, Count].X = withBlock1.X;
                    MinorYTickInner[2, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));

                    // Y3
                    MinorYTickOuter[3, Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width - MinorTickLength);
                    MinorYTickOuter[3, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                    MinorYTickInner[3, Count].X = withBlock1.X + withBlock1.Width;
                    MinorYTickInner[3, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));

                    // Y4
                    MinorYTickOuter[4, Count].X = (int)Math.Round(withBlock1.X + withBlock1.Width + MinorTickLength);
                    MinorYTickOuter[4, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                    MinorYTickInner[4, Count].X = withBlock1.X + withBlock1.Width;
                    MinorYTickInner[4, Count].Y = (int)Math.Round(withBlock1.Bottom - withBlock1.Height / (double)(NumberOfMinorTicks - 1) * (Count - 1));
                }
            }

            Width = Math.Abs(MajorXTickInner[1].X - MajorXTickInner[2].X);
            Height = (myGraphSurface.Bottom - MajorXTickOuter[1].Y) / 2d;
            this.X_AxisFont = new Font(this.X_AxisFont.Name, (float)Increment);
            while (!((double)this.Grafx.Graphics.MeasureString("99999", this.X_AxisFont).Width > Width | (double)this.Grafx.Graphics.MeasureString("99999", this.X_AxisFont).Height > Height))
            {
                Increment += 0.1d;
                this.X_AxisFont = new Font(this.X_AxisFont.Name, (float)Increment);
            }
            Increment = 0.1d;

            int MaxLength = 1;
            for (Count = 1; Count <= 4; Count++)
            {
                YAxisLabel[Count] = this.Y_PrimaryLabel[Count] + Constants.vbCrLf + "(" + this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[Count]] + " " + this.Y_UnitsLabel[Count] + ")";
                if (this.Y_PrimaryLabel[Count].ToString().Length > this.Y_PrimaryLabel[MaxLength].ToString().Length)
                {
                    MaxLength = Count;
                }
            }
            // CHECK - Remove the following prior to release
            // For Count = 1 To 4
            // If Y_primarylabel(Count).ToString.Length > Y_primarylabel(MaxLength).ToString.Length Then
            // MaxLength = Count
            // End If
            // Next

            Width = myDataRectangle.Left;
            Height = myDataRectangle.Top / 2d;
            this.Y_AxisFont = new Font(this.Y_AxisFont.Name, (float)Increment);
            // CHECK - Remove the following prior to release
            // Do Until Grafx.Graphics.MeasureString(Y_PrimaryLabel(MaxLength), Y_AxisFont).Width > Width Or Grafx.Graphics.MeasureString(Y_PrimaryLabel(MaxLength), Y_AxisFont).Height > Height
            while (!((double)this.Grafx.Graphics.MeasureString(YAxisLabel[MaxLength], this.Y_AxisFont).Width > Width | (double)this.Grafx.Graphics.MeasureString(YAxisLabel[MaxLength], this.Y_AxisFont).Height > Height))
            {
                Increment += 0.1d;
                this.Y_AxisFont = new Font(this.Y_AxisFont.Name, (float)Increment);
            }

            var loopTo2 = NumberOfMajorTicks;
            for (Count = 1; Count <= loopTo2; Count++)
            {
                XTickLabelPositions[Count].X = (int)Math.Round((float)MajorXTickOuter[Count].X - this.Grafx.Graphics.MeasureString(XTickLabels[Count], this.X_AxisFont).Width / 2f);
                XTickLabelPositions[Count].Y = MajorXTickOuter[Count].Y;
                // Y1
                YTickLabelPositions[1, Count].Y = (int)Math.Round((float)MajorYTickOuter[1, Count].Y - this.Grafx.Graphics.MeasureString(YTickLabels[1, Count], this.Y_AxisFont).Height / 2f);
                YTickLabelPositions[1, Count].X = (int)Math.Round((float)MajorYTickOuter[1, Count].X - this.Grafx.Graphics.MeasureString(YTickLabels[1, Count], this.Y_AxisFont).Width);
                // Y2
                YTickLabelPositions[2, Count].Y = (int)Math.Round((float)MajorYTickOuter[2, Count].Y - this.Grafx.Graphics.MeasureString(YTickLabels[2, Count], this.Y_AxisFont).Height / 2f);
                YTickLabelPositions[2, Count].X = MajorYTickOuter[2, Count].X; // - grafx.Graphics.MeasureString(YTickLabels(2, Count), Y_axisfont).Width
                                                                               // Y3
                YTickLabelPositions[3, Count].Y = (int)Math.Round((float)MajorYTickOuter[3, Count].Y - this.Grafx.Graphics.MeasureString(YTickLabels[3, Count], this.Y_AxisFont).Height / 2f);
                YTickLabelPositions[3, Count].X = (int)Math.Round((float)MajorYTickOuter[3, Count].X - this.Grafx.Graphics.MeasureString(YTickLabels[3, Count], this.Y_AxisFont).Width);
                // Y4
                YTickLabelPositions[4, Count].Y = (int)Math.Round((float)MajorYTickOuter[4, Count].Y - this.Grafx.Graphics.MeasureString(YTickLabels[4, Count], this.Y_AxisFont).Height / 2f);
                YTickLabelPositions[4, Count].X = MajorYTickOuter[4, Count].X; // - grafx.Graphics.MeasureString(YTickLabels(4, Count), Y_axisfont).Width
            }

            XAxisLabel = "Time (seconds)";
            // CHECK - Remove the following prior to release
            // For Count = 1 To 4
            // YAxisLabel(Count) = Y_PrimaryLabel(Count) & vbCrLf & "(" & myMinCurMaxAbb(Y_MinCurMaxPointer(Count)) & " " & Y_UnitsLabel(Count) & ")"
            // Next
            XOptionPosition.X = (int)Math.Round(myDataRectangle.Width / 2d + MajorXTickOuter[1].X - (double)(this.Grafx.Graphics.MeasureString(XAxisLabel, this.X_AxisFont).Width / 2f));
            XOptionPosition.Y = (int)Math.Round((float)MajorXTickOuter[1].Y + this.Grafx.Graphics.MeasureString(XAxisLabel, this.X_AxisFont).Height);
            // Y1
            YOptionPosition[1].X = myGraphSurface.Left;
            YOptionPosition[1].Y = (int)Math.Round((float)YTickLabelPositions[1, NumberOfMajorTicks].Y - this.Grafx.Graphics.MeasureString(YAxisLabel[1], this.Y_AxisFont).Height);
            // Y2
            YOptionPosition[2].X = myDataRectangle.Left; // myGraphSurface.Left 
            YOptionPosition[2].Y = (int)Math.Round((float)YTickLabelPositions[2, NumberOfMajorTicks].Y - this.Grafx.Graphics.MeasureString(YAxisLabel[2], this.Y_AxisFont).Height);
            // Y3
            YOptionPosition[3].X = (int)Math.Round((float)myDataRectangle.Right - this.Grafx.Graphics.MeasureString(YAxisLabel[3], this.Y_AxisFont).Width); // myGraphSurface.Left
            YOptionPosition[3].Y = (int)Math.Round((float)YTickLabelPositions[3, NumberOfMajorTicks].Y - this.Grafx.Graphics.MeasureString(YAxisLabel[3], this.Y_AxisFont).Height);
            // Y4
            YOptionPosition[4].X = myDataRectangle.Right;
            YOptionPosition[4].Y = (int)Math.Round((float)YTickLabelPositions[4, NumberOfMajorTicks].Y - this.Grafx.Graphics.MeasureString(YAxisLabel[4], this.Y_AxisFont).Height);

            ResetSDForm();
        }
        public override void ResetSDForm()
        {
            this.X_Result = 0d;
            X_Time = 0d;
            OldSessionTime = 0d;
            Redraw();
            for (int count = 1, loopTo = this.Y_Number_Allowed; count <= loopTo; count++)
                OldY[count] = (int)Math.Round((double)myDataRectangle.Bottom - (0d - this.Y_Minimum[count]) / (this.Y_Maximum[count] - this.Y_Minimum[count]) * (double)myDataRectangle.Height);
        }
        public override string ControlSpecificSerializationData()
        {
            return default;
        }

        public override void DrawToBuffer(Graphics g)
        {
            // Calc the result and labels
            int Count;

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;


            if (this.TimerTiggered == false)
            {
                Redraw();
            }
            else
            {
                {
                    var withBlock = this.Grafx.Graphics;
                    if (X_Time >= this.X_Maximum)
                    {
                        X_Time = 0d;
                        Redraw();
                    }

                    X_Time = X_Time + (this.X_Result - OldSessionTime);
                    OldSessionTime = this.X_Result;
                    if (X_Time >= this.X_Maximum)
                    {
                        NewX = myDataRectangle.Width + myDataRectangle.Left;
                    }
                    else
                    {
                        NewX = (int)Math.Round((X_Time - this.X_Minimum) / (this.X_Maximum - this.X_Minimum) * (double)myDataRectangle.Width + (double)myDataRectangle.Left);
                    }
                    for (Count = 1; Count <= 4; Count++)
                    {
                        if (this.IsThisYSelected[Count] == true)
                        {
                            if (this.Y_Result[Count] > this.Y_Maximum[Count])
                                this.Y_Result[Count] = this.Y_Maximum[Count];
                            if (this.Y_Result[Count] < this.Y_Minimum[Count])
                                this.Y_Result[Count] = this.Y_Minimum[Count];
                            NewY[Count] = (int)Math.Round((double)myDataRectangle.Bottom - (this.Y_Result[Count] - this.Y_Minimum[Count]) / (this.Y_Maximum[Count] - this.Y_Minimum[Count]) * (double)myDataRectangle.Height);
                            switch (this.myConfiguration ?? "")
                            {
                                case var @case when @case == "Lines":
                                    {
                                        withBlock.DrawLine(this.Y_DataPen[Count], OldX, OldY[Count], NewX, NewY[Count]);
                                        OldY[Count] = NewY[Count];
                                        break;
                                    }
                                case var case1 when case1 == "Points":
                                    {
                                        withBlock.DrawEllipse(this.Y_DataPen[Count], (float)NewX, (float)NewY[Count], this.Y_DataPen[Count].Width, this.Y_DataPen[Count].Width);
                                        break;
                                    }
                            }
                        }
                    }
                    OldX = NewX;

                }
            }
        }
        private void Redraw()
        {
            int Tickcount;
            int PlotYCount;
            {
                var withBlock = this.Grafx.Graphics;
                withBlock.Clear(this.BackClr);

                var loopTo = NumberOfMajorTicks;
                for (Tickcount = 1; Tickcount <= loopTo; Tickcount++)
                {
                    withBlock.DrawLine(this.AxisPen, MajorXTickOuter[Tickcount], MajorXTickInner[Tickcount]);
                    withBlock.DrawString(XTickLabels[Tickcount], this.X_AxisFont, this.AxisBrush, XTickLabelPositions[Tickcount]);
                    for (PlotYCount = 1; PlotYCount <= 4; PlotYCount++)
                    {
                        if (this.IsThisYSelected[PlotYCount] == true)
                        {
                            withBlock.DrawLine(this.AxisPen, MajorYTickOuter[PlotYCount, Tickcount], MajorYTickInner[PlotYCount, Tickcount]);
                            withBlock.DrawString(YTickLabels[PlotYCount, Tickcount], this.Y_AxisFont, this.AxisBrush, YTickLabelPositions[PlotYCount, Tickcount]);
                        }
                    }
                }

                withBlock.DrawLine(this.AxisPen, MajorXTickInner[1], MajorXTickInner[NumberOfMajorTicks]);
                withBlock.DrawLine(this.AxisPen, MajorYTickInner[1, 1], MajorYTickInner[1, NumberOfMajorTicks]);
                if (this.IsThisYSelected[3] == true | this.IsThisYSelected[4] == true)
                    withBlock.DrawLine(this.AxisPen, MajorYTickInner[3, 1], MajorYTickInner[3, NumberOfMajorTicks]);
                var loopTo1 = NumberOfMinorTicks;
                for (Tickcount = 1; Tickcount <= loopTo1; Tickcount++)
                {
                    withBlock.DrawLine(this.AxisPen, MinorXTickOuter[Tickcount], MinorXTickInner[Tickcount]);
                    for (PlotYCount = 1; PlotYCount <= 4; PlotYCount++)
                    {
                        if (this.IsThisYSelected[PlotYCount] == true)
                        {
                            withBlock.DrawLine(this.AxisPen, MinorYTickOuter[PlotYCount, Tickcount], MinorYTickInner[PlotYCount, Tickcount]);
                        }
                    }
                }

                withBlock.DrawString(XAxisLabel, this.X_AxisFont, this.AxisBrush, XOptionPosition);

                for (PlotYCount = 1; PlotYCount <= 4; PlotYCount++)
                {
                    if (this.IsThisYSelected[PlotYCount] == true)
                    {
                        withBlock.DrawString(YAxisLabel[PlotYCount], this.Y_AxisFont, this.Y_DataBrush[PlotYCount], YOptionPosition[PlotYCount]);
                    }
                }

                NewX = (int)Math.Round((0d - this.X_Minimum) / (this.X_Maximum - this.X_Minimum) * (double)myDataRectangle.Width + (double)myDataRectangle.Left);
                OldX = NewX;

                // For PlotYCount = 1 To 4
                // NewY(PlotYCount) = myDataRectangle.Bottom - (((0 - Y_Minimum(PlotYCount)) / (Y_Maximum(PlotYCount) - Y_Minimum(PlotYCount))) * myDataRectangle.Height)
                // OldY(PlotYCount) = NewY(PlotYCount)
                // Next

            }
        }
        public override void ShowTheMenu()
        {
            // This needs to be overidden from the parent as there are two options for data, x and y
            // and which one we select is based on where the mouseclick happened
            var Where = new Point();
            Where = this.PointToClient(Control.MousePosition);


            Where.X = Control.MousePosition.X;
            Where.Y = Control.MousePosition.Y;
            // First check that we are above the X axis
            if (this.PointToClient(Control.MousePosition).Y < myDataRectangle.Bottom && this.PointToClient(Control.MousePosition).Y > myDataRectangle.Top)
            {
                switch (this.PointToClient(Control.MousePosition).X)
                {
                    case var @case when @case < myDataRectangle.Left:
                    {
                        this.XY_Selected = 1;
                        this.IsThisYSelected[1] = true;
                        break;
                    }
                    case var case1 when case1 > myDataRectangle.Right:
                    {
                        this.XY_Selected = 4;
                        this.IsThisYSelected[4] = true;
                        break;
                    }

                    default:
                    {
                        if ((double)this.PointToClient(Control.MousePosition).X < myDataRectangle.Left + myDataRectangle.Width / 2d)
                        {
                            this.XY_Selected = 2;
                            this.IsThisYSelected[2] = true;
                        }
                        else
                        {
                            this.XY_Selected = 3;
                            this.IsThisYSelected[3] = true;
                        }

                        break;
                    }
                }
                this.Contextmnu.Show(Where);
            }

        }
    }
}
