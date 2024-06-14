#define QueryPerformance

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
// SimpleDyno
// Damian Cunningham 2010 - 2014
// Volker Veidt 2024 - 
using System.IO;
using System.IO.Ports;
//using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SimpleDyno
{
    public partial class SimpleDyno : Form
    {
        #region Compiler Constants
        // These constants are used to control how the app is compiled
        // Triggers performance monitoring

#if QueryPerformance
        private long StartWatch;
        private long StopWatch;
        private long WatchTickConversion;
        private bool A;
        private const int P_FREQ = 0;
        private const int P_TIME = 1;
        private double[,] PerformanceData = new double[3, 201]; // hold frequency value and performance values
        private int PerfBufferCount;
#endif
        #endregion
        #region API structures
        // Structures Required by winmm.dll
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WAVEFORMATEX
        {
            public short wFormatTag;
            public short nchannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public short nBlockAlign;
            public short wBitsPerSample;
            public short cbSize;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WAVEHDR
        {
            public IntPtr lpData;
            public int dwBufferLength;
            public int dwBytesRecorded;
            public IntPtr dwUser;
            public int dwFlags;
            public int dwLoops;
            public IntPtr lpNext;
            public IntPtr reserved;
        }
        static SimpleDyno()
        {
            // For new graphical interface
            f = new List<SimpleDynoSubForm>();
        }
        #endregion
        #region  Windows Form Designer generated code 
        public SimpleDyno() : base()
        {
            A = QueryPerformanceFrequency(out WatchTickConversion);
            myCallBackFunction = new WaveCallBackProcedure(MyWaveCallBackProcedure);
            focusStopwatch = Stopwatch.StartNew();
            serialSimuThread = new Thread(new ThreadStart(serialSimuFunc));

            InitializeComponent();

            //Program.MainI.Load += (object sender, EventArgs e) => SimpleDyno_Load(sender, e);
            //Program.MainI.Closed += (object sender, FormClosedEventArgs e) => SimpleDyno_FormClosed(sender, e);
            //Program.MainI.Activated += (object sender, EventArgs e) => SimpleDyno_Activated(sender, e);
            //Program.MainI.Shown += (object sender, EventArgs e) => SimpleDyno_Shown(sender, e);
            //This call is required by the Windows Form Designer.
        }
        #endregion
        #region API declarations
        // Function declarations for winmm.dll and kernel32.dll
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInOpen(ref IntPtr lphWaveIn, IntPtr uDeviceID, ref WAVEFORMATEX lpFormat, WaveCallBackProcedure dwCallback, IntPtr dwInstance, int dwFlags);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInClose(IntPtr hWaveIn);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInReset(IntPtr hWaveIn);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInStart(IntPtr hWaveIn);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInStop(IntPtr hWaveIn);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInAddBuffer(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInPrepareHeader(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int waveInUnprepareHeader(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);
        // QueryPerformanceCounter and  QueryPerformanceFrequency are used for performance testing only
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);
        #endregion
        #region Constants
        // General constants
        public const string SDVersion = "6_5"; // Version Information
        private const double Phi = 1.61803398875d; // Golden Ratio
        internal const double BitsToVoltage = 1024d / 5d;   // 10 bit voltage conversion 
        internal const int MAXDATAPOINTS = 50000; // Maximum Power Run and Analysis Data Points
        internal const int MAXDATAFILES = 3; // Maximum number of Analysis files

        // These are standard winmm and waveinput constants 
        private const int WAVE_FORMAT_PCM = 1;
        private const int WAVE_MAPPER = -1;
        private const int CALLBACK_FUNCTION = 0x30000;
        private const int WIM_DATA = 0x3C0;
        private const int BITS_PER_SAMPLE = 8;
        private const int NUMBER_OF_BUFFERS = 20;

        // Constants for new data structure approach
        // Primary Dimensions for DATA
        internal const int SESSIONTIME = 0;
        internal const int RPM1_ROLLER = 1;
        internal const int RPM1_WHEEL = 2;
        internal const int RPM1_MOTOR = 3;
        internal const int CHAN1_FREQUENCY = 4;
        internal const int CHAN1_PULSEWIDTH = 5;
        internal const int CHAN1_DUTYCYCLE = 6;
        internal const int SPEED = 7;
        internal const int RPM2 = 8;
        internal const int RPM2_RATIO = 9;
        internal const int RPM2_ROLLOUT = 10;
        internal const int CHAN2_FREQUENCY = 11;
        internal const int CHAN2_PULSEWIDTH = 12;
        internal const int CHAN2_DUTYCYCLE = 13;
        internal const int TORQUE_ROLLER = 14;
        internal const int TORQUE_WHEEL = 15;
        internal const int TORQUE_MOTOR = 16;
        internal const int TORQUE_COASTDOWN = 17;
        internal const int CORRECTED_TORQUE_ROLLER = 18;
        internal const int CORRECTED_TORQUE_WHEEL = 19;
        internal const int CORRECTED_TORQUE_MOTOR = 20;
        internal const int POWER = 21;
        internal const int POWER_COASTDOWN = 22;
        internal const int CORRECTED_POWER_ROLLER = 23;
        internal const int CORRECTED_POWER_WHEEL = 24;
        internal const int CORRECTED_POWER_MOTOR = 25;
        internal const int DRAG = 26;
        internal const int VOLTS = 27;
        internal const int AMPS = 28;
        internal const int WATTS_IN = 29;
        internal const int EFFICIENCY = 30;
        internal const int CORRECTED_EFFICIENCY = 31;
        internal const int TEMPERATURE1 = 32;
        internal const int TEMPERATURE2 = 33;
        internal const int PIN04VALUE = 34;
        internal const int PIN05VALUE = 35;

        // CHECK INTEGERS FOR QUERY PERFORMANCE IF WE LEAVE RUNDOWN IN
#if QueryPerformance
        internal const int PERFORMANCE = 36;
        internal const int LAST = 37;
#else
        
        internal const int LAST = 36; // CHECK - PRE RUNDOWN THIS WAS 27
#endif
        // Secondary Dimensions for DATA
        private const int MINIMUM = 0;
        private const int ACTUAL = 1;
        private const int MAXIMUM = 2;
        private const int MINCURMAXPOINTER = 3;

        #endregion
        #region SimpleDyno Function Declarations
        // Wave call back function declaration
        public delegate int WaveCallBackProcedure(IntPtr hwi, int uMsg, int dwInstance, int dwParam1, int dwParam2);
        private WaveCallBackProcedure myCallBackFunction; // use with callback

        // Custom Rounding and Formatting Functions
        internal double CustomRound(double Sent)
        {
            double CustomRoundRet = default;
            // This is not particularly fast, but it is not used often?
            double TenCount = 1d;
            if (Sent > 0d)
            {
                Sent = Sent / 5d;
                while (Sent <= 1d)
                {
                    Sent = Sent * 10d;
                    TenCount = TenCount / 10d;
                }
                if (Conversion.Int(Sent) > Sent)
                {
                    Sent = Conversion.Int(Sent) * TenCount * 5d;
                }
                else
                {
                    Sent = Conversion.Int(Sent + 1d) * TenCount * 5d;
                }
            }
            CustomRoundRet = Sent;
            return CustomRoundRet;
        }

        // Formatting function for numbers and significant digits presented
        internal string NewCustomFormat(double sent)
        {
            string NewCustomFormatRet = default;
            string TempFormat;
            switch (sent)
            {
                case var @case when @case >= 100d:
                    {
                        TempFormat = "0";
                        break;
                    }
                case var case1 when case1 >= 10d:
                    {
                        TempFormat = "0.0";
                        break;
                    }
                case var case2 when case2 >= 1d:
                    {
                        TempFormat = "0.00";
                        break;
                    }
                case var case3 when case3 >= 0.1d:
                    {
                        TempFormat = "0.000";
                        break;
                    }
                case var case4 when case4 >= 0.001d: // 0.01
                    {
                        TempFormat = "0.0000";
                        break;
                    }

                default:
                    {
                        TempFormat = "0";
                        break;
                    }
            }
            NewCustomFormatRet = sent.ToString(TempFormat);
            return NewCustomFormatRet;
        }
        #endregion
        #region SimpleDyno Definitions 
        // Version Strings
        public static string MainTitle = "SimpleDyno HS CS" + SDVersion + " by DamoRC / LNi / JAh / vmx17";
        public static string PowerRunVersion = "POWER_RUN_" + SDVersion;
        public static string LogRawVersion = "LOG_RAW_" + SDVersion;
        public static string InterfaceVersion = "SimpleDyno_Interface_" + SDVersion;

        // Sub Forms
        private static Dyno s_frmDyno;
        public static Dyno FrmDyno
        {
            get { return s_frmDyno; }
            set { s_frmDyno = value; }
        }
        private static COM s_frmCOM;
        public static COM FrmCom
        {
            get { return s_frmCOM; }
            set { s_frmCOM = value; }
        }
        private static Analysis s_frmAnalysis;
        public static Analysis FrmAnalysis
        {
            get { return s_frmAnalysis; }
            set { s_frmAnalysis = value; }
        }
        private static Fit s_frmFit;
        public static Fit FrmFit
        {
            get { return s_frmFit; }
            set { s_frmFit = value; }
        }
        private static Correction s_frmCorrection;
        public static Correction FrmCorrection
        {
            get { return s_frmCorrection; }
            set { s_frmCorrection = value; }
        }

        private double m_tempDouble; // Global String Temporary Variable for Double Conversion

        // Serial Port Coms and measures
        private SerialPort m_serialPort = new SerialPort();
        private bool m_COMPortsAvailable = false;
        public static string[] COMPortMessage;
        public static double Voltage1;
        public static double Voltage2;
        public static double A0Voltage1;
        public static double A0Voltage2;
        public static double VoltageSlope;
        public static double VoltageIntercept;
        public static double Current1;
        public static double Current2;
        public static double A1Voltage1;
        public static double A1Voltage2;
        public static double CurrentSlope;
        public static double CurrentIntercept;
        public static double Temp1Temperature1;
        public static double Temp1Temperature2;
        public static double A2Voltage1;
        public static double A2Voltage2;
        public static double Temperature1Slope;
        public static double Temperature1Intercept;
        public static double Temp2Temperature1;
        public static double Temp2Temperature2;
        public static double A3Voltage1;
        public static double A3Voltage2;
        public static double Temperature2Slope;
        public static double Temperature2Intercept;
        public static double A4Value1;
        public static double A4Value2;
        public static double A4Voltage1;
        public static double A4Voltage2;
        public static double A4ValueSlope;
        public static double A4ValueIntercept;
        public static double A5Value1;
        public static double A5Value2;
        public static double A5Voltage1;
        public static double A5Voltage2;
        public static double A5ValueSlope;
        public static double A5ValueIntercept;
        public static double Resistance1;
        public static double Resistance2;
        public static bool A5PowerRunControl;
        private static List<SimpleDynoSubForm> _f;

        //public static virtual List<SimpleDynoSubForm> f
        public static List<SimpleDynoSubForm> f
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _f;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _f = value;
            }
        }

        // NOTE - The following delclarations use the largest primary and secondary dimensions
        // Friend" declarations are to allow passing the information to the new graphical interface classes
        public static double[,] Data = new double[37, 4];
        public static string[] DataTags = new string[37];
        public static double[,] DataUnits = new double[37, 6]; // allows for 5 different units for each Data value
        public static string[] DataUnitTags = new string[37]; // labels for the Various units
        public static bool[] DataAreUsed = new bool[37];

        // Use the new Data Structure approach for the collected data from power runs
        public static double[,] CollectedData = new double[37, 50001];

        // General
        private int i;
        private int j;
        private int k;
        private int k2;
        private string AllowedCharacters = "0123456789" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        private string[] AvailableChannels = new[] { "1 Channel", "2 Channels" };
        private string[] AvailableSampleRates = new[] { "11025 Hertz", "22050 Hertz", "44100 Hertz" };
        private string[] AvailableBufferSizes = new[] { "256 bytes", "512 bytes", "1024 bytes", "2048 bytes", "4096 bytes" };

        private string[] AcquisitionOptions = new[] { "Audio Only", "Audio & COM Port Sensing", "COM Port Only" };

        public static bool Formloaded = false;
        public static bool RunSimulation = false;

        // Wave Specific
        public static int SAMPLE_RATE;
        public static int NUMBER_OF_CHANNELS;
        private int BUFFER_SIZE;
        private double BytesToSeconds;
        private double ElapsedTimeUnit;
        private GCHandle bufferpin0;
        private GCHandle bufferpin1;
        private GCHandle bufferpin2;
        private GCHandle bufferpin3;
        private GCHandle bufferpin4;
        private GCHandle bufferpin5;
        private GCHandle bufferpin6;
        private GCHandle bufferpin7;
        private GCHandle bufferpin8;
        private GCHandle bufferpin9;
        private GCHandle bufferpin10;
        private GCHandle bufferpin11;
        private GCHandle bufferpin12;
        private GCHandle bufferpin13;
        private GCHandle bufferpin14;
        private GCHandle bufferpin15;
        private GCHandle bufferpin16;
        private GCHandle bufferpin17;
        private GCHandle bufferpin18;
        private GCHandle bufferpin19;
        private byte[] RawWaveData;
        private byte[] RawWaveData0;
        private byte[] RawWaveData1;
        private byte[] RawWaveData2;
        private byte[] RawWaveData3;
        private byte[] RawWaveData4;
        private byte[] RawWaveData5;
        private byte[] RawWaveData6;
        private byte[] RawWaveData7;
        private byte[] RawWaveData8;
        private byte[] RawWaveData9;
        private byte[] RawWaveData10;
        private byte[] RawWaveData11;
        private byte[] RawWaveData12;
        private byte[] RawWaveData13;
        private byte[] RawWaveData14;
        private byte[] RawWaveData15;
        private byte[] RawWaveData16;
        private byte[] RawWaveData17;
        private byte[] RawWaveData18;
        private byte[] RawWaveData19;
        private WAVEFORMATEX waveFormat;
        private WAVEHDR[] WaveBufferHeaders;
        private IntPtr WaveInHandle;
        private int BufferCount = 0;
        private bool WavesStarted = false;
        private double WaitForNewSignal;
        private bool InCallBackProcedure = false;
        private bool UseAdvancedProcessing = false;


        // Signal Plotting Specific
        private bool SignalPinned = false;
        private Graphics SignalWindowBMP;
        public Bitmap SignalBitmap;
        private Pen Channel1SignalPen = new Pen(Color.Red);
        private Pen Channel2SignalPen = new Pen(Color.Yellow);
        private Pen Channel1ThresholdPen = new Pen(Color.Green);
        private Pen Channel2ThresholdPen = new Pen(Color.Blue);
        private int LastYPosition;
        private int NextYPosition;
        private int LastYPosition2;
        private int NextYPosition2;
        private int PicSignalHeight;
        private int PicSignalWidth;
        private double SignalXConversion;
        private double SignalYConversion;
        private int SignalThresholdYConverted;
        private int SignalThreshold2YConverted;
        private double CurrentSignalXPosition = 0d;

        // Data Collection Mode
        public static int WhichDataMode;
        public const int LIVE = 0;
        public const int LOGRAW = 1;
        public const int POWERRUN = 2;

        // Dyno Specific Variables that need to be declared here for performance reasons
        public static double GearRatio;
        public static double WheelCircumference;

        // High and Low Signal Thresholds, Channels 1 and 2
        public static double HighSignalThreshold;
        public static double HighSignalThreshold2;

        // Dyno Calculations Specific
        public static double DynoMomentOfInertia;
        public static double IdealMomentOfInertia;
        public static double IdealRollerMass;
        public static double ForceAir;
        public static double RollerRPMtoSpeed;
        public static double RollerRadsPerSecToMetersPerSec;
        public static double RollerRPMtoWheelRPM;
        public static double RollerRPMtoMotorRPM;
        public static bool FoundHighSignal2 = false;
        public static bool FoundHighSignal = false;
        public static int LastHighBufferPosition2;
        public static int LastHighBufferPosition;
        public static double ElapsedTime2;
        public static double TempTorqueMax;
        public static double OldAngularVelocity;
        public static double ElapsedTime;
        public static int LastSignal;
        public static double NewElapsedTimeCorrection;
        public static double OldElapsedTimeCorrection;
        public static int LastSignal2;
        public static double NewElapsedTimeCorrection2;
        public static double OldElapsedTimeCorrection2;
        public static double ElapsedTimeToRadPerSec;
        public static double ElapsedTimeToRadPerSec2;
        public static double TotalElapsedTime;
        public static double TotalElapsedTime2;
        private int WhichSignal;
        private int WhichSignal2;
        private const int HIGHSIGNAL = 0;
        private const int LOWSIGNAL = 1;
        public static double SpeedAir;

        // File Handling Specific
        private string m_settingsDirectory;
        private string m_settingsFile;
        private string m_defaultViewFile;// = @"\DefaultView.sdi";
        private StreamWriter DataOutputFile;
        // Private DataInputFile As StreamReader 'In Main, this is used to load other peoples raw data when GearRatio is 999
        private string LogRawDataFileName;
        public static string LogPowerRunDataFileName;
        // Private ParameterInputFile As StreamReader
        // Private ParameterOutputFile As StreamWriter

        // Raw data and curve fitting specific
        public static bool StopFitting = false;
        public static bool ProcessingData = false;
        public static int DataPoints;
        public static double PowerRunThreshold; // = 1
        private double ActualPowerRunThreshold;
        private double MinimumPowerRunPoints;
        private bool StopAddingBuffers = false;

        // Time measurement for limiting subform activation rate
        private Stopwatch focusStopwatch = Stopwatch.StartNew();

        #endregion
        #region Form Load, WndProc, Button and Trackbar Events, Delgates and Close
        private void SimpleDyno_Load(object sender, EventArgs e)
        {
#if QueryPerformance
            btnPerformanceTest.Visible = true;
            cmbBufferSize.Visible = true;
#else

            btnPerformanceTest.Visible = false;
            cmbBufferSize.Visible = false;
#endif
            // Create Instances of the sub forms
            s_frmDyno = new Dyno();
            s_frmCOM = new COM();
            s_frmAnalysis = new Analysis();
            s_frmFit = new Fit();
            s_frmCorrection = new Correction();
            // get folders
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            m_settingsDirectory = Path.Combine(appDataPath, "SimpleDyno");
            if (!Directory.Exists(m_settingsDirectory))
            {
                Directory.CreateDirectory(m_settingsDirectory);
            }
            m_defaultViewFile = Path.Combine(m_settingsDirectory, "DefaultView.sdi");

            // Populate combo boxes
            cmbChannels.Items.AddRange(AvailableChannels);
            cmbChannels.SelectedIndex = 0;
            cmbSampleRate.Items.AddRange(AvailableSampleRates);
            cmbSampleRate.SelectedIndex = 0;
            cmbBufferSize.Items.AddRange(AvailableBufferSizes);
            cmbBufferSize.SelectedIndex = 0;

            // Setup the arrays for the data structure
            PrepareArrays();

            // Check and load available COM Ports
            GetAvailableCOMPorts();

            if (m_COMPortsAvailable == true)
            {
                cmbAcquisition.Items.AddRange(AcquisitionOptions);
            }
            else
            {
                cmbAcquisition.Items.Add(AcquisitionOptions[0]);
            }
            cmbAcquisition.SelectedIndex = 0;

            s_frmAnalysis.Analysis_Setup();
            s_frmFit.Fit_Setup();

            // Load saved setting
            LoadParametersFromFile();

            s_frmDyno.Dyno_Setup();
            s_frmCOM.COM_Setup();

            // Setup graphics data
            PrepareGraphicsParameters();

            SetupTextBoxCharacterHandling();

            // Set Size and Title
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height; // if you  have more than two display ...
            Text = MainTitle;

            // Open Up the default interface
            LoadInterface();

            // Uncomment to enable serial simulation
            RunSimulation = true;
            serialSimuThread.Start();
        }
        private void SimpleDyno_Shown(object sender, EventArgs e)
        {
            Formloaded = true;
        }
        private void SimpleDyno_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveParametersToFile();
            btnClose_Click(this, EventArgs.Empty);
            ShutDownWaves();
            myCallBackFunction = null;
            SerialClose();
            RunSimulation = false;
            serialSimuThread.Join();
        }
        private void btnDyno_Click(object sender, EventArgs e)
        {
            btnHide_Click(this, EventArgs.Empty);
            s_frmDyno.ShowDialog();
        }
        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            btnHide_Click(this, EventArgs.Empty);
            s_frmAnalysis.ShowDialog();
            s_frmAnalysis.pnlOverlaySetup();
        }
        private void btnCOM_Click(object sender, EventArgs e)
        {
            btnHide_Click(this, EventArgs.Empty);
            s_frmCOM.ShowDialog();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            btnHide_Click(this, EventArgs.Empty);
            s_frmCorrection.ShowDialog();
        }
        private void pnlSignalWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // set the RPM2 channel threshold
                txtThreshold2.Text = (256d - pnlSignalWindow.PointToClient(MousePosition).Y / (double)pnlSignalWindow.Height * 256d).ToString();
                HighSignalThreshold2 = Conversions.ToDouble(txtThreshold2.Text); // 256 - (pnlSignalWindow.PointToClient(Control.MousePosition).Y) / pnlSignalWindow.Height * 256
                if (HighSignalThreshold2 > 128d)
                    WhichSignal2 = HIGHSIGNAL;
                else
                    WhichSignal2 = LOWSIGNAL;
                SignalThreshold2YConverted = (int)Math.Round(PicSignalHeight - HighSignalThreshold2 * SignalYConversion);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (pnlSignalWindow.Right - pnlSignalWindow.PointToClient(MousePosition).X > 10)
                {
                    // set the RPM1 channel threshold
                    txtThreshold1.Text = (256d - pnlSignalWindow.PointToClient(MousePosition).Y / (double)pnlSignalWindow.Height * 256d).ToString();
                    HighSignalThreshold = Conversions.ToDouble(txtThreshold1.Text); // 256 - (pnlSignalWindow.PointToClient(Control.MousePosition).Y) / pnlSignalWindow.Height * 256
                    if (HighSignalThreshold > 128d)
                        WhichSignal = HIGHSIGNAL;
                    else
                        WhichSignal = LOWSIGNAL;
                    SignalThresholdYConverted = (int)Math.Round(PicSignalHeight - HighSignalThreshold * SignalYConversion);
                }
                else
                {
                    SignalPinned = !SignalPinned;
                }

            }
        }
        private void pnlSignalWindow_MouseEnter(object sender, EventArgs e)
        {
            if (WavesStarted)
            {
                {
                    var withBlock = pnlSignalWindow;
                    withBlock.Left = withBlock.Right - Width + 10;
                    withBlock.Width = Width - 10;
                }
                PrepareGraphicsParameters();
            }
        }
        private void pnlSignalWindow_MouseLeave(object sender, EventArgs e)
        {
            // Debug.Print(SignalPinned.ToString)
            if (WavesStarted)
            {
                if (SignalPinned == false)
                {
                    {
                        var withBlock = pnlSignalWindow;
                        withBlock.Left = withBlock.Right - 25;
                        withBlock.Width = 25;
                    }
                    PrepareGraphicsParameters();
                }
            }
        }

        private void pnlSignalWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (pnlSignalWindow.Right - pnlSignalWindow.PointToClient(MousePosition).X < 10)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }
        private void pnlSignalWindow_Paint_1(object sender, PaintEventArgs e)
        {
            pnlSignalWindow.BackgroundImage = SignalBitmap;
        }

        public delegate void SetControlBackColor_Delegate(Control Control, Color Color);
        private void SetControlBackColor_ThreadSafe(Control Control, Color Color)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetControlBackColor_Delegate(SetControlBackColor_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control, Color });
            }
            else
            {
                Control.BackColor = Color;
            }
        }
        public delegate void SetControlEnabled_Delegate(Control Control);
        internal void SetControlEnabled_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetControlEnabled_Delegate(SetControlEnabled_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Enabled = true;
            }
        }
        public delegate void SetMouseBusy_Delegate(Control Control);
        internal void SetMouseBusy_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetMouseBusy_Delegate(SetMouseBusy_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Cursor = Cursors.WaitCursor;
            }
        }
        public delegate void SetMouseNormal_Delegate(Control Control);
        internal void SetMouseNormal_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetMouseNormal_Delegate(SetMouseNormal_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Cursor = Cursors.Arrow;
            }
        }
        public delegate void SetControlDisabled_Delegate(Control Control);
        internal void SetControlDisabled_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetControlDisabled_Delegate(SetControlDisabled_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Enabled = false;
            }
        }
        public delegate void PicRefresh_Delegate(PictureBox Picture);
        internal void PicRefresh_ThreadSafe(PictureBox Picture)
        {
            if (Picture.InvokeRequired)
            {
                var MyDelegate = new PicRefresh_Delegate(PicRefresh_ThreadSafe);
                Invoke(MyDelegate, new object[] { Picture });
            }
            else
            {
                Picture.Refresh();
            }
        }
        public delegate void ControlRefresh_Delegate(Control Control);
        internal void ControlRefresh_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new ControlRefresh_Delegate(ControlRefresh_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Refresh();
            }
        }
        public delegate void ControlUpdate_Delegate(Control Control);
        internal void ControlUpdate_ThreadSafe(Control Control)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new ControlUpdate_Delegate(ControlUpdate_ThreadSafe);
                Invoke(MyDelegate, new object[] { Control });
            }
            else
            {
                Control.Update();
            }
        }
        public delegate void SetControlText_Delegate(Control Control, string text);
        public void SetControlText_Threadsafe(Control Control, string text)
        {
            if (Control.InvokeRequired)
            {
                var MyDelegate = new SetControlText_Delegate(SetControlText_Threadsafe);
                Invoke(MyDelegate, new object[] { Control, text });
            }
            else
            {
                Control.Text = text;
            }
        }
        public delegate void ClickButtonThreadsafe_Delegate(Button Button, object sender, EventArgs Args);
        internal void ClickButton_Threadsafe(Button Button, object sender, EventArgs Args)
        {
            if (Button.InvokeRequired)
            {
                var MyDelegate = new ClickButtonThreadsafe_Delegate(ClickButton_Threadsafe);
                Invoke(MyDelegate, new object[] { Button, Text, sender, Args });
            }
            else
            {

            }
        }
        private void chkAdvancedProcessing_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAdvancedProcessing.Checked == true)
            {
                UseAdvancedProcessing = true;
            }
            else
            {
                UseAdvancedProcessing = false;
            }
        }
        internal bool CheckNumericalLimits(double SentMin, double SentMax, double SentValue)
        {
            if (SentValue >= SentMin && SentValue <= SentMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void btnStartPowerRun_Click(object sender, EventArgs e)
        {
            TogglePowerRun();
        }
        private void btnResetMaxima_Click(object sender, EventArgs e)
        {
            ResetValues();
        }
        private void btnStartLoggingRaw_Click(object sender, EventArgs e)
        {
            // Copied power run code
            try
            {
                if (WhichDataMode == LOGRAW) // We are stopping the log raw session and should write the data
                {
                    // WRITE THE DATA
                    var DataOutputFile = new StreamWriter(LogRawDataFileName);
                    // NOTE: The data files are space delimited
                    // Write out the header information
                    DataOutputFile.WriteLine(LogRawVersion); // Confirms log raw version
                    DataOutputFile.WriteLine(LogRawDataFileName + Constants.vbCrLf + DateTime.Today.ToString() + Constants.vbCrLf);
                    DataOutputFile.WriteLine("Acquisition: " + cmbAcquisition.SelectedItem.ToString());
                    DataOutputFile.WriteLine("Number_of_Channels: " + NUMBER_OF_CHANNELS.ToString());
                    DataOutputFile.WriteLine("Sampling_Rate " + SAMPLE_RATE.ToString());
                    if (cmbCOMPorts.SelectedItem is not null)
                    {
                        DataOutputFile.WriteLine("COM_Port: " + cmbCOMPorts.SelectedItem.ToString());
                    }
                    else
                    {
                        DataOutputFile.WriteLine("No_COM_Port_Selected");
                    }
                    if (cmbBaudRate.SelectedItem is not null)
                    {
                        DataOutputFile.WriteLine("Baud_Rate: " + cmbBaudRate.SelectedItem.ToString());
                    }
                    else
                    {
                        DataOutputFile.WriteLine("No_Baud_Rate_Selected");
                    }
                    DataOutputFile.WriteLine("Car_Mass: " + s_frmDyno.CarMass.ToString() + " grams");
                    DataOutputFile.WriteLine("Frontal_Area: " + s_frmDyno.FrontalArea.ToString() + " mm2");
                    DataOutputFile.WriteLine("Drag_Coefficient: " + s_frmDyno.DragCoefficient.ToString());
                    DataOutputFile.WriteLine("Gear_Ratio: " + GearRatio.ToString());
                    DataOutputFile.WriteLine("Wheel_Diameter: " + s_frmDyno.WheelDiameter.ToString() + " mm");
                    DataOutputFile.WriteLine("Roller_Diameter: " + s_frmDyno.RollerDiameter.ToString() + " mm");
                    DataOutputFile.WriteLine("Roller_Wall_Thickness: " + s_frmDyno.RollerWallThickness.ToString() + " mm");
                    DataOutputFile.WriteLine("Roller_Mass: " + s_frmDyno.RollerMass.ToString() + " grams");
                    DataOutputFile.WriteLine("Axle_Diameter: " + s_frmDyno.AxleDiameter.ToString() + " mm");
                    DataOutputFile.WriteLine("Axle_Mass: " + s_frmDyno.AxleMass.ToString() + " grams");
                    DataOutputFile.WriteLine("End_Cap_Mass: " + s_frmDyno.EndCapMass.ToString() + " grams");
                    DataOutputFile.WriteLine("Extra_Diameter: " + s_frmDyno.ExtraDiameter.ToString() + " mm");
                    DataOutputFile.WriteLine("Extra_Wall_Thickness: " + s_frmDyno.ExtraWallThickness.ToString() + " mm");
                    DataOutputFile.WriteLine("Extra_Mass: " + s_frmDyno.ExtraMass.ToString() + " grams");
                    DataOutputFile.WriteLine("Target_MOI: " + IdealMomentOfInertia.ToString() + " kg/m2");
                    DataOutputFile.WriteLine("Actual_MOI: " + DynoMomentOfInertia.ToString() + " kg/m2");
                    DataOutputFile.WriteLine("Target_Roller_Mass: " + IdealRollerMass.ToString() + " grams");
                    DataOutputFile.WriteLine("Signals_Per_RPM1: " + s_frmDyno.SignalsPerRPM.ToString());
                    DataOutputFile.WriteLine("Signals_Per_RPM2: " + s_frmDyno.SignalsPerRPM2.ToString());
                    DataOutputFile.WriteLine("Channel_1_Threshold " + HighSignalThreshold.ToString());
                    DataOutputFile.WriteLine("Channel_2_Threshold " + HighSignalThreshold2.ToString());
                    // The following not needed for Log Raw
                    // .WriteLine("Run_RPM_Threshold " & PowerRunThreshold.ToString)
                    // .WriteLine("Run_Spike_Removal_Threshold " & Fit.PowerRunSpikeLevel.ToString)
                    DataOutputFile.WriteLine(Constants.vbCrLf);

                    // Create the column headings string based on the Data structure 
                    // Only Primary SI units of the values are written
                    string tempstring = "";
                    string[] tempsplit;
                    int paramcount;
                    int count;

                    // Add the raw data.  In V6 we are also calculating the raw torques, powers etc. This makes the file larger but will make Excel work easier
                    DataOutputFile.WriteLine(Constants.vbCrLf + "PRIMARY_CHANNEL_RAW_DATA");
                    DataOutputFile.WriteLine("NUMBER_OF_POINTS_COLLECTED" + " " + DataPoints.ToString());
                    // Again, create the header row
                    tempstring = "";
                    for (paramcount = 0; paramcount <= LAST - 1; paramcount++)
                    {
                        tempsplit = Strings.Split(DataUnitTags[paramcount], " ");
                        tempstring = tempstring + DataTags[paramcount].Replace(" ", "_") + "(" + tempsplit[0] + ") ";
                    }
                    // Write the column headings
                    DataOutputFile.WriteLine(tempstring);
                    // Need to set the zeroth value to support using the count and count-1 approach to torque and power calculations
                    CollectedData[RPM1_ROLLER, 0] = CollectedData[RPM1_ROLLER, 1];
                    var loopTo = DataPoints - 1;
                    for (count = 1; count <= loopTo; count++)
                    {
                        // re-calc speed, wheel and motor RPMs based on collected data
                        CollectedData[SPEED, count] = CollectedData[RPM1_ROLLER, count] * RollerRadsPerSecToMetersPerSec;
                        CollectedData[RPM1_WHEEL, count] = CollectedData[RPM1_ROLLER, count] * RollerRPMtoWheelRPM;
                        CollectedData[RPM1_MOTOR, count] = CollectedData[RPM1_ROLLER, count] * RollerRPMtoMotorRPM;
                        // re-calc roller torque and power useing the collected data
                        CollectedData[TORQUE_ROLLER, count] = (CollectedData[RPM1_ROLLER, count] - CollectedData[RPM1_ROLLER, count - 1]) / (CollectedData[SESSIONTIME, count] - CollectedData[SESSIONTIME, count - 1]) * DynoMomentOfInertia; // this is the roller torque, should calc the wheel and motor at this point also
                        // NOTE - new power calculation uses (new-old) / 2
                        CollectedData[POWER, count] = CollectedData[TORQUE_ROLLER, count] * ((CollectedData[RPM1_ROLLER, count] + CollectedData[RPM1_ROLLER, count - 1]) / 2d);
                        // now re-calc wheel and motor torque based on Power
                        CollectedData[TORQUE_WHEEL, count] = CollectedData[POWER, count] / CollectedData[RPM1_WHEEL, count];
                        CollectedData[TORQUE_MOTOR, count] = CollectedData[POWER, count] / CollectedData[RPM1_MOTOR, count];
                        // recalc Drag and set a max speed based on it
                        CollectedData[DRAG, count] = Math.Pow(CollectedData[SPEED, count], 3d) * ForceAir;
                        // Update other parameters requiring calculations
                        // Main.RPM2 will be already there but the ratio and rollout need to be calculated
                        if (CollectedData[RPM2, count] != 0d)
                        {
                            CollectedData[RPM2_RATIO, count] = CollectedData[RPM2, count] / CollectedData[RPM1_WHEEL, count];
                            CollectedData[RPM2_ROLLOUT, count] = WheelCircumference / CollectedData[RPM2_RATIO, count];
                        }
                        else
                        {
                            CollectedData[RPM2_RATIO, count] = 0d;
                            CollectedData[RPM2_ROLLOUT, count] = 0d;
                        }
                        // Volts and Amps will already be there but watts in and efficiency need to be added
                        CollectedData[WATTS_IN, count] = CollectedData[VOLTS, count] * CollectedData[AMPS, count];
                        if (CollectedData[WATTS_IN, count] != 0d)
                        {
                            CollectedData[EFFICIENCY, count] = CollectedData[POWER, count] / CollectedData[WATTS_IN, count] * 100d;
                        }
                        else
                        {
                            CollectedData[EFFICIENCY, count] = 0d;
                        }
                        // Build the results string...
                        tempstring = "";
                        for (paramcount = 0; paramcount <= LAST - 1; paramcount++)
                        {
                            tempsplit = Strings.Split(DataUnitTags[paramcount], " "); // How many units are there
                            tempstring = tempstring + CollectedData[paramcount, count] * DataUnits[paramcount, 0] + " "; // DataTags(paramcount).Replace(" ", "_") & "(" & tempsplit(unitcount) & ") "
                        }
                        // ...and write it
                        DataOutputFile.WriteLine(tempstring);

                    }
                    // Save the file
                    DataOutputFile.Close();
                    btnStartLoggingRaw.Enabled = true;

                    // /////////////////////END COPIED CODE
                    btnStartPowerRun.Enabled = true;
                    btnShow_Click(this, EventArgs.Empty);
                    {
                        var withBlock = btnStartLoggingRaw;
                        withBlock.BackColor = DefaultBackColor;
                    }
                    WhichDataMode = LIVE;
                }
                else
                {
                    btnHide_Click(this, EventArgs.Empty);
                    {
                        var withBlock1 = SaveFileDialog1;
                        withBlock1.Reset();
                        withBlock1.Filter = "Log Raw files (*.sdr)|*.sdr";
                        withBlock1.ShowDialog();
                    }
                    if (!string.IsNullOrEmpty(SaveFileDialog1.FileName))
                    {
                        LogRawDataFileName = SaveFileDialog1.FileName;
                        ResetValues();
                        DataPoints = 0;
                        Data[SESSIONTIME, ACTUAL] = 0d;
                        btnStartPowerRun.Enabled = false;
                        btnShow_Click(this, EventArgs.Empty);
                        {
                            var withBlock2 = btnStartLoggingRaw;
                            withBlock2.BackColor = Color.Red;
                        }
                        WhichDataMode = LOGRAW;
                    }
                    else
                    {
                        btnShow_Click(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception e1)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("btnStartLoggingRaw Error: " + e1.ToString(), MsgBoxStyle.Exclamation);
                btnShow_Click(this, EventArgs.Empty);
                Environment.Exit(0);
            }

        }

        private void SimpleDyno_Activated(object sender, EventArgs e)
        {
            // Activate the subforms to bring them foreground when main form is brought foreground
            // This would be endless loop if not limited with a timer
            if (focusStopwatch.ElapsedMilliseconds > 500L)
            {
                focusStopwatch.Restart();
                foreach (SimpleDynoSubForm SDFrm in f)
                {
                    SDFrm.Activate();
                    // Reactivate main form
                    Focus();
                }
            }
        }
        #endregion
        #region Text Box Checking
        // Assigns single sub to handle text box entries ensuring only allowed chars are entered
        private void SetupTextBoxCharacterHandling()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox & !ReferenceEquals(c, SessionTextBox))
                {
                    c.KeyPress += txtControl_KeyPress;
                }
            }
            foreach (Control c in s_frmDyno.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyPress += txtControl_KeyPress;
                }
            }
            foreach (Control c in s_frmCOM.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyPress += txtControl_KeyPress;
                }
            }
            foreach (Control c in s_frmAnalysis.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyPress += txtControl_KeyPress;
                }
            }
            foreach (Control c in s_frmFit.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyPress += txtControl_KeyPress;
                }
            }
        }
        private void txtControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Strings.Asc(e.KeyChar) != 8) // If not backspace
            {
                if (AllowedCharacters.IndexOf(e.KeyChar) == -1)
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar == '\r')
                SendKeys.Send("{TAB}");
        }
        private void txtInterface_TextChanged(object sender, EventArgs e)
        {
            // This is an invisible box so text validation should not be needed
            lblInterface.Text = txtInterface.Text.Substring(txtInterface.Text.LastIndexOf(@"\") + 1);
        }
        private void txtZeroTimeDetect_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0.1d;
            double LocalMax = 2d;
            if (double.TryParse(((TextBox)sender).Text, out m_tempDouble) && CheckNumericalLimits(LocalMin, LocalMax, m_tempDouble))
            {
                WaitForNewSignal = m_tempDouble;
            }
            else
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                btnShow_Click(this, EventArgs.Empty);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = WaitForNewSignal.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtPowerRunThreshold_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out m_tempDouble) && CheckNumericalLimits(LocalMin, LocalMax, m_tempDouble))
            {
                PowerRunThreshold = m_tempDouble;
                ActualPowerRunThreshold = PowerRunThreshold / 60d * 2d * Math.PI; // convert it to rads/s
                                                                                  // Trying setting the minimum number of points to be collected in here also
                                                                                  // CHECK - this should also be set when Signals per RPM changes
                MinimumPowerRunPoints = s_frmDyno.SignalsPerRPM * 10d; // This somewhat arbitrary 
            }
            else
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                btnShow_Click(this, EventArgs.Empty);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = PowerRunThreshold.ToString();
                    withBlock.Focus();
                }
            }
        }

        #endregion
        #region Read, Write, Update, and Reset Parameters
        private void LoadParametersFromFile()
        {
            try
            {
                string Temp;
                string[] TempSplit;
                double TempIndex;
                int start;
                int finish;
                string TempItem;
                var cmbNew = new ComboBox();
                var txtNew = new TextBox();
                VScrollBar scrlNew;
                CheckBox cbNew;
                if (Directory.Exists(m_settingsDirectory)) // Some Version of SimpleDyno has been run before
                {
                    m_settingsFile = Path.Combine(m_settingsDirectory, "SimpleDynoSettings.sds");
                    string old_path = Path.Combine(m_settingsDirectory, "SimpleDynoSettings_6_3.sds");

                    if (File.Exists(m_settingsFile)) // From 6.4 on Settings files is simply SimpleDynoSettings.sds
                    {
                        var ParameterInputFile = new StreamReader(m_settingsFile);
                        Temp = ParameterInputFile.ReadToEnd();
                        ParameterInputFile.Close();
                        TempSplit = Strings.Split(Temp, Constants.vbCrLf);
                        switch (TempSplit[0] ?? "")
                        {
                            case var @case when @case == MainTitle:
                            case "SimpleDyno 6_4 by DamoRC": // This is the settings file created by the current version so no issues
                            {
                                var SortedControls = new List<Control>();
                                foreach (Control c in Controls)
                                    SortedControls.Add(c);
                                foreach (Control c in s_frmDyno.Controls)
                                    SortedControls.Add(c);
                                foreach (Control c in s_frmCOM.Controls)
                                    SortedControls.Add(c);
                                foreach (Control c in s_frmAnalysis.Controls)
                                    SortedControls.Add(c);
                                foreach (Control c in s_frmFit.Controls)
                                    SortedControls.Add(c);
                                // Need to use sorted list so that cmbData are loaded before cmbUnits
                                SortedControls.Sort(SortControls);
                                foreach (Control c in SortedControls)
                                {
                                    start = Strings.InStr(Temp, c.Name.ToString());
                                    if (start != 0)
                                    {
                                        start += c.Name.Length;
                                        finish = Strings.InStr(start, Temp, Constants.vbCrLf);
                                        // For the combo boxes, 6.3 used index, 6.4 and up use Item text so
                                        if (c is ComboBox)
                                        {
                                            // Need some way of distinguishing versions here
                                            cmbNew = (ComboBox)c;
                                            // TempIndex = CDbl(Temp.Substring(start, finish - start - 1))
                                            TempItem = Temp.Substring(start, finish - start - 1);
                                            int t;
                                            var loopTo = cmbNew.Items.Count - 1;
                                            for (t = 0; t <= loopTo; t++)
                                            {
                                                if ((cmbNew.Items[t].ToString() ?? "") == (TempItem ?? ""))
                                                {
                                                    TempIndex = t;
                                                    cmbNew.SelectedIndex = t;
                                                    c.Select();
                                                }
                                            }

                                            // If TempIndex <= cmbNew.Items.Count - 1 Then
                                            // cmbNew.SelectedIndex = CInt(Temp.Substring(start, finish - start - 1))
                                            // c.Select()
                                            // End If
                                        }
                                        if (c is TextBox)
                                        {
                                            c.Text = Temp.Substring(start, finish - start - 1);
                                            c.Select();
                                        }
                                        if (c is VScrollBar)
                                        {
                                            scrlNew = (VScrollBar)c;
                                            TempIndex = Conversions.ToDouble(Temp.Substring(start, finish - start - 1));
                                            scrlNew.Value = (int)Math.Round(TempIndex);
                                            c.Select();
                                        }
                                        if (c is CheckBox)
                                        {
                                            cbNew = (CheckBox)c;
                                            cbNew.Checked = Temp.Substring(start, finish - start - 1).Equals(bool.TrueString);
                                            c.Select();
                                        }
                                    }
                                }
                                btnStartAcquisition.Select();
                                break;
                            }

                            default:
                            {
                                Interaction.MsgBox("Wrong format in: " + m_settingsFile + " Settings not loaded!", MsgBoxStyle.Exclamation);
                                break;
                            }
                        }
                    }

                    else if (File.Exists(old_path)) // this is a one off for moving from 6.3 forward
                    {
                        var ParameterInputFile = new StreamReader(old_path);
                        Temp = ParameterInputFile.ReadToEnd();
                        ParameterInputFile.Close();
                        TempSplit = Strings.Split(Temp, Constants.vbCrLf);
                        var SortedControls = new List<Control>();
                        foreach (Control c in Controls)
                            SortedControls.Add(c);
                        foreach (Control c in s_frmDyno.Controls)
                            SortedControls.Add(c);
                        foreach (Control c in s_frmCOM.Controls)
                            SortedControls.Add(c);
                        foreach (Control c in s_frmAnalysis.Controls)
                            SortedControls.Add(c);
                        foreach (Control c in s_frmFit.Controls)
                            SortedControls.Add(c);
                        // Need to use sorted list so that cmbData are loaded before cmbUnits
                        SortedControls.Sort(SortControls);
                        foreach (Control c in SortedControls)
                        {
                            start = Strings.InStr(Temp, c.Name.ToString());
                            if (start != 0)
                            {
                                start += c.Name.Length;
                                finish = Strings.InStr(start, Temp, Constants.vbCrLf);
                                // For the combo boxes, 6.3 used index, 6.4 and up use Item text so
                                if (c is ComboBox)
                                {
                                    cmbNew = (ComboBox)c;
                                    // TempItem = Temp.Substring(start, finish - start - 1)
                                    // Dim t As Integer
                                    // For t = 0 To cmbNew.Items.Count - 1
                                    // If cmbNew.Items(t).ToString = TempItem Then
                                    // TempIndex = t
                                    // cmbNew.SelectedIndex = t
                                    // c.Select()
                                    // End If
                                    // Next
                                    TempIndex = Conversions.ToDouble(Temp.Substring(start, finish - start - 1));
                                    if (TempIndex <= cmbNew.Items.Count - 1)
                                    {
                                        cmbNew.SelectedIndex = Conversions.ToInteger(Temp.Substring(start, finish - start - 1));
                                        c.Select();
                                    }
                                }
                                if (c is TextBox)
                                {
                                    c.Text = Temp.Substring(start, finish - start - 1);
                                    c.Select();
                                }
                                if (c is VScrollBar)
                                {
                                    scrlNew = (VScrollBar)c;
                                    TempIndex = Conversions.ToDouble(Temp.Substring(start, finish - start - 1));
                                    scrlNew.Value = (int)Math.Round(TempIndex);
                                    c.Select();
                                }
                            }
                        }
                        btnStartAcquisition.Select();
                    }
                    else
                    {
                        SaveParametersToFile();
                        LoadParametersFromFile();
                        CreateDefaultView();
                    }
                }
                else
                {
                    Directory.CreateDirectory(m_settingsDirectory);
                    SaveParametersToFile();
                    LoadParametersFromFile();
                    CreateDefaultView();
                }
            }
            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("LoadParametersFromFile Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
        }
        private int SortControls(Control a, Control b)
        {
            // This function supports sorting to the controls in LoadParameters
            return a.Name.CompareTo(b.Name);
        }
        private void CreateDefaultView()
        {
            // Default view has 4 SD form controls, 2 labels, a 270 degree gauge and a plot
            int p = 1;
            int g = 2;
            int l1 = 3;
            int l2 = 4;
            var DefaultControls = new Rectangle[5];
            // Going to assume the main form will eventually be 1/phi^3 tall, where phi is golden ratio height
            // For this to work, the width must be 1.170820393 the screen res height
            double ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            double PhiWidth = Screen.PrimaryScreen.WorkingArea.Height - Height;
            // Plot will be sqaure and the largest one
            DefaultControls[p].Height = (int)Math.Round(PhiWidth); // / Phi)
            DefaultControls[p].Width = (int)Math.Round(PhiWidth); // / Phi)
                                                                  // Gauge is next
            DefaultControls[g].Height = (int)Math.Round(PhiWidth / Phi); // ^ 2)
            DefaultControls[g].Width = (int)Math.Round(PhiWidth / Phi); // ^ 2)
                                                                        // Labels are last
            DefaultControls[l1].Height = (int)Math.Round(PhiWidth / Math.Pow(Phi, 2d)); // ^ 3)
            DefaultControls[l1].Width = (int)Math.Round(PhiWidth / Phi / 2d); // ^ 2 / 2)
            DefaultControls[l2].Height = (int)Math.Round(PhiWidth / Math.Pow(Phi, 2d)); // ^ 3)
            DefaultControls[l2].Width = (int)Math.Round(PhiWidth / Phi / 2d); // ^ 2 / 2)
                                                                              // Now set the positions
                                                                              // First, the gauge
            DefaultControls[g].X = (int)Math.Round((ScreenWidth - PhiWidth - PhiWidth / Phi) / 2d);
            DefaultControls[g].Y = 0;
            // Then the labels
            DefaultControls[l1].X = DefaultControls[g].Left;
            DefaultControls[l1].Y = DefaultControls[g].Bottom;
            DefaultControls[l2].X = DefaultControls[l1].Right;
            DefaultControls[l2].Y = DefaultControls[g].Bottom;
            // now the plot
            DefaultControls[p].X = DefaultControls[g].Right;
            DefaultControls[p].Y = DefaultControls[g].Top;
            // set up the string and write it
            string TempString = InterfaceVersion + Constants.vbCrLf;
            string Splitter = "_";
            // Serialized SD Forms are left, top, height, width
            TempString += "Gauge" + Constants.vbCrLf;
            TempString += DefaultControls[g].X + Splitter + DefaultControls[g].Y + Splitter + DefaultControls[g].Height + Splitter + DefaultControls[g].Width + Splitter;
            TempString += "270 deg_1_30_White_Black_0_0_0_0_0_Arial_Green_1_1_1_0_5000_False_" + Constants.vbCrLf;
            TempString += "Label" + Constants.vbCrLf;
            TempString += DefaultControls[l1].X + Splitter + DefaultControls[l1].Y + Splitter + DefaultControls[l1].Height + Splitter + DefaultControls[l1].Width + Splitter;
            TempString += "Vertical_1_300_White_Black_0_0_0_0_0_Arial_Green_1_1_1_0_1000_False_" + Constants.vbCrLf;
            TempString += "Label" + Constants.vbCrLf;
            TempString += DefaultControls[l2].X + Splitter + DefaultControls[l2].Y + Splitter + DefaultControls[l2].Height + Splitter + DefaultControls[l2].Width + Splitter;
            TempString += "Vertical_1_300_White_Black_0_0_0_0_0_Arial_Green_1_2_1_0_1000_False_" + Constants.vbCrLf;
            TempString += "MultiYTimeGraph" + Constants.vbCrLf;
            TempString += DefaultControls[p].X + Splitter + DefaultControls[p].Y + Splitter + DefaultControls[p].Height + Splitter + DefaultControls[p].Width + Splitter;
            TempString += "Lines_4_30_White_Black_0_1_0_0_10_Arial_Green_1_1_1_0_5000_True_Green_0_0_0_0_5000_False_Green_0_0_0_0_5000_False_Green_0_0_0_0_5000_False_";
            var DefaultViewOutputFile = new StreamWriter(m_defaultViewFile);
            // InterfaceName = m_settingsDirectory & m_defaultViewFile
            txtInterface.Text = m_defaultViewFile;
            DefaultViewOutputFile.WriteLine(TempString);
            DefaultViewOutputFile.Close();
        }
        private void SaveParametersToFile()
        {
            try
            {
                ComboBox cmbNew;
                VScrollBar scrlNew;
                CheckBox cbNew;
                var ParameterOutputFile = new StreamWriter(m_settingsFile);
                ParameterOutputFile.WriteLine(MainTitle);

                var SortedControls = new List<Control>();
                foreach (Control c in Controls)
                    SortedControls.Add(c);
                foreach (Control c in s_frmDyno.Controls)
                    SortedControls.Add(c);
                foreach (Control c in s_frmCOM.Controls)
                    SortedControls.Add(c);
                foreach (Control c in s_frmAnalysis.Controls)
                    SortedControls.Add(c);
                foreach (Control c in s_frmFit.Controls)
                    SortedControls.Add(c);

                foreach (Control c in SortedControls)
                {
                    if (c is TextBox)
                    {
                        ParameterOutputFile.WriteLine("[" + c.Name.ToString() + "]" + c.Text);
                    }
                    if (c is ComboBox)
                    {
                        cmbNew = (ComboBox)c;
                        if (cmbNew.SelectedIndex != -1)
                        {
                            ParameterOutputFile.WriteLine("[" + c.Name.ToString() + "]" + cmbNew.SelectedItem.ToString());
                        }
                    }
                    if (c is VScrollBar)
                    {
                        scrlNew = (VScrollBar)c;
                        ParameterOutputFile.WriteLine("[" + c.Name.ToString() + "]" + scrlNew.Value);
                    }
                    if (c is CheckBox)
                    {
                        cbNew = (CheckBox)c;
                        ParameterOutputFile.WriteLine("[" + c.Name.ToString() + "]" + cbNew.Checked.ToString());
                    }
                }

                // For Each c As Control In Me.Controls
                // If TypeOf c Is TextBox Then
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & c.Text)
                // End If
                // If TypeOf c Is ComboBox Then
                // cmbNew = DirectCast(c, ComboBox)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & cmbNew.SelectedItem.ToString)
                // End If
                // Next

                // For Each c As Control In s_frmDyno.Controls
                // If TypeOf c Is TextBox Then
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & c.Text)
                // End If
                // If TypeOf c Is ComboBox Then
                // cmbNew = DirectCast(c, ComboBox)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & cmbNew.SelectedItem.ToString)
                // End If
                // Next

                // For Each c As Control In s_frmCOM.Controls
                // If TypeOf c Is TextBox Then
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & c.Text)
                // End If
                // If TypeOf c Is ComboBox Then
                // cmbNew = DirectCast(c, ComboBox)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & cmbNew.SelectedItem.ToString)
                // End If
                // Next

                // For Each c As Control In s_frmAnalysis.Controls
                // If TypeOf c Is TextBox Then
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & c.Text)
                // End If
                // If TypeOf c Is ComboBox Then
                // cmbNew = DirectCast(c, ComboBox)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & cmbNew.SelectedItem.ToString)
                // End If
                // Next

                // For Each c As Control In s_frmFit.Controls
                // If TypeOf c Is TextBox Then
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & c.Text)
                // End If
                // If TypeOf c Is ComboBox Then
                // cmbNew = DirectCast(c, ComboBox)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & cmbNew.SelectedItem.ToString)
                // End If
                // If TypeOf c Is VScrollBar Then
                // scrlNew = DirectCast(c, VScrollBar)
                // ParameterOutputFile.WriteLine("[" & c.Name.ToString & "]" & scrlNew.Value)
                // End If
                // Next

                ParameterOutputFile.Close();
            }

            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("From SaveParameters Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
        }
        private void PrepareArrays()
        {

            // Set all parameters to be available in interface components
            for (int count = 1; count <= LAST - 1; count++)
                DataAreUsed[count] = true;

            // The following are not available live but only in Analysis 
            DataAreUsed[POWER_COASTDOWN] = false;
            DataAreUsed[TORQUE_COASTDOWN] = false;
            // The following are available live, but never on startup
            DataAreUsed[CORRECTED_EFFICIENCY] = false;
            DataAreUsed[CORRECTED_POWER_MOTOR] = false;
            DataAreUsed[CORRECTED_POWER_ROLLER] = false;
            DataAreUsed[CORRECTED_POWER_WHEEL] = false;
            DataAreUsed[CORRECTED_TORQUE_MOTOR] = false;
            DataAreUsed[CORRECTED_TORQUE_ROLLER] = false;
            DataAreUsed[CORRECTED_TORQUE_WHEEL] = false;

            DataTags[RPM1_ROLLER] = "RPM1 Roller";
            DataUnitTags[RPM1_ROLLER] = "rad/s RPM";
            DataUnits[RPM1_ROLLER, 0] = 1d;
            DataUnits[RPM1_ROLLER, 1] = 60d / (2d * Math.PI);

            DataTags[RPM1_WHEEL] = "RPM1 Wheel";
            DataUnitTags[RPM1_WHEEL] = "rad/s RPM";
            DataUnits[RPM1_WHEEL, 0] = 1d;
            DataUnits[RPM1_WHEEL, 1] = 60d / (2d * Math.PI);

            DataTags[RPM1_MOTOR] = "RPM1 Motor";
            DataUnitTags[RPM1_MOTOR] = "rad/s RPM";
            DataUnits[RPM1_MOTOR, 0] = 1d;
            DataUnits[RPM1_MOTOR, 1] = 60d / (2d * Math.PI);

            DataTags[RPM2] = "RPM2";
            DataUnitTags[RPM2] = "rad/s RPM";
            DataUnits[RPM2, 0] = 1d;
            DataUnits[RPM2, 1] = 60d / (2d * Math.PI);

            DataTags[RPM2_RATIO] = "Ratio";
            DataUnitTags[RPM2_RATIO] = "M/W";
            DataUnits[RPM2_RATIO, 0] = 1d;
            Data[RPM2_RATIO, MINIMUM] = 100000d;

            DataTags[RPM2_ROLLOUT] = "Rollout";
            DataUnitTags[RPM2_ROLLOUT] = "mm cm in";
            DataUnits[RPM2_ROLLOUT, 0] = 1d;
            DataUnits[RPM2_ROLLOUT, 1] = 0.1d;
            DataUnits[RPM2_ROLLOUT, 2] = 0.0393701d;
            Data[RPM2_ROLLOUT, MINIMUM] = 100000d;

            DataTags[SPEED] = "Speed";
            DataUnitTags[SPEED] = "m/s MPH KPH";
            DataUnits[SPEED, 0] = 1d;
            DataUnits[SPEED, 1] = 2.23694d;
            DataUnits[SPEED, 2] = 3.6d;

            DataTags[TORQUE_ROLLER] = "Roller Torque";
            DataUnitTags[TORQUE_ROLLER] = "N.m g.cm oz.in lb.ft";
            DataUnits[TORQUE_ROLLER, 0] = 1d;
            DataUnits[TORQUE_ROLLER, 1] = 10197.16d;
            DataUnits[TORQUE_ROLLER, 2] = 141.612d;
            DataUnits[TORQUE_ROLLER, 3] = 0.737562d;

            DataTags[TORQUE_WHEEL] = "Wheel Torque";
            DataUnitTags[TORQUE_WHEEL] = "N.m g.cm oz.in lb.ft";
            DataUnits[TORQUE_WHEEL, 0] = 1d;
            DataUnits[TORQUE_WHEEL, 1] = 10197.16d;
            DataUnits[TORQUE_WHEEL, 2] = 141.612d;
            DataUnits[TORQUE_WHEEL, 3] = 0.737562d;

            DataTags[TORQUE_MOTOR] = "Motor Torque";
            DataUnitTags[TORQUE_MOTOR] = "N.m g.cm oz.in lb.ft";
            DataUnits[TORQUE_MOTOR, 0] = 1d;
            DataUnits[TORQUE_MOTOR, 1] = 10197.16d;
            DataUnits[TORQUE_MOTOR, 2] = 141.612d;
            DataUnits[TORQUE_MOTOR, 3] = 0.737562d;

            DataTags[POWER] = "Power";
            DataUnitTags[POWER] = "W kW HP";
            DataUnits[POWER, 0] = 1d;
            DataUnits[POWER, 1] = 0.001d;
            DataUnits[POWER, 2] = 0.00134d;

            DataTags[DRAG] = "Drag";
            DataUnitTags[DRAG] = "W kW HP";
            DataUnits[DRAG, 0] = 1d;
            DataUnits[DRAG, 1] = 0.001d;
            DataUnits[DRAG, 2] = 0.00134d;

            DataTags[VOLTS] = "Voltage";
            DataUnitTags[VOLTS] = "V kV";
            DataUnits[VOLTS, 0] = 1d;
            DataUnits[VOLTS, 1] = 0.001d;
            Data[VOLTS, MINIMUM] = 100000d;

            DataTags[AMPS] = "Current";
            DataUnitTags[AMPS] = "A mA";
            DataUnits[AMPS, 0] = 1d;
            DataUnits[AMPS, 1] = 1000d;
            Data[AMPS, MINIMUM] = 100000d;

            DataTags[WATTS_IN] = "Watts In";
            DataUnitTags[WATTS_IN] = "W kW";
            DataUnits[WATTS_IN, 0] = 1d;
            DataUnits[WATTS_IN, 1] = 0.001d;
            Data[WATTS_IN, MINIMUM] = 100000d;

            DataTags[EFFICIENCY] = "Efficiency";
            DataUnitTags[EFFICIENCY] = "%";
            DataUnits[EFFICIENCY, 0] = 1d;
            Data[EFFICIENCY, MINIMUM] = 10000d;

            DataTags[CORRECTED_EFFICIENCY] = "Corr. Efficiency";
            DataUnitTags[CORRECTED_EFFICIENCY] = "%";
            DataUnits[CORRECTED_EFFICIENCY, 0] = 1d;
            Data[CORRECTED_EFFICIENCY, MINIMUM] = 10000d;


            DataTags[TEMPERATURE1] = "Temperature1";
            DataUnitTags[TEMPERATURE1] = "°C";
            DataUnits[TEMPERATURE1, 0] = 1d;
            Data[TEMPERATURE1, MINIMUM] = 10000d;

            DataTags[TEMPERATURE2] = "Temperature2";
            DataUnitTags[TEMPERATURE2] = "°C";
            DataUnits[TEMPERATURE2, 0] = 1d;
            Data[TEMPERATURE1, MINIMUM] = 10000d;

            DataTags[PIN04VALUE] = "Pin 4 Value";
            DataUnitTags[PIN04VALUE] = "Units";
            DataUnits[PIN04VALUE, 0] = 1d;
            Data[PIN04VALUE, MINIMUM] = 10000d;

            DataTags[PIN05VALUE] = "Pin 5 Value";
            DataUnitTags[PIN05VALUE] = "Units";
            DataUnits[PIN05VALUE, 0] = 1d;
            Data[PIN05VALUE, MINIMUM] = 10000d;

            DataTags[SESSIONTIME] = "Time";
            DataUnitTags[SESSIONTIME] = "Sec Min Hr";
            DataUnits[SESSIONTIME, 0] = 1d;
            DataUnits[SESSIONTIME, 1] = 1d / 60d;
            DataUnits[SESSIONTIME, 2] = 1d / 3600d;

            DataTags[CHAN1_FREQUENCY] = "Ch1 Frequency";
            DataUnitTags[CHAN1_FREQUENCY] = "Hz";
            DataUnits[CHAN1_FREQUENCY, 0] = 1d;

            DataTags[CHAN1_PULSEWIDTH] = "Ch1 Pulse Width";
            DataUnitTags[CHAN1_PULSEWIDTH] = "ms";
            DataUnits[CHAN1_PULSEWIDTH, 0] = 1d;

            DataTags[CHAN1_DUTYCYCLE] = "Ch1 Duty Cycle";
            DataUnitTags[CHAN1_DUTYCYCLE] = "%";
            DataUnits[CHAN1_DUTYCYCLE, 0] = 1d;

            DataTags[CHAN2_FREQUENCY] = "Ch2 Frequency";
            DataUnitTags[CHAN2_FREQUENCY] = "Hz";
            DataUnits[CHAN2_FREQUENCY, 0] = 1d;

            DataTags[CHAN2_PULSEWIDTH] = "Ch2 Pulse Width";
            DataUnitTags[CHAN2_PULSEWIDTH] = "ms";
            DataUnits[CHAN2_PULSEWIDTH, 0] = 1d;

            DataTags[CHAN2_DUTYCYCLE] = "Ch2 Duty Cycle";
            DataUnitTags[CHAN2_DUTYCYCLE] = "%";
            DataUnits[CHAN2_DUTYCYCLE, 0] = 1d;

            // NEW RUNDOWN SETS
            DataTags[TORQUE_COASTDOWN] = "Coast Down Torque";
            DataUnitTags[TORQUE_COASTDOWN] = "N.m g.cm oz.in lb.ft";
            DataUnits[TORQUE_COASTDOWN, 0] = 1d;
            DataUnits[TORQUE_COASTDOWN, 1] = 10197.16d;
            DataUnits[TORQUE_COASTDOWN, 2] = 141.612d;
            DataUnits[TORQUE_COASTDOWN, 3] = 0.737562d;

            DataTags[POWER_COASTDOWN] = "Coast Down Power";
            DataUnitTags[POWER_COASTDOWN] = "W kW HP";
            DataUnits[POWER_COASTDOWN, 0] = 1d;
            DataUnits[POWER_COASTDOWN, 1] = 0.001d;
            DataUnits[POWER_COASTDOWN, 2] = 0.00134d;

            // NEW CORRECTED TORQUE AND POWER SETS
            DataTags[CORRECTED_TORQUE_ROLLER] = "Corr. Roller Torque";
            DataUnitTags[CORRECTED_TORQUE_ROLLER] = "N.m g.cm oz.in lb.ft";
            DataUnits[CORRECTED_TORQUE_ROLLER, 0] = 1d;
            DataUnits[CORRECTED_TORQUE_ROLLER, 1] = 10197.16d;
            DataUnits[CORRECTED_TORQUE_ROLLER, 2] = 141.612d;
            DataUnits[CORRECTED_TORQUE_ROLLER, 3] = 0.737562d;

            DataTags[CORRECTED_TORQUE_WHEEL] = "Corr. Wheel Torque";
            DataUnitTags[CORRECTED_TORQUE_WHEEL] = "N.m g.cm oz.in lb.ft";
            DataUnits[CORRECTED_TORQUE_WHEEL, 0] = 1d;
            DataUnits[CORRECTED_TORQUE_WHEEL, 1] = 10197.16d;
            DataUnits[CORRECTED_TORQUE_WHEEL, 2] = 141.612d;
            DataUnits[CORRECTED_TORQUE_WHEEL, 3] = 0.737562d;

            DataTags[CORRECTED_TORQUE_MOTOR] = "Corr. Motor Torque";
            DataUnitTags[CORRECTED_TORQUE_MOTOR] = "N.m g.cm oz.in lb.ft";
            DataUnits[CORRECTED_TORQUE_MOTOR, 0] = 1d;
            DataUnits[CORRECTED_TORQUE_MOTOR, 1] = 10197.16d;
            DataUnits[CORRECTED_TORQUE_MOTOR, 2] = 141.612d;
            DataUnits[CORRECTED_TORQUE_MOTOR, 3] = 0.737562d;

            DataTags[CORRECTED_POWER_ROLLER] = "Corr. Roller Power";
            DataUnitTags[CORRECTED_POWER_ROLLER] = "W kW HP";
            DataUnits[CORRECTED_POWER_ROLLER, 0] = 1d;
            DataUnits[CORRECTED_POWER_ROLLER, 1] = 0.001d;
            DataUnits[CORRECTED_POWER_ROLLER, 2] = 0.00134d;

            DataTags[CORRECTED_POWER_WHEEL] = "Corr. Wheel Power";
            DataUnitTags[CORRECTED_POWER_WHEEL] = "W kW HP";
            DataUnits[CORRECTED_POWER_WHEEL, 0] = 1d;
            DataUnits[CORRECTED_POWER_WHEEL, 1] = 0.001d;
            DataUnits[CORRECTED_POWER_WHEEL, 2] = 0.00134d;

            DataTags[CORRECTED_POWER_MOTOR] = "Corr. Motor Power";
            DataUnitTags[CORRECTED_POWER_MOTOR] = "W kW HP";
            DataUnits[CORRECTED_POWER_MOTOR, 0] = 1d;
            DataUnits[CORRECTED_POWER_MOTOR, 1] = 0.001d;
            DataUnits[CORRECTED_POWER_MOTOR, 2] = 0.00134d;
#if QueryPerformance
            DataTags[PERFORMANCE] = "Performance";
            DataUnitTags[PERFORMANCE] = "%";
            DataUnits[PERFORMANCE, 0] = 1;
#endif
        }
        internal void COMPortCalibration()
        {
            double TempIntercept1;
            double TempIntercept2;

            VoltageSlope = (Voltage2 - Voltage1) / ((A0Voltage2 - A0Voltage1) * BitsToVoltage);
            TempIntercept1 = Voltage1 - VoltageSlope * A0Voltage1 * BitsToVoltage;
            TempIntercept2 = Voltage2 - VoltageSlope * A0Voltage2 * BitsToVoltage;
            VoltageIntercept = (TempIntercept1 + TempIntercept2) / 2d;

            CurrentSlope = (Current2 - Current1) / ((A1Voltage2 - A1Voltage1) * BitsToVoltage);
            TempIntercept1 = Current1 - CurrentSlope * A1Voltage1 * BitsToVoltage;
            TempIntercept2 = Current2 - CurrentSlope * A1Voltage2 * BitsToVoltage;
            CurrentIntercept = (TempIntercept1 + TempIntercept2) / 2d;

            Temperature1Slope = (Temp1Temperature2 - Temp1Temperature1) / ((A2Voltage2 - A2Voltage1) * BitsToVoltage);
            TempIntercept1 = Temp1Temperature1 - Temperature1Slope * A2Voltage1 * BitsToVoltage;
            TempIntercept2 = Temp1Temperature2 - Temperature1Slope * A2Voltage2 * BitsToVoltage;
            Temperature1Intercept = (TempIntercept1 + TempIntercept2) / 2d;

            Temperature2Slope = (Temp2Temperature2 - Temp2Temperature1) / ((A3Voltage2 - A3Voltage1) * BitsToVoltage);
            TempIntercept1 = Temp2Temperature1 - Temperature2Slope * A3Voltage1 * BitsToVoltage;
            TempIntercept2 = Temp2Temperature2 - Temperature2Slope * A3Voltage2 * BitsToVoltage;
            Temperature2Intercept = (TempIntercept1 + TempIntercept2) / 2d;
            // 
            A4ValueSlope = (A4Value2 - A4Value1) / ((A4Voltage2 - A4Voltage1) * BitsToVoltage);
            TempIntercept1 = A4Value1 - A4ValueSlope * A4Voltage1 * BitsToVoltage;
            TempIntercept2 = A4Value2 - A4ValueSlope * A4Voltage2 * BitsToVoltage;
            A4ValueIntercept = (TempIntercept1 + TempIntercept2) / 2d;

            A5ValueSlope = (A5Value2 - A5Value1) / ((A5Voltage2 - A5Voltage1) * BitsToVoltage);
            TempIntercept1 = A5Value1 - A5ValueSlope * A5Voltage1 * BitsToVoltage;
            TempIntercept2 = A5Value2 - A5ValueSlope * A5Voltage2 * BitsToVoltage;
            A5ValueIntercept = (TempIntercept1 + TempIntercept2) / 2d;
        }

        private void PrepareGraphicsParameters()
        {
            try
            {

                {
                    var withBlock = pnlSignalWindow;
                    PicSignalHeight = withBlock.Height;
                    PicSignalWidth = withBlock.Width;
                }

                SignalBitmap = new Bitmap(PicSignalWidth, PicSignalHeight);
                SignalWindowBMP = Graphics.FromImage(SignalBitmap);

                SignalXConversion = PicSignalWidth / (double)BUFFER_SIZE * NUMBER_OF_CHANNELS;
                SignalYConversion = PicSignalHeight / Math.Pow(2d, BITS_PER_SAMPLE);

                HighSignalThreshold2 = Conversions.ToDouble(txtThreshold2.Text);
                HighSignalThreshold = Conversions.ToDouble(txtThreshold1.Text);

                if (HighSignalThreshold > 128d)
                    WhichSignal = HIGHSIGNAL;
                else
                    WhichSignal = LOWSIGNAL;
                SignalThresholdYConverted = (int)Math.Round(PicSignalHeight - HighSignalThreshold * SignalYConversion);
                if (HighSignalThreshold2 > 128d)
                    WhichSignal2 = HIGHSIGNAL;
                else
                    WhichSignal2 = LOWSIGNAL;
                SignalThreshold2YConverted = (int)Math.Round(PicSignalHeight - HighSignalThreshold2 * SignalYConversion);
            }

            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("PrepareGraphicsParameters Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);

            }
        }
        private void ResetValues()
        {
            try
            {
                int ParameterCount;

                for (ParameterCount = 0; ParameterCount <= LAST - 1; ParameterCount++)
                {
                    Data[ParameterCount, MINIMUM] = 999999d;
                    Data[ParameterCount, ACTUAL] = 0d;
                    Data[ParameterCount, MAXIMUM] = 0d;
                }

                ResetAllYTimeGraphs();
                Data[SESSIONTIME, ACTUAL] = 0d;
            }

            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("ResetValues Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
        }

        #endregion
        #region Initialize, Process and Shutdown Wave Input
        private void InitializeWaveInput()
        {

            {
                ref var withBlock = ref waveFormat;
                withBlock.wFormatTag = WAVE_FORMAT_PCM;       // uncompressed Pulse Code Modulation
                withBlock.nchannels = (short)NUMBER_OF_CHANNELS;    // 1 for mono 2 for stereo
                withBlock.nSamplesPerSec = SAMPLE_RATE;       // 44100 is CD quality
                withBlock.wBitsPerSample = BITS_PER_SAMPLE;               // BITS_PER_SAMPLE 'CD quality is 16
                withBlock.nBlockAlign = (short)Math.Round(withBlock.nchannels * (withBlock.wBitsPerSample / 8d));
                withBlock.nAvgBytesPerSec = withBlock.nSamplesPerSec * withBlock.nBlockAlign;
                withBlock.cbSize = 0;
            }

            i = waveInOpen(ref WaveInHandle, (IntPtr)WAVE_MAPPER, ref waveFormat, myCallBackFunction, IntPtr.Zero, CALLBACK_FUNCTION);
            if (i != 0)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("InitializeWaveInput / WaveInOpen Error", MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }

            bufferpin0 = GCHandle.Alloc(RawWaveData0, GCHandleType.Pinned);
            bufferpin1 = GCHandle.Alloc(RawWaveData1, GCHandleType.Pinned);
            bufferpin2 = GCHandle.Alloc(RawWaveData2, GCHandleType.Pinned);
            bufferpin3 = GCHandle.Alloc(RawWaveData3, GCHandleType.Pinned);
            bufferpin4 = GCHandle.Alloc(RawWaveData4, GCHandleType.Pinned);
            bufferpin5 = GCHandle.Alloc(RawWaveData5, GCHandleType.Pinned);
            bufferpin6 = GCHandle.Alloc(RawWaveData6, GCHandleType.Pinned);
            bufferpin7 = GCHandle.Alloc(RawWaveData7, GCHandleType.Pinned);
            bufferpin8 = GCHandle.Alloc(RawWaveData8, GCHandleType.Pinned);
            bufferpin9 = GCHandle.Alloc(RawWaveData9, GCHandleType.Pinned);
            bufferpin10 = GCHandle.Alloc(RawWaveData10, GCHandleType.Pinned);
            bufferpin11 = GCHandle.Alloc(RawWaveData11, GCHandleType.Pinned);
            bufferpin12 = GCHandle.Alloc(RawWaveData12, GCHandleType.Pinned);
            bufferpin13 = GCHandle.Alloc(RawWaveData13, GCHandleType.Pinned);
            bufferpin14 = GCHandle.Alloc(RawWaveData14, GCHandleType.Pinned);
            bufferpin15 = GCHandle.Alloc(RawWaveData15, GCHandleType.Pinned);
            bufferpin16 = GCHandle.Alloc(RawWaveData16, GCHandleType.Pinned);
            bufferpin17 = GCHandle.Alloc(RawWaveData17, GCHandleType.Pinned);
            bufferpin18 = GCHandle.Alloc(RawWaveData18, GCHandleType.Pinned);
            bufferpin19 = GCHandle.Alloc(RawWaveData19, GCHandleType.Pinned);




            for (j = 0; j <= NUMBER_OF_BUFFERS - 1; j++)
            {
                {
                    ref var withBlock1 = ref WaveBufferHeaders[j];
                    withBlock1.dwBufferLength = BUFFER_SIZE;
                    withBlock1.dwFlags = 0;
                    withBlock1.dwBytesRecorded = 0;
                    withBlock1.dwLoops = 0;
                    withBlock1.dwUser = (IntPtr)0;
                }

            }

            WaveBufferHeaders[0].lpData = bufferpin0.AddrOfPinnedObject();
            WaveBufferHeaders[1].lpData = bufferpin1.AddrOfPinnedObject();
            WaveBufferHeaders[2].lpData = bufferpin2.AddrOfPinnedObject();
            WaveBufferHeaders[3].lpData = bufferpin3.AddrOfPinnedObject();
            WaveBufferHeaders[4].lpData = bufferpin4.AddrOfPinnedObject();
            WaveBufferHeaders[5].lpData = bufferpin5.AddrOfPinnedObject();
            WaveBufferHeaders[6].lpData = bufferpin6.AddrOfPinnedObject();
            WaveBufferHeaders[7].lpData = bufferpin7.AddrOfPinnedObject();
            WaveBufferHeaders[8].lpData = bufferpin8.AddrOfPinnedObject();
            WaveBufferHeaders[9].lpData = bufferpin9.AddrOfPinnedObject();
            WaveBufferHeaders[10].lpData = bufferpin10.AddrOfPinnedObject();
            WaveBufferHeaders[11].lpData = bufferpin11.AddrOfPinnedObject();
            WaveBufferHeaders[12].lpData = bufferpin12.AddrOfPinnedObject();
            WaveBufferHeaders[13].lpData = bufferpin13.AddrOfPinnedObject();
            WaveBufferHeaders[14].lpData = bufferpin14.AddrOfPinnedObject();
            WaveBufferHeaders[15].lpData = bufferpin15.AddrOfPinnedObject();
            WaveBufferHeaders[16].lpData = bufferpin16.AddrOfPinnedObject();
            WaveBufferHeaders[17].lpData = bufferpin17.AddrOfPinnedObject();
            WaveBufferHeaders[18].lpData = bufferpin18.AddrOfPinnedObject();
            WaveBufferHeaders[19].lpData = bufferpin19.AddrOfPinnedObject();

            for (j = 0; j <= NUMBER_OF_BUFFERS - 1; j++)
            {
                i = waveInPrepareHeader(WaveInHandle, ref WaveBufferHeaders[j], Marshal.SizeOf(WaveBufferHeaders[j]));
                if (i != 0)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("InitializeWaveInput / waveInPrepareHeader Error", MsgBoxStyle.Exclamation);
                    Environment.Exit(0);
                }
            }

            for (j = 0; j <= NUMBER_OF_BUFFERS - 1; j++)
            {
                i = waveInAddBuffer(WaveInHandle, ref WaveBufferHeaders[j], Marshal.SizeOf(WaveBufferHeaders[j]));
                if (i != 0)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("InitializeWaveInput / waveInAddBuffer Error", MsgBoxStyle.Exclamation);
                    Environment.Exit(0);
                }
            }

            i = waveInStart(WaveInHandle);
            if (i != 0)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("InitializeWaveInput / waveInStart Error", MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
            else
            {
                WavesStarted = true;
            }

        }
        private int MyWaveCallBackProcedure(IntPtr hwi, int uMsg, int dwInstance, int dwParam1, int dwParam2)
        {
            if (StopAddingBuffers == false)
            {
                if (uMsg == WIM_DATA & WaveBufferHeaders[BufferCount].dwBytesRecorded == BUFFER_SIZE)
                {
                    myWaveHandler_ProcessWave();
                }
            }

            return default;
        }
        private void myWaveHandler_ProcessWave()
        {
            try
            {
#if QueryPerformance
                QueryPerformanceCounter(ref StartWatch);
#endif
                if (StopAddingBuffers == false) // StopAddingBuffers is true when ShutDownWaves has been called
                {

                    Marshal.Copy(WaveBufferHeaders[BufferCount].lpData, RawWaveData, 0, WaveBufferHeaders[BufferCount].dwBytesRecorded); // Copy the current buffer (based on buffercount) to the working buffer, RawWaveData
                    WaveBufferHeaders[BufferCount].dwBytesRecorded = 0;     // Reset bytes recorded...
                    i = waveInAddBuffer(WaveInHandle, ref WaveBufferHeaders[BufferCount], Marshal.SizeOf(WaveBufferHeaders[BufferCount]));  // ...and add the buffer back to the queue
                    if (i != 0) // Check that there were no problems adding back the buffer.'This could be skipped in a release version using a compiler constant
                    {
                        btnHide_Click(this, EventArgs.Empty);
                        Interaction.MsgBox("myWaveHandler_ProcessWave / waveInAddBuffer Error" + i, MsgBoxStyle.Exclamation);
                        Environment.Exit(0);
                    }

                    SignalWindowBMP.Clear(Color.Black); // Clear the signal window and draw the RPM1 threshold line
                    SignalWindowBMP.DrawLine(Channel1ThresholdPen, 0, SignalThresholdYConverted, PicSignalWidth, SignalThresholdYConverted);

                    var loopTo = BUFFER_SIZE - 1;
                    for (j = 0; NUMBER_OF_CHANNELS >= 0 ? j <= loopTo : j >= loopTo; j += NUMBER_OF_CHANNELS) // Main loop through the raw data
                    {

                        NextYPosition = (int)Math.Round(PicSignalHeight - RawWaveData[j] * SignalYConversion); // Calculate the next drawing position for the signal
                        SignalWindowBMP.DrawLine(Channel1SignalPen, (int)Math.Round(CurrentSignalXPosition - SignalXConversion), LastYPosition, (int)Math.Round(CurrentSignalXPosition), NextYPosition);  // Draw the line
                        LastYPosition = NextYPosition; // Remember the new drawing position for the next drawing operation

                        Data[SESSIONTIME, ACTUAL] += ElapsedTimeUnit; // Increase the session time value - used by the Graphing form for time info

                        if ((WhichSignal == HIGHSIGNAL && RawWaveData[j] > HighSignalThreshold) | (WhichSignal == LOWSIGNAL && RawWaveData[j] < HighSignalThreshold)) // Check is we have found a signal depending on where the threshold line is set
                        {

                            k = k + NUMBER_OF_CHANNELS; // Counting bytes for pulsewidth

                            if (FoundHighSignal == false) // if FoundHighSigal is false, then this is the start of a new pulse rather than already being in a pulse
                            {
                                FoundHighSignal = true;  // flag that we are inside a pulse - this gets set to false when we drop below the threshold
                                switch (UseAdvancedProcessing) // Calculate elapsed time simply by the byte count since last or by interpolation through the threshold line
                                {
                                    case var @case when @case == false:
                                        {
                                            ElapsedTime = (j - LastHighBufferPosition) * BytesToSeconds;
                                            break;
                                        }
                                    case var case1 when case1 == true:
                                        {
                                            NewElapsedTimeCorrection = Math.Abs((RawWaveData[j] - HighSignalThreshold) / (RawWaveData[j] - LastSignal));
                                            ElapsedTime = (j - LastHighBufferPosition + OldElapsedTimeCorrection - NewElapsedTimeCorrection) * BytesToSeconds;
                                            OldElapsedTimeCorrection = NewElapsedTimeCorrection;
                                            break;
                                        }
                                }
                                LastHighBufferPosition = j;    // set the new buffer position from which to count the next set of bytes

                                Data[CHAN1_FREQUENCY, ACTUAL] = 1d / ElapsedTime; // calculate frequency for scope work - this should be compiler constant controlled
                                if (Data[CHAN1_FREQUENCY, ACTUAL] > Data[CHAN1_FREQUENCY, MAXIMUM])
                                    Data[CHAN1_FREQUENCY, MAXIMUM] = Data[CHAN1_FREQUENCY, ACTUAL];
                                if (Data[CHAN1_FREQUENCY, ACTUAL] < Data[CHAN1_FREQUENCY, MINIMUM])
                                    Data[CHAN1_FREQUENCY, MINIMUM] = Data[CHAN1_FREQUENCY, ACTUAL];

                                Data[RPM1_ROLLER, ACTUAL] = ElapsedTimeToRadPerSec / ElapsedTime; // calculate roller angular velocity in Rad/s
                                Data[RPM1_WHEEL, ACTUAL] = Data[RPM1_ROLLER, ACTUAL] * RollerRPMtoWheelRPM; // calculate the wheel and motor angular velocities based on roller and wheel diameters and gear ratio
                                Data[RPM1_MOTOR, ACTUAL] = Data[RPM1_WHEEL, ACTUAL] * GearRatio;
                                Data[SPEED, ACTUAL] = Data[RPM1_ROLLER, ACTUAL] * RollerRadsPerSecToMetersPerSec; // calculate the speed (meters/s) based on roller rad/s
                                Data[DRAG, ACTUAL] = Math.Pow(Data[SPEED, ACTUAL], 3d) * ForceAir;  // calculate drag based on vehicle speed (meters/s)

                                if (Data[RPM1_ROLLER, ACTUAL] > Data[RPM1_ROLLER, MAXIMUM])  // set the maximum values for roller, wheel, and motor RPM; Speed and Drag
                                {
                                    Data[RPM1_ROLLER, MAXIMUM] = Data[RPM1_ROLLER, ACTUAL];
                                    Data[RPM1_WHEEL, MAXIMUM] = Data[RPM1_WHEEL, ACTUAL];
                                    Data[RPM1_MOTOR, MAXIMUM] = Data[RPM1_MOTOR, ACTUAL];
                                    Data[SPEED, MAXIMUM] = Data[SPEED, ACTUAL];
                                    Data[DRAG, MAXIMUM] = Data[DRAG, ACTUAL];
                                }

                                if (Data[RPM1_ROLLER, ACTUAL] < Data[RPM1_ROLLER, MINIMUM])
                                {
                                    Data[RPM1_ROLLER, MINIMUM] = Data[RPM1_ROLLER, ACTUAL];
                                    Data[RPM1_WHEEL, MINIMUM] = Data[RPM1_WHEEL, ACTUAL];
                                    Data[RPM1_MOTOR, MINIMUM] = Data[RPM1_MOTOR, ACTUAL];
                                    Data[SPEED, MINIMUM] = Data[SPEED, ACTUAL];
                                    Data[DRAG, MINIMUM] = Data[DRAG, ACTUAL];
                                }

                                Data[TORQUE_ROLLER, ACTUAL] = (Data[RPM1_ROLLER, ACTUAL] - OldAngularVelocity) / ElapsedTime * DynoMomentOfInertia; // calculate torque based on angular acceleration (delta speed per time) and MOI
                                                                                                                                                    // removing the power calc based on average 06DEC13
                                                                                                                                                    // Data(POWER, ACTUAL) = Data(TORQUE_ROLLER, ACTUAL) * ((Data(RPM1_ROLLER, ACTUAL) + OldAngularVelocity) / 2) 'calculate power based on torque and average angular velocity between two points
                                Data[POWER, ACTUAL] = Data[TORQUE_ROLLER, ACTUAL] * Data[RPM1_ROLLER, ACTUAL]; // + OldAngularVelocity) / 2) 'calculate power based on torque and average angular velocity between two points
                                if (Data[POWER, ACTUAL] > 0d)
                                {
                                    Data[EFFICIENCY, ACTUAL] = Data[WATTS_IN, ACTUAL] / Data[POWER, ACTUAL];
                                }
                                else
                                {
                                    Data[EFFICIENCY, ACTUAL] = 0d;
                                }
                                // Original versions of wheel and motor torque relied on back calc from Power
                                Data[TORQUE_WHEEL, ACTUAL] = Data[POWER, ACTUAL] / Data[RPM1_WHEEL, ACTUAL]; // back calculate the torque at the wheel and motor based on the calculated power
                                Data[TORQUE_MOTOR, ACTUAL] = Data[POWER, ACTUAL] / Data[RPM1_MOTOR, ACTUAL];

                                if (Data[TORQUE_ROLLER, ACTUAL] > Data[TORQUE_ROLLER, MAXIMUM]) // set the maximum values for torque
                                {
                                    Data[TORQUE_ROLLER, MAXIMUM] = Data[TORQUE_ROLLER, ACTUAL];
                                    Data[TORQUE_WHEEL, MAXIMUM] = Data[TORQUE_WHEEL, ACTUAL];
                                    Data[TORQUE_MOTOR, MAXIMUM] = Data[TORQUE_MOTOR, ACTUAL];
                                }

                                if (Data[TORQUE_ROLLER, ACTUAL] < Data[TORQUE_ROLLER, MINIMUM])
                                {
                                    Data[TORQUE_ROLLER, MINIMUM] = Data[TORQUE_ROLLER, ACTUAL];
                                    Data[TORQUE_WHEEL, MINIMUM] = Data[TORQUE_WHEEL, ACTUAL];
                                    Data[TORQUE_MOTOR, MINIMUM] = Data[TORQUE_MOTOR, ACTUAL];
                                }

                                if (Data[POWER, ACTUAL] > Data[POWER, MAXIMUM]) // set the maximum value for power
                                {
                                    Data[POWER, MAXIMUM] = Data[POWER, ACTUAL];
                                }
                                if (Data[POWER, ACTUAL] < Data[POWER, MINIMUM])
                                {
                                    Data[POWER, MINIMUM] = Data[POWER, ACTUAL];
                                }
                                if (Data[EFFICIENCY, ACTUAL] > Data[EFFICIENCY, MAXIMUM]) // set the maximum value for power
                                {
                                    Data[EFFICIENCY, MAXIMUM] = Data[EFFICIENCY, ACTUAL];
                                }
                                if (Data[EFFICIENCY, ACTUAL] < Data[EFFICIENCY, MINIMUM]) // set the maximum value for power
                                {
                                    Data[EFFICIENCY, MINIMUM] = Data[EFFICIENCY, ACTUAL];
                                }

                                OldAngularVelocity = Data[RPM1_ROLLER, ACTUAL]; // remember the current angular velocity for next pulse

                                switch (WhichDataMode) // Live, PowerRun or LogRaw ?
                                {
                                    case var case2 when case2 == LIVE:
                                        {
                                            break;
                                        }
                                    // Don't do anything.  This helps skip through the Select Case Faster
                                    case var case3 when case3 == POWERRUN:
                                        {
                                            if (Data[RPM1_ROLLER, ACTUAL] > ActualPowerRunThreshold)
                                            {
                                                DataPoints += 1;
                                                if (DataPoints == 1)
                                                {
                                                    TotalElapsedTime = 0d;
                                                }
                                                else
                                                {
                                                    TotalElapsedTime += ElapsedTime;
                                                }
                                                if (DataPoints == MinimumPowerRunPoints)
                                                    btnStartPowerRun.BackColor = Color.Green;
                                                CollectedData[SESSIONTIME, DataPoints] = TotalElapsedTime;
                                                CollectedData[RPM1_ROLLER, DataPoints] = Data[RPM1_ROLLER, ACTUAL];
                                                CollectedData[RPM2, DataPoints] = Data[RPM2, ACTUAL];
                                                CollectedData[VOLTS, DataPoints] = Data[VOLTS, ACTUAL];
                                                CollectedData[AMPS, DataPoints] = Data[AMPS, ACTUAL];
                                                CollectedData[TEMPERATURE1, DataPoints] = Data[TEMPERATURE1, ACTUAL];
                                                CollectedData[TEMPERATURE2, DataPoints] = Data[TEMPERATURE2, ACTUAL];
                                                CollectedData[PIN04VALUE, DataPoints] = Data[PIN04VALUE, ACTUAL];
                                                CollectedData[PIN05VALUE, DataPoints] = Data[PIN05VALUE, ACTUAL];
                                                CollectedData[CHAN1_FREQUENCY, DataPoints] = Data[CHAN1_FREQUENCY, ACTUAL];
                                                CollectedData[CHAN1_PULSEWIDTH, DataPoints] = Data[CHAN1_PULSEWIDTH, ACTUAL];
                                                CollectedData[CHAN1_DUTYCYCLE, DataPoints] = Data[CHAN1_DUTYCYCLE, ACTUAL];
                                                CollectedData[CHAN2_FREQUENCY, DataPoints] = Data[CHAN2_FREQUENCY, ACTUAL];
                                                CollectedData[CHAN2_PULSEWIDTH, DataPoints] = Data[CHAN2_PULSEWIDTH, ACTUAL];
                                                CollectedData[CHAN2_DUTYCYCLE, DataPoints] = Data[CHAN2_DUTYCYCLE, ACTUAL];
                                                // DataPoints += 1
                                                if (DataPoints == MAXDATAPOINTS)
                                                {
                                                    DataPoints = MAXDATAPOINTS - 1;
                                                    // btnStartPowerRun.BackColor = Color.Red
                                                }
                                            }

                                            break;
                                        }
                                    case var case4 when case4 == LOGRAW:
                                        {
                                            DataPoints += 1;
                                            if (DataPoints == 1)
                                            {
                                                btnStartLoggingRaw.BackColor = Color.Green;
                                                TotalElapsedTime = 0d;
                                            }
                                            else
                                            {
                                                TotalElapsedTime += ElapsedTime;
                                            }
                                            CollectedData[SESSIONTIME, DataPoints] = TotalElapsedTime;
                                            CollectedData[RPM1_ROLLER, DataPoints] = Data[RPM1_ROLLER, ACTUAL];
                                            CollectedData[RPM2, DataPoints] = Data[RPM2, ACTUAL];
                                            CollectedData[VOLTS, DataPoints] = Data[VOLTS, ACTUAL];
                                            CollectedData[AMPS, DataPoints] = Data[AMPS, ACTUAL];
                                            CollectedData[TEMPERATURE1, DataPoints] = Data[TEMPERATURE1, ACTUAL];
                                            CollectedData[TEMPERATURE2, DataPoints] = Data[TEMPERATURE2, ACTUAL];
                                            CollectedData[PIN04VALUE, DataPoints] = Data[PIN04VALUE, ACTUAL];
                                            CollectedData[PIN05VALUE, DataPoints] = Data[PIN05VALUE, ACTUAL];
                                            CollectedData[CHAN1_FREQUENCY, DataPoints] = Data[CHAN1_FREQUENCY, ACTUAL];
                                            CollectedData[CHAN1_PULSEWIDTH, DataPoints] = Data[CHAN1_PULSEWIDTH, ACTUAL];
                                            CollectedData[CHAN1_DUTYCYCLE, DataPoints] = Data[CHAN1_DUTYCYCLE, ACTUAL];
                                            CollectedData[CHAN2_FREQUENCY, DataPoints] = Data[CHAN2_FREQUENCY, ACTUAL];
                                            CollectedData[CHAN2_PULSEWIDTH, DataPoints] = Data[CHAN2_PULSEWIDTH, ACTUAL];
                                            CollectedData[CHAN2_DUTYCYCLE, DataPoints] = Data[CHAN2_DUTYCYCLE, ACTUAL];
                                            // DataPoints += 1
                                            if (DataPoints == MAXDATAPOINTS)
                                            {
                                                DataPoints = MAXDATAPOINTS - 1;
                                                btnStartLoggingRaw.BackColor = Color.Red;
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                        if ((WhichSignal == HIGHSIGNAL && RawWaveData[j] <= HighSignalThreshold - 3d) | (WhichSignal == LOWSIGNAL && RawWaveData[j] >= HighSignalThreshold + 3d))
                        {
                            if (FoundHighSignal == true)
                            {
                                Data[CHAN1_PULSEWIDTH, ACTUAL] = k * BytesToSeconds * 1000d;
                                if (Data[CHAN1_PULSEWIDTH, ACTUAL] > Data[CHAN1_PULSEWIDTH, MAXIMUM])
                                    Data[CHAN1_PULSEWIDTH, MAXIMUM] = Data[CHAN1_PULSEWIDTH, ACTUAL];
                                if (Data[CHAN1_PULSEWIDTH, ACTUAL] < Data[CHAN1_PULSEWIDTH, MINIMUM])
                                    Data[CHAN1_PULSEWIDTH, MINIMUM] = Data[CHAN1_PULSEWIDTH, ACTUAL];
                                k = 0;
                                Data[CHAN1_DUTYCYCLE, ACTUAL] = Data[CHAN1_PULSEWIDTH, ACTUAL] * Data[CHAN1_FREQUENCY, ACTUAL] / 10d;
                                if (Data[CHAN1_DUTYCYCLE, ACTUAL] > Data[CHAN1_DUTYCYCLE, MAXIMUM])
                                    Data[CHAN1_DUTYCYCLE, MAXIMUM] = Data[CHAN1_DUTYCYCLE, ACTUAL];
                                if (Data[CHAN1_DUTYCYCLE, ACTUAL] < Data[CHAN1_DUTYCYCLE, MINIMUM])
                                    Data[CHAN1_DUTYCYCLE, MINIMUM] = Data[CHAN1_DUTYCYCLE, ACTUAL];
                            }
                            FoundHighSignal = false;
                        }

                        LastSignal = RawWaveData[j]; // remember the last high signal and the current correction time

                        if (NUMBER_OF_CHANNELS == 2)
                        {

                            SignalWindowBMP.DrawLine(Channel2ThresholdPen, 0, SignalThreshold2YConverted, PicSignalWidth, SignalThreshold2YConverted);
                            NextYPosition2 = (int)Math.Round(PicSignalHeight - RawWaveData[j + 1] * SignalYConversion) + 1;    // calculate coordinate for next channel 2 signal point
                            SignalWindowBMP.DrawLine(Channel2SignalPen, (int)Math.Round(CurrentSignalXPosition - SignalXConversion), LastYPosition2, (int)Math.Round(CurrentSignalXPosition), NextYPosition2); // draw line to the newly calculated point...
                            LastYPosition2 = NextYPosition2; // ...and remember the new position for the next cycle

                            if ((WhichSignal2 == HIGHSIGNAL && RawWaveData[j + 1] > HighSignalThreshold2) | (WhichSignal2 == LOWSIGNAL && RawWaveData[j + 1] < HighSignalThreshold2)) // Check is we have found a signal depending on where the threshold line is set
                            {

                                k2 = k2 + NUMBER_OF_CHANNELS; // count bytes for channel 2 pulsewidth

                                if (FoundHighSignal2 == false)    // if FoundHighSigal is false, then this is the start of a new pulse not the middle of an existing pulse
                                {

                                    FoundHighSignal2 = true; // flag that we are in the middle of a pulse

                                    switch (UseAdvancedProcessing) // Calculate elapsed time simply by the byte count since last or by interpolation through the threshold line
                                    {
                                        case var case5 when case5 == false:
                                            {
                                                ElapsedTime2 = (j + 1 - LastHighBufferPosition2) * BytesToSeconds; // calculate the elapsed time by multiplying the number of bytes since the last pulse 'by the time taken for each byte (which depends on the sampling rate)
                                                break;
                                            }
                                        case var case6 when case6 == true:
                                            {
                                                NewElapsedTimeCorrection2 = Math.Abs((RawWaveData[j + 1] - HighSignalThreshold) / (RawWaveData[j + 1] - LastSignal2));
                                                ElapsedTime2 = (j + 1 - LastHighBufferPosition2 + OldElapsedTimeCorrection2 - NewElapsedTimeCorrection2) * BytesToSeconds;
                                                OldElapsedTimeCorrection2 = NewElapsedTimeCorrection2;
                                                break;
                                            }
                                    }

                                    LastHighBufferPosition2 = j + 1;   // set the current buffer position for the next pulse

                                    Data[CHAN2_FREQUENCY, ACTUAL] = 1d / ElapsedTime2; // calculate frequency for scope work 
                                    if (Data[CHAN2_FREQUENCY, ACTUAL] > Data[CHAN2_FREQUENCY, MAXIMUM])
                                        Data[CHAN2_FREQUENCY, MAXIMUM] = Data[CHAN2_FREQUENCY, ACTUAL];
                                    if (Data[CHAN2_FREQUENCY, ACTUAL] < Data[CHAN2_FREQUENCY, MINIMUM])
                                        Data[CHAN2_FREQUENCY, MINIMUM] = Data[CHAN2_FREQUENCY, ACTUAL];


                                    Data[RPM2, ACTUAL] = ElapsedTimeToRadPerSec2 / ElapsedTime2; // calculate roller angular velocity in Rad/s
                                    Data[RPM2_RATIO, ACTUAL] = Data[RPM2, ACTUAL] / Data[RPM1_WHEEL, ACTUAL]; // calculate the ratios between RPM2 and RPM1 - wheel

                                    Data[RPM2_ROLLOUT, ACTUAL] = WheelCircumference / Data[RPM2_RATIO, ACTUAL]; // calculate Rollout (default unit is mm).  This assumes RPM2 is measuring motor RPM   'Rollout is the number of mm traveled for 1 rotation of the wheel

                                    if (Data[RPM2, ACTUAL] > Data[RPM2, MAXIMUM]) // check and set maximum values for RPM2, Ratio and rollout
                                    {
                                        Data[RPM2, MAXIMUM] = Data[RPM2, ACTUAL];
                                    }
                                    if (Data[RPM2, ACTUAL] < Data[RPM2, MINIMUM])
                                    {
                                        Data[RPM2, MINIMUM] = Data[RPM2, ACTUAL];
                                    }
                                    if (Data[RPM2_RATIO, ACTUAL] > Data[RPM2_RATIO, MAXIMUM])
                                    {
                                        Data[RPM2_RATIO, MAXIMUM] = Data[RPM2_RATIO, ACTUAL];
                                    }
                                    if (Data[RPM2_RATIO, ACTUAL] < Data[RPM2_RATIO, MINIMUM])
                                    {
                                        Data[RPM2_RATIO, MINIMUM] = Data[RPM2_RATIO, ACTUAL];
                                    }
                                    if (Data[RPM2_ROLLOUT, ACTUAL] > Data[RPM2_ROLLOUT, MAXIMUM])
                                    {
                                        Data[RPM2_ROLLOUT, MAXIMUM] = Data[RPM2_ROLLOUT, ACTUAL];
                                    }
                                    if (Data[RPM2_ROLLOUT, ACTUAL] < Data[RPM2_ROLLOUT, MINIMUM])
                                    {
                                        Data[RPM2_ROLLOUT, MINIMUM] = Data[RPM2_ROLLOUT, ACTUAL];
                                    }
                                }
                            }
                            if ((WhichSignal2 == HIGHSIGNAL && RawWaveData[j + 1] <= HighSignalThreshold2 - 3d) | (WhichSignal2 == LOWSIGNAL && RawWaveData[j + 1] >= HighSignalThreshold2 + 3d))
                            {
                                if (FoundHighSignal2 == true)
                                {
                                    Data[CHAN2_PULSEWIDTH, ACTUAL] = k2 * BytesToSeconds * 1000d;
                                    k2 = 0;
                                    if (Data[CHAN2_PULSEWIDTH, ACTUAL] > Data[CHAN2_PULSEWIDTH, MAXIMUM])
                                        Data[CHAN2_PULSEWIDTH, MAXIMUM] = Data[CHAN2_PULSEWIDTH, ACTUAL];
                                    if (Data[CHAN2_PULSEWIDTH, ACTUAL] < Data[CHAN2_PULSEWIDTH, MINIMUM])
                                        Data[CHAN2_PULSEWIDTH, MINIMUM] = Data[CHAN2_PULSEWIDTH, ACTUAL];
                                    Data[CHAN2_DUTYCYCLE, ACTUAL] = Data[CHAN2_PULSEWIDTH, ACTUAL] * Data[CHAN2_FREQUENCY, ACTUAL] / 10d;
                                    if (Data[CHAN2_DUTYCYCLE, ACTUAL] > Data[CHAN2_DUTYCYCLE, MAXIMUM])
                                        Data[CHAN2_DUTYCYCLE, MAXIMUM] = Data[CHAN2_DUTYCYCLE, ACTUAL];
                                    if (Data[CHAN2_DUTYCYCLE, ACTUAL] < Data[CHAN2_DUTYCYCLE, MINIMUM])
                                        Data[CHAN2_DUTYCYCLE, MINIMUM] = Data[CHAN2_DUTYCYCLE, ACTUAL];
                                }

                                FoundHighSignal2 = false;

                            }
                            LastSignal2 = RawWaveData[j + 1]; // remember the last high signal and the current correction time
                        }
                        CurrentSignalXPosition = (CurrentSignalXPosition + SignalXConversion) % PicSignalWidth;

                    }

                    pnlSignalWindow.Invalidate();

                    if (NUMBER_OF_CHANNELS == 2)
                    {
                        LastHighBufferPosition2 = LastHighBufferPosition2 - BUFFER_SIZE;
                        if (LastHighBufferPosition2 / (double)SAMPLE_RATE * -1 > WaitForNewSignal * NUMBER_OF_CHANNELS)
                        {
                            Data[RPM2, ACTUAL] = 0d;
                            Data[RPM2_RATIO, ACTUAL] = 0d;
                            Data[RPM2_ROLLOUT, ACTUAL] = 0d;
                            Data[CHAN2_DUTYCYCLE, ACTUAL] = 0d;
                            Data[CHAN2_FREQUENCY, ACTUAL] = 0d;
                            Data[CHAN2_PULSEWIDTH, ACTUAL] = 0d;
                        }
                    }

                    LastHighBufferPosition = LastHighBufferPosition - BUFFER_SIZE;
                    if (LastHighBufferPosition / (double)SAMPLE_RATE * -1 > WaitForNewSignal * NUMBER_OF_CHANNELS)
                    {
                        Data[RPM1_ROLLER, ACTUAL] = 0d;
                        Data[RPM1_WHEEL, ACTUAL] = 0d;
                        Data[RPM1_MOTOR, ACTUAL] = 0d;
                        Data[SPEED, ACTUAL] = 0d;
                        Data[DRAG, ACTUAL] = 0d;
                        Data[TORQUE_ROLLER, ACTUAL] = 0d;
                        Data[TORQUE_WHEEL, ACTUAL] = 0d;
                        Data[TORQUE_MOTOR, ACTUAL] = 0d;
                        Data[POWER, ACTUAL] = 0d;
                        Data[RPM2_RATIO, ACTUAL] = 0d;
                        Data[CHAN1_DUTYCYCLE, ACTUAL] = 0d;
                        Data[CHAN1_FREQUENCY, ACTUAL] = 0d;
                        Data[CHAN1_PULSEWIDTH, ACTUAL] = 0d;
                        if (WhichDataMode == LOGRAW)
                        {
                            TotalElapsedTime += ElapsedTime;
                            CollectedData[SESSIONTIME, DataPoints] = TotalElapsedTime;
                            CollectedData[RPM1_ROLLER, DataPoints] = Data[RPM1_ROLLER, ACTUAL];
                            CollectedData[RPM2, DataPoints] = Data[RPM2, ACTUAL];
                            CollectedData[VOLTS, DataPoints] = Data[VOLTS, ACTUAL];
                            CollectedData[AMPS, DataPoints] = Data[AMPS, ACTUAL];
                            CollectedData[TEMPERATURE1, DataPoints] = Data[TEMPERATURE1, ACTUAL];
                            CollectedData[TEMPERATURE2, DataPoints] = Data[TEMPERATURE2, ACTUAL];
                            CollectedData[PIN04VALUE, DataPoints] = Data[PIN04VALUE, ACTUAL];
                            CollectedData[PIN05VALUE, DataPoints] = Data[PIN05VALUE, ACTUAL];
                            CollectedData[CHAN1_FREQUENCY, DataPoints] = Data[CHAN1_FREQUENCY, ACTUAL];
                            CollectedData[CHAN1_PULSEWIDTH, DataPoints] = Data[CHAN1_PULSEWIDTH, ACTUAL];
                            CollectedData[CHAN1_DUTYCYCLE, DataPoints] = Data[CHAN1_DUTYCYCLE, ACTUAL];
                            CollectedData[CHAN2_FREQUENCY, DataPoints] = Data[CHAN2_FREQUENCY, ACTUAL];
                            CollectedData[CHAN2_PULSEWIDTH, DataPoints] = Data[CHAN2_PULSEWIDTH, ACTUAL];
                            CollectedData[CHAN2_DUTYCYCLE, DataPoints] = Data[CHAN2_DUTYCYCLE, ACTUAL];
                            DataPoints += 1;
                            if (DataPoints == MAXDATAPOINTS)
                            {
                                DataPoints = MAXDATAPOINTS - 1;
                                btnStartLoggingRaw.BackColor = Color.Red;
                            }
                        }
                    }

                    // Automatically stop powerrun once RPM1 falls low again
                    if (WhichDataMode == POWERRUN && DataPoints > MinimumPowerRunPoints && Data[RPM1_ROLLER, ACTUAL] <= ActualPowerRunThreshold)
                    {
                        SetControlBackColor_ThreadSafe(btnStartPowerRun, DefaultBackColor);
                        // DataPoints -= 1
                        PauseForms();
                        WhichDataMode = LIVE;
                    }
                }
                BufferCount = BufferCount + 1;
                if (BufferCount > NUMBER_OF_BUFFERS - 1)
                    BufferCount = 0;
#if QueryPerformance
                QueryPerformanceCounter(ref StopWatch);
                if (PerfBufferCount < 150) PerfBufferCount += 1;
                Data[PERFORMANCE, ACTUAL] = ((StopWatch - StartWatch) / (double)WatchTickConversion * 1000) / (BUFFER_SIZE / (double)SAMPLE_RATE * 1000) * 100;
                PerformanceData[P_FREQ, PerfBufferCount] = Data[CHAN1_FREQUENCY, ACTUAL];
                PerformanceData[P_TIME, PerfBufferCount] = Data[PERFORMANCE, ACTUAL];
                // 11MAY2011 Sub running at an average of 0.01 secs with buffers being retured at 3200 (buffsize) / 44100 (rate) which is 0.07 seconds
                // 29NOV2012 Sub running at average 8 ms with theoretical callback every 72 ms
                // 17DEC12 Sub Running at 8 - 10 ms with theoretical callback every 72 ms
                // 12APR13 Sub Running at around 14 ms 
                // 19SEP13 Sub Running at around 20 ms for 2 channels, 44100 Hz, doesn't seem to be significantly affected id COM port is open.
                // 19SEP13 Max Processing time for 2Ch, 44K, COM Port 9600 Baud is 47ms
#endif
            }
            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("myWaveHandler_ProcessWave Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                Environment.Exit(0);
            }
        }
        private void ShutDownWaves()
        {
            if (WavesStarted)
            {
                try
                {
                    int temp;
                    int counter;
                    StopAddingBuffers = true;
                    do
                    {
                        Application.DoEvents();
                        temp = 0;
                        for (counter = 0; counter <= NUMBER_OF_BUFFERS - 1; counter++)
                        {
                            if (WaveBufferHeaders[counter].dwBytesRecorded == WaveBufferHeaders[counter].dwBufferLength)
                            {
                                temp = temp + 1;
                            }
                        }
                    }
                    while (temp != NUMBER_OF_BUFFERS); // Not InCallBackProcedure
                    i = waveInReset(WaveInHandle);
                    if (i != 0)
                    {
                        btnHide_Click(this, EventArgs.Empty);
                        Interaction.MsgBox("ShutDownWaves / waveInReset Error", MsgBoxStyle.Exclamation);
                        Environment.Exit(0);
                    }
                    i = waveInStop(WaveInHandle);
                    if (i != 0)
                    {
                        btnHide_Click(this, EventArgs.Empty);
                        Interaction.MsgBox("ShutDownWaves / waveInStop Error", MsgBoxStyle.Exclamation);
                        Environment.Exit(0);
                    }
                    for (j = 0; j <= NUMBER_OF_BUFFERS - 1; j++)
                    {
                        i = waveInUnprepareHeader(WaveInHandle, ref WaveBufferHeaders[j], Marshal.SizeOf(WaveBufferHeaders[j]));
                        if (i != 0)
                        {
                            btnHide_Click(this, EventArgs.Empty);
                            Interaction.MsgBox("ShutDownWaves / waveInUnprepareHeader Error" + i, MsgBoxStyle.Exclamation);
                            Environment.Exit(0);
                        }
                    }
                    bufferpin0.Free();
                    bufferpin1.Free();
                    bufferpin2.Free();
                    bufferpin3.Free();
                    bufferpin4.Free();
                    bufferpin5.Free();
                    bufferpin6.Free();
                    bufferpin7.Free();
                    bufferpin8.Free();
                    bufferpin9.Free();
                    bufferpin10.Free();
                    bufferpin11.Free();
                    bufferpin12.Free();
                    bufferpin13.Free();
                    bufferpin14.Free();
                    bufferpin15.Free();
                    bufferpin16.Free();
                    bufferpin17.Free();
                    bufferpin18.Free();
                    bufferpin19.Free();
                    i = waveInClose(WaveInHandle);
                    if (i != 0)
                    {
                        btnHide_Click(this, EventArgs.Empty);
                        Interaction.MsgBox("ShutDownWaves / waveInClose Error", MsgBoxStyle.Exclamation);
                        Environment.Exit(0);
                    }
                    else
                    {

                    }
                    BufferCount = 0;
                    StopAddingBuffers = false;
                    WavesStarted = false;
                }
                catch (Exception e)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("ShutDownWaves Error: " + e.ToString(), MsgBoxStyle.Exclamation);
                    Environment.Exit(0);
                }
            }
        }
        #endregion
        #region Power run filename handling
        private void SessionTextBox_TextChanged(object sender, EventArgs e)
        {
            updateSessionPostfix();
        }

        private void SetDirButton_Click(object sender, EventArgs e)
        {
            if (txtPowerrunDir.Text.Length > 0)
            {
                SelectFolderDialog.SelectedPath = txtPowerrunDir.Text;
            }

            if (SelectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                PowerrunDirLabel.Text = SelectFolderDialog.SelectedPath;
                txtPowerrunDir.Text = SelectFolderDialog.SelectedPath;
            }
        }

        private void txtPowerrunDir_TextChanged(object sender, EventArgs e)
        {
            // This is an invisible box so text validation should not be needed
            PowerrunDirLabel.Text = "Power run dir: " + txtPowerrunDir.Text;
            updateSessionPostfix();
        }

        public void updateSessionPostfix()
        {
            if (!Directory.Exists(txtPowerrunDir.Text))
            {
                return;
            }

            string postfixProto = "";
            string filePathProto = "";

            for (int index = 1; index <= 999; index++)
            {
                postfixProto = "_" + index.ToString() + ".sdp";
                filePathProto = txtPowerrunDir.Text + @"\" + SessionTextBox.Text + postfixProto;
                if (!File.Exists(filePathProto))
                {
                    break;
                }
            }
            PostfixLabel.Text = postfixProto;
        }
        #endregion
        #region Serial Port Communications
        private void cmbAcquisition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Strings.InStr(cmbAcquisition.SelectedItem.ToString(), "Audio") != 0)
            {
                cmbChannels.Enabled = true;
                cmbSampleRate.Enabled = true;
            }
            else
            {
                cmbChannels.Enabled = false;
                cmbSampleRate.Enabled = false;
            }
            if (Strings.InStr(cmbAcquisition.SelectedItem.ToString(), "COM") != 0)
            {
                cmbCOMPorts.Enabled = true;
                cmbBaudRate.Enabled = true;
                {
                    ref var withBlock = ref s_frmFit;
                    withBlock.rdoCurrent.Enabled = true;
                    withBlock.rdoVoltage.Enabled = true;
                    withBlock.txtCurrentSmooth.Enabled = true;
                    withBlock.txtVoltageSmooth.Enabled = true;
                    withBlock.scrlCurrentSmooth.Enabled = true;
                    withBlock.scrlVoltageSmooth.Enabled = true;
                }
            }
            else
            {
                cmbCOMPorts.Enabled = false;
                cmbBaudRate.Enabled = false;
                {
                    ref var withBlock1 = ref s_frmFit;
                    withBlock1.rdoCurrent.Enabled = false;
                    withBlock1.rdoVoltage.Enabled = false;
                    withBlock1.txtCurrentSmooth.Enabled = false;
                    withBlock1.txtVoltageSmooth.Enabled = false;
                    withBlock1.scrlCurrentSmooth.Enabled = false;
                    withBlock1.scrlVoltageSmooth.Enabled = false;
                }
            }
        }
        private void btnStartAcquisition_Click(object sender, EventArgs e)
        {
            btnStartAcquisition.Enabled = false;
            PauseForms();
            ShutDownWaves();
            SerialClose();
            SignalWindowBMP.Clear(DefaultBackColor);
            pnlSignalWindow.BackgroundImage = SignalBitmap;
            pnlSignalWindow.Invalidate();
            SetControlBackColor_ThreadSafe(lblCOMActive, DefaultBackColor);
            // Set all parameters to be available in interface components
            for (int count = 1; count <= LAST - 1; count++)
                DataAreUsed[count] = true;

            // Disable all calibrate buttons on the COM form
            foreach (Control c in s_frmCOM.Controls)
            {
                if (c is Button)
                {
                    c.Enabled = false;
                }
            }

            if (Strings.InStr(cmbAcquisition.SelectedItem.ToString(), "Audio") != 0)
            {
                SAMPLE_RATE = Conversions.ToInteger(cmbSampleRate.SelectedItem.ToString().Substring(0, cmbSampleRate.SelectedItem.ToString().IndexOf(" ")));
                NUMBER_OF_CHANNELS = Conversions.ToInteger(cmbChannels.SelectedItem.ToString().Substring(0, cmbChannels.SelectedItem.ToString().IndexOf(" ")));
#if QueryPerformanc
                BUFFER_SIZE = Conversions.ToInteger(cmbBufferSize.SelectedItem.ToString().Substring(0, cmbBufferSize.SelectedItem.ToString().IndexOf(" "))) * NUMBER_OF_CHANNELS;
#else
                switch (SAMPLE_RATE)
                {
                    case var @case when @case == 11025:
                    {
                        BUFFER_SIZE = 1024 * NUMBER_OF_CHANNELS;
                        break;
                    }
                    case var case1 when case1 == 22050:
                    {
                        BUFFER_SIZE = 2048 * NUMBER_OF_CHANNELS;
                        break;
                    }
                    case var case2 when case2 == 44100:
                    {
                        BUFFER_SIZE = 4096 * NUMBER_OF_CHANNELS;
                        break;
                    }
                }
#endif
                if (NUMBER_OF_CHANNELS == 1)
                {
                    DataAreUsed[RPM2] = false;
                    DataAreUsed[RPM2_RATIO] = false;
                    DataAreUsed[RPM2_ROLLOUT] = false;
                    DataAreUsed[CHAN2_FREQUENCY] = false;
                    DataAreUsed[CHAN2_PULSEWIDTH] = false;
                    DataAreUsed[CHAN2_DUTYCYCLE] = false;
                }

                RawWaveData = new byte[BUFFER_SIZE];
                RawWaveData0 = new byte[BUFFER_SIZE];
                RawWaveData1 = new byte[BUFFER_SIZE];
                RawWaveData2 = new byte[BUFFER_SIZE];
                RawWaveData3 = new byte[BUFFER_SIZE];
                RawWaveData4 = new byte[BUFFER_SIZE];
                RawWaveData5 = new byte[BUFFER_SIZE];
                RawWaveData6 = new byte[BUFFER_SIZE];
                RawWaveData7 = new byte[BUFFER_SIZE];
                RawWaveData8 = new byte[BUFFER_SIZE];
                RawWaveData9 = new byte[BUFFER_SIZE];
                RawWaveData10 = new byte[BUFFER_SIZE];
                RawWaveData11 = new byte[BUFFER_SIZE];
                RawWaveData12 = new byte[BUFFER_SIZE];
                RawWaveData13 = new byte[BUFFER_SIZE];
                RawWaveData14 = new byte[BUFFER_SIZE];
                RawWaveData15 = new byte[BUFFER_SIZE];
                RawWaveData16 = new byte[BUFFER_SIZE];
                RawWaveData17 = new byte[BUFFER_SIZE];
                RawWaveData18 = new byte[BUFFER_SIZE];
                RawWaveData19 = new byte[BUFFER_SIZE];

                WaveBufferHeaders = new WAVEHDR[20];

                BytesToSeconds = 1d / (SAMPLE_RATE * NUMBER_OF_CHANNELS);
                ElapsedTimeUnit = BytesToSeconds * NUMBER_OF_CHANNELS;
                PrepareGraphicsParameters();
                InitializeWaveInput();
            }
            else
            {
                // CHECK - CLEAR THE SIGNAL WINDOW SCREEN TO GRAY IF NOT USED
                // Scope stuff only uses audio
                DataAreUsed[CHAN1_FREQUENCY] = false;
                DataAreUsed[CHAN1_PULSEWIDTH] = false;
                DataAreUsed[CHAN1_DUTYCYCLE] = false;
                DataAreUsed[CHAN2_FREQUENCY] = false;
                DataAreUsed[CHAN2_PULSEWIDTH] = false;
                DataAreUsed[CHAN2_DUTYCYCLE] = false;

            }
            try
            {
                string? portname = cmbAcquisition.SelectedItem?.ToString();
                if (portname!.ToUpper().Equals("COM", StringComparison.CurrentCultureIgnoreCase))
                {
                    string? sent_port = cmbCOMPorts?.Items[cmbCOMPorts.SelectedIndex]?.ToString();
                    SerialOpen(sent_port!, Conversions.ToInteger(cmbBaudRate.Items[cmbBaudRate.SelectedIndex]));
                }
                else
                {
                    DataAreUsed[VOLTS] = false;
                    DataAreUsed[AMPS] = false;
                    DataAreUsed[WATTS_IN] = false;
                    DataAreUsed[EFFICIENCY] = false;
                    DataAreUsed[TEMPERATURE1] = false;
                    DataAreUsed[TEMPERATURE2] = false;
                    DataAreUsed[PIN04VALUE] = false;
                    DataAreUsed[PIN05VALUE] = false;
                }
            }
            catch
            {
                throw new NullReferenceException("ComAquisition ComboBox or ComPorts ComboBox selected is null.");
            }
            btnResetMaxima_Click(this, EventArgs.Empty);
            btnStartAcquisition.Enabled = true;

            // Need to update the menus available in already loaded graphical interface components
            // CHECK = Ultimately we should reconsider creating the menus live on right click to allow for min max to be listed in textboxes
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.CreateTheMenu();
            RestartForms();
            triggerSimulationIfAvailable = true;
        }
        private bool ClosingCOMPort = false;
        private void GetAvailableCOMPorts()
        {
            string Problem;
            Problem = "No Value";
            try
            {
                Problem = "Setting COM Available to false";
                m_COMPortsAvailable = false;
                Problem = "Getting Names of Serial Ports";
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 0)
                {
                    Problem = "About to check port name";
                    m_COMPortsAvailable = true;
                    for (int i = 0; i < ports.Length; i++)
                    {
                        Problem = "Found COM - adding to CMB";
                        cmbCOMPorts.Items.Add(ports[i]);
                    }
                }
                else
                {
                    Problem = "Could not get any serial ports.";
                    throw new Exception();
                }
                Problem = "Check Available COM Ports Status";
                if (m_COMPortsAvailable)
                {
                    Problem = "Yes - COM port available setting index to 0";
                    cmbCOMPorts.SelectedIndex = 0;
                    Problem = "Creating Baud string";
                    string[] AvailableBauds = new string[] { 9600.ToString(), 14400.ToString(), 19200.ToString(), 28800.ToString(), 38400.ToString(), 57600.ToString(), 115200.ToString() };
                    Problem = "Adding Baud to CMB";
                    cmbBaudRate.Items.AddRange(AvailableBauds);
                    Problem = "Setting Index to 0";
                    cmbBaudRate.SelectedIndex = 0;
                }
            }
            // MsgBox("No Problems Found", MsgBoxStyle.OkOnly)
            catch (Exception ex)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox($"Error found is \"{Problem}\" {ex.Message}", MsgBoxStyle.Exclamation);
                btnShow_Click(this, EventArgs.Empty);
            }
        }
        private void SerialOpen(string SentPort, int SentRate)
        {
            while (m_serialPort.IsOpen != false)
                Application.DoEvents();
            SentPort = "COM" + SentPort.Substring(SentPort.IndexOf("(COM") + 4).TrimEnd(')');
            m_serialPort = new SerialPort(SentPort);
            m_serialPort.BaudRate = SentRate;
            m_serialPort.Parity = Parity.None;
            m_serialPort.StopBits = StopBits.One;
            m_serialPort.DataBits = 8;
            m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 500;
            // AddHandler m_serialPort.DataReceived, AddressOf DataReceivedHandler
            try
            {
                m_serialPort.Open();
                m_serialPort.DiscardInBuffer();
                m_serialPort.DataReceived += DataReceivedHandler;
                // Enable Calibration buttons on com form
                foreach (Control c in s_frmCOM.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = true;
                    }
                }
            }
            catch (Exception e)
            {
                btnHide_Click(this, EventArgs.Empty);
                Interaction.MsgBox("Error reading COM Port.", (MsgBoxStyle)Constants.vbOK);
                if (m_serialPort.IsOpen)
                    m_serialPort.Close();
                // Enable Calibration buttons on com form
                foreach (Control c in s_frmCOM.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = false;
                    }
                }
                btnShow_Click(this, EventArgs.Empty);
            }
        }
        private void SerialClose()
        {
            ClosingCOMPort = true;
            if (m_serialPort.IsOpen)
            {
                m_serialPort.DataReceived -= DataReceivedHandler;
                var t = default(int);
                // CHECK - this is a real hack
                while (t != 100000)
                {
                    t += 1;
                    Application.DoEvents();
                }
                m_serialPort.Close();
            }
            ClosingCOMPort = false;
        }
        private int _DataReceivedLine_rgbvalue = 150;
        private int _DataReceivedLine_rgbincrement = 10;
        private double _DataReceivedLine_OldSessionTime = default;
        private double _DataReceivedLine_RPM1OldTriggerTime = default;
        private double _DataReceivedLine_RPM2OldTriggerTime = default;
        private void DataReceivedLine(string line)
        {
            if (!ClosingCOMPort)
            {
                try
                {
                    var Locker = new object();
                    double RPM1ElapsedTime;
                    var RPM1NewTriggerTime = default(double);
                    double RPM2ElapsedTime;
                    var RPM2NewTriggerTime = default(double);

                    COMPortMessage = Strings.Split(line, ",");

                    _DataReceivedLine_rgbvalue += _DataReceivedLine_rgbincrement;
                    if (_DataReceivedLine_rgbvalue > 240 | _DataReceivedLine_rgbvalue < 50)
                        _DataReceivedLine_rgbincrement *= -1;
                    SetControlBackColor_ThreadSafe(lblCOMActive, Color.FromArgb(0, _DataReceivedLine_rgbvalue, _DataReceivedLine_rgbvalue));

                    if (COMPortMessage.Length == 11) // Timestamp, 2 new Time values,  2 interrupt times, 6 ports
                    {
                        lock (Locker)
                        {
                            Data[VOLTS, ACTUAL] = VoltageIntercept + VoltageSlope * Conversions.ToDouble(COMPortMessage[5]); // Convert Volts signal to volts
                            Data[AMPS, ACTUAL] = CurrentIntercept + CurrentSlope * Conversions.ToDouble(COMPortMessage[6]); // Convert Current signal to amps
                            Data[TEMPERATURE1, ACTUAL] = Temperature1Intercept + Temperature1Slope * Conversions.ToDouble(COMPortMessage[7]);
                            Data[TEMPERATURE2, ACTUAL] = Temperature2Intercept + Temperature2Slope * Conversions.ToDouble(COMPortMessage[8]);
                            Data[PIN04VALUE, ACTUAL] = A4ValueIntercept + A4ValueSlope * Conversions.ToDouble(COMPortMessage[9]);
                            Data[PIN05VALUE, ACTUAL] = A5ValueIntercept + A5ValueSlope * Conversions.ToDouble(COMPortMessage[10]);

                            if (s_frmCOM.Calibrating)
                            {
                                for (int t = 0; t <= 5; t++)
                                    s_frmCOM.CalibrationValues[t] += Conversions.ToDouble(COMPortMessage[t + 5]);
                                s_frmCOM.NumberOfCalibrationValues += 1L;
                            }

                            Data[WATTS_IN, ACTUAL] = Data[VOLTS, ACTUAL] * Data[AMPS, ACTUAL];

                            if (Data[AMPS, ACTUAL] < Data[AMPS, MINIMUM])
                                Data[AMPS, MINIMUM] = Data[AMPS, ACTUAL];
                            if (Data[AMPS, ACTUAL] > Data[AMPS, MAXIMUM])
                                Data[AMPS, MAXIMUM] = Data[AMPS, ACTUAL];
                            if (Data[VOLTS, ACTUAL] < Data[VOLTS, MINIMUM])
                                Data[VOLTS, MINIMUM] = Data[VOLTS, ACTUAL];
                            if (Data[VOLTS, ACTUAL] > Data[VOLTS, MAXIMUM])
                                Data[VOLTS, MAXIMUM] = Data[VOLTS, ACTUAL];
                            if (Data[WATTS_IN, ACTUAL] < Data[WATTS_IN, MINIMUM])
                                Data[WATTS_IN, MINIMUM] = Data[WATTS_IN, ACTUAL];
                            if (Data[WATTS_IN, ACTUAL] > Data[WATTS_IN, MAXIMUM])
                                Data[WATTS_IN, MAXIMUM] = Data[WATTS_IN, ACTUAL];
                            if (Data[TEMPERATURE1, ACTUAL] > Data[TEMPERATURE1, MAXIMUM])
                                Data[TEMPERATURE1, MAXIMUM] = Data[TEMPERATURE1, ACTUAL];
                            if (Data[TEMPERATURE1, ACTUAL] < Data[TEMPERATURE1, MINIMUM])
                                Data[TEMPERATURE1, MINIMUM] = Data[TEMPERATURE1, ACTUAL];
                            if (Data[TEMPERATURE2, ACTUAL] > Data[TEMPERATURE2, MAXIMUM])
                                Data[TEMPERATURE2, MAXIMUM] = Data[TEMPERATURE2, ACTUAL];
                            if (Data[TEMPERATURE2, ACTUAL] < Data[TEMPERATURE2, MINIMUM])
                                Data[TEMPERATURE2, MINIMUM] = Data[TEMPERATURE2, ACTUAL];

                            if (Data[PIN04VALUE, ACTUAL] > Data[PIN04VALUE, MAXIMUM])
                                Data[PIN04VALUE, MAXIMUM] = Data[PIN04VALUE, ACTUAL];
                            if (Data[PIN04VALUE, ACTUAL] < Data[PIN04VALUE, MINIMUM])
                                Data[PIN04VALUE, MINIMUM] = Data[PIN04VALUE, ACTUAL];

                            if (Data[PIN05VALUE, ACTUAL] > Data[PIN05VALUE, MAXIMUM])
                                Data[PIN05VALUE, MAXIMUM] = Data[PIN05VALUE, ACTUAL];
                            if (Data[PIN05VALUE, ACTUAL] < Data[PIN05VALUE, MINIMUM])
                                Data[PIN05VALUE, MINIMUM] = Data[PIN05VALUE, ACTUAL];

                            if (!WavesStarted) // Use COM data for timing and RPM
                            {
                                Data[SESSIONTIME, ACTUAL] = Conversions.ToDouble(COMPortMessage[0]) / 1000000d; // Increase the session time value - used by the Graphing form for time info
                                ElapsedTime = Data[SESSIONTIME, ACTUAL] - _DataReceivedLine_OldSessionTime; // CHECK - Are we actually using this anywhere for com communications?
                                RPM1ElapsedTime = Conversions.ToDouble(COMPortMessage[2]) / 1000000d; // RPM1
                                RPM1NewTriggerTime = Conversions.ToDouble(COMPortMessage[1]) / 1000000d - RPM1ElapsedTime / 2d; // Set RPM1 timestamp in the middle of the trigger interval
                                if (RPM1NewTriggerTime != _DataReceivedLine_RPM1OldTriggerTime) // New trigger detected, go ahead and process RPM relevant info
                                {
                                    Data[RPM1_ROLLER, ACTUAL] = ElapsedTimeToRadPerSec / RPM1ElapsedTime;
                                    Data[RPM1_WHEEL, ACTUAL] = Data[RPM1_ROLLER, ACTUAL] * RollerRPMtoWheelRPM; // calculate the wheel and motor angular velocities based on roller and wheel diameters and gear ratio
                                    Data[RPM1_MOTOR, ACTUAL] = Data[RPM1_WHEEL, ACTUAL] * GearRatio;
                                    Data[SPEED, ACTUAL] = Data[RPM1_ROLLER, ACTUAL] * RollerRadsPerSecToMetersPerSec; // calculate the speed (meters/s) based on roller rad/s
                                    Data[DRAG, ACTUAL] = Math.Pow(Data[SPEED, ACTUAL], 3d) * ForceAir; // calculate drag based on vehicle speed (meters/s)
                                    if (Data[RPM1_ROLLER, ACTUAL] > Data[RPM1_ROLLER, MAXIMUM]) // set the maximum values for roller, wheel, and motor RPM; Speed and Drag
                                    {
                                        Data[RPM1_ROLLER, MAXIMUM] = Data[RPM1_ROLLER, ACTUAL];
                                        Data[RPM1_WHEEL, MAXIMUM] = Data[RPM1_WHEEL, ACTUAL];
                                        Data[RPM1_MOTOR, MAXIMUM] = Data[RPM1_MOTOR, ACTUAL];
                                        Data[SPEED, MAXIMUM] = Data[SPEED, ACTUAL];
                                        Data[DRAG, MAXIMUM] = Data[DRAG, ACTUAL];
                                    }
                                    if (Data[RPM1_ROLLER, ACTUAL] < Data[RPM1_ROLLER, MINIMUM]) // set the maximum values for roller, wheel, and motor RPM; Speed and Drag
                                    {
                                        Data[RPM1_ROLLER, MINIMUM] = Data[RPM1_ROLLER, ACTUAL];
                                        Data[RPM1_WHEEL, MINIMUM] = Data[RPM1_WHEEL, ACTUAL];
                                        Data[RPM1_MOTOR, MINIMUM] = Data[RPM1_MOTOR, ACTUAL];
                                        Data[SPEED, MINIMUM] = Data[SPEED, ACTUAL];
                                        Data[DRAG, MINIMUM] = Data[DRAG, ACTUAL];
                                    }
                                    // calculate torque based on angular acceleration (delta speed per time) and MOI
                                    Data[TORQUE_ROLLER, ACTUAL] = (Data[RPM1_ROLLER, ACTUAL] - OldAngularVelocity) / (RPM1NewTriggerTime - _DataReceivedLine_RPM1OldTriggerTime) * DynoMomentOfInertia;
                                    Data[POWER, ACTUAL] = Data[TORQUE_ROLLER, ACTUAL] * Data[RPM1_ROLLER, ACTUAL]; // + OldAngularVelocity) / 2) 'calculate power based on torque and average angular velocity between two points
                                    if (Data[POWER, ACTUAL] > 0d)
                                    {
                                        Data[EFFICIENCY, ACTUAL] = Data[WATTS_IN, ACTUAL] / Data[POWER, ACTUAL];
                                    }
                                    else
                                    {
                                        Data[EFFICIENCY, ACTUAL] = 0d;
                                    }
                                    Data[TORQUE_WHEEL, ACTUAL] = Data[POWER, ACTUAL] / Data[RPM1_WHEEL, ACTUAL]; // back calculate the torque at the wheel and motor based on the calculated power
                                    Data[TORQUE_MOTOR, ACTUAL] = Data[POWER, ACTUAL] / Data[RPM1_MOTOR, ACTUAL];

                                    if (Data[TORQUE_ROLLER, ACTUAL] > Data[TORQUE_ROLLER, MAXIMUM]) // set the maximum values for torque
                                    {
                                        Data[TORQUE_ROLLER, MAXIMUM] = Data[TORQUE_ROLLER, ACTUAL];
                                        Data[TORQUE_WHEEL, MAXIMUM] = Data[TORQUE_WHEEL, ACTUAL];
                                        Data[TORQUE_MOTOR, MAXIMUM] = Data[TORQUE_MOTOR, ACTUAL];
                                    }
                                    if (Data[TORQUE_ROLLER, ACTUAL] < Data[TORQUE_ROLLER, MINIMUM]) // set the maximum values for torque
                                    {
                                        Data[TORQUE_ROLLER, MINIMUM] = Data[TORQUE_ROLLER, ACTUAL];
                                        Data[TORQUE_WHEEL, MINIMUM] = Data[TORQUE_WHEEL, ACTUAL];
                                        Data[TORQUE_MOTOR, MINIMUM] = Data[TORQUE_MOTOR, ACTUAL];
                                    }
                                    if (Data[POWER, ACTUAL] > Data[POWER, MAXIMUM]) // set the maximum value for power
                                    {
                                        Data[POWER, MAXIMUM] = Data[POWER, ACTUAL];
                                    }
                                    if (Data[POWER, ACTUAL] < Data[POWER, MINIMUM]) // set the maximum value for power
                                    {
                                        Data[POWER, MINIMUM] = Data[POWER, ACTUAL];
                                    }
                                    if (Data[EFFICIENCY, ACTUAL] > Data[EFFICIENCY, MAXIMUM]) // set the maximum value for power
                                    {
                                        Data[EFFICIENCY, MAXIMUM] = Data[EFFICIENCY, ACTUAL];
                                    }
                                    if (Data[EFFICIENCY, ACTUAL] < Data[EFFICIENCY, MINIMUM]) // set the maximum value for power
                                    {
                                        Data[EFFICIENCY, MINIMUM] = Data[EFFICIENCY, ACTUAL];
                                    }
                                    OldAngularVelocity = Data[RPM1_ROLLER, ACTUAL]; // remember the current angular velocity for next RPM reading
                                    switch (WhichDataMode)
                                    {
                                        case var @case when @case == LIVE:
                                            {
                                                break;
                                            }
                                        // Don't do anything.  This helps skip through the Select Case Faster
                                        case var case1 when case1 == POWERRUN:
                                            {
                                                if (Data[RPM1_ROLLER, ACTUAL] > ActualPowerRunThreshold)
                                                {
                                                    DataPoints += 1;
                                                    if (DataPoints == 1)
                                                    {
                                                        TotalElapsedTime = 0d;
                                                    }
                                                    else
                                                    {
                                                        TotalElapsedTime += RPM1NewTriggerTime - _DataReceivedLine_RPM1OldTriggerTime;
                                                    }
                                                    if (DataPoints == MinimumPowerRunPoints)
                                                        btnStartPowerRun.BackColor = Color.Green;
                                                    CollectedData[SESSIONTIME, DataPoints] = TotalElapsedTime;
                                                    CollectedData[RPM1_ROLLER, DataPoints] = Data[RPM1_ROLLER, ACTUAL];
                                                    CollectedData[RPM2, DataPoints] = Data[RPM2, ACTUAL];
                                                    CollectedData[VOLTS, DataPoints] = Data[VOLTS, ACTUAL];
                                                    CollectedData[AMPS, DataPoints] = Data[AMPS, ACTUAL];
                                                    CollectedData[TEMPERATURE1, DataPoints] = Data[TEMPERATURE1, ACTUAL];
                                                    CollectedData[TEMPERATURE2, DataPoints] = Data[TEMPERATURE2, ACTUAL];
                                                    CollectedData[PIN04VALUE, DataPoints] = Data[PIN04VALUE, ACTUAL];
                                                    CollectedData[PIN05VALUE, DataPoints] = Data[PIN05VALUE, ACTUAL];
                                                    // DataPoints += 1
                                                    if (DataPoints == MAXDATAPOINTS)
                                                    {
                                                        DataPoints = MAXDATAPOINTS - 1;
                                                        btnStartPowerRun.BackColor = Color.Red;
                                                    }
                                                }

                                                break;
                                            }

                                        case var case2 when case2 == LOGRAW:
                                            {
                                                DataPoints += 1;
                                                if (DataPoints == 1)
                                                {
                                                    TotalElapsedTime = 0d;
                                                    btnStartLoggingRaw.BackColor = Color.Green;
                                                }
                                                else
                                                {
                                                    TotalElapsedTime += ElapsedTime;
                                                }
                                                CollectedData[SESSIONTIME, DataPoints] = TotalElapsedTime;
                                                CollectedData[RPM1_ROLLER, DataPoints] = Data[RPM1_ROLLER, ACTUAL];
                                                CollectedData[RPM2, DataPoints] = Data[RPM2, ACTUAL];
                                                CollectedData[VOLTS, DataPoints] = Data[VOLTS, ACTUAL];
                                                CollectedData[AMPS, DataPoints] = Data[AMPS, ACTUAL];
                                                CollectedData[TEMPERATURE1, DataPoints] = Data[TEMPERATURE1, ACTUAL];
                                                CollectedData[TEMPERATURE2, DataPoints] = Data[TEMPERATURE2, ACTUAL];
                                                CollectedData[PIN04VALUE, DataPoints] = Data[PIN04VALUE, ACTUAL];
                                                CollectedData[PIN05VALUE, DataPoints] = Data[PIN05VALUE, ACTUAL];
                                                // DataPoints += 1
                                                if (DataPoints == MAXDATAPOINTS)
                                                {
                                                    DataPoints = MAXDATAPOINTS - 1;
                                                }

                                                break;
                                            }
                                    }
                                }
                                else if (Data[SESSIONTIME, ACTUAL] - RPM1NewTriggerTime > WaitForNewSignal)
                                {
                                    Data[RPM1_ROLLER, ACTUAL] = 0d;
                                    Data[RPM1_WHEEL, ACTUAL] = 0d;
                                    Data[RPM1_MOTOR, ACTUAL] = 0d;
                                    Data[SPEED, ACTUAL] = 0d;
                                    Data[TORQUE_ROLLER, ACTUAL] = 0d;
                                    Data[TORQUE_WHEEL, ACTUAL] = 0d;
                                    Data[TORQUE_MOTOR, ACTUAL] = 0d;
                                    Data[POWER, ACTUAL] = 0d;
                                    Data[RPM2_RATIO, ACTUAL] = 0d;
                                }
                                RPM2NewTriggerTime = Conversions.ToDouble(COMPortMessage[3]) / 1000000d;
                                RPM2ElapsedTime = Conversions.ToDouble(COMPortMessage[4]) / 1000000d;
                                if (RPM2NewTriggerTime != _DataReceivedLine_RPM2OldTriggerTime)
                                {
                                    Data[RPM2, ACTUAL] = ElapsedTimeToRadPerSec / RPM2ElapsedTime;
                                    Data[RPM2_RATIO, ACTUAL] = Data[RPM2, ACTUAL] / Data[RPM1_WHEEL, ACTUAL];
                                    Data[RPM2_ROLLOUT, ACTUAL] = WheelCircumference / Data[RPM2_RATIO, ACTUAL];
                                    if (Data[RPM2, ACTUAL] > Data[RPM2, MAXIMUM])
                                    {
                                        Data[RPM2, MAXIMUM] = Data[RPM2, ACTUAL];
                                    }
                                    if (Data[RPM2_RATIO, ACTUAL] > Data[RPM2_RATIO, MAXIMUM])
                                    {
                                        Data[RPM2_RATIO, MAXIMUM] = Data[RPM2_RATIO, ACTUAL];
                                    }
                                    if (Data[RPM2_ROLLOUT, ACTUAL] > Data[RPM2_ROLLOUT, MAXIMUM])
                                    {
                                        Data[RPM2_ROLLOUT, MAXIMUM] = Data[RPM2_ROLLOUT, ACTUAL];
                                    }
                                    if (Data[RPM2, ACTUAL] < Data[RPM2, MINIMUM])
                                    {
                                        Data[RPM2, MINIMUM] = Data[RPM2, ACTUAL];
                                    }
                                    if (Data[RPM2_RATIO, ACTUAL] < Data[RPM2_RATIO, MINIMUM])
                                    {
                                        Data[RPM2_RATIO, MINIMUM] = Data[RPM2_RATIO, ACTUAL];
                                    }
                                    if (Data[RPM2_ROLLOUT, ACTUAL] < Data[RPM2_ROLLOUT, MINIMUM])
                                    {
                                        Data[RPM2_ROLLOUT, MINIMUM] = Data[RPM2_ROLLOUT, ACTUAL];
                                    }
                                }
                                else if (Data[SESSIONTIME, ACTUAL] - RPM2NewTriggerTime > WaitForNewSignal)
                                {
                                    Data[RPM2, ACTUAL] = 0d;
                                    Data[RPM2_RATIO, ACTUAL] = 0d;
                                    Data[RPM2_ROLLOUT, ACTUAL] = 0d;
                                }
                            }

                            _DataReceivedLine_OldSessionTime = Data[SESSIONTIME, ACTUAL];
                            _DataReceivedLine_RPM1OldTriggerTime = RPM1NewTriggerTime;
                            _DataReceivedLine_RPM2OldTriggerTime = RPM2NewTriggerTime;

                        }

                        DoPowerRunSerialControl(A5ValueIntercept + A5ValueSlope * Conversions.ToDouble(COMPortMessage[10]), COMPortMessage);

                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentVolts, NewCustomFormat(Data[VOLTS, ACTUAL]));
                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentAmps, NewCustomFormat(Data[AMPS, ACTUAL]));
                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentTemperature1, NewCustomFormat(Data[TEMPERATURE1, ACTUAL]));
                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentTemperature2, NewCustomFormat(Data[TEMPERATURE2, ACTUAL]));
                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentPinA4, NewCustomFormat(Data[PIN04VALUE, ACTUAL]));
                        this.SetControlText_Threadsafe(s_frmCOM.lblCurrentPinA5, NewCustomFormat(Data[PIN05VALUE, ACTUAL]));

                    }
                }
                catch (Exception ex)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("Serial Port Data Received Error: " + ex.ToString(), MsgBoxStyle.Exclamation);
                    // btnShow_Click(Me, EventArgs.Empty)
                    Environment.Exit(0);
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (!ClosingCOMPort)
            {
                try
                {
                    SerialPort sp = (SerialPort)sender;
                    string temp;
                    temp = sp.ReadLine();
                    DataReceivedLine(temp);
                }
                catch (Exception ex)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("Serial Port Data Received Error: " + ex.ToString(), MsgBoxStyle.Exclamation);
                    // btnShow_Click(Me, EventArgs.Empty)
                    Environment.Exit(0);
                }
            }
        }
        private bool _DoPowerRunSerialControl_A5PrevState = false;
        private bool _DoPowerRunSerialControl_A5FirstFalseDetected = false;
        private Stopwatch _DoPowerRunSerialControl_bouncePreventTimer = Stopwatch.StartNew();

        // Serial port triggered events for the power run state
        private void DoPowerRunSerialControl(double A5Value, string[] message)
        {
            if (A5PowerRunControl)
            {
                double A5Threshold = A5ValueIntercept + (A5Value2 - A5Value1) / 2d;
                bool A5State = A5Value > A5Threshold;

                if (!_DoPowerRunSerialControl_A5FirstFalseDetected & !A5State)
                {
                    _DoPowerRunSerialControl_A5FirstFalseDetected = true;
                }

                // Wait for A5 control to be off before considering its value
                if (_DoPowerRunSerialControl_bouncePreventTimer.ElapsedMilliseconds > 1000L & _DoPowerRunSerialControl_A5FirstFalseDetected)
                {
                    // Only trigger from A5 changes so that autostop due to roller stop is possible even when A5 remains up
                    if (WhichDataMode == POWERRUN & _DoPowerRunSerialControl_A5PrevState & !A5State)
                    {
                        TogglePowerRun();
                        _DoPowerRunSerialControl_bouncePreventTimer = Stopwatch.StartNew();
                    }
                    else if (WhichDataMode != POWERRUN & A5State & !_DoPowerRunSerialControl_A5PrevState)
                    {
                        TogglePowerRun();
                        _DoPowerRunSerialControl_bouncePreventTimer = Stopwatch.StartNew();
                    }
                    _DoPowerRunSerialControl_A5PrevState = A5State;
                }

                // Manually started powerrun automatically stops when roller RPM fall to zero
                if (WhichDataMode == POWERRUN && DataPoints > MinimumPowerRunPoints && Data[RPM1_ROLLER, ACTUAL] <= ActualPowerRunThreshold)
                {
                    TogglePowerRun();
                }
            }
        }

        private void TogglePowerRun()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TogglePowerRun));
            }
            else
            {
                try
                {
                    // Regardless of why we clicked it, the radio button for rpm on the fit form should be set for RPM
                    s_frmFit.rdoRPM1.Checked = true;
                    if (WhichDataMode == POWERRUN) // We are cancelling the power run
                    {
                        btnStartLoggingRaw.Enabled = true;
                        btnShow_Click(this, EventArgs.Empty);
                        {
                            var withBlock = btnStartPowerRun;
                            withBlock.BackColor = DefaultBackColor;
                        }

                        if (DataPoints <= MinimumPowerRunPoints)
                        {
                            // Unable to proceed with fitting if not enough data
                            StopFitting = true;
                        }

                        WhichDataMode = LIVE; // This will tell Fit.vb that poer run has ended and it is time to fit
                    }
                    else
                    {
                        btnHide_Click(this, EventArgs.Empty);

                        LogPowerRunDataFileName = txtPowerrunDir.Text + @"\" + SessionTextBox.Text + PostfixLabel.Text;
                        ResetValues();
                        DataPoints = 0;
                        // DataPoints2 = 0
                        btnStartLoggingRaw.Enabled = false;
                        btnShow_Click(this, EventArgs.Empty);
                        {
                            var withBlock1 = btnStartPowerRun;
                            withBlock1.BackColor = Color.Red;
                        }
                        WhichDataMode = POWERRUN;
                        StopFitting = false;
                        s_frmFit.ProcessData();

                    } // Put Fit.vb in mode where it waits for data until WhichDataMode changes
                }
                catch (Exception e1)
                {
                    btnHide_Click(this, EventArgs.Empty);
                    Interaction.MsgBox("btnStartPowerRun_Click Error: " + e1.ToString(), MsgBoxStyle.Exclamation);
                    btnShow_Click(this, EventArgs.Empty);
                    Environment.Exit(0);
                }
            }
        }

        #endregion
        #region New Interface Code
        private void btnLoad_Click(object sender, EventArgs e)
        {
            btnHide_Click(this, EventArgs.Empty);
            {
                var withBlock = OpenFileDialog1;
                withBlock.Reset();
                withBlock.Filter = "Interface files (*.sdi)|*.sdi";
                withBlock.ShowDialog();
            }

            // templine dfor no reason
            if (!string.IsNullOrEmpty(OpenFileDialog1.FileName))
            {
                btnClose_Click(this, EventArgs.Empty);
                txtInterface.Text = OpenFileDialog1.FileName;
                LoadInterface();
            }
            else
            {
                switch (txtInterface.Text ?? "")
                {
                    case var @case when @case == "No Interface Loaded":
                        {
                            btnSave.Enabled = false;
                            btnSaveAs.Enabled = false;
                            btnClose.Enabled = false;
                            btnHide.Enabled = false;
                            btnShow.Enabled = false;
                            break;
                        }

                    default:
                        {
                            btnShow_Click(this, EventArgs.Empty);
                            btnSave.Enabled = true;
                            btnSaveAs.Enabled = true;
                            btnClose.Enabled = true;
                            btnHide.Enabled = true;
                            btnShow.Enabled = false;
                            break;
                        }
                }

            }
        }
        private void LoadInterface()
        {
            if (txtInterface.Text != "No Interface Loaded")
            {
                string TempString;
                var InterfaceInputFile = new StreamReader(txtInterface.Text);
                TempString = InterfaceInputFile.ReadLine();
                switch (TempString ?? "")
                {
                    case var @case when @case == InterfaceVersion:
                    case "SimpleDyno_Interface_6_4":
                    {
                        while (!InterfaceInputFile.EndOfStream)
                        {
                            TempString = InterfaceInputFile.ReadLine();
                            if (TempString == "Label")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                f.Add(new SimpleDynoSubLabel());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "Label";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                f[f.Count - 1].CreateFromSerializedData(ref TempString);
                            }
                            else if (TempString == "Gauge")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                f.Add(new SimpleDynoSubGauge());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "Gauge";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                f[f.Count - 1].CreateFromSerializedData(ref TempString);
                            }
                            else if (TempString == "MultiYTimeGraph")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                f.Add(new SimpleDynoSubMultiYTimeGraph());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "MultiYTimeGraph";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                f[f.Count - 1].CreateFromSerializedData(ref TempString);
                            }
                        }
                        btnSave.Enabled = true;
                        btnSaveAs.Enabled = true;
                        btnClose.Enabled = true;
                        btnHide.Enabled = true;
                        btnShow.Enabled = false;
                        InterfaceInputFile.Close();
                        InterfaceInputFile.Dispose();
                        break;
                    }
                    case var case1 when case1 == "SimpleDyno_Interface_6_3":
                    {
                        while (!InterfaceInputFile.EndOfStream)
                        {
                            TempString = InterfaceInputFile.ReadLine();
                            if (TempString == "Label")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                f.Add(new SimpleDynoSubLabel());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "Label";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                // This is the point to translate old pointers to new pointers

                                string argSentSerialInformation = InterfaceConvert_63_toCurrent(TempString);
                                f[f.Count - 1].CreateFromSerializedData(ref argSentSerialInformation);
                            }
                            else if (TempString == "Gauge")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                Debug.Print(TempString);
                                f.Add(new SimpleDynoSubGauge());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "Gauge";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                string argSentSerialInformation1 = InterfaceConvert_63_toCurrent(TempString);
                                f[f.Count - 1].CreateFromSerializedData(ref argSentSerialInformation1);
                            }
                            else if (TempString == "MultiYTimeGraph")
                            {
                                TempString = InterfaceInputFile.ReadLine();
                                f.Add(new SimpleDynoSubMultiYTimeGraph());
                                global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
                                global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
                                f[f.Count - 1].myType = "MultiYTimeGraph";
                                f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
                                string argSentSerialInformation2 = InterfaceConvert_63_toCurrent(TempString);
                                f[f.Count - 1].CreateFromSerializedData(ref argSentSerialInformation2);
                            }
                        }
                        btnSave.Enabled = true;
                        btnSaveAs.Enabled = true;
                        btnClose.Enabled = true;
                        btnHide.Enabled = true;
                        btnShow.Enabled = false;
                        InterfaceInputFile.Close();
                        InterfaceInputFile.Dispose();
                        btnSave_Click(this, EventArgs.Empty); // This added here to make sure any version changes are saved
                        break;
                    }

                    default:
                    {
                        btnHide_Click(this, EventArgs.Empty);
                        Interaction.MsgBox("Not a valid Interface File", Constants.vbOKOnly);
                        btnShow_Click(this, EventArgs.Empty);
                        InterfaceInputFile.Close();
                        InterfaceInputFile.Dispose();
                        break;
                    }
                }
            }
        }
        private string InterfaceConvert_63_toCurrent(string Sent)
        {
            string InterfaceConvert_63_toCurrentRet = default;
            // Receives the 6.3 version of the string and updates the x and y pointers to the new ones
            // Because the only x pointer in 6.3 was time, there is no need to convert this one.
            string[] TempSplit = Strings.Split(Sent, "_");
            int TempYNumberAllowed = Conversions.ToInteger(TempSplit[5]);
            for (int Count = 1, loopTo = TempYNumberAllowed; Count <= loopTo; Count++)
            {
                switch (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]))
                {
                    case var @case when @case == 21:
                    case 22:
                    case 23:
                    {
                        // CHAN1_FREQUENCY = 21   'CHAN1_FREQUENCY = 4
                        // CHAN1_PULSEWIDTH = 22  'CHAN1_PULSEWIDTH = 5
                        // CHAN1_DUTYCYCLE = 23   'CHAN1_DUTYCYCLE = 6
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) - 17).ToString();
                        break;
                    }
                    case var case1 when case1 == 4:
                    case 5:
                    case 6:
                    case 7:
                    {
                        // SPEED = 4              'SPEED = 7
                        // RPM2 = 5               'RPM2 = 8
                        // RPM2_RATIO = 6         'RPM2_RATIO = 9
                        // RPM2_ROLLOUT = 7       'RPM2_ROLLOUT = 10
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) + 3).ToString();
                        break;
                    }
                    case var case2 when case2 == 24:
                    case 25:
                    case 26:
                    {
                        // CHAN2_FREQUENCY = 24   'CHAN2_FREQUENCY = 11
                        // CHAN2_PULSEWIDTH = 25  'CHAN2_PULSEWIDTH = 12
                        // CHAN2_DUTYCYCLE = 26   'CHAN2_DUTYCYCLE = 13
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) - 13).ToString();
                        break;
                    }
                    case var case3 when case3 == 8:
                    case 9:
                    case 10:
                    {
                        // TORQUE_ROLLER = 8      'TORQUE_ROLLER = 14
                        // TORQUE_WHEEL = 9       'TORQUE_WHEEL = 15
                        // TORQUE_MOTOR = 10      'TORQUE_MOTOR = 16
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) + 6).ToString();
                        break;
                    }
                    case var case4 when case4 == 11:
                    {
                        // POWER = 11             'POWER = 21
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) + 10).ToString();
                        break;
                    }
                    case var case5 when case5 == 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    {
                        // DRAG = 12              'DRAG = 26
                        // VOLTS = 13             'VOLTS = 27
                        // AMPS = 14              'AMPS = 28
                        // WATTS_IN = 15          'WATTS_IN = 29
                        // EFFICIENCY = 16        'EFFICIENCY = 30
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) + 14).ToString();
                        break;
                    }
                    case var case6 when case6 == 17:
                    case var case7 when case7 == 18:
                    case var case8 when case8 == 19:
                    case var case9 when case9 == 20:
                    {
                        // TEMPERATURE1 = 17      'TEMPERATURE1 = 32
                        // TEMPERATURE2 = 18      'TEMPERATURE2 = 33
                        // PIN04VALUE = 19        'PIN04VALUE = 34
                        // PIN05VALUE = 20        'PIN05VALUE = 35
                        TempSplit[16 + (Count - 1) * 7] = (Conversions.ToInteger(TempSplit[16 + (Count - 1) * 7]) + 15).ToString();
                        break;
                    }
                }
            }

            // Now Rebuild the string
            string TempReply = "";
            for (int Count = 0, loopTo1 = Information.UBound(TempSplit); Count <= loopTo1; Count++)
                TempReply = TempReply + TempSplit[Count] + "_";
            InterfaceConvert_63_toCurrentRet = TempReply;
            return InterfaceConvert_63_toCurrentRet;

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string TempString = InterfaceVersion + Constants.vbCrLf;
            var frmcount = default(int);
            foreach (SimpleDynoSubForm SDFrm in f)
            {
                if (SDFrm.Visible != false)
                {
                    frmcount += 1;
                    if (SDFrm.myType == "Label")
                        TempString = TempString + "Label" + Constants.vbCrLf;
                    if (SDFrm.myType == "Gauge")
                        TempString = TempString + "Gauge" + Constants.vbCrLf;
                    if (SDFrm.myType == "MultiYTimeGraph")
                        TempString = TempString + "MultiYTimeGraph" + Constants.vbCrLf;
                    TempString = TempString + SDFrm.ReportForSerialization() + Constants.vbCrLf;
                }
            }
            var InterfaceOutPutFile = new StreamWriter(txtInterface.Text);
            InterfaceOutPutFile.WriteLine(TempString);
            InterfaceOutPutFile.Close();
            InterfaceOutPutFile.Dispose();
        }
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            string TempString = InterfaceVersion + Constants.vbCrLf;
            var frmcount = default(int);
            foreach (SimpleDynoSubForm SDFrm in f)
            {
                if (SDFrm.Visible != false)
                {
                    frmcount += 1;
                    if (SDFrm.myType == "Label")
                        TempString = TempString + "Label" + Constants.vbCrLf;
                    if (SDFrm.myType == "Gauge")
                        TempString = TempString + "Gauge" + Constants.vbCrLf;
                    if (SDFrm.myType == "MultiYTimeGraph")
                        TempString = TempString + "MultiYTimeGraph" + Constants.vbCrLf;
                    TempString = TempString + SDFrm.ReportForSerialization() + Constants.vbCrLf;
                }
            }
            btnHide_Click(this, EventArgs.Empty);
            {
                var withBlock = SaveFileDialog1;
                withBlock.Reset();
                withBlock.Filter = "Text files (*.sdi)|*.sdi";
                withBlock.ShowDialog();
                if (!string.IsNullOrEmpty(withBlock.FileName))
                {
                    txtInterface.Text = withBlock.FileName;
                    var InterfaceOutPutFile = new StreamWriter(txtInterface.Text);
                    InterfaceOutPutFile.WriteLine(TempString);
                    InterfaceOutPutFile.Close();
                    InterfaceOutPutFile.Dispose();
                }
            }
            btnShow_Click(this, EventArgs.Empty);
        }
        internal void btnHide_Click(object sender, EventArgs e)
        {
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.HideYourSelf();
            btnShow.Enabled = true;
            btnHide.Enabled = false;
        }
        internal void btnShow_Click(object sender, EventArgs e)
        {
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.ShowYourSelf();
            btnShow.Enabled = false;
            btnHide.Enabled = true;
        }
        internal void PauseForms()
        {
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.Pause();
        }
        internal void RestartForms()
        {
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.Restart();
        }
        internal void ResetAllYTimeGraphs()
        {
            foreach (SimpleDynoSubForm SDFrm in f)
            {
                if (SDFrm.myType == "MultiYTimeGraph")
                {
                    SDFrm.ResetSDForm();
                }
            }
        }
        private void btnNewLabel_Click(object sender, EventArgs e)
        {
            // New Label
            btnShow_Click(this, EventArgs.Empty);
            btnSaveAs.Enabled = true;
            btnClose.Enabled = true;
            btnHide.Enabled = true;
            f.Add(new SimpleDynoSubLabel());
            global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
            global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
            f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
        }
        private void btnNewGauge_Click(object sender, EventArgs e)
        {
            // New Gauge
            btnShow_Click(this, EventArgs.Empty);
            btnSaveAs.Enabled = true;
            btnClose.Enabled = true;
            btnHide.Enabled = true;
            f.Add(new SimpleDynoSubGauge());
            global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
            global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
            f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
        }
        private void btnMultiYTime_Click(object sender, EventArgs e)
        {
            btnShow_Click(this, EventArgs.Empty);
            btnSaveAs.Enabled = true;
            btnClose.Enabled = true;
            btnHide.Enabled = true;
            f.Add(new SimpleDynoSubMultiYTimeGraph());
            global::SimpleDyno.SimpleDynoSubForm.RemoveYourself += this.RemoveForm;
            global::SimpleDyno.SimpleDynoSubForm.SetToMyFormat += this.SetAllFormats;
            f[f.Count - 1].Initialize(f.Count - 1, ref Data, ref DataTags, ref DataUnits, ref DataUnitTags, ref DataAreUsed);
        }
        private void btnResetSDForm_Click(object sender, EventArgs e)
        {
            ResetAllYTimeGraphs();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Remove interface if it is closed down
            while (f.Count != 0)
            {
                f[f.Count - 1].Dispose();
                f.Remove(f[f.Count - 1]);
            }
            txtInterface.Text = "No Interface Loaded";
            btnClose.Enabled = false;
            btnShow.Enabled = false;
            btnHide.Enabled = false;
            btnSave.Enabled = false;
            btnSaveAs.Enabled = false;
        }
        private void RemoveForm(int SentToRemove)
        {
            List<SimpleDynoSubForm> formsToRemove = new List<SimpleDynoSubForm>();
            foreach (SimpleDynoSubForm SDFrm in f)
            {
                if (SDFrm.myNumber == SentToRemove)
                {
                    formsToRemove.Add(SDFrm);
                    //f.Remove(SDFrm);
                    //SDFrm.Dispose();
                    //SDFrm = (SimpleDynoSubForm)null;
                    //break;
                }
            }
            foreach (SimpleDynoSubForm form in formsToRemove)
            {
                f.Remove(form);
                form.Dispose();
            }
        }
        private void SetAllFormats(string SentFormat)
        {
            foreach (SimpleDynoSubForm SDFrm in f)
                SDFrm.SetMyFormat(SentFormat);
        }
        #endregion
        #region Performance Testing
