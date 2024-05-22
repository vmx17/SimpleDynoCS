using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{
    public partial class SimpleDynoSubGauge : SimpleDynoSubForm
    {

        private Rectangle myGaugeSurface;
        private Rectangle myDialRectangle;

        // Dim BackgroundImage As Bitmap
        // Dim BackgroundGraphics As Graphics
        private double TempTemp;

        private Point[] MajorTickOuter;
        private Point[] MajorTickInner;
        private Point[] MinorTickOuter;
        private Point[] MinorTickInner;
        private Point[] TickLabelPositions;
        private Point ParameterPosition;
        private Point UnitPosition;
        private Point EndOfNeedle;
        private Point Center;
        private int SweepClockwise;
        private int NumberOfMajorTicks;
        private int NumberOfMinorTicks;
        private int LongestLabel;
        private float Angle;
        private float PointAngle;
        private float StartAngle;
        private string[] TickLabels;

        // CHECK - PULL TIMER FROM RELEASE
        // Private TestTimer As Timer
        // CHECK - PULL TIMER FROM RELEASE
        private void testtick(object sender, EventArgs e)
        {
            Angle += 5f;
            if (Angle == 360f)
            {
                Angle = 10f;
                StartAngle += 5f;
            }
            ControlSpecificResize();
        }
        public override void ControlSpecificInitialization()
        {


            this.myType = "Gauge";
            this.Y_Number_Allowed = 1;
            this.XY_Selected = 1;

            // ///////
            // 'CHECK - PULL TIMER FROM RELEASE
            // TestTimer = New Timer
            // TestTimer.Interval = 20
            // TestTimer.Stop()
            // '//////
            // AddHandler Me.TestTimer.Tick, AddressOf testtick

            this.myConfiguration = "270 270 1";
            Angle = 270f;
            PointAngle = 270f; // CHECK - not been used now - forward compatability item
            StartAngle = PointAngle - Angle / 2f;
            SweepClockwise = 1;
            NumberOfMajorTicks = 5;
            NumberOfMinorTicks = 21;

            MajorTickOuter = new Point[NumberOfMajorTicks + 1];
            MajorTickInner = new Point[NumberOfMajorTicks + 1];
            MinorTickOuter = new Point[NumberOfMinorTicks + 1];
            MinorTickInner = new Point[NumberOfMinorTicks + 1];
            TickLabelPositions = new Point[NumberOfMajorTicks + 1];
            TickLabels = new string[NumberOfMajorTicks + 1];

        }
        public override void ControlSpecificResize()
        {

            int Count;
            double MajorTickLength;
            double MinorTickLength;
            double Increment;

            {
                ref var withBlock = ref myGaugeSurface;
                withBlock.Width = (int)Math.Round((double)this.ClientSize.Width * 0.9d); // padding 1% each side
                withBlock.Height = (int)Math.Round((double)this.ClientSize.Height * 0.9d); // padding 1% each side
                withBlock.X = (int)Math.Round((double)this.ClientSize.Width * 0.05d); // Puts the drawing surface top corner
                withBlock.Y = (int)Math.Round((double)this.ClientSize.Height * 0.05d); // in a posisition to pad 5 all around
            }

            double MinX = 1d;
            double MinY = 1d;
            double MaxX = -1;
            double MaxY = -1;
            double TempX;
            double TempY;
            double TempWidth;
            double TempHeight;
            double TempCenterX;
            double TempCenterY;

            for (int Arc = (int)Math.Round(StartAngle), loopTo = (int)Math.Round(StartAngle + Angle); Arc <= loopTo; Arc++)
            {
                TempX = Math.Cos(this.ConvertedToRadians((double)(360 - Arc)));
                TempY = Math.Sin(this.ConvertedToRadians((double)(360 - Arc)));
                if (TempX < MinX)
                    MinX = TempX;
                if (TempX > MaxX)
                    MaxX = TempX;
                if (TempY < MinY)
                    MinY = TempY;
                if (TempY > MaxY)
                    MaxY = TempY;
            }

            MaxX = (int)Math.Round(MaxX * 1000d) / 1000d;
            MaxY = (int)Math.Round(MaxY * 1000d) / 1000d;
            MinX = (int)Math.Round(MinX * 1000d) / 1000d;
            MinY = (int)Math.Round(MinY * 1000d) / 1000d;

            if (MinX >= 0d)
            {
                TempWidth = MaxX;
                TempCenterX = 0d;
            }
            else if (MaxX > 0d)
            {
                TempWidth = Math.Abs(MaxX - MinX);
                TempCenterX = TempWidth / Math.Abs(TempWidth / MinX);
            }
            else
            {
                TempWidth = Math.Abs(MinX);
                TempCenterX = TempWidth;
            } // 1

            if (MinY >= 0d)
            {
                TempHeight = MaxY;
                TempCenterY = TempHeight;
            }
            else if (MaxY > 0d)
            {
                TempHeight = Math.Abs(MaxY - MinY);
                TempCenterY = TempHeight / Math.Abs(TempHeight / MaxY);
            }
            else
            {
                TempHeight = Math.Abs(MinY);
                TempCenterY = 0d;
            }

            double FoldWidth;
            double FoldHeight;
            FoldWidth = myGaugeSurface.Width / TempWidth;
            FoldHeight = myGaugeSurface.Height / TempHeight;

            if (FoldWidth >= FoldHeight)
            {
                myDialRectangle.Height = (int)Math.Round(2d * FoldHeight);
                myDialRectangle.Width = (int)Math.Round(2d * FoldHeight);
                MajorTickLength = myDialRectangle.Height * 0.15d;
                MinorTickLength = MajorTickLength / 2d;
                Center.X = (int)Math.Round(myGaugeSurface.X + myGaugeSurface.Width / 2d - TempWidth * FoldHeight / 2d + TempWidth * FoldHeight * TempCenterX / TempWidth);
                Center.Y = (int)Math.Round(myGaugeSurface.Y + myGaugeSurface.Height * (TempCenterY / TempHeight));
            }
            else
            {
                myDialRectangle.Height = (int)Math.Round(2d * FoldWidth);
                myDialRectangle.Width = (int)Math.Round(2d * FoldWidth);
                MajorTickLength = myDialRectangle.Width * 0.15d;
                MinorTickLength = MajorTickLength / 2d;
                Center.X = (int)Math.Round(myGaugeSurface.X + myGaugeSurface.Width * (TempCenterX / TempWidth));
                Center.Y = (int)Math.Round(myGaugeSurface.Y + myGaugeSurface.Height / 2d - TempHeight * FoldWidth / 2d + TempHeight * FoldWidth * TempCenterY / TempHeight);
            }

            myDialRectangle.X = Center.X - (int)Math.Round(myDialRectangle.Width / 2d);
            myDialRectangle.Y = Center.Y - (int)Math.Round(myDialRectangle.Height / 2d);

            {
                ref var withBlock1 = ref myDialRectangle;
                var loopTo1 = NumberOfMajorTicks;
                for (Count = 1; Count <= loopTo1; Count++)
                {
                    MajorTickOuter[Count].X = (int)Math.Round((double)Center.X + withBlock1.Width / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1)))));
                    MajorTickOuter[Count].Y = (int)Math.Round((double)Center.Y + withBlock1.Height / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1)))));
                    MajorTickInner[Count].X = (int)Math.Round((double)Center.X + (withBlock1.Width - MajorTickLength) / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1)))));
                    MajorTickInner[Count].Y = (int)Math.Round((double)Center.Y + (withBlock1.Height - MajorTickLength) / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1)))));
                    if (SweepClockwise == 1)
                    {
                        TickLabels[Count] = this.NewCustomFormat(this.Y_Minimum[this.Y_Number_Allowed] + (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1));
                    }
                    else
                    {
                        TickLabels[Count] = this.NewCustomFormat(this.Y_Maximum[this.Y_Number_Allowed] - (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) / (double)(NumberOfMajorTicks - 1) * (double)(Count - 1));
                    }
                }
                var loopTo2 = NumberOfMinorTicks;
                for (Count = 1; Count <= loopTo2; Count++)
                {
                    MinorTickOuter[Count].X = (int)Math.Round((double)Center.X + withBlock1.Width / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMinorTicks - 1) * (Count - 1)))));
                    MinorTickOuter[Count].Y = (int)Math.Round((double)Center.Y + withBlock1.Height / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMinorTicks - 1) * (Count - 1)))));
                    MinorTickInner[Count].X = (int)Math.Round((double)Center.X + (withBlock1.Width - MinorTickLength) / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMinorTicks - 1) * (Count - 1)))));
                    MinorTickInner[Count].Y = (int)Math.Round((double)Center.Y + (withBlock1.Height - MinorTickLength) / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMinorTicks - 1) * (Count - 1)))));
                }
            }

            var TickLabelWidths = new double[NumberOfMajorTicks + 1];
            var TickLabelHeights = new double[NumberOfMajorTicks + 1];
            {
                ref var withBlock2 = ref myDialRectangle;
                double l;
                int Score;

                Increment = 0d;
                do
                {
                    Increment += 0.1d;
                    this.Y_AxisFont = new Font(this.Y_AxisFont.Name, (float)Increment);
                    Score = 0;
                    var loopTo3 = NumberOfMajorTicks;
                    for (Count = 1; Count <= loopTo3; Count++)
                    {
                        TickLabelWidths[Count] = (double)this.Grafx.Graphics.MeasureString(TickLabels[Count], this.Y_AxisFont).Width;
                        TickLabelHeights[Count] = (double)this.Grafx.Graphics.MeasureString(TickLabels[Count], this.Y_AxisFont).Height;
                        l = Math.Pow(Math.Pow(TickLabelWidths[Count] / 2d, 2d) + Math.Pow(TickLabelHeights[Count], 2d), 0.5d);
                        TickLabelPositions[Count].X = (int)Math.Round((double)Center.X + (withBlock2.Width - MajorTickLength - l) / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1))))) - (int)Math.Round(this.Grafx.Graphics.MeasureString(TickLabels[Count], this.Y_AxisFont).Width / 2f);
                        TickLabelPositions[Count].Y = (int)Math.Round((double)Center.Y + (withBlock2.Height - MajorTickLength - l) / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle / (NumberOfMajorTicks - 1) * (Count - 1))))) - (int)Math.Round(this.Grafx.Graphics.MeasureString(TickLabels[Count], this.Y_AxisFont).Height / 2f);
                    }
                    TickLabelHeights[0] = (double)(this.Grafx.Graphics.MeasureString(this.Y_PrimaryLabel[this.Y_Number_Allowed], this.Y_AxisFont).Height * 2f); // To cover primary lavel and units
                    TickLabelWidths[0] = (double)this.Grafx.Graphics.MeasureString(this.Y_PrimaryLabel[this.Y_Number_Allowed], this.Y_AxisFont).Width;
                    // This option based on centering in the gaugesurface
                    // TickLabelPositions(0).Y = CInt(myGaugeSurface.Y + myGaugeSurface.Height / 2 - TickLabelHeights(0) / 4) '(Center.Y + (MajorTickInner(3).Y - Center.Y) / 2 - TickLabelHeights(0) / 2)
                    // TickLabelPositions(0).X = CInt(myGaugeSurface.X + myGaugeSurface.Width / 2 - TickLabelWidths(0) / 2) 'CInt(Center.X + (MajorTickInner(3).X - Center.X) / 2 - TickLabelWidths(0) / 2)
                    // This option based on centering between needle centre and tick 3
                    TickLabelPositions[0].Y = (int)Math.Round(Center.Y + (MajorTickInner[3].Y - Center.Y) / 2d - TickLabelHeights[0] / 2d);
                    TickLabelPositions[0].X = (int)Math.Round(Center.X + (MajorTickInner[3].X - Center.X) / 2d - TickLabelWidths[0] / 2d);

                    for (int o = 0, loopTo4 = NumberOfMajorTicks; o <= loopTo4; o++)
                    {
                        for (int i = 0, loopTo5 = NumberOfMajorTicks; i <= loopTo5; i++)
                        {
                            if (TickLabelPositions[o].X < TickLabelPositions[i].X + TickLabelWidths[i] && TickLabelPositions[o].X + TickLabelWidths[o] > TickLabelPositions[i].X && TickLabelPositions[o].Y < TickLabelPositions[i].Y + TickLabelHeights[i] && TickLabelPositions[o].Y + TickLabelHeights[o] > TickLabelPositions[i].Y)


                            {
                                // No overlap
                                Score += 1;
                            }
                            else
                            {

                            }
                        }
                    }
                }

                while (Score <= NumberOfMajorTicks + 1);
                // Need to check that the end ticks (1 and 5) are not outside the Gaugesurface area
                if (TickLabelPositions[1].X < this.ClientRectangle.X)
                    TickLabelPositions[1].X = this.ClientRectangle.X;
                if (TickLabelPositions[5].X < this.ClientRectangle.X)
                    TickLabelPositions[5].X = this.ClientRectangle.X;
                if (TickLabelPositions[1].Y < this.ClientRectangle.Y)
                    TickLabelPositions[1].Y = this.ClientRectangle.Y;
                if (TickLabelPositions[5].Y < this.ClientRectangle.Y)
                    TickLabelPositions[5].Y = this.ClientRectangle.Y;

            }

            ParameterPosition.Y = TickLabelPositions[0].Y;
            ParameterPosition.X = TickLabelPositions[0].X;

            UnitPosition.X = (int)Math.Round(ParameterPosition.X + TickLabelWidths[0] / 2d - (double)(this.Grafx.Graphics.MeasureString(this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]] + " " + this.Y_UnitsLabel[this.Y_Number_Allowed], this.Y_AxisFont).Width / 2f));
            UnitPosition.Y = ParameterPosition.Y + this.Y_AxisFont.Height;

            this.Y_DataPen[this.XY_Selected].Width = 3f;

        }
        public override void DrawToBuffer(Graphics g) // WHY SEND A G AS AN ARGUMENT? - REMOVE
        {

            int TickCount;
            if (this.Y_Result[this.XY_Selected] > this.Y_Maximum[this.Y_Number_Allowed])
                this.Y_Result[this.XY_Selected] = this.Y_Maximum[this.Y_Number_Allowed];
            if (this.Y_Result[this.XY_Selected] < this.Y_Minimum[this.Y_Number_Allowed])
                this.Y_Result[this.XY_Selected] = this.Y_Minimum[this.Y_Number_Allowed];
            this.Grafx.Graphics.Clear(this.BackClr);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            {
                var withBlock = this.Grafx.Graphics;
                // ///////
                // / NEXT SECTION NEW APPROACH 6.5 TRYING TO AVOID SELECT CASE
                withBlock.DrawArc(this.AxisPen, myDialRectangle, StartAngle, Angle);
                var loopTo = NumberOfMajorTicks;
                for (TickCount = 1; TickCount <= loopTo; TickCount++)
                {
                    withBlock.DrawLine(this.AxisPen, MajorTickOuter[TickCount], MajorTickInner[TickCount]);
                    withBlock.DrawString(TickLabels[TickCount], this.Y_AxisFont, this.AxisBrush, TickLabelPositions[TickCount]);
                }
                var loopTo1 = NumberOfMinorTicks;
                for (TickCount = 1; TickCount <= loopTo1; TickCount++)
                    withBlock.DrawLine(this.AxisPen, MinorTickOuter[TickCount], MinorTickInner[TickCount]);
                withBlock.DrawString(this.Y_PrimaryLabel[this.XY_Selected], this.Y_AxisFont, this.AxisBrush, ParameterPosition);
                withBlock.DrawString(this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]] + " " + this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont, this.AxisBrush, UnitPosition);


                {
                    ref var withBlock1 = ref myDialRectangle;
                    if (SweepClockwise == 1)
                    {
                        EndOfNeedle.X = (int)Math.Round((double)Center.X + withBlock1.Width / 2d * Math.Cos(this.ConvertedToRadians((double)StartAngle + (this.Y_Result[this.XY_Selected] - this.Y_Minimum[this.Y_Number_Allowed]) / (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) * (double)Angle)));
                        EndOfNeedle.Y = (int)Math.Round((double)Center.Y + withBlock1.Height / 2d * Math.Sin(this.ConvertedToRadians((double)StartAngle + (this.Y_Result[this.XY_Selected] - this.Y_Minimum[this.Y_Number_Allowed]) / (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) * (double)Angle)));
                    }
                    else
                    {
                        EndOfNeedle.X = (int)Math.Round((double)Center.X + withBlock1.Width / 2d * Math.Cos(this.ConvertedToRadians((double)(StartAngle + Angle) - (this.Y_Result[this.XY_Selected] - this.Y_Minimum[this.Y_Number_Allowed]) / (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) * (double)Angle)));
                        EndOfNeedle.Y = (int)Math.Round((double)Center.Y + withBlock1.Height / 2d * Math.Sin(this.ConvertedToRadians((double)(StartAngle + Angle) - (this.Y_Result[this.XY_Selected] - this.Y_Minimum[this.Y_Number_Allowed]) / (this.Y_Maximum[this.Y_Number_Allowed] - this.Y_Minimum[this.Y_Number_Allowed]) * (double)Angle)));
                    }
                }
                withBlock.DrawLine(this.Y_DataPen[this.XY_Selected], Center, EndOfNeedle);

                // / END OF NEW SECTION
                // //////////////

            }

        }
        public override void AddControlSpecificOptionItems()
        {

            ToolStripMenuItem TestStrip;
            string str1;
            string[] str2;
            string[] str3;


            // str1 = "Configuration"
            // str2 = {"90 deg", "180 deg", "270 deg"}
            // str3 = {"Up", "Right", "Down", "Left"} 'Note - Only Up supported now - this will allow forward compatability later

            // TestStrip = CreateAToolStripMenuItem("O", str1, str2, str3)
            // contextmnu.Items.Add(TestStrip)

            str1 = "Configuration";
            str2 = new[] { "Arc width (degrees)", "Direction (degrees)" };
            str3 = new[] { "TXT" };

            TestStrip = this.CreateAToolStripMenuItem("F", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

            str1 = "Sweep Direction";
            str2 = new[] { "Clockwise", "Anticlockwise" };
            str3 = Array.Empty<string>();

            TestStrip = this.CreateAToolStripMenuItem("O", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

            str1 = "Range";
            str2 = new[] { "Minimum", "Maximum" };
            str3 = new[] { "TXT" };

            TestStrip = this.CreateAToolStripMenuItem("M", str1, str2, str3);
            this.Contextmnu.Items.Add(TestStrip);

        }
        public override void ControlSpecificOptionSelection(string Sent)
        {
            switch (Sent ?? "")
            {
                case var @case when @case == "O_0":
                    {
                        SweepClockwise = 1;
                        this.myConfiguration = Angle.ToString() + " " + PointAngle.ToString() + " " + SweepClockwise;
                        break;
                    }
                case var case1 when case1 == "O_1":
                    {
                        SweepClockwise = 0;
                        this.myConfiguration = Angle.ToString() + " " + PointAngle.ToString() + " " + SweepClockwise;
                        break;
                    }

                default:
                    {
                        string[] Temp;
                        Temp = Strings.Split(Sent, " ");
                        if (Temp[0] == "M_0_0")
                            this.Y_Minimum[this.Y_Number_Allowed] = Conversions.ToDouble(Temp[1]);
                        if (Temp[0] == "M_1_0")
                            this.Y_Maximum[this.Y_Number_Allowed] = Conversions.ToDouble(Temp[1]);
                        if (Temp[0] == "F_0_0")
                        {
                            Angle = Conversions.ToSingle(Temp[1]);
                            // PointAngle = 180 'CHECK - not used at the moment
                            StartAngle = PointAngle - Angle / 2f;
                            this.myConfiguration = Angle.ToString() + " " + PointAngle.ToString() + " " + SweepClockwise;
                        }
                        if (Temp[0] == "F_1_0")
                        {
                            // Angle = CSng(Temp(1))
                            PointAngle = Conversions.ToSingle(Temp[1]); // 180 'CHECK - not used at the moment
                            StartAngle = PointAngle - Angle / 2f;
                            this.myConfiguration = Angle.ToString() + " " + PointAngle.ToString() + " " + SweepClockwise;
                        }

                        break;
                    }
            }
        }
        public override string ControlSpecificSerializationData()
        {
            return default;

        }
        public override void ControlSpecficCreateFromSerializedData(string[] Sent)
        {
            // Original version was the following line, but misses the point direction.  Need to check for pre-6.5 configuration string
            // Angle = CSng(Split(myConfiguration, " ")(0))
            string[] TempString;
            TempString = Strings.Split(this.myConfiguration, " ");
            Angle = Conversions.ToSingle(TempString[0]);
            if (Information.UBound(TempString) == 1) // This is an older gauge version
            {
                PointAngle = 270f; // Up 'CHECK IT MAY MAKE MORE SENSE TO STORE THE ARC STARTING ANGLE
                SweepClockwise = 1;
                this.myConfiguration = Angle.ToString() + " " + PointAngle.ToString() + " " + SweepClockwise.ToString();
            }
            else
            {
                PointAngle = Conversions.ToSingle(TempString[1]);
                SweepClockwise = Conversions.ToInteger(TempString[2]);
            }
            StartAngle = PointAngle - Angle / 2f;
        }
    }
}
