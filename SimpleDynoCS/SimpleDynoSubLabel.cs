using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleDyno
{
    public partial class SimpleDynoSubLabel : SimpleDynoSubForm
    {
        // Control Specific Text Positions
        private Point ParameterLabel;
        private Point ResultLabel;
        private Point UnitLabel;

        public SimpleDynoSubLabel()
        {
            InitializeComponent();
        }
        // Control specific configuration variable
        public override void ControlSpecificInitialization()
        {
            this.myType = "Label";
            this.Y_Number_Allowed = 1;
            this.XY_Selected = 1;
            this.myConfiguration = "Vertical";
        }
        public override void ControlSpecificResize()
        {

            float Increment = 0.1f;
            string DataTestString = "999999"; // Assumes no value displayed will be > 999999

            // Need to find the longer of the Parameter and unit strings
            string LabelTestString = " ";
            for (int Count = 1, loopTo = this.Y_Number_Allowed; Count <= loopTo; Count++)
            {
                if (this.Y_PrimaryLabel[Count].Length > LabelTestString.Length)
                    LabelTestString = this.Y_PrimaryLabel[Count];
                if (this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]].Length + " ".Length + this.Y_UnitsLabel[Count].Length > LabelTestString.Length)
                    LabelTestString = this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]].Length + " " + this.Y_UnitsLabel[Count];
            }

            switch (this.myConfiguration ?? "")
            {
                case var @case when @case == "Vertical":
                {
                    // Divide The window height into GR proportion
                    float DataFontHeight = (float)this.ClientSize.Height / SimpleDynoSubForm.GoldenRatio;
                    // The remainder divided by two is the height for the primary and unit labels
                    float LabelFontHeight = ((float)this.ClientSize.Height - DataFontHeight) / 2f;
                    // create a temporary font
                    var TempFont = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, Increment);
                    // now scale the data font
                    while (!(this.Grafx.Graphics.MeasureString(DataTestString, TempFont).Width >= (float)this.ClientSize.Width | this.Grafx.Graphics.MeasureString(DataTestString, TempFont).Height >= DataFontHeight))
                        TempFont = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, TempFont.Size + Increment);
                    // set the datafont to the tempfont size
                    this.Y_DataFont[this.Y_Number_Allowed] = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, TempFont.Size);
                    // now repeat for the label font
                    // reset tempfont
                    TempFont = new Font(this.Y_AxisFont.Name, Increment);
                    // scale the labelfont
                    while (!(this.Grafx.Graphics.MeasureString(LabelTestString, TempFont).Width >= (float)this.ClientSize.Width | this.Grafx.Graphics.MeasureString(LabelTestString, TempFont).Height >= LabelFontHeight))
                        TempFont = new Font(this.Y_AxisFont.Name, TempFont.Size + Increment);
                    // set the labelfont to the tempfont size
                    this.Y_AxisFont = new Font(this.Y_AxisFont.Name, TempFont.Size);
                    // Set up text positions based on available data
                    {
                        var withBlock = this.Grafx.Graphics;
                        ParameterLabel.X = (int)Math.Round(((float)this.ClientSize.Width - withBlock.MeasureString(this.Y_PrimaryLabel[this.XY_Selected], this.Y_AxisFont).Width) / 2f);
                        ResultLabel.Y = (int)Math.Round(((float)this.ClientSize.Height - withBlock.MeasureString(DataTestString, this.Y_DataFont[this.XY_Selected]).Height) / 2f);
                        ParameterLabel.Y = (int)Math.Round(((float)ResultLabel.Y - withBlock.MeasureString(this.Y_PrimaryLabel[this.XY_Selected], this.Y_AxisFont).Height) / 2f);
                        UnitLabel.X = (int)Math.Round(((float)this.ClientSize.Width - withBlock.MeasureString(this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]] + " " + this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont).Width) / 2f);
                        UnitLabel.Y = (int)Math.Round((float)this.ClientSize.Height - withBlock.MeasureString(this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont).Height - (float)ParameterLabel.Y);
                    }

                    break;
                }
                case var case1 when case1 == "Horizontal":
                {
                    // Divide The window height into GR proportion
                    double DataFontWidth = (double)this.ClientSize.Width / 2d; // GoldenRatio
                                                                                // The remainder divided by two is the height for the primary and unit labels
                    double LabelFontWidth = ((double)this.ClientSize.Width - DataFontWidth) / 2d;
                    // create a temporary font
                    var TempFont = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, Increment);
                    // now scale the data font
                    while (!((double)this.Grafx.Graphics.MeasureString(DataTestString, TempFont).Width >= DataFontWidth | this.Grafx.Graphics.MeasureString(DataTestString, TempFont).Height >= (float)this.ClientSize.Height))
                        TempFont = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, TempFont.Size + Increment);
                    // set the datafont to the tempfont size
                    this.Y_DataFont[this.Y_Number_Allowed] = new Font(this.Y_DataFont[this.Y_Number_Allowed].Name, TempFont.Size);
                    // now repeat for the label font
                    // reset tempfont
                    TempFont = new Font(this.Y_AxisFont.Name, Increment);
                    // scale the labelfont
                    while (!((double)this.Grafx.Graphics.MeasureString(LabelTestString, TempFont).Width >= LabelFontWidth | this.Grafx.Graphics.MeasureString(LabelTestString, TempFont).Height >= (float)this.ClientSize.Height))
                        TempFont = new Font(this.Y_AxisFont.Name, TempFont.Size + Increment);
                    // set the labelfont to the tempfont size
                    this.Y_AxisFont = new Font(this.Y_AxisFont.Name, TempFont.Size);
                    // Set up text positions based on available data
                    {
                        var withBlock1 = this.Grafx.Graphics;
                        ParameterLabel.Y = (int)Math.Round(((float)this.ClientSize.Height - withBlock1.MeasureString(this.Y_PrimaryLabel[this.XY_Selected], this.Y_AxisFont).Height) / 2f);
                        ParameterLabel.X = this.ClientRectangle.Left;
                        ResultLabel.Y = (int)Math.Round(((float)this.ClientSize.Height - withBlock1.MeasureString(DataTestString, this.Y_DataFont[this.XY_Selected]).Height) / 2f);
                        UnitLabel.X = (int)Math.Round((float)this.ClientRectangle.Right - withBlock1.MeasureString(this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]] + " " + this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont).Width);
                        UnitLabel.Y = (int)Math.Round(((float)this.ClientSize.Height - withBlock1.MeasureString(this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont).Height) / 2f);
                    }
                    break;
                }
            }
        }
        public override void DrawToBuffer(Graphics g)
        {
            string StringResult;
            // replace the custom format with new version
            // StringResult = Y_Result(XY_Selected).ToString(CustomFormat(Y_Result(XY_Selected)))
            StringResult = this.NewCustomFormat(this.Y_Result[this.XY_Selected]); // .ToString(CustomFormat(Y_Result(XY_Selected)))
            ResultLabel.X = (int)Math.Round(((float)this.ClientSize.Width - this.Grafx.Graphics.MeasureString(StringResult, this.Y_DataFont[this.XY_Selected]).Width) / 2f);

            this.Grafx.Graphics.Clear(this.BackClr);

            {
                var withBlock = this.Grafx.Graphics;
                withBlock.DrawString(this.Y_PrimaryLabel[this.XY_Selected], this.Y_AxisFont, this.AxisBrush, ParameterLabel);
                withBlock.DrawString(StringResult, this.Y_DataFont[this.XY_Selected], this.Y_DataBrush[this.XY_Selected], ResultLabel);
                withBlock.DrawString(this.myMinCurMaxAbb[this.Y_MinCurMaxPointer[this.XY_Selected]] + " " + this.Y_UnitsLabel[this.XY_Selected], this.Y_AxisFont, this.AxisBrush, UnitLabel);
            }
        }
        public override void AddControlSpecificOptionItems()
        {
            ToolStripMenuItem TestStrip;
            string str1;
            string[] str2;
            string[] str3;

            str1 = "Configuration";
            str2 = new[] { "Vertical", "Horizontal" };
            str3 = Array.Empty<string>();

            TestStrip = this.CreateAToolStripMenuItem("O", str1, str2, str3); // , str4, str5)
            this.Contextmnu.Items.Add(TestStrip);
        }
        public override void ControlSpecificOptionSelection(string Sent)
        {
            switch (Sent ?? "")
            {
                case var @case when @case == "O_0":
                {
                    this.myConfiguration = "Vertical";
                    break;
                }
                case var case1 when case1 == "O_1":
                {
                    this.myConfiguration = "Horizontal";
                    break;
                }
            }
        }
        public override string ControlSpecificSerializationData() // - REMOVE COLOR ETC THAT SHOULD BE HANDLED BY THE PARENT CLASS
        {
            return default;
        }
        public override void ControlSpecficCreateFromSerializedData(string[] Sent)
        {

        }
    }
}