#if QueryPerformance
        private void btnPerformanceTest_Click(object sender, EventArgs e)
        {
            // Cycle through all listed buffer sizes, sampling rates, channel numbers and +/- adv processing
            // Collect approx 100 points per
            // Calculate average, stdev, min, max, based on middle 60 points.  Also count P_Time events > 100 
            // First, ADV off
            double SumPerf;
            double SumFreq;
            double AvePerf;
            double AveFreq;
            double StdPerf;
            double StdFreq;
            double MinPerf;
            double MinFreq;
            double MaxPerf;
            double MaxFreq;
            double PerfGreaterThan100;
            int i;
            int j;
            int k;
            int t;
            PerformanceData = new double[3, 201];
            chkAdvancedProcessing.Checked = false;
            var loopTo = cmbChannels.Items.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                cmbChannels.SelectedIndex = i;
                var loopTo1 = cmbBufferSize.Items.Count - 1;
                for (j = 0; j <= loopTo1; j++)
                {
                    cmbBufferSize.SelectedIndex = j;
                    var loopTo2 = cmbSampleRate.Items.Count - 1;
                    for (k = 0; k <= loopTo2; k++)
                    {
                        cmbSampleRate.SelectedIndex = k;
                        btnStartAcquisition_Click(this, EventArgs.Empty);
                        do
                            Application.DoEvents();
                        while (PerfBufferCount < 100);
                        SumPerf = 0d;
                        SumFreq = 0d;
                        MinPerf = 100000d;
                        MinFreq = 100000d;
                        MaxPerf = 0d;
                        MaxFreq = 0d;
                        PerfGreaterThan100 = 0d;
                        AvePerf = 0d;
                        AveFreq = 0d;
                        StdPerf = 0d;
                        StdFreq = 0d;
                        for (t = 21; t <= 80; t++)
                        {
                            SumPerf = SumPerf + PerformanceData[P_TIME, t];
                            SumFreq = SumFreq + PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_TIME, t] < MinPerf)
                                MinPerf = PerformanceData[P_TIME, t];
                            if (PerformanceData[P_TIME, t] > MaxPerf)
                                MaxPerf = PerformanceData[P_TIME, t];
                            if (PerformanceData[P_FREQ, t] < MinFreq)
                                MinFreq = PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_FREQ, t] > MaxFreq)
                                MaxFreq = PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_TIME, t] > 100d)
                                PerfGreaterThan100 += 1d;
                            // Debug.Print(PerformanceData(P_FREQ, t).ToString & " " & PerformanceData(P_TIME, t).ToString)
                        }
                        AvePerf = SumPerf / 60d;
                        AveFreq = SumFreq / 60d;
                        SumPerf = 0d;
                        SumFreq = 0d;
                        for (t = 21; t <= 80; t++)
                        {
                            SumPerf = SumPerf + Math.Pow(PerformanceData[P_TIME, t] - AvePerf, 2d);
                            SumFreq = SumFreq + Math.Pow(PerformanceData[P_FREQ, t] - AveFreq, 2d);
                        }
                        StdPerf = Math.Pow(SumPerf / 59d, 0.5d);
                        StdFreq = Math.Pow(SumFreq / 59d, 0.5d);
                        Debug.Print("Adv" + chkAdvancedProcessing.CheckState.ToString() + " " + NUMBER_OF_CHANNELS + " " + BUFFER_SIZE + " " + SAMPLE_RATE + " Freq (min,max,ave,std): " + MinFreq + " " + MaxFreq + " " + AveFreq + " " + StdFreq + " Perf (min,max,ave,std,#>100): " + MinPerf + " " + MaxPerf + " " + AvePerf + " " + StdPerf + " " + PerfGreaterThan100);
                        PerfBufferCount = 0;
                    }
                }
            }
            chkAdvancedProcessing.Checked = true;
            var loopTo3 = cmbChannels.Items.Count - 1;
            for (i = 0; i <= loopTo3; i++)
            {
                cmbChannels.SelectedIndex = i;
                var loopTo4 = cmbBufferSize.Items.Count - 1;
                for (j = 0; j <= loopTo4; j++)
                {
                    cmbBufferSize.SelectedIndex = j;
                    var loopTo5 = cmbSampleRate.Items.Count - 1;
                    for (k = 0; k <= loopTo5; k++)
                    {
                        cmbSampleRate.SelectedIndex = k;
                        btnStartAcquisition_Click(this, EventArgs.Empty);
                        do
                            Application.DoEvents();
                        while (PerfBufferCount < 100);
                        SumPerf = 0d;
                        SumFreq = 0d;
                        MinPerf = 100000d;
                        MinFreq = 100000d;
                        MaxPerf = 0d;
                        MaxFreq = 0d;
                        PerfGreaterThan100 = 0d;
                        AvePerf = 0d;
                        AveFreq = 0d;
                        StdPerf = 0d;
                        StdFreq = 0d;
                        for (t = 21; t <= 80; t++)
                        {
                            SumPerf = SumPerf + PerformanceData[P_TIME, t];
                            SumFreq = SumFreq + PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_TIME, t] < MinPerf)
                                MinPerf = PerformanceData[P_TIME, t];
                            if (PerformanceData[P_TIME, t] > MaxPerf)
                                MaxPerf = PerformanceData[P_TIME, t];
                            if (PerformanceData[P_FREQ, t] < MinFreq)
                                MinFreq = PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_FREQ, t] > MaxFreq)
                                MaxFreq = PerformanceData[P_FREQ, t];
                            if (PerformanceData[P_TIME, t] > 100d)
                                PerfGreaterThan100 += 1d;
                            // Debug.Print(PerformanceData(P_FREQ, t).ToString & " " & PerformanceData(P_TIME, t).ToString)
                        }
                        AvePerf = SumPerf / 60d;
                        AveFreq = SumFreq / 60d;
                        SumPerf = 0d;
                        SumFreq = 0d;
                        for (t = 21; t <= 80; t++)
                        {
                            SumPerf = SumPerf + Math.Pow(PerformanceData[P_TIME, t] - AvePerf, 2d);
                            SumFreq = SumFreq + Math.Pow(PerformanceData[P_FREQ, t] - AveFreq, 2d);
                        }
                        StdPerf = Math.Pow(SumPerf / 59d, 0.5d);
                        StdFreq = Math.Pow(SumFreq / 59d, 0.5d);
                        Debug.Print("Adv" + chkAdvancedProcessing.CheckState.ToString() + " " + NUMBER_OF_CHANNELS + " " + BUFFER_SIZE + " " + SAMPLE_RATE + " Freq (min,max,ave,std): " + MinFreq + " " + MaxFreq + " " + AveFreq + " " + StdFreq + " Perf (min,max,ave,std,#>100): " + MinPerf + " " + MaxPerf + " " + AvePerf + " " + StdPerf + " " + PerfGreaterThan100);
                        PerfBufferCount = 0;
                    }
                }
            }
        }
