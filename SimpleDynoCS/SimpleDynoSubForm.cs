using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{

    // Friend Enum SerialParameters
    // 'Enumerate the basic components of each display object 
    // Left = 0
    // Top
    // Height
    // Width
    // Configuration
    // Y_Number
    // Timer
    // LastMember
    // End Enum
    public abstract partial class SimpleDynoSubForm : Form
    {
        private BufferedGraphicsContext context;

        internal bool Resizing = false;

        private Point MouseDownPosition;
        private bool IsLeft = false;

        internal string myType = "SD Form";
        internal string myConfiguration;
        internal int myNumber;

        private string AllowedCharacters = "-0123456789" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        internal string Splitter = "_";
        internal string[] myMinCurMax = new[] { "Minimum", "Actual", "Maximum" };
        internal string[] myMinCurMaxAbb = new[] { "Min", "", "Max" };
        internal const float GoldenRatio = 1.61803398875f;

        internal BufferedGraphics Grafx;
        internal ContextMenuStrip Contextmnu;
        internal ColorDialog Colormnu = new ColorDialog();
        internal FontDialog Fontmnu = new FontDialog();

        internal Color BackClr;
        internal Color AxisClr;
        internal Color[] Y_DataClr;

        internal SolidBrush AxisBrush;
        internal SolidBrush[] Y_DataBrush;

        internal Pen AxisPen;
        internal Pen[] Y_DataPen;

        internal Font X_AxisFont;
        internal Font Y_AxisFont;
        internal Font[] Y_DataFont;

        //internal System.Windows.Forms.Timer timer1;
        internal bool TimerTiggered;

        internal int Y_Number_Allowed;
        internal int XY_Selected;

        internal int X_PrimaryPointer;
        internal int X_MinCurMaxPointer;
        internal int X_UnitPointer;
        internal double X_Minimum;
        internal double X_Maximum;
        internal double X_Result;
        internal string X_PrimaryLabel;
        internal string X_UnitsLabel;
        internal string X_MinMaxCurLabel;

        internal int[] Y_PrimaryPointer;
        internal int[] Y_MinCurMaxPointer;
        internal int[] Y_UnitPointer;
        internal double[] Y_Minimum;
        internal double[] Y_Maximum;
        internal double[] Y_Result;
        internal string[] Y_PrimaryLabel;
        internal string[] Y_UnitsLabel;
        internal string[] Y_MinMaxCurLabel;
        internal bool[] IsThisYSelected = new bool[5];

        private const int GridSnap = 10;

        internal double[,] CopyOfData;
        internal double[,] CopyOfUnits;
        internal string[] CopyOfDataNames;
        internal string[] CopyOfUnitsNames;
        internal bool[] CopyofDataAreUsed;

        public static event RemoveYourselfEventHandler RemoveYourself;

        public delegate void RemoveYourselfEventHandler(int WhichNumberAmI);
        public static event SetToMyFormatEventHandler SetToMyFormat;

        public delegate void SetToMyFormatEventHandler(string MyFormat);


        public abstract void ControlSpecificResize();
        public abstract void DrawToBuffer(Graphics g);
        public abstract void AddControlSpecificOptionItems();
        public abstract void ControlSpecificInitialization();
        public abstract void ControlSpecificOptionSelection(string Sent);
        public abstract string ControlSpecificSerializationData();
        public abstract void ControlSpecficCreateFromSerializedData(string[] Sent);


        public SimpleDynoSubForm()
        {
            InitializeComponent();
            DoubleClick += SimpleDynoSubForm_DoubleClick;
            MouseUp += SimpleDynoSubForm_MouseUp;
            MouseMove += SDForm_MouseMove;
            // Nothing Here
        }

        internal void Initialize(int WhichNumberAmI, ref double[,] SentData, ref string[] SentDataNames, ref double[,] SentUnits, ref string[] SentUnitsNames, ref bool[] SentDataAreUsed) // ADD CONSTRUCTION CALLS HERE)
        {

            ClientSize = new Size(200, 200);
            ShowIcon = false;
            ShowInTaskbar = false;
            ControlBox = false;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            TopMost = false;

            myNumber = WhichNumberAmI;
            CopyOfData = SentData;
            CopyOfUnits = SentUnits;
            CopyOfDataNames = SentDataNames;
            CopyOfUnitsNames = SentUnitsNames;
            CopyofDataAreUsed = SentDataAreUsed;

            ControlSpecificInitialization();

            Y_PrimaryPointer = new int[Y_Number_Allowed + 1];
            Y_MinCurMaxPointer = new int[Y_Number_Allowed + 1];
            Y_UnitPointer = new int[Y_Number_Allowed + 1];
            Y_Minimum = new double[Y_Number_Allowed + 1];
            Y_Maximum = new double[Y_Number_Allowed + 1];
            Y_Result = new double[Y_Number_Allowed + 1];

            BackClr = Color.White;
            AxisClr = Color.Black;
            AxisBrush = new SolidBrush(AxisClr);
            AxisPen = new Pen(AxisClr, 2f);
            X_AxisFont = new Font("Arial", 5f);
            Y_AxisFont = new Font("Arial", 5f);

            Y_DataClr = new Color[Y_Number_Allowed + 1];
            Y_DataBrush = new SolidBrush[Y_Number_Allowed + 1];
            Y_DataPen = new Pen[Y_Number_Allowed + 1];
            Y_DataFont = new Font[Y_Number_Allowed + 1];
            Y_PrimaryLabel = new string[Y_Number_Allowed + 1];
            Y_UnitsLabel = new string[Y_Number_Allowed + 1];
            Y_MinMaxCurLabel = new string[Y_Number_Allowed + 1];

            for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
            {
                Y_DataClr[Count] = Color.Green;
                Y_DataBrush[Count] = new SolidBrush(Y_DataClr[Count]);
                Y_DataPen[Count] = new Pen(Y_DataClr[Count]);
                Y_DataFont[Count] = new Font("Arial", 5f);
                Y_PrimaryLabel[Count] = "Parameter";
                Y_UnitsLabel[Count] = "Unit";
                Y_MinMaxCurLabel[Count] = "Act";
                Y_Maximum[Count] = 1000d;
                Y_Minimum[Count] = 0d;
            }

            //timer1 = new System.Windows.Forms.Timer();
            //timer1.Interval = 1000;

            //timer1.Tick += SDForm_TimerTick;
            MouseDown += SDForm_MouseDown;
            MouseMove += SDForm_MouseMove;
            Resize += SDForm_Resize;
            Move += SDForm_Move;

            CreateTheMenu();

            context = BufferedGraphicsManager.Current;

            SDForm_Resize(this, EventArgs.Empty);

            timer1.Start();

            Show();

        }
        public string ReportForSerialization()
        {
            string TempReport;

                var withBlock = this;
                TempReport = withBlock.Location.X + Splitter + withBlock.Location.Y + Splitter + withBlock.Height + Splitter + withBlock.Width + Splitter; // dimensions and posisition of window
                TempReport += myConfiguration + Splitter + Y_Number_Allowed + Splitter + timer1.Interval + Splitter;   // specific configuration and number of Y_datasets
                TempReport += ColorTranslator.ToHtml(BackClr) + Splitter + ColorTranslator.ToHtml(AxisClr) + Splitter; // basic colors
                TempReport += X_PrimaryPointer + Splitter + X_MinCurMaxPointer + Splitter + X_UnitPointer + Splitter;  // X data pointer
                TempReport += X_Minimum + Splitter + X_Maximum + Splitter;                                             // X min and max
                TempReport += X_AxisFont.Name + Splitter;
                for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
                {
                    TempReport += ColorTranslator.ToHtml(Y_DataClr[Count]) + Splitter;
                    TempReport += Y_PrimaryPointer[Count] + Splitter;
                    TempReport += Y_MinCurMaxPointer[Count] + Splitter;
                    TempReport += Y_UnitPointer[Count] + Splitter; // y data pointers
                    TempReport += Y_Minimum[Count] + Splitter;
                    TempReport += Y_Maximum[Count] + Splitter;
                    TempReport += IsThisYSelected[Count] + Splitter;
                }

            TempReport = TempReport + ControlSpecificSerializationData();
            return TempReport;
        }
        public void CreateFromSerializedData(ref string SentSerialInformation)
        {

            string[] Parameters;
            var NewLocation = default(Point);
            var NewSize = default(Size);

            Parameters = Strings.Split(SentSerialInformation, Splitter);

            NewLocation.X = Conversions.ToInteger(Parameters[0]);
            NewLocation.Y = Conversions.ToInteger(Parameters[1]);
            NewSize.Height = Conversions.ToInteger(Parameters[2]);
            NewSize.Width = Conversions.ToInteger(Parameters[3]); // position and size
            myConfiguration = Parameters[4];
            Y_Number_Allowed = Conversions.ToInteger(Parameters[5]);
            timer1.Interval = Conversions.ToInteger(Parameters[6]); // basic configuration
            BackClr = ColorTranslator.FromHtml(Parameters[7]);
            AxisClr = ColorTranslator.FromHtml(Parameters[8]); // basic colors
            AxisBrush.Color = AxisClr;
            AxisPen.Color = AxisClr;
            X_PrimaryPointer = Conversions.ToInteger(Parameters[9]);
            X_MinCurMaxPointer = Conversions.ToInteger(Parameters[10]);
            X_UnitPointer = Conversions.ToInteger(Parameters[11]);  // X data pointer
            X_PrimaryLabel = CopyOfDataNames[X_PrimaryPointer];
            X_UnitsLabel = Strings.Split(CopyOfUnitsNames[X_PrimaryPointer])[X_UnitPointer];
            X_MinMaxCurLabel = myMinCurMax[X_MinCurMaxPointer];

            X_Minimum = Conversions.ToDouble(Parameters[12]);
            X_Maximum = Conversions.ToDouble(Parameters[13]); // X min and max
            X_AxisFont = new Font(Parameters[14], 5f);
            Y_AxisFont = new Font(Parameters[14], 5f);


            for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
            {
                Y_DataClr[Count] = ColorTranslator.FromHtml(Parameters[15 + (Count - 1) * 7]);
                Y_DataBrush[Count].Color = Y_DataClr[Count];
                Y_DataPen[Count].Color = Y_DataClr[Count];
                Y_PrimaryPointer[Count] = Conversions.ToInteger(Parameters[16 + (Count - 1) * 7]);
                Y_PrimaryLabel[Count] = CopyOfDataNames[Y_PrimaryPointer[Count]];
                Y_MinCurMaxPointer[Count] = Conversions.ToInteger(Parameters[17 + (Count - 1) * 7]);
                Y_MinMaxCurLabel[Count] = myMinCurMax[Y_MinCurMaxPointer[Count]];
                Y_UnitPointer[Count] = Conversions.ToInteger(Parameters[18 + (Count - 1) * 7]);
                Y_UnitsLabel[Count] = Strings.Split(CopyOfUnitsNames[Y_PrimaryPointer[Count]])[Y_UnitPointer[Count]];
                Y_Minimum[Count] = Conversions.ToDouble(Parameters[19 + (Count - 1) * 7]);
                Y_Maximum[Count] = Conversions.ToDouble(Parameters[20 + (Count - 1) * 7]);
                IsThisYSelected[Count] = Conversions.ToBoolean(Parameters[21 + (Count - 1) * 7]);
                Y_DataFont[Count] = new Font(Parameters[14], 5f);
            }

            Location = NewLocation;
            Size = NewSize;

            ControlSpecficCreateFromSerializedData(Parameters);

            SDForm_Resize(this, EventArgs.Empty);

            DrawToBuffer(Grafx.Graphics);
            Refresh();
        }
        public void HideYourSelf()
        {
            // Me.WindowState = FormWindowState.Minimized
            Visible = false;
        }
        public void ShowYourSelf()
        {
            // Me.WindowState = FormWindowState.Normal
            Visible = true;
        }
        public void Pause()
        {
            timer1.Stop();
        }
        public void Restart()
        {
            timer1.Start();
        }
        public void CreateTheMenu()
        {
            ToolStripMenuItem TestStrip;

            string str1;
            string[] str2;
            string[] str3;
            int Count;

            TestStrip = new ToolStripMenuItem();

            Contextmnu = new ContextMenuStrip();

            var loopTo = Information.UBound(CopyOfDataNames);
            for (Count = 0; Count <= loopTo; Count++) // - 1 'Beta - to allow session time to be sent but not displayed
            {
                if (CopyofDataAreUsed[Count])
                {
                    str1 = CopyOfDataNames[Count];
                    str2 = myMinCurMax; // {"Min", "Current", "Max"}
                    str3 = Strings.Split(CopyOfUnitsNames[Count], " ");
                    TestStrip = CreateAToolStripMenuItem(Count.ToString(), str1, str2, str3);
                    Contextmnu.Items.Add(TestStrip);
                }

            }

            Contextmnu.Items.Add("-");

            // ************************************************
            // Next up are the options specific to the control such as horiz/vertical for label, narrow/flat/wide for gauge

            AddControlSpecificOptionItems();

            // ***********************************************
            // Color options are no longer control specific - add standardized form here

            TestStrip = new ToolStripMenuItem();
            str1 = "Set Colors";
            str2 = new[] { "Background", "Axes", "Data", "Apply to All" };
            str3 = Array.Empty<string>();

            TestStrip = CreateAToolStripMenuItem("C", str1, str2, str3); // , str4, str5)
            Contextmnu.Items.Add(TestStrip);

            // ***********************************************
            // Font menu for the form
            str1 = "Set Font";
            str2 = Array.Empty<string>();
            str3 = Array.Empty<string>();
            TestStrip = CreateAToolStripMenuItem("F", str1, str2, str3); // , str4, str5)
            Contextmnu.Items.Add(TestStrip);
            // ***********************************************
            // Refresh rates are common for all controls

            str1 = "Refresh rate";
            str2 = new[] { "10 msec", "30 msec", "100 msec", "300 msec", "1000 msec" };
            str3 = Array.Empty<string>();
            TestStrip = CreateAToolStripMenuItem("R", str1, str2, str3);
            Contextmnu.Items.Add(TestStrip);
        }
        internal ToolStripMenuItem CreateAToolStripMenuItem(string StartTag, string Level1, string[] Level2, string[] Level3)
        {

            ToolStripMenuItem Level1Menu;
            var Level2Menu = new ToolStripMenuItem[Information.UBound(Level2) + 1];
            var Level3Menu = new ToolStripMenuItem[Information.UBound(Level3) + 1];


            int Level2Count;
            int Level3Count;

            EventHandler Remove = ClickTheMenu;
            KeyPressEventHandler Check = CheckText;

            Level1Menu = new ToolStripMenuItem();
            Level1Menu.Tag = StartTag;
            Level1Menu.Text = Level1;
            Level1Menu.Click += Remove;
            // .CheckOnClick = True
            var loopTo = Information.UBound(Level2);
            for (Level2Count = 0; Level2Count <= loopTo; Level2Count++)
            {
                Level2Menu[Level2Count] = new ToolStripMenuItem();
                {
                    ref var withBlock = ref Level2Menu[Level2Count];
                    withBlock.Tag = StartTag + "_" + Level2Count.ToString();
                    withBlock.Text = Level2[Level2Count];
                    Level1Menu.Click -= Remove;
                    withBlock.Click += Remove;
                    // Level1Menu.CheckOnClick = False
                    // .CheckOnClick = True
                    var loopTo1 = Information.UBound(Level3);
                    for (Level3Count = 0; Level3Count <= loopTo1; Level3Count++)
                    {
                        Level3Menu[Level3Count] = new ToolStripMenuItem();
                        if (Level3[Level3Count] != "TXT")
                        {
                            {
                                ref var withBlock1 = ref Level3Menu[Level3Count];
                                withBlock1.Tag = StartTag + "_" + Level2Count.ToString() + "_" + Level3Count.ToString();
                                withBlock1.Text = Level3[Level3Count];
                                withBlock1.Click += Remove;
                                // Level1Menu.CheckOnClick = False
                                // .CheckOnClick = True
                            }
                            withBlock.DropDownItems.Add(Level3Menu[Level3Count]);
                        }
                        else
                        {
                            var t = new ToolStripTextBox();
                            t.Tag = StartTag + "_" + Level2Count.ToString() + "_" + Level3Count.ToString();
                            t.KeyPress += Check;
                            withBlock.DropDownItems.Add(t);
                        }
                        Level2Menu[Level2Count].Click -= Remove;
                    }
                }
                Level1Menu.DropDownItems.Add(Level2Menu[Level2Count]);
            }
            return Level1Menu;
        }
        public virtual void ShowTheMenu()
        {
            var Where = new Point();
            Where.X = Right;
            Where.Y = Top;
            Contextmnu.Show(Where);
        }
        public virtual void CheckText(object objsender, KeyPressEventArgs e)
        {
            ToolStripTextBox sender = (ToolStripTextBox)objsender;
            if (e.KeyChar != '\b')
            {
                if (AllowedCharacters.IndexOf(e.KeyChar) == -1)
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar == '\r')
            {
                ControlSpecificOptionSelection(sender.Tag.ToString() + " " + sender.Text.ToString());
                SDForm_Resize(this, EventArgs.Empty);
                DrawToBuffer(Grafx.Graphics);
                Contextmnu.Close();
                Refresh();
            }
        }
        public virtual void UpdateForm(ToolStripTextBox sender, EventArgs e)
        {
            ControlSpecificOptionSelection(sender.Tag.ToString() + " " + sender.Text.ToString());
            SDForm_Resize(this, EventArgs.Empty);
            DrawToBuffer(Grafx.Graphics);
            Refresh();
        }

        // Overridable Sub ClickTheMenu(ByVal sender As ToolStripMenuItem, ByVal e As System.EventArgs)
        public virtual void ClickTheMenu(object objsender, EventArgs e)
        {
            // Because there are control specific possible entries may need to use a combination of if then else and select case statements to process these correctly
            // timers are consistent among controls so this can be processed by the parent
            ToolStripMenuItem sender;
            sender = (ToolStripMenuItem)objsender;
            switch (sender.Tag.ToString() ?? "")
            {
                case var @case when @case == "R_0":
                    {
                        timer1.Interval = 10;
                        break;
                    }
                case var case1 when case1 == "R_1":
                    {
                        timer1.Interval = 30;
                        break;
                    }
                case var case2 when case2 == "R_2":
                    {
                        timer1.Interval = 100;
                        break;
                    }
                case var case3 when case3 == "R_3":
                    {
                        timer1.Interval = 300;
                        break;
                    }
                case var case4 when case4 == "R_4":
                    {
                        timer1.Interval = 1000;
                        break;
                    }
                case var case5 when case5 == "C_0": // Background
                    {
                        Colormnu.ShowDialog();
                        BackClr = Colormnu.Color;
                        Contextmnu.Close();
                        break;
                    }
                case var case6 when case6 == "C_1": // Axis
                    {
                        Colormnu.ShowDialog();
                        AxisClr = Colormnu.Color;
                        AxisBrush.Color = AxisClr;
                        AxisPen.Color = AxisClr;
                        Contextmnu.Close();
                        break;
                    }
                case var case7 when case7 == "C_2": // Data
                    {
                        Colormnu.ShowDialog();
                        Y_DataClr[XY_Selected] = Colormnu.Color;
                        Y_DataBrush[XY_Selected].Color = Y_DataClr[XY_Selected];
                        Y_DataPen[XY_Selected].Color = Y_DataClr[XY_Selected];
                        Contextmnu.Close();
                        break;
                    }
                case var case8 when case8 == "C_3": // Apply format to all
                    {
                        SetToMyFormat?.Invoke(GetMyFormat());
                        break;
                    }
                case var case9 when case9 == "F": // Font
                    {
                        Fontmnu.ShowDialog();
                        if (Fontmnu.Font.FontFamily.IsStyleAvailable(FontStyle.Regular))
                        {
                            Y_AxisFont = Fontmnu.Font;
                            X_AxisFont = Fontmnu.Font;
                            for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
                                Y_DataFont[Count] = Fontmnu.Font;
                        }

                        break;
                    }

                default:
                    {
                        if (Strings.InStr(sender.Tag.ToString(), "O") != 0 | Strings.InStr(sender.Tag.ToString(), "M") != 0 | Strings.InStr(sender.Tag.ToString(), "X") != 0)
                        {
                            // check for control specific options
                            ControlSpecificOptionSelection(sender.Tag.ToString());
                        }
                        else
                        {
                            // Split the tag into individual chars
                            string[] Pointers = Strings.Split(sender.Tag.ToString(), "_");
                            // each tag will have min/max and units at the end, and primary at the front
                            // so if there are only 3 chars then no option or suboption was used
                            switch (XY_Selected)
                            {
                                case var case10 when case10 == 0: // X axis
                                    {
                                        X_PrimaryPointer = Conversions.ToInteger(Pointers[0]);
                                        X_MinCurMaxPointer = Conversions.ToInteger(Pointers[1]);
                                        X_UnitPointer = Conversions.ToInteger(Pointers[2]);
                                        X_PrimaryLabel = CopyOfDataNames[X_PrimaryPointer];
                                        X_MinMaxCurLabel = myMinCurMax[X_MinCurMaxPointer];
                                        X_UnitsLabel = Strings.Split(CopyOfUnitsNames[X_PrimaryPointer])[X_UnitPointer];
                                        break;
                                    }

                                default:
                                    {
                                        Y_PrimaryPointer[XY_Selected] = Conversions.ToInteger(Pointers[0]);
                                        Y_MinCurMaxPointer[XY_Selected] = Conversions.ToInteger(Pointers[1]);
                                        Y_UnitPointer[XY_Selected] = Conversions.ToInteger(Pointers[2]);
                                        Y_PrimaryLabel[XY_Selected] = CopyOfDataNames[Y_PrimaryPointer[XY_Selected]];
                                        Y_MinMaxCurLabel[XY_Selected] = myMinCurMax[Y_MinCurMaxPointer[XY_Selected]];
                                        Y_UnitsLabel[XY_Selected] = Strings.Split(CopyOfUnitsNames[Y_PrimaryPointer[XY_Selected]])[Y_UnitPointer[XY_Selected]];
                                        break;
                                    }
                            }
                        }

                        break;
                    }

            }

            SDForm_Resize(this, EventArgs.Empty);
            DrawToBuffer(Grafx.Graphics);
            Refresh();

        }
        internal string GetMyFormat()
        {
            string GetMyFormatRet = default;
            // Dim GetMyFormat As String

            {
                //ref var withBlock = ref this;
                // GetMyFormat = .Location.X & Splitter & .Location.Y & Splitter & .Height & Splitter & .Width & Splitter 'dimensions and posisition of window
                // GetMyFormat += myConfiguration & Splitter & Y_Number_Allowed & Splitter & timer1.Interval & Splitter   'specific configuration and number of Y_datasets
                GetMyFormatRet += ColorTranslator.ToHtml(BackClr) + Splitter + ColorTranslator.ToHtml(AxisClr) + Splitter; // basic colors
                                                                                                                           // GetMyFormat += X_PrimaryPointer & Splitter & X_MinCurMaxPointer & Splitter & X_UnitPointer & Splitter  'X data pointer
                                                                                                                           // GetMyFormat += X_Minimum & Splitter & X_Maximum & Splitter                                             'X min and max
                GetMyFormatRet += X_AxisFont.Name + Splitter;
                for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
                    // GetMyFormat += Y_PrimaryPointer(Count) & Splitter
                    // GetMyFormat += Y_MinCurMaxPointer(Count) & Splitter
                    // GetMyFormat += Y_UnitPointer(Count) & Splitter 'y data pointers
                    // GetMyFormat += Y_Minimum(Count) & Splitter
                    // GetMyFormat += Y_Maximum(Count) & Splitter
                    // GetMyFormat += IsThisYSelected(Count) & Splitter
                    GetMyFormatRet += ColorTranslator.ToHtml(Y_DataClr[Count]) + Splitter;

            }
            // TempReport = TempReport & ControlSpecificSerializationData()
            return GetMyFormatRet;
        }
        internal void SetMyFormat(string SentFormat)
        {
            string[] Parameters;
            // Dim NewLocation As System.Drawing.Point
            // Dim NewSize As System.Drawing.Size

            Parameters = Strings.Split(SentFormat, Splitter);

            // NewLocation.X = Parameters(0) : NewLocation.Y = Parameters(1) : NewSize.Height = Parameters(2) : NewSize.Width = Parameters(3) 'position and size
            // myConfiguration = Parameters(4) : Y_Number_Allowed = Parameters(5) : timer1.Interval = Parameters(6) 'basic configuration
            BackClr = ColorTranslator.FromHtml(Parameters[0]);
            AxisClr = ColorTranslator.FromHtml(Parameters[1]); // basic colors
            AxisBrush.Color = AxisClr;
            AxisPen.Color = AxisClr;
            // X_PrimaryPointer = Parameters(9) : X_MinCurMaxPointer = Parameters(10) : X_UnitPointer = Parameters(11)  'X data pointer
            // X_PrimaryLabel = CopyOfDataNames(X_PrimaryPointer)
            // X_UnitsLabel = Split(CopyOfUnitsNames(X_PrimaryPointer))(X_UnitPointer)
            // X_MinMaxCurLabel = myMinCurMax(X_MinCurMaxPointer)

            // X_Minimum = Parameters(12) : X_Maximum = Parameters(13) 'X min and max
            X_AxisFont = new Font(Parameters[2], 5f);
            Y_AxisFont = new Font(Parameters[2], 5f);


            for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
            {
                Y_DataClr[Count] = ColorTranslator.FromHtml(Parameters[3]);
                Y_DataBrush[Count].Color = Y_DataClr[Count];
                Y_DataPen[Count].Color = Y_DataClr[Count];
                // Y_PrimaryPointer(Count) = Parameters(16 + (Count - 1) * 7)
                // Y_PrimaryLabel(Count) = CopyOfDataNames(Y_PrimaryPointer(Count))
                // Y_MinCurMaxPointer(Count) = Parameters(17 + (Count - 1) * 7)
                // Y_MinMaxCurLabel(Count) = myMinCurMax(Y_MinCurMaxPointer(Count))
                // Y_UnitPointer(Count) = Parameters(18 + (Count - 1) * 7)
                // Y_UnitsLabel(Count) = Split(CopyOfUnitsNames(Y_PrimaryPointer(Count)))(Y_UnitPointer(Count))
                // Y_Minimum(Count) = Parameters(19 + (Count - 1) * 7)
                // Y_Maximum(Count) = Parameters(20 + (Count - 1) * 7)
                // IsThisYSelected(Count) = Parameters(21 + (Count - 1) * 7)
                Y_DataFont[Count] = new Font(Parameters[2], 5f);
            }
            SDForm_Resize(this, EventArgs.Empty);
            DrawToBuffer(Grafx.Graphics);
            Refresh();
        }

        internal double ConvertedToRadians(double Sent)
        {
            return Sent * Math.PI / 180d;
        }
        public virtual void ResetSDForm()
        {

        }
        private void SDForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IsLeft = false;
                ShowTheMenu();
                DrawToBuffer(Grafx.Graphics);
                Refresh();
            }
            else if (e.Button == MouseButtons.Left)
            {
                IsLeft = true;
                MouseDownPosition.X = MousePosition.X - Location.X;
                MouseDownPosition.Y = MousePosition.Y - Location.Y;
            }
        }
        private void SimpleDynoSubForm_DoubleClick(object sender, EventArgs e)
        {
            // HideYourSelf() 'CHECK - this just hides the form - does not remove it from the list (so the form gets saved and re-appears)
            RemoveYourself?.Invoke(myNumber);
        }
        private void SimpleDynoSubForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                AlignTheForm();
            IsLeft = false;
        }
        private void SDForm_TimerTick(object sender, EventArgs e)
        {
            TimerTiggered = true;
            X_Result = CopyOfData[X_PrimaryPointer, X_MinCurMaxPointer] * CopyOfUnits[X_PrimaryPointer, X_UnitPointer];
            for (int Count = 1, loopTo = Y_Number_Allowed; Count <= loopTo; Count++)
                Y_Result[Count] = CopyOfData[Y_PrimaryPointer[Count], Y_MinCurMaxPointer[Count]] * CopyOfUnits[Y_PrimaryPointer[Count], Y_UnitPointer[Count]];
            DrawToBuffer(Grafx.Graphics);
            TimerTiggered = false;
            Refresh();
        }
        internal void SDForm_Resize(object sender, EventArgs e)
        {

            if (Resizing == false)
            {
                Resizing = true;

                if (WindowState != FormWindowState.Minimized)
                {
                    // Crude Snap to Grid
                    var TempSize = default(Size);

                    if (ClientSize.Height < 20)
                    {
                        TempSize.Height = 20;
                        TempSize.Width = ClientSize.Width;
                        ClientSize = TempSize;
                    }
                    if (ClientSize.Width < 20)
                    {
                        TempSize.Width = 20;
                        TempSize.Height = ClientSize.Height;
                        ClientSize = TempSize;
                    }
                    TempSize.Height = (int)Math.Round(ClientSize.Height / (double)GridSnap) * GridSnap;
                    TempSize.Width = (int)Math.Round(ClientSize.Width / (double)GridSnap) * GridSnap;
                    ClientSize = TempSize;

                    // Re-create the graphics buffer for a new window size.
                    context.MaximumBuffer = new Size(Width + 1, Height + 1);
                    if (Grafx is not null)
                    {
                        Grafx.Dispose();
                        Grafx = null;
                    }
                    Grafx = context.Allocate(CreateGraphics(), new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));

                    ControlSpecificResize();

                    // Cause the background to be cleared and redraw.
                    DrawToBuffer(Grafx.Graphics);
                    Refresh();
                }
                Resizing = false;
            }
        }
        private void SDForm_Move(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized & !IsLeft)
                AlignTheForm();
        }
        public virtual void SDForm_MouseMove(object sender, EventArgs e)
        {
            var NewMousePosition = MousePosition;
            var NewWindowPosition = default(Point);
            if (IsLeft)
            {
                NewWindowPosition.X = NewMousePosition.X - MouseDownPosition.X;
                NewWindowPosition.Y = NewMousePosition.Y - MouseDownPosition.Y;
                Location = NewWindowPosition;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Grafx.Render(e.Graphics);
        }
        private void AlignTheForm()
        {
            var NewLocation = default(Point);
            NewLocation.X = (int)Math.Round(Location.X / (double)GridSnap) * GridSnap;
            NewLocation.Y = (int)Math.Round(Location.Y / (double)GridSnap) * GridSnap;
            Location = NewLocation;
        }
        internal string NewCustomFormat(double sent)
        {
            // Dim TempFormat As String
            switch (sent)
            {
                case var @case when @case >= 100d:
                    {
                        // TempFormat = "0"
                        return sent.ToString("0");
                    }
                case var case1 when case1 >= 10d:
                    {
                        // TempFormat = "0.0"
                        return sent.ToString("0.0");
                    }
                case var case2 when case2 >= 1d:
                    {
                        // TempFormat = "0.00"
                        return sent.ToString("0.00");
                    }
                case var case3 when case3 >= 0.1d:
                    {
                        // TempFormat = "0.000"
                        return sent.ToString("0.000");
                    }
                case var case4 when case4 >= 0.01d:
                    {
                        // TempFormat = "0.0000"
                        return sent.ToString("0.0000");
                    }

                default:
                    {
                        // TempFormat = "0"
                        return sent.ToString("0");
                    }
            }
        }
    }
}