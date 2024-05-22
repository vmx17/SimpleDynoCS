using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{
    public partial class Analysis
    {
        // Overlay Specific
        private Graphics OverlayBMP;
        private Bitmap OverlayBitMap;
        private int PicOverlayHeight;
        private int PicOverlayWidth;
        private int OverlayFileCount = 0;

        private StreamReader DataInputFile;
        private string[] OverlayFiles;
        // CHECK - THIS IS A LOCAL VALUE OF TICKLENGTH - MAY NEED TO RESCALE WITH RESIZE
        private int TickLength = 10;
        private bool PlotDrag;
        private double XOverlayStartFraction;
        private double XOverlayEndFraction;
        private double YOverlayStartFraction;
        private double YOverlayEndFraction;

        private int GraphStartCoordX;
        private int GraphEndCoordX;
        private int GraphStartCoordY;
        private int GraphEndCoordY;
        private int GraphWidthInCoords;
        private int GraphHeightInCoords;

        private double xRangeLenFraction;
        private double xRangeStartFraction;
        private const int MAXDATAFILES = 5;
        private double[,,] AnalyzedData = new double[6, 37, 50001];

        public Analysis()
        {
            InitializeComponent();
        }
        internal void Analysis_Setup()
        {
            OverlayFiles = new string[6];

            Size = Screen.PrimaryScreen.WorkingArea.Size;

            {
                var withBlock = pnlDataOverlay;
                withBlock.Width = ClientSize.Width - 165;
                withBlock.Height = ClientSize.Height - 5;
                PicOverlayWidth = withBlock.Width;
                PicOverlayHeight = withBlock.Height;
            }

            OverlayBitMap = new Bitmap(PicOverlayWidth, PicOverlayHeight);
            OverlayBMP = Graphics.FromImage(OverlayBitMap);

            string tempstring = "";
            string[] tempsplit2;
            int paramcount;
            for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++)
                // tempstring = tempstring & Main.DataTags(paramcount).Replace(" ", "_") & " "
                tempstring = tempstring + SimpleDyno.DataTags[paramcount] + "_";
            tempsplit2 = Strings.Split(tempstring, "_");

            Array.Resize(ref tempsplit2, Information.UBound(tempsplit2));
            // For paramcount = 0 To UBound(tempsplit2)
            // tempsplit2(paramcount) = tempsplit2(paramcount).Replace("_", " ")
            // Next
            cmbOverlayDataX.Items.AddRange(tempsplit2);
            tempstring = tempstring + "None";
            tempsplit2 = Strings.Split(tempstring, "_");
            cmbOverlayDataY1.Items.AddRange(tempsplit2);
            cmbOverlayDataY2.Items.AddRange(tempsplit2);
            cmbOverlayDataY3.Items.AddRange(tempsplit2);
            cmbOverlayDataY4.Items.AddRange(tempsplit2);
            tempstring = "";
            tempsplit2 = Strings.Split(SimpleDyno.DataUnitTags[SimpleDyno.SPEED], " ");
            cmbOverlayCorrectedSpeedUnits.Items.AddRange(tempsplit2);
            cmbOverlayDataX.SelectedIndex = 0;
            cmbOverlayDataY1.SelectedIndex = 0;
            cmbOverlayDataY2.SelectedIndex = 0;
            cmbOverlayDataY3.SelectedIndex = 0;
            cmbOverlayDataY4.SelectedIndex = 0;
            cmbOverlayCorrectedSpeedUnits.SelectedIndex = 0;

            pnlOverlaySetup();
        }
        private void Analysis_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                Hide();
                e.Cancel = true;
                Program.MainI.btnShow_Click(this, EventArgs.Empty);
            }
        }

        // Draw to the given bitmap, following the global draw area limits in order to not draw outside it
        public void DrawWithLimits(ref Graphics graph, Pen pen, int x1, int y1, int x2, int y2)
        {
            if (x1 >= GraphStartCoordX & x2 >= GraphStartCoordX & x1 <= GraphEndCoordX & x2 <= GraphEndCoordX & y1 >= GraphStartCoordY & y2 >= GraphStartCoordY & y1 <= GraphEndCoordY & y2 <= GraphEndCoordY)
            {
                graph.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public void pnlOverlaySetup()
        {
            // CHECK - THIS WHOLE SUB COULD DO WITH REWORKING 
            if (SimpleDyno.Formloaded)
            {
                int FileCount;
                int Counter;
                double TickInterval;
                string TempString;

                // Define the "equal spacing" values - these allow the data to be plotted evenly across so that the line types can be seen clearly
                int EqualSpacingCount = 500;
                var EqualSpacingInterval = new double[6];
                var EqualSpacingPointers = new double[6, EqualSpacingCount + 1];

                // Find the correct font width for the columns
                int ColumnWidth = (int)Math.Round(PicOverlayWidth / 8d); // 0.5 for legend, 1.5 for file, and 6 columns of data
                float Increment = 0.1f; // Rate at which we will increase the font size until we have one that fits
                var TempFont = new Font("Arial", Increment); // New temporary font for sizing
                while (OverlayBMP.MeasureString("Corr. Wheel Torque A", TempFont).Width <= ColumnWidth) // test string is a little longer than the longest column header expected
                    TempFont = new Font("Arial", TempFont.Size + Increment); // increase the size of the font
                var ResultsFont = new Font("Arial", TempFont.Size); // Font for results
                var HeadingsFont = new Font("Arial", TempFont.Size, FontStyle.Bold); // Font for column headings
                var DashStyleFont = new Font("Arial", TempFont.Size, FontStyle.Italic); // Font for legend
                int LineTypeColumn = 5; // sets small margin before legend text
                int FileColumn = LineTypeColumn + (int)Math.Round(ColumnWidth / 2d); // 1/2 column width for legend
                int XColumn = FileColumn + (int)Math.Round(ColumnWidth * 1.75d); // 1 1/2 column widths for filename 
                int Y1Column = XColumn + ColumnWidth;
                int Y2Column = Y1Column + ColumnWidth;
                int Y3Column = Y2Column + ColumnWidth;
                int Y4Column = Y3Column + ColumnWidth;
                int YDragColumn = Y4Column + ColumnWidth;

                // Using the defined fonts find the correct positions for each line
                int Titleline = 5; // sets small top margin
                int UnitsLine = Titleline + TempFont.Height;
                var ResultsLine = new int[6];
                ResultsLine[1] = UnitsLine + TempFont.Height;
                ResultsLine[2] = ResultsLine[1] + TempFont.Height;
                ResultsLine[3] = ResultsLine[2] + TempFont.Height;
                ResultsLine[4] = ResultsLine[3] + TempFont.Height;
                ResultsLine[5] = ResultsLine[4] + TempFont.Height;


                // CHECK - Ideally we should calculate the window size based on these new fonts and line and column definitions
                // However, seems to work fine for 900x600 and 1366x768 as of 01JUL13

                var xSet = new double[50001];
                var y1Set = new double[50001];
                var y2Set = new double[50001];
                var y3Set = new double[50001];
                var y4Set = new double[50001];
                var xMax = new double[6];
                var y1Max = new double[6];
                var y2Max = new double[6];
                var y3Max = new double[6];
                var y4Max = new double[6];
                var DragMax = new double[6];
                var y1MaxAtX = new double[6];
                var y2MaxAtX = new double[6];
                var y3MaxAtX = new double[6];
                var y4MaxAtX = new double[6];
                var DragMaxAtX = new double[6];
                var y1MaxAtSelectedX = new double[6];
                var y2MaxAtSelectedX = new double[6];
                var y3MaxAtSelectedX = new double[6];
                var y4MaxAtSelectedX = new double[6];
                var DragMaxAtSelectedX = new double[6];
                var XMaxDifferencePointer = new long[6];
                xRangeStartFraction = 0d;
                xRangeLenFraction = 0d;
                var y1Axis = default(double);
                var y2Axis = default(double);
                var y3Axis = default(double);
                var y4Axis = default(double);

                double DragCompare;

                var AxisFont = new Font("Arial", 8f);
                var AxisPen = new Pen(Color.Black, 1f);
                var AxisBrush = new SolidBrush(AxisPen.Color);

                var XFont = new Font("Arial", 8f);
                var XPen = new Pen(Color.Black, 2f);
                var XBrush = new SolidBrush(XPen.Color);

                var Y1Font = new Font("Arial", 9f);
                var Y1Pen = new Pen(Color.Blue, 2f);
                var Y1Brush = new SolidBrush(Y1Pen.Color);

                var Y2Font = new Font("Arial", 9f);
                var Y2Pen = new Pen(Color.Red, 2f);
                var Y2Brush = new SolidBrush(Y2Pen.Color);

                var Y3Font = new Font("Arial", 9f);
                var Y3Pen = new Pen(Color.Green, 2f);
                var Y3Brush = new SolidBrush(Y3Pen.Color);

                var Y4Font = new Font("Arial", 9f);
                var Y4Pen = new Pen(Color.Orange, 2f);
                var Y4Brush = new SolidBrush(Y4Pen.Color);

                var DragFont = new Font("Arial", 8f);
                var DragPen = new Pen(Color.Purple, 2f);
                var DragBrush = new SolidBrush(DragPen.Color);

                var OverlayDashes = new DashStyle[6];

                OverlayDashes[1] = DashStyle.Solid;
                OverlayDashes[2] = DashStyle.Dash;
                OverlayDashes[3] = DashStyle.Dot;
                OverlayDashes[4] = DashStyle.DashDot;
                OverlayDashes[5] = DashStyle.DashDotDot;

                YOverlayStartFraction = 0.1d + OverlayFileCount * 0.0275d;
                YOverlayEndFraction = 0.925d;
                XOverlayStartFraction = 0.125d;
                XOverlayEndFraction = 0.875d;

                GraphStartCoordX = (int)Math.Round(PicOverlayWidth * XOverlayStartFraction);
                GraphEndCoordX = (int)Math.Round(PicOverlayWidth * XOverlayEndFraction);
                GraphStartCoordY = (int)Math.Round(PicOverlayHeight * YOverlayStartFraction);
                GraphEndCoordY = (int)Math.Round(PicOverlayHeight * YOverlayEndFraction);
                GraphWidthInCoords = (int)Math.Round((XOverlayEndFraction - XOverlayStartFraction) * PicOverlayWidth);
                GraphHeightInCoords = (int)Math.Round((YOverlayEndFraction - YOverlayStartFraction) * PicOverlayHeight);

                double XMaxDifference;

                var loopTo = OverlayFileCount;
                for (FileCount = 1; FileCount <= loopTo; FileCount++)
                {
                    XMaxDifference = 100000d;
                    var loopTo1 = (int)Math.Round(AnalyzedData[FileCount, SimpleDyno.SESSIONTIME, 0]);
                    for (Counter = 1; Counter <= loopTo1; Counter++) // OvPoints(FileCount)
                    {
                        if (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter] > xMax[FileCount])
                        {
                            xMax[FileCount] = AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter];
                            if (xMax[FileCount] > xRangeLenFraction)
                            {
                                xRangeLenFraction = xMax[FileCount];
                            }
                        }
                        // Check to see if we have passed the selected X value - this needs more work?
                        if (Math.Abs(AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter] - OverlayXSelected) < XMaxDifference)
                        {
                            XMaxDifference = Math.Abs(AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter] - OverlayXSelected);
                            XMaxDifferencePointer[FileCount] = Counter;
                        }
                        if (AnalyzedData[FileCount, cmbOverlayDataY1.SelectedIndex, Counter] > y1Max[FileCount])
                        {
                            y1Max[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY1.SelectedIndex, Counter];
                            y1MaxAtX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter];
                            if (y1Max[FileCount] > y1Axis)
                            {
                                y1Axis = y1Max[FileCount];
                            }
                        }
                        if (AnalyzedData[FileCount, cmbOverlayDataY2.SelectedIndex, Counter] > y2Max[FileCount])
                        {
                            y2Max[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY2.SelectedIndex, Counter];
                            y2MaxAtX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter];
                            if (y2Max[FileCount] > y2Axis)
                            {
                                y2Axis = y2Max[FileCount];
                            }
                        }
                        if (AnalyzedData[FileCount, cmbOverlayDataY3.SelectedIndex, Counter] > y3Max[FileCount])
                        {
                            y3Max[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY3.SelectedIndex, Counter];
                            y3MaxAtX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter];
                            if (y3Max[FileCount] > y3Axis)
                            {
                                y3Axis = y3Max[FileCount];
                            }
                        }
                        if (AnalyzedData[FileCount, cmbOverlayDataY4.SelectedIndex, Counter] > y4Max[FileCount])
                        {
                            y4Max[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY4.SelectedIndex, Counter];
                            y4MaxAtX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter];
                            if (y4Max[FileCount] > y4Axis)
                            {
                                y4Axis = y4Max[FileCount];
                            }
                        }
                    }
                    y1MaxAtSelectedX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY1.SelectedIndex, (int)XMaxDifferencePointer[FileCount]];
                    y2MaxAtSelectedX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY2.SelectedIndex, (int)XMaxDifferencePointer[FileCount]];
                    y3MaxAtSelectedX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY3.SelectedIndex, (int)XMaxDifferencePointer[FileCount]];
                    y4MaxAtSelectedX[FileCount] = AnalyzedData[FileCount, cmbOverlayDataY4.SelectedIndex, (int)XMaxDifferencePointer[FileCount]];
                }

                int Interval;
                // calculate the equal spacing pointer values
                // CHECK IF THIS SHOULD BE BEFORE OR AFTER THE CUSTOMROUND
                // CHECK  - this should have the last point and the first point
                var loopTo2 = OverlayFileCount;
                for (FileCount = 1; FileCount <= loopTo2; FileCount++)
                {
                    EqualSpacingInterval[FileCount] = xMax[FileCount] / EqualSpacingCount;
                    Interval = 0;
                    var loopTo3 = (int)Math.Round(AnalyzedData[FileCount, SimpleDyno.SESSIONTIME, 0]);
                    for (Counter = 1; Counter <= loopTo3; Counter++)
                    {
                        if (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, Counter] > Interval * EqualSpacingInterval[FileCount])
                        {
                            Interval += 1;
                            if (Interval <= EqualSpacingCount)
                            {
                                EqualSpacingPointers[FileCount, Interval] = Counter;
                            }
                        }
                    }
                }

                // Set Axes to 1 if there are no data (i.e. the axis would be set to zero)
                if (xRangeLenFraction == 0d)
                    xRangeLenFraction = 1d;
                if (y1Axis == 0d)
                    y1Axis = 1d;
                if (y2Axis == 0d)
                    y2Axis = 1d;
                if (y3Axis == 0d)
                    y3Axis = 1d;
                if (y4Axis == 0d)
                    y4Axis = 1d;

                // Apply X-axis range settings if valid ones are given
                double xRangeStartInActualUnit = 0d;
                try
                {
                    xRangeStartInActualUnit = Convert.ToInt32(TextBox_XStart.Text);
                    xRangeStartFraction = xRangeStartInActualUnit / SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex];
                }
                catch (Exception e)
                {
                }
                // Apply this for the situation where X range end is not manually given
                xRangeLenFraction -= xRangeStartFraction;
                double xRangeLenInActualUnit = xRangeLenFraction * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex];
                try
                {
                    xRangeLenInActualUnit = Convert.ToInt32(TextBox_XEnd.Text);
                    xRangeLenInActualUnit -= xRangeStartInActualUnit;
                    xRangeLenFraction = xRangeLenInActualUnit / SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex];
                }
                catch (Exception e)
                {
                }

                // CHECK - THESE Might be better off using new custom round or format
                xRangeLenFraction = Program.MainI.CustomRound(xRangeLenFraction);
                y1Axis = Program.MainI.CustomRound(y1Axis);
                y2Axis = Program.MainI.CustomRound(y2Axis);
                y3Axis = Program.MainI.CustomRound(y3Axis);
                y4Axis = Program.MainI.CustomRound(y4Axis);

                // Okay - attempt to deal with the "Drag" questions
                // If drag is a selected Y value then if power is also a selected Y value, the Yaxes should be the same and the units should be the same
                // This is not going to work without arrays, so simply calculate the corrected speed and stick it in the corner if drag has been selected
                // is it worth setting up an array for this?

                {
                    ref var withBlock = ref OverlayBMP;
                    withBlock.Clear(Color.White);
                    // Draw Three Axes (Left, Right and Bottom)
                    withBlock.DrawLine(AxisPen, GraphStartCoordX, GraphEndCoordY, GraphEndCoordX, GraphEndCoordY);
                    withBlock.DrawLine(AxisPen, GraphStartCoordX, GraphStartCoordY, GraphStartCoordX, GraphEndCoordY);
                    withBlock.DrawLine(AxisPen, GraphEndCoordX, GraphStartCoordY, GraphEndCoordX, GraphEndCoordY);
                    // X-Axis Details
                    // Calculate the space between ticks
                    TickInterval = PicOverlayWidth * (XOverlayEndFraction - XOverlayStartFraction) * 1d / 10d;
                    for (Counter = 0; Counter <= 9; Counter++)
                    {
                        // generate the tick label
                        TempString = Program.MainI.NewCustomFormat(xRangeStartInActualUnit + xRangeLenInActualUnit / 10d * (Counter + 1));
                        // draw the tick
                        withBlock.DrawLine(AxisPen, (int)Math.Round(GraphStartCoordX + TickInterval * (Counter + 1)), GraphEndCoordY, (int)Math.Round(GraphStartCoordX + TickInterval * (Counter + 1)), GraphEndCoordY + TickLength);
                        // draw the label
                        withBlock.DrawString(TempString, AxisFont, AxisBrush, (int)Math.Round(GraphStartCoordX + TickInterval * (Counter + 1) - (double)(withBlock.MeasureString(TempString, AxisFont).Width / 2f)), GraphEndCoordY + TickLength);
                    }
                    // generate the axis title string...
                    TempString = SimpleDyno.DataTags[cmbOverlayDataX.SelectedIndex] + " (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex] + ")";
                    // ...and draw.
                    withBlock.DrawString(TempString, HeadingsFont, AxisBrush, (int)Math.Round(PicOverlayWidth / 2d - (double)(withBlock.MeasureString(TempString, HeadingsFont).Width / 2f)), GraphEndCoordY + TickLength + withBlock.MeasureString("9", AxisFont).Height);
                    // prefix the string with "Max" for the results at the top of the page...
                    // Version 6.4 moving the Max prefix to the units line to avoid overcrowding
                    // If OverlayPlotMax Then
                    // TempString = "Max " & Main.DataTags(cmbOverlayDataX.SelectedIndex)
                    // Else
                    TempString = SimpleDyno.DataTags[cmbOverlayDataX.SelectedIndex];
                    // CHECK FOR REMOVAL Debug.Print(OverlayXSelected & " " & xRangeLenFraction)
                    withBlock.DrawLine(Pens.Gray, (int)Math.Round(GraphStartCoordX + (OverlayXSelected - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), GraphStartCoordY, (int)Math.Round(GraphStartCoordX + (OverlayXSelected - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), GraphEndCoordY);
                    // End If
                    // and draw the column heading
                    withBlock.DrawString(TempString, HeadingsFont, AxisBrush, XColumn - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                    // generate the units string
                    if (OverlayPlotMax)
                    {
                        TempString = "Max (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex] + ")";
                    }
                    else
                    {
                        TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex] + ")";
                    }
                    // TempString = "(" & Split(Main.DataUnitTags(cmbOverlayDataX.SelectedIndex), " ")(cmbOverlayUnitsX.SelectedIndex) & ")"
                    // and draw the units heading
                    withBlock.DrawString(TempString, HeadingsFont, AxisBrush, XColumn - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                    // Write the filenames and the X max results
                    var loopTo4 = OverlayFileCount;
                    for (FileCount = 1; FileCount <= loopTo4; FileCount++)
                    {
                        withBlock.DrawString(OverlayDashes[FileCount].ToString(), DashStyleFont, AxisBrush, 0f, ResultsLine[FileCount]);
                        // .DrawString(OverlayFiles(FileCount), ResultsFont, AxisBrush, FileColumn - .MeasureString(OverlayFiles(FileCount), ResultsFont).Width, ResultsLine(FileCount))
                        // This should draw left justified string for the file name title
                        withBlock.DrawString(OverlayFiles[FileCount], ResultsFont, AxisBrush, FileColumn, ResultsLine[FileCount]);
                        if (OverlayPlotMax)
                        {
                            TempString = Program.MainI.NewCustomFormat(xMax[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]);
                            withBlock.DrawString(TempString, ResultsFont, AxisBrush, XColumn - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                        }
                        else
                        {
                            TempString = Program.MainI.NewCustomFormat(OverlayXSelected * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]);
                            withBlock.DrawString(TempString, ResultsFont, AxisBrush, XColumn - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                        }
                    }

                    // Same pattern used for X ticks for each of the Y axes / Ticks / Results provided the specific Y column has been selected
                    if (cmbOverlayDataY1.SelectedIndex != SimpleDyno.LAST)
                    {
                        TickInterval = PicOverlayHeight * (YOverlayEndFraction - YOverlayStartFraction) * 1d / 5d;
                        for (Counter = 0; Counter <= 4; Counter++)
                        {
                            TempString = Program.MainI.NewCustomFormat(y1Axis * SimpleDyno.DataUnits[cmbOverlayDataY1.SelectedIndex, cmbOverlayUnitsY1.SelectedIndex] / 5d * (double)(5 - Counter));
                            withBlock.DrawLine(AxisPen, GraphStartCoordX - TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter), GraphStartCoordX, (int)Math.Round(GraphStartCoordY + TickInterval * Counter));
                            withBlock.DrawString(TempString, AxisFont, AxisBrush, (int)Math.Round(GraphStartCoordX - TickLength - withBlock.MeasureString(TempString, AxisFont).Width), (int)Math.Round(GraphStartCoordY + TickInterval * Counter - (double)(withBlock.MeasureString(TempString, AxisFont).Height / 2f)));
                        }
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY1.SelectedIndex] + Constants.vbCrLf + "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY1.SelectedIndex], " ")[cmbOverlayUnitsY1.SelectedIndex] + ")";
                        withBlock.DrawString(TempString, Y1Font, Y1Brush, (int)Math.Round(GraphStartCoordX - withBlock.MeasureString(TempString, Y1Font).Width), (int)Math.Round(GraphStartCoordY - 5 - withBlock.MeasureString(TempString, Y1Font).Height)); // * 1.5))
                                                                                                                                                                                                                                                              // If OverlayPlotMax Then
                                                                                                                                                                                                                                                              // TempString = "Max " & Main.DataTags(cmbOverlayDataY1.SelectedIndex)
                                                                                                                                                                                                                                                              // .DrawString(TempString, HeadingsFont, AxisBrush, Y1Column - .MeasureString(TempString, HeadingsFont).Width / 2, Titleline)
                                                                                                                                                                                                                                                              // Else
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY1.SelectedIndex];
                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y1Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                        // End If
                        if (OverlayPlotMax)
                        {
                            TempString = "Max (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY1.SelectedIndex], " ")[cmbOverlayUnitsY1.SelectedIndex] + ")";
                        }
                        else
                        {
                            TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY1.SelectedIndex], " ")[cmbOverlayUnitsY1.SelectedIndex] + ")";
                        }
                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y1Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                        var loopTo5 = OverlayFileCount;
                        for (FileCount = 1; FileCount <= loopTo5; FileCount++)
                        {
                            if (OverlayPlotMax)
                            {
                                TempString = Program.MainI.NewCustomFormat(y1Max[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY1.SelectedIndex, cmbOverlayUnitsY1.SelectedIndex]) + " @ " + Program.MainI.NewCustomFormat(y1MaxAtX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]) + " " + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex];
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y1Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }
                            else
                            {
                                TempString = Program.MainI.NewCustomFormat(y1MaxAtSelectedX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY1.SelectedIndex, cmbOverlayUnitsY1.SelectedIndex]); // & " @ " & Main.NewCustomFormat(OverlayXSelected * Main.DataUnits(cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataX.SelectedIndex), " ")(cmbOverlayUnitsX.SelectedIndex)
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y1Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }

                            Y1Pen.DashStyle = OverlayDashes[FileCount];
                            var loopTo6 = EqualSpacingCount - 1;
                            for (Counter = 2; Counter <= loopTo6; Counter++)
                                DrawWithLimits(ref OverlayBMP, Y1Pen, (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY1.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] / y1Axis * GraphHeightInCoords), (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY1.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] / y1Axis * GraphHeightInCoords));
                        }
                    }

                    if (cmbOverlayDataY2.SelectedIndex != SimpleDyno.LAST)
                    {
                        TickInterval = PicOverlayHeight * (YOverlayEndFraction - YOverlayStartFraction) * 1d / 5d;
                        for (Counter = 0; Counter <= 4; Counter++)
                        {
                            TempString = Program.MainI.NewCustomFormat(y2Axis * SimpleDyno.DataUnits[cmbOverlayDataY2.SelectedIndex, cmbOverlayUnitsY2.SelectedIndex] / 5d * (double)(5 - Counter));
                            withBlock.DrawLine(AxisPen, GraphStartCoordX, (int)Math.Round(GraphStartCoordY + TickInterval * Counter), GraphStartCoordX + TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter));
                            withBlock.DrawString(TempString, AxisFont, AxisBrush, GraphStartCoordX + TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter - (double)(withBlock.MeasureString(TempString, AxisFont).Height / 2f)));
                        }
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY2.SelectedIndex] + Constants.vbCrLf + "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY2.SelectedIndex], " ")[cmbOverlayUnitsY2.SelectedIndex] + ")";
                        withBlock.DrawString(TempString, Y2Font, Y2Brush, GraphStartCoordX, (int)Math.Round(GraphStartCoordY - 5 - withBlock.MeasureString(TempString, Y2Font).Height)); // * 1.5))
                                                                                                                                                                                         // If OverlayPlotMax Then
                                                                                                                                                                                         // TempString = "Max " & Main.DataTags(cmbOverlayDataY2.SelectedIndex)
                                                                                                                                                                                         // .DrawString(TempString, HeadingsFont, AxisBrush, Y2Column - .MeasureString(TempString, HeadingsFont).Width / 2, Titleline)
                                                                                                                                                                                         // Else
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY2.SelectedIndex];
                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y2Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                        // End If
                        if (OverlayPlotMax)
                        {
                            TempString = "Max (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY2.SelectedIndex], " ")[cmbOverlayUnitsY2.SelectedIndex] + ")";
                        }
                        else
                        {
                            TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY2.SelectedIndex], " ")[cmbOverlayUnitsY2.SelectedIndex] + ")";
                        }

                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y2Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                        var loopTo7 = OverlayFileCount;
                        for (FileCount = 1; FileCount <= loopTo7; FileCount++)
                        {
                            if (OverlayPlotMax)
                            {
                                TempString = Program.MainI.NewCustomFormat(y2Max[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY2.SelectedIndex, cmbOverlayUnitsY2.SelectedIndex]) + " @ " + Program.MainI.NewCustomFormat(y2MaxAtX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]) + " " + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex];
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y2Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }
                            else
                            {
                                TempString = Program.MainI.NewCustomFormat(y2MaxAtSelectedX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY2.SelectedIndex, cmbOverlayUnitsY2.SelectedIndex]); // & " @ " & Main.NewCustomFormat(OverlayXSelected * Main.DataUnits(cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataX.SelectedIndex), " ")(cmbOverlayUnitsX.SelectedIndex)
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y2Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }
                            Y2Pen.DashStyle = OverlayDashes[FileCount];
                            var loopTo8 = EqualSpacingCount - 1;
                            for (Counter = 2; Counter <= loopTo8; Counter++)
                                DrawWithLimits(ref OverlayBMP, Y2Pen, (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY2.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] / y2Axis * GraphHeightInCoords), (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY2.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] / y2Axis * GraphHeightInCoords));
                        }
                    }

                    if (cmbOverlayDataY3.SelectedIndex != SimpleDyno.LAST)
                    {
                        TickInterval = PicOverlayHeight * (YOverlayEndFraction - YOverlayStartFraction) * 1d / 5d;
                        for (Counter = 0; Counter <= 4; Counter++)
                        {
                            TempString = Program.MainI.NewCustomFormat(y3Axis * SimpleDyno.DataUnits[cmbOverlayDataY3.SelectedIndex, cmbOverlayUnitsY3.SelectedIndex] / 5d * (double)(5 - Counter));
                            withBlock.DrawLine(AxisPen, GraphEndCoordX - TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter), GraphEndCoordX, (int)Math.Round(GraphStartCoordY + TickInterval * Counter));
                            withBlock.DrawString(TempString, AxisFont, AxisBrush, (int)Math.Round(GraphEndCoordX - TickLength - withBlock.MeasureString(TempString, AxisFont).Width), (int)Math.Round(GraphStartCoordY + TickInterval * Counter - (double)(withBlock.MeasureString(TempString, AxisFont).Height / 2f)));
                        }
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY3.SelectedIndex] + Constants.vbCrLf + "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY3.SelectedIndex], " ")[cmbOverlayUnitsY3.SelectedIndex] + ")";
                        withBlock.DrawString(TempString, Y3Font, Y3Brush, (int)Math.Round(GraphEndCoordX - withBlock.MeasureString(TempString, Y3Font).Width), (int)Math.Round(GraphStartCoordY - 5 - withBlock.MeasureString(TempString, Y3Font).Height)); // * 1.5))
                                                                                                                                                                                                                                                            // If OverlayPlotMax Then
                                                                                                                                                                                                                                                            // TempString = "Max " & Main.DataTags(cmbOverlayDataY3.SelectedIndex)
                                                                                                                                                                                                                                                            // .DrawString(TempString, HeadingsFont, AxisBrush, Y3Column - .MeasureString(TempString, HeadingsFont).Width / 2, Titleline)
                                                                                                                                                                                                                                                            // Else
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY3.SelectedIndex];
                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y3Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                        // End If
                        if (OverlayPlotMax)
                        {
                            TempString = "Max (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY3.SelectedIndex], " ")[cmbOverlayUnitsY3.SelectedIndex] + ")";
                        }
                        else
                        {
                            TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY3.SelectedIndex], " ")[cmbOverlayUnitsY3.SelectedIndex] + ")";
                        }

                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y3Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                        var loopTo9 = OverlayFileCount;
                        for (FileCount = 1; FileCount <= loopTo9; FileCount++)
                        {
                            if (OverlayPlotMax)
                            {
                                TempString = Program.MainI.NewCustomFormat(y3Max[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY3.SelectedIndex, cmbOverlayUnitsY3.SelectedIndex]) + " @ " + Program.MainI.NewCustomFormat(y3MaxAtX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]) + " " + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex];
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y3Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }
                            else
                            {
                                TempString = Program.MainI.NewCustomFormat(y3MaxAtSelectedX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY3.SelectedIndex, cmbOverlayUnitsY3.SelectedIndex]); // & " @ " & Main.NewCustomFormat(OverlayXSelected * Main.DataUnits(cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataX.SelectedIndex), " ")(cmbOverlayUnitsX.SelectedIndex)
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y3Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }

                            Y3Pen.DashStyle = OverlayDashes[FileCount];
                            var loopTo10 = EqualSpacingCount - 1;
                            for (Counter = 2; Counter <= loopTo10; Counter++)
                                DrawWithLimits(ref OverlayBMP, Y3Pen, (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY3.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] / y3Axis * GraphHeightInCoords), (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY3.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] / y3Axis * GraphHeightInCoords));
                        }
                    }

                    if (cmbOverlayDataY4.SelectedIndex != SimpleDyno.LAST)
                    {
                        TickInterval = PicOverlayHeight * (YOverlayEndFraction - YOverlayStartFraction) * 1d / 5d;
                        for (Counter = 0; Counter <= 4; Counter++)
                        {
                            TempString = Program.MainI.NewCustomFormat(y4Axis * SimpleDyno.DataUnits[cmbOverlayDataY4.SelectedIndex, cmbOverlayUnitsY4.SelectedIndex] / 5d * (double)(5 - Counter));
                            withBlock.DrawLine(AxisPen, GraphEndCoordX, (int)Math.Round(GraphStartCoordY + TickInterval * Counter), GraphEndCoordX + TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter));
                            withBlock.DrawString(TempString, AxisFont, AxisBrush, GraphEndCoordX + TickLength, (int)Math.Round(GraphStartCoordY + TickInterval * Counter - (double)(withBlock.MeasureString(TempString, AxisFont).Height / 2f)));
                        }
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY4.SelectedIndex] + Constants.vbCrLf + "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY4.SelectedIndex], " ")[cmbOverlayUnitsY4.SelectedIndex] + ")";
                        withBlock.DrawString(TempString, Y4Font, Y4Brush, GraphEndCoordX, (int)Math.Round(GraphStartCoordY - 5 - withBlock.MeasureString(TempString, Y4Font).Height)); // * 1.5))
                                                                                                                                                                                       // If OverlayPlotMax Then
                                                                                                                                                                                       // TempString = "Max " & Main.DataTags(cmbOverlayDataY4.SelectedIndex)
                                                                                                                                                                                       // .DrawString(TempString, HeadingsFont, AxisBrush, Y4Column - .MeasureString(TempString, HeadingsFont).Width / 2, Titleline)
                                                                                                                                                                                       // Else
                        TempString = SimpleDyno.DataTags[cmbOverlayDataY4.SelectedIndex];
                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y4Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                        // End If
                        if (OverlayPlotMax)
                        {
                            TempString = "Max (" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY4.SelectedIndex], " ")[cmbOverlayUnitsY4.SelectedIndex] + ")";
                        }
                        else
                        {
                            TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY4.SelectedIndex], " ")[cmbOverlayUnitsY4.SelectedIndex] + ")";
                        }

                        withBlock.DrawString(TempString, HeadingsFont, AxisBrush, Y4Column - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                        var loopTo11 = OverlayFileCount;
                        for (FileCount = 1; FileCount <= loopTo11; FileCount++)
                        {
                            if (OverlayPlotMax)
                            {
                                TempString = Program.MainI.NewCustomFormat(y4Max[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY4.SelectedIndex, cmbOverlayUnitsY4.SelectedIndex]) + " @ " + Program.MainI.NewCustomFormat(y4MaxAtX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]) + " " + Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex], " ")[cmbOverlayUnitsX.SelectedIndex];
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y4Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }
                            else
                            {
                                TempString = Program.MainI.NewCustomFormat(y4MaxAtSelectedX[FileCount] * SimpleDyno.DataUnits[cmbOverlayDataY4.SelectedIndex, cmbOverlayUnitsY4.SelectedIndex]); // & " @ " & Main.NewCustomFormat(OverlayXSelected * Main.DataUnits(cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataX.SelectedIndex), " ")(cmbOverlayUnitsX.SelectedIndex)
                                withBlock.DrawString(TempString, ResultsFont, AxisBrush, Y4Column - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                            }

                            Y4Pen.DashStyle = OverlayDashes[FileCount];
                            var loopTo12 = EqualSpacingCount - 1;
                            for (Counter = 2; Counter <= loopTo12; Counter++)
                                DrawWithLimits(ref OverlayBMP, Y4Pen, (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY4.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter])] / y4Axis * GraphHeightInCoords), (int)Math.Round(GraphStartCoordX + (AnalyzedData[FileCount, cmbOverlayDataX.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] - xRangeStartFraction) / xRangeLenFraction * GraphWidthInCoords), (int)Math.Round(GraphEndCoordY - AnalyzedData[FileCount, cmbOverlayDataY4.SelectedIndex, (int)Math.Round(EqualSpacingPointers[FileCount, Counter + 1])] / y4Axis * GraphHeightInCoords));
                        }
                    }

                    // Hack Job for corrected speed
                    TempString = "Max Corr. Speed"; // & DataTags(DRAG)
                    withBlock.DrawString(TempString, HeadingsFont, AxisBrush, YDragColumn - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, Titleline);
                    TempString = "(" + Strings.Split(SimpleDyno.DataUnitTags[SimpleDyno.SPEED], " ")[cmbOverlayCorrectedSpeedUnits.SelectedIndex] + ")";
                    withBlock.DrawString(TempString, HeadingsFont, AxisBrush, YDragColumn - withBlock.MeasureString(TempString, HeadingsFont).Width / 2f, UnitsLine);
                    var loopTo13 = OverlayFileCount;
                    for (FileCount = 1; FileCount <= loopTo13; FileCount++)
                    {
                        DragCompare = double.MaxValue;
                        Counter = 2;
                        do
                            Counter += 1;
                        while (!(AnalyzedData[FileCount, SimpleDyno.POWER, Counter] - AnalyzedData[FileCount, SimpleDyno.DRAG, Counter] < 0d | (double)Counter == AnalyzedData[FileCount, SimpleDyno.SESSIONTIME, 0]));
                        TempString = Program.MainI.NewCustomFormat(AnalyzedData[FileCount, SimpleDyno.SPEED, Counter] * SimpleDyno.DataUnits[SimpleDyno.SPEED, cmbOverlayCorrectedSpeedUnits.SelectedIndex]);  // & " @ " & NewCustomFormat(y4MaxAtX(FileCount) * DataUnits(cmbOverlayX.SelectedIndex, cmbOverlayXUnits.SelectedIndex)) & " " & Split(DataUnitTags(cmbOverlayX.SelectedIndex), " ")(cmbOverlayXUnits.SelectedIndex)
                        withBlock.DrawString(TempString, ResultsFont, AxisBrush, YDragColumn - withBlock.MeasureString(TempString, ResultsFont).Width / 2f, ResultsLine[FileCount]);
                    }

                }

                pnlDataOverlay.BackgroundImage = OverlayBitMap;
                pnlDataOverlay.Invalidate();

            }
        }
        internal void btnAddOverlayFile_Click_1(object sender, EventArgs e)
        {

            string temp;
            string[] line;
            int PointCount;
            string CopyFileName;
            StreamWriter DataCopyfile;

            if (!e.Equals(EventArgs.Empty))
            {
                {
                    var withBlock = OpenFileDialog1;
                    withBlock.Reset();
                    withBlock.Filter = "Power Run files v6.3+ (*.sdp)|*.sdp|Power Run Files v5.5+ (*.txt)|*.txt";
                    withBlock.ShowDialog();
                }
            }
            if (!string.IsNullOrEmpty(OpenFileDialog1.FileName))
            {
                DataInputFile = new StreamReader(OpenFileDialog1.FileName);
                {
                    ref var withBlock1 = ref DataInputFile;
                    temp = withBlock1.ReadLine();
                    switch (temp ?? "")
                    {
                        case var @case when @case == SimpleDyno.PowerRunVersion:
                        case "POWER_RUN_6_3":
                        case "POWER_RUN_6_4": // This is a valid current version file
                            {
                                OverlayFileCount += 1;
                                if (OverlayFileCount == MAXDATAFILES)
                                {
                                    btnAddOverlayFile.Enabled = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Checked = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Enabled = false;
                                }
                                OverlayFiles[OverlayFileCount] = OpenFileDialog1.FileName.Substring(OpenFileDialog1.FileName.LastIndexOf(@"\") + 1);
                                do
                                    temp = withBlock1.ReadLine();
                                while (!temp.StartsWith("NUMBER_OF_POINTS_FIT"));
                                AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0] = Conversions.ToDouble(temp.Substring(temp.LastIndexOf(" "))); // used the empty holder to remember the number of fit points
                                temp = withBlock1.ReadLine(); // starting

                                string ColumnTitles;
                                string[] TitlesSplit;
                                string SearchString;
                                string[] DataLine;
                                string[] UnitName;
                                int ParamCount;
                                int ParamPosition;

                                ColumnTitles = withBlock1.ReadLine(); // titles
                                TitlesSplit = Strings.Split(ColumnTitles, " ");

                                var loopTo = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                for (PointCount = 1; PointCount <= loopTo; PointCount++)
                                {
                                    DataLine = Strings.Split(withBlock1.ReadLine(), " "); // reads all the values on this line into a string array
                                    for (ParamCount = 0; ParamCount <= SimpleDyno.LAST - 1; ParamCount++)
                                    {
                                        // This is how the titles are created in the fitting code except we do not add the space
                                        UnitName = Strings.Split(SimpleDyno.DataUnitTags[ParamCount], " ");
                                        SearchString = SimpleDyno.DataTags[ParamCount].Replace(" ", "_") + "_(" + UnitName[0] + ")";
                                        ParamPosition = Array.IndexOf(TitlesSplit, SearchString);
                                        if (ParamPosition != -1)
                                        {
                                            AnalyzedData[OverlayFileCount, ParamCount, PointCount] = Conversions.ToDouble(DataLine[ParamPosition]);
                                        }
                                    }
                                }

                                break;
                            }
                        case var case1 when case1 == "POWER_CURVE_6_0": // These were the earlier beta testing versions for uno
                            {
                                // There are a number of different versions carrying this heading
                                // Main differences between these and 6.3+ versions are no "_" between parameter and unit and Watts in was called Watts_(e)
                                Interaction.MsgBox("A copy of " + OpenFileDialog1.FileName + " will be saved as a new version .sdp file.", Constants.vbOKOnly);
                                // Convert old data to new data
                                // open the copy file
                                Program.MainI.SetMouseBusy_ThreadSafe(this);
                                CopyFileName = OpenFileDialog1.FileName.Substring(0, OpenFileDialog1.FileName.Length - 4) + ".sdp";
                                DataCopyfile = new StreamWriter(CopyFileName);
                                DataCopyfile.WriteLine(SimpleDyno.PowerRunVersion);
                                DataCopyfile.WriteLine(CopyFileName);
                                // load it up as if it were a version 6 file
                                OverlayFileCount += 1;
                                if (OverlayFileCount == MAXDATAFILES)
                                {
                                    btnAddOverlayFile.Enabled = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Checked = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Enabled = false;
                                }
                                OverlayFiles[OverlayFileCount] = CopyFileName.Substring(CopyFileName.LastIndexOf(@"\") + 1);
                                // Now read through the lines and copy them to the new file
                                double temprollerdiameter;
                                double tempwheeldiameter;
                                double tempgearratio;
                                temp = withBlock1.ReadLine(); // original file name line
                                do
                                {
                                    temp = withBlock1.ReadLine();
                                    DataCopyfile.WriteLine(temp);
                                    // while we are at it - look for roller dia, wheel dia and gear ratio
                                    if (temp.LastIndexOf(" ") != -1)
                                    {
                                        // Debug.Print(temp & temp.LastIndexOf(" "))
                                        if (temp.Split(' ')[1] == "Gear_Ratio:")
                                            tempgearratio = Conversions.ToDouble(temp.Split(' ')[2]);
                                        if (temp.Split(' ')[1] == "Wheel_Diameter:")
                                            tempwheeldiameter = Conversions.ToDouble(temp.Split(' ')[2]);
                                        if (temp.Split(' ')[1] == "Roller_Diameter:")
                                            temprollerdiameter = Conversions.ToDouble(temp.Split(' ')[2]);
                                    }
                                }
                                // Loop Until temp.LastIndexOf("Target_Roller_Mass") <> -1 'this takes us to the end of the old headings
                                while (temp != "PRIMARY_CHANNEL_CURVE_FIT_DATA");
                                // line that holds the number of datapoints
                                temp = withBlock1.ReadLine();
                                DataCopyfile.WriteLine(temp);
                                AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0] = Conversions.ToDouble(temp.Substring(temp.LastIndexOf(" ")));
                                // line that holds the starting point
                                temp = withBlock1.ReadLine();
                                DataCopyfile.WriteLine(temp);

                                string ColumnTitles;
                                string[] TitlesSplit;
                                string SearchString;
                                string[] DataLine;
                                string[] UnitName;
                                int ParamCount;
                                int ParamPosition;

                                ColumnTitles = withBlock1.ReadLine(); // titles
                                                                      // now depending on the title line, difference approaches are required.
                                                                      // First substitute the "Watts_(e)" string to the current "Watts In" string including the splitter "_"
                                if (ColumnTitles.Contains("Point") && ColumnTitles.Contains("SystemTime")) // this is an early beta version.
                                {
                                    var loopTo1 = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                    for (PointCount = 1; PointCount <= loopTo1; PointCount++)
                                    {
                                        temp = withBlock1.ReadLine();
                                        line = temp.Split(' ');
                                        AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, PointCount] = Conversions.ToDouble(line[1]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_ROLLER, PointCount] = Conversions.ToDouble(line[6]); // / Main.DataUnits(Main.RPM1_ROLLER, 1) 'convert old RPM to new rad/s
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_WHEEL, PointCount] = Conversions.ToDouble(line[7]); // / Main.DataUnits(Main.RPM1_ROLLER, 1) 'convert old RPM to new rad/s
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_MOTOR, PointCount] = Conversions.ToDouble(line[8]); // / Main.DataUnits(Main.RPM1_ROLLER, 1) 'convert old RPM to new rad/s
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM2, PointCount] = Conversions.ToDouble(line[10]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM2_RATIO, PointCount] = Conversions.ToDouble(line[11]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.RPM2_ROLLOUT, PointCount] = Conversions.ToDouble(line[12]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.SPEED, PointCount] = Conversions.ToDouble(line[15]); // / Main.DataUnits(Main.SPEED, 1) 'convert old MPH to new m/s
                                        AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_ROLLER, PointCount] = Conversions.ToDouble(line[18]); // already in N.m
                                        AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_WHEEL, PointCount] = Conversions.ToDouble(line[19]); // AnalyzedData(OverlayFileCount, Main.POWER, PointCount) * (tempwheeldiameter / temprollerdiameter)
                                        AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_MOTOR, PointCount] = Conversions.ToDouble(line[20]); // AnalyzedData(OverlayFileCount, Main.TORQUE_WHEEL, PointCount) / tempgearratio
                                        AnalyzedData[OverlayFileCount, SimpleDyno.POWER, PointCount] = Conversions.ToDouble(line[30]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.DRAG, PointCount] = Conversions.ToDouble(line[33]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.VOLTS, PointCount] = Conversions.ToDouble(line[36]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.AMPS, PointCount] = Conversions.ToDouble(line[39]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.WATTS_IN, PointCount] = Conversions.ToDouble(line[40]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.EFFICIENCY, PointCount] = Conversions.ToDouble(line[42]);
                                        AnalyzedData[OverlayFileCount, SimpleDyno.TEMPERATURE1, PointCount] = Conversions.ToDouble(line[43]);
                                        // Everything else is going to be '0'
                                    }
                                }
                                else // It looks more like the current version, but not quite.
                                {
                                    ColumnTitles = ColumnTitles.Replace("Watts_(e)", "Watts_In");
                                    // No replace all "(" at the beginning of the units with "_("
                                    ColumnTitles = ColumnTitles.Replace("(", "_(");

                                    TitlesSplit = Strings.Split(ColumnTitles, " ");

                                    var loopTo2 = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                    for (PointCount = 1; PointCount <= loopTo2; PointCount++)
                                    {
                                        DataLine = Strings.Split(withBlock1.ReadLine(), " "); // reads all the values on this line into a string array
                                        for (ParamCount = 0; ParamCount <= SimpleDyno.LAST - 1; ParamCount++)
                                        {
                                            // This is how the titles are created in the fitting code except we do not add the space
                                            UnitName = Strings.Split(SimpleDyno.DataUnitTags[ParamCount], " ");
                                            SearchString = SimpleDyno.DataTags[ParamCount].Replace(" ", "_") + "_(" + UnitName[0] + ")";
                                            ParamPosition = Array.IndexOf(TitlesSplit, SearchString);
                                            if (ParamPosition != -1)
                                            {
                                                AnalyzedData[OverlayFileCount, ParamCount, PointCount] = Conversions.ToDouble(DataLine[ParamPosition]);
                                            }
                                        }
                                    }
                                }
                                // Now write all of the analyzed data to the datacopy file as if it were a power run
                                // write the new heading line
                                string tempstring = "";
                                string[] tempsplit;
                                for (ParamCount = 0; ParamCount <= SimpleDyno.LAST - 1; ParamCount++)
                                {
                                    tempsplit = Strings.Split(SimpleDyno.DataUnitTags[ParamCount], " ");
                                    tempstring = tempstring + SimpleDyno.DataTags[ParamCount].Replace(" ", "_") + "_(" + tempsplit[0] + ") ";
                                }
                                // Write the column headings
                                DataCopyfile.WriteLine(tempstring);
                                // now write out the new file format
                                var loopTo3 = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                for (PointCount = 1; PointCount <= loopTo3; PointCount++)
                                {
                                    tempstring = ""; // count.ToString & " "
                                    for (ParamCount = 0; ParamCount <= SimpleDyno.LAST - 1; ParamCount++) // CHECK - time is now the last column which will mess up the overlay routine .
                                    {
                                        tempsplit = Strings.Split(SimpleDyno.DataUnitTags[ParamCount], " "); // How many units are there
                                        tempstring = tempstring + AnalyzedData[OverlayFileCount, ParamCount, PointCount] * SimpleDyno.DataUnits[ParamCount, 0] + " "; // DataTags(paramcount).Replace(" ", "_") & "(" & tempsplit(unitcount) & ") "
                                    }
                                    // ...and write it
                                    DataCopyfile.WriteLine(tempstring);
                                }
                                DataCopyfile.WriteLine(Constants.vbCrLf);
                                while (!withBlock1.EndOfStream)
                                {
                                    temp = withBlock1.ReadLine();
                                    DataCopyfile.WriteLine(temp);
                                }
                                DataCopyfile.Close();
                                Program.MainI.SetMouseNormal_ThreadSafe(this);
                                break;
                            }
                        case var case2 when case2 == "POWER_CURVE": // We are assuming that this is a SD 5.5 Power Run File.
                            {
                                Interaction.MsgBox("A copy of " + OpenFileDialog1.FileName + " will be saved as a new version .sdp file.", Constants.vbOKOnly);
                                // Convert old data to new data
                                // open the copy file
                                Program.MainI.SetMouseBusy_ThreadSafe(this);
                                CopyFileName = OpenFileDialog1.FileName.Substring(0, OpenFileDialog1.FileName.Length - 4) + ".sdp";
                                DataCopyfile = new StreamWriter(CopyFileName);
                                DataCopyfile.WriteLine(SimpleDyno.PowerRunVersion);
                                DataCopyfile.WriteLine(CopyFileName);
                                // load it up as if it were a version 6 file
                                OverlayFileCount += 1;
                                if (OverlayFileCount == MAXDATAFILES)
                                {
                                    btnAddOverlayFile.Enabled = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Checked = false;
                                    SimpleDyno.frmFit.chkAddOrNew.Enabled = false;
                                }
                                OverlayFiles[OverlayFileCount] = CopyFileName.Substring(CopyFileName.LastIndexOf(@"\") + 1);
                                // Now read through the lines and copy them to the new file
                                var temprollerdiameter = default(double);
                                var tempwheeldiameter = default(double);
                                var tempgearratio = default(double);
                                temp = withBlock1.ReadLine(); // original file name line
                                do
                                {
                                    temp = withBlock1.ReadLine();
                                    DataCopyfile.WriteLine(temp);
                                    // while we are at it - look for roller dia, wheel dia and gear ratio
                                    if (temp.LastIndexOf(" ") != -1)
                                    {
                                        // Debug.Print(temp & temp.LastIndexOf(" "))
                                        if (temp.Split(' ')[1] == "Gear_Ratio:")
                                            tempgearratio = Conversions.ToDouble(temp.Split(' ')[2]);
                                        if (temp.Split(' ')[1] == "Wheel_Diameter:")
                                            tempwheeldiameter = Conversions.ToDouble(temp.Split(' ')[2]);
                                        if (temp.Split(' ')[1] == "Roller_Diameter:")
                                            temprollerdiameter = Conversions.ToDouble(temp.Split(' ')[2]);
                                    }
                                }
                                // Loop Until temp.LastIndexOf("Target_Roller_Mass") <> -1 'this takes us to the end of the old headings
                                while (temp != "PRIMARY_CHANNEL_CURVE_FIT_DATA");
                                // line that holds the number of datapoints
                                temp = withBlock1.ReadLine();
                                DataCopyfile.WriteLine(temp);
                                AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0] = Conversions.ToDouble(temp.Substring(temp.LastIndexOf(" ")));
                                // line that holds the starting point
                                temp = withBlock1.ReadLine();
                                DataCopyfile.WriteLine(temp);
                                // next is the original heading line which we will discard
                                temp = withBlock1.ReadLine();
                                // write the new heading line
                                string tempstring = "";
                                string[] tempsplit;
                                int paramcount;
                                for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++)
                                {
                                    tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " ");
                                    tempstring = tempstring + SimpleDyno.DataTags[paramcount].Replace(" ", "_") + "_(" + tempsplit[0] + ") ";
                                }
                                // Write the column headings
                                DataCopyfile.WriteLine(tempstring);
                                // now read in all of the fit data 
                                var loopTo4 = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                for (PointCount = 1; PointCount <= loopTo4; PointCount++)
                                {
                                    temp = withBlock1.ReadLine();
                                    line = temp.Split(' ');
                                    // This is the old line format
                                    // Point Time RollerRPM WheelRPM MotorRPM SpeedMPH SpeedKPH PowerWatts PowerHP TorqueNm Torqueinoz Torquecmg DragWatts DragHP
                                    // 1    2     3         4         5        6       7         8         9       10          11       12       13        14
                                    // This is the new line format
                                    // Time(Sec) RPM1_Roller(rad/s) RPM1_Wheel(rad/s) RPM1_Motor(rad/s) Speed(m/s) RPM2(rad/s) Ratio(M/W) Rollout(mm) Roller_Torque(N.m) Wheel_Torque(N.m) Motor_Torque(N.m) Power(W) Drag(W) Voltage(V) Current(A) Watts_(e)(W) Efficiency(%) Temperature(°C) 
                                    // So...
                                    AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, PointCount] = Conversions.ToDouble(line[1]);
                                    AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_ROLLER, PointCount] = Conversions.ToDouble(line[2]) / SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1]; // convert old RPM to new rad/s
                                    AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_WHEEL, PointCount] = Conversions.ToDouble(line[3]) / SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1]; // convert old RPM to new rad/s
                                    AnalyzedData[OverlayFileCount, SimpleDyno.RPM1_MOTOR, PointCount] = Conversions.ToDouble(line[4]) / SimpleDyno.DataUnits[SimpleDyno.RPM1_ROLLER, 1]; // convert old RPM to new rad/s
                                    AnalyzedData[OverlayFileCount, SimpleDyno.SPEED, PointCount] = Conversions.ToDouble(line[5]) / SimpleDyno.DataUnits[SimpleDyno.SPEED, 1]; // convert old MPH to new m/s
                                    AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_ROLLER, PointCount] = Conversions.ToDouble(line[9]); // already in N.m
                                    AnalyzedData[OverlayFileCount, SimpleDyno.POWER, PointCount] = Conversions.ToDouble(line[7]);
                                    AnalyzedData[OverlayFileCount, SimpleDyno.DRAG, PointCount] = Conversions.ToDouble(line[12]);
                                    // recalc the motor and wheel torques
                                    AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_WHEEL, PointCount] = AnalyzedData[OverlayFileCount, SimpleDyno.POWER, PointCount] * (tempwheeldiameter / temprollerdiameter);
                                    AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_MOTOR, PointCount] = AnalyzedData[OverlayFileCount, SimpleDyno.TORQUE_WHEEL, PointCount] / tempgearratio;
                                    // Everything else is going to be '0'
                                }
                                // now write out the new file format
                                var loopTo5 = (int)Math.Round(AnalyzedData[OverlayFileCount, SimpleDyno.SESSIONTIME, 0]);
                                for (PointCount = 1; PointCount <= loopTo5; PointCount++)
                                {
                                    tempstring = ""; // count.ToString & " "
                                    for (paramcount = 0; paramcount <= SimpleDyno.LAST - 1; paramcount++) // CHECK - time is now the last column which will mess up the overlay routine .
                                    {
                                        tempsplit = Strings.Split(SimpleDyno.DataUnitTags[paramcount], " "); // How many units are there
                                        tempstring = tempstring + AnalyzedData[OverlayFileCount, paramcount, PointCount] * SimpleDyno.DataUnits[paramcount, 0] + " "; // DataTags(paramcount).Replace(" ", "_") & "(" & tempsplit(unitcount) & ") "
                                    }
                                    // ...and write it
                                    DataCopyfile.WriteLine(tempstring);
                                }
                                DataCopyfile.WriteLine(Constants.vbCrLf);
                                while (!withBlock1.EndOfStream)
                                {
                                    temp = withBlock1.ReadLine();
                                    DataCopyfile.WriteLine(temp);
                                }
                                DataCopyfile.Close();
                                Program.MainI.SetMouseNormal_ThreadSafe(this);
                                break;
                            }

                        default:
                            {
                                Interaction.MsgBox("Could not open file.  If this is a Power Run created by an older SimpleDyno version please email it to damorc1@hotmail.com so that a fix can be made available", Constants.vbOKOnly);
                                break;
                            }
                    }
                }
                DataInputFile.Close();
            }
            pnlOverlaySetup();
        }
        internal void btnClearOverlay_Click_1(object sender, EventArgs e)
        {
            AnalyzedData = new double[6, 37, 50001];
            OverlayFileCount = 0;
            btnAddOverlayFile.Enabled = true;
            SimpleDyno.frmFit.chkAddOrNew.Enabled = true;
            pnlOverlaySetup();
        }
        private void btnSaveOverlay_Click_1(object sender, EventArgs e)
        {
            {
                var withBlock = SaveFileDialog1;
                withBlock.Reset();
                withBlock.Filter = "Bitmap files (*.bmp)|*.bmp";
                withBlock.ShowDialog();
            }
            if (!string.IsNullOrEmpty(SaveFileDialog1.FileName))
            {
                OverlayBitMap.Save(SaveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }
        private void cmbOverlayX_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOverlayUnitsX.Items.Clear();
            if (cmbOverlayDataX.SelectedIndex != SimpleDyno.LAST)
            {
                cmbOverlayUnitsX.Items.AddRange(Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataX.SelectedIndex]));
                cmbOverlayUnitsX.SelectedIndex = 0;
            }
            else
            {
                cmbOverlayUnitsX.Items.Add("--");
                cmbOverlayUnitsX.SelectedIndex = 0;
            }
            pnlOverlaySetup();
        }
        private void cmbOverlayY1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOverlayUnitsY1.Items.Clear();
            if (cmbOverlayDataY1.SelectedIndex != SimpleDyno.LAST)
            {
                cmbOverlayUnitsY1.Items.AddRange(Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY1.SelectedIndex]));
                cmbOverlayUnitsY1.SelectedIndex = 0;
            }
            else
            {
                cmbOverlayUnitsY1.Items.Add("--");
                cmbOverlayUnitsY1.SelectedIndex = 0;
            }
            pnlOverlaySetup();
        }
        private void cmbOverlayY2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOverlayUnitsY2.Items.Clear();
            if (cmbOverlayDataY2.SelectedIndex != SimpleDyno.LAST)
            {
                cmbOverlayUnitsY2.Items.AddRange(Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY2.SelectedIndex]));
                cmbOverlayUnitsY2.SelectedIndex = 0;
            }
            else
            {
                cmbOverlayUnitsY2.Items.Add("--");
                cmbOverlayUnitsY2.SelectedIndex = 0;
            }
            pnlOverlaySetup();
        }
        private void cmbOverlayY3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOverlayUnitsY3.Items.Clear();
            if (cmbOverlayDataY3.SelectedIndex != SimpleDyno.LAST)
            {
                cmbOverlayUnitsY3.Items.AddRange(Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY3.SelectedIndex]));
                cmbOverlayUnitsY3.SelectedIndex = 0;
            }
            else
            {
                cmbOverlayUnitsY3.Items.Add("--");
                cmbOverlayUnitsY3.SelectedIndex = 0;
            }
            pnlOverlaySetup();
        }
        private void cmbOverlayY4_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOverlayUnitsY4.Items.Clear();
            if (cmbOverlayDataY4.SelectedIndex != SimpleDyno.LAST)
            {
                cmbOverlayUnitsY4.Items.AddRange(Strings.Split(SimpleDyno.DataUnitTags[cmbOverlayDataY4.SelectedIndex]));
                cmbOverlayUnitsY4.SelectedIndex = 0;
            }
            else
            {
                cmbOverlayUnitsY4.Items.Add("--");
                cmbOverlayUnitsY4.SelectedIndex = 0;
            }
            pnlOverlaySetup();
        }
        private void cmbOverlayXUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void cmbOverlayY1Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void cmbOverlayY2Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void cmbOverlayY3Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void cmbOverlayY4Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void cmbOverlayCorrectedSpeedUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void TextBox_XStart_changed(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void TextBox_XEnd_Changed(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }

        // CHECK IF YOU WANT TO PULL ALL DECLARATIONS TO THE TOP
        private double OverlayXSelected;
        private bool OverlayPlotMax = true;
        private void pnlDataOverlay_Click(object sender, EventArgs e)
        {
            pnlOverlaySetup();
        }
        private void pnlDataOverlay_MouseMove(object sender, MouseEventArgs e)
        {
            // Display the X values based on the drawing window coordinates
            int MouseX;
            int MouseY;
            int LeftLimit;
            int RightLimit;
            int TopLimit;
            int BottomLimit;

            MouseX = pnlDataOverlay.PointToClient(MousePosition).X;
            MouseY = pnlDataOverlay.PointToClient(MousePosition).Y;

            LeftLimit = GraphStartCoordX;
            RightLimit = GraphEndCoordX;
            TopLimit = GraphStartCoordY;
            BottomLimit = GraphEndCoordY;

            if (MouseX < LeftLimit | MouseX > RightLimit)
            {
                OverlayXSelected = 0d;
                OverlayPlotMax = true;
            }
            else if (MouseY < TopLimit | MouseY > BottomLimit)
            {
                OverlayXSelected = 0d;
                OverlayPlotMax = true;
            }
            else
            {
                // OverlayXSelected is the X value in the primary units being plotted
                OverlayXSelected = (MouseX - LeftLimit) / (double)(RightLimit - LeftLimit) * xRangeLenFraction + xRangeStartFraction;
                OverlayPlotMax = false;
                lblCurrentXValue.Text = Program.MainI.NewCustomFormat(OverlayXSelected * SimpleDyno.DataUnits[cmbOverlayDataX.SelectedIndex, cmbOverlayUnitsX.SelectedIndex]) + " " + cmbOverlayUnitsX.SelectedItem.ToString();
            }
        }
    }
}