#endif
        #endregion
        #region Simulation
        private bool triggerSimulationIfAvailable = false;
        // Thread function for running the simulated serial data
        private void serialSimuFunc()
        {
            string dirOfExecutable = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string simuFilePath = new Uri(dirOfExecutable + @"\serial_simulation.csv").LocalPath;

            StreamReader fileReader = null;

            string line;
            bool wasTriggered = false;
            long previousTimestamp = 0L;

            while (RunSimulation)
            {
                if (triggerSimulationIfAvailable)
                {

                    // Open the file when entering powerrun
                    if (!wasTriggered)
                    {
                        try
                        {
                            fileReader = new StreamReader(simuFilePath, Encoding.Default);
                        }
                        catch (Exception e1)
                        {
                            // Nothing to do
                        }
                        wasTriggered = true;
                    }

                    if (!(fileReader == null))
                    {
                        line = fileReader.ReadLine();
                        if (line == null)
                        {
                            fileReader.Close();
                            fileReader = null;
                            triggerSimulationIfAvailable = false;
                        }
                        else
                        {
                            // Sleep according to simulated arduino timestamps 
                            COMPortMessage = Strings.Split(line, ",");
                            long currentTimestamp = Conversions.ToInteger(COMPortMessage[0]);
                            if (previousTimestamp != 0L & currentTimestamp - previousTimestamp > 0L)
                            {
                                Thread.Sleep((int)Math.Round((currentTimestamp - previousTimestamp) / 1000d));
                            }
                            previousTimestamp = currentTimestamp;
                            DataReceivedLine(line);
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    wasTriggered = false;
                    Thread.Sleep(500);
                }

            }
        }

        private Thread serialSimuThread;
        #endregion
    }
    #region DoubleBufferPanel Class
    //public class DoubleBufferPanel : Panel
    //{
    //    public DoubleBufferPanel()
    //    {
    //        DoubleBuffered = true;
    //        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
    //        UpdateStyles();
    //    }
    //}
    #endregion
}
