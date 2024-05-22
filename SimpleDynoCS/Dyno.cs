using System;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Windows.ApplicationModel.Resources.Core;

namespace SimpleDyno
{
    public partial class Dyno : Form
    {
        // Dyno Parameters
        internal double CarMass;
        internal double AxleMass;
        internal double AxleDiameter;
        internal double EndCapMass;
        internal double FrontalArea;
        internal double DragCoefficient;
        internal double SignalsPerRPM;
        internal double SignalsPerRPM2;
        internal double WheelDiameter;
        internal double RollerDiameter;
        internal double RollerCircumference;
        internal double RollerWallThickness;
        internal double RollerMass;
        internal double ExtraDiameter;
        internal double ExtraWallThickness;
        internal double ExtraMass;
        internal double RPM1RPM2Ratio;
        internal double RawMOI;

        // Dyno Calculations
        internal double IdealMomentOfInertia;
        internal double IdealRollerMass;

        // Global Temporary Double for Checking the numeric input of the textboxes
        private double TempDouble;

        // Resouce Manager
        System.Resources.ResourceManager m_rManager;

        public Dyno()
        {
            InitializeComponent();
            m_rManager = new System.Resources.ResourceManager("SimpleDyno.Resources", Assembly.GetExecutingAssembly());
        }
        internal void Dyno_Setup()
        {
            txtCarMass.Select();
        }
        private void Dyno_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Force utilization of the latest input values by tricking any current textbox execute its Leave event
            FakeFocusTextBox.Visible = true;
            FakeFocusTextBox.Focus();
            FakeFocusTextBox.Visible = false;
            UpdateMomentOfInertias();

            // Prevents form from actually closing, rather it hides
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                this.Hide();
                e.Cancel = true;
                Program.MainI.btnShow_Click(this, EventArgs.Empty);
            }
        }
        private void txtCarMass_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("CarMass");
            lblDynoSettings.Text = "Enter the mass of the car in grams.  This value has NO impact on the accuracy of the dyno results.  " + "The value entered is used in combination with the roller diameter to calculate a target moment of inertia for the rollers.  " + "A dyno that reaches this target value (Target Dyno MOI = 100%) will most closely represent real world conditions for the car.  " + "The actual moment of inertia (Actual Dyno MOI) for your dyno is updated each time you make new entries for roller, end cap, and axle dimensions and weights.  " + "You can use this information to help design your dyno.  If you find that you dyno is 'underweight' you can use additional discs at the end of the rollers or axles " + "(known as 'Extras') to increase the Actual Dyno MOI.";
        }
        private void txtCarMass_Leave(object sender, EventArgs e)
        {
            double LocalMin = 1d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                CarMass = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = CarMass.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtFrontalArea_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("FrontalArea");
            lblDynoSettings.Text = "Enter the frontal area of your car in mm squared.  This value has NO impact on the accuracy of the dyno results.  " + "The value is calculated by multiplying the height of the car by the width of the car (both measures in mm).  " + "The frontal area is used in combination with the drag coefficient to calculate power losses due to air resistance.  " + "These losses are calulated and recorded during 'Power Run' sessions and the power losses due to drag cn be plotted.  " + "Where the power output curve and the drag curve intersect provides an estimate of corrected top speed.";
        }
        private void txtFrontalArea_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                FrontalArea = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = FrontalArea.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtDragCoefficient_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("DragImage");
            lblDynoSettings.Text = "Enter your estimate for the drag coefficient for your car.  This value has NO impact on the accuracy of the dyno results.  " + "Typical values will range from 0.5 - 1.0.  " + "The drag coefficient is used in combination with the frontal area  to calculate power losses due to air resistance.  " + "These losses are calulated and recorded during 'Power Run' sessions and the power losses due to drag cn be plotted.  " + "Where the power output curve and the drag curve intersect provides an estimate of corrected top speed.";
        }
        private void txtDragCoefficient_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 1d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                DragCoefficient = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = DragCoefficient.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtGearRatio_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("GearRatio");
            lblDynoSettings.Text = "Enter the gear ratio for your drivetrain.  This value has NO impact on the accuracy of the dyno results.  " + "This number is used to back calculate the RPM of the motor from the RPM of the rollers and wheels.  " + "In a simple drive train, this ratio is the number of spur teeth (A) divided by the number of pinion teeth (B).  " + "This value is typically greater than 1.";
        }
        private void txtGearRatio_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0.1d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                SimpleDyno.GearRatio = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = SimpleDyno.GearRatio.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtWheelDiameter_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("WheelDiameter");
            lblDynoSettings.Text = "Enter your car's wheel diameter in mm.  This value has NO impact on the accuracy of the dyno results.  " + "The value is used to calculate wheel RPM based on roller RPM and, in conjuction with the gear ratio, " + "the value is further used to calculate  motor RPM. ";
        }
        private void txtWheelDiameter_Leave(object sender, EventArgs e)
        {
            double LocalMin = 1d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                WheelDiameter = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = WheelDiameter.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtRollerDiameter_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("RollerDiameter");
            lblDynoSettings.Text = "Enter the diameter of your rollers in mm.  This value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your rollers and therefore impacts torque and power results.  " + "Additionally, but less critically, this value is used to calculate the speed of your vehicle and the RPM of your wheels and motor.  " + "Note that only the roller diameter (and not the car wheel diameter) is used to calculate vehicle speed";
        }
        private void txtRollerDiameter_Leave(object sender, EventArgs e)
        {
            double LocalMin = 1d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                RollerDiameter = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = RollerDiameter.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtRollerWallThickness_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("ollerWallThickness");
            lblDynoSettings.Text = "Enter the roller wall thickness in mm.  This value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your rollers and therefore impacts the torque and power results.  " + "Additionally, based on the assumed design of your rollers, the roller wall thickness is used to calculate the End Cap diameter " + "(i.e. it is assumed that your End Caps fit into the end of your rollers).  Note: If you are using a solid roller, this value should be half of the entered value for Roller Diameter" + " This value cannot be greater than 1/2 the value entered for Roller Diameter";
        }
        private void txtRollerWallThickness_Leave(object sender, EventArgs e)
        {
            double LocalMin = 1d;
            double LocalMax = RollerDiameter / 2d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                RollerWallThickness = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = RollerWallThickness.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtRollerMass_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("RollerMass");
            lblDynoSettings.Text = "Enter the roller mass in grams.  This value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your rollers and therefore impacts the torque and power results.  " + "Enter the mass of the driven rollers only.  For example if you have a two roller dyno (one roller per axle) but are testing a rear wheel drive car, " + "only enter the mass of the rear roller.  If you are testing an AWD / 4WD car, enter the combined masses of both rollers.";
        }
        private void txtRollerMass_Leave(object sender, EventArgs e)
        {
            double LocalMin = 1d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                RollerMass = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = RollerMass.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtAxleDiameter_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("AxleDiameter");
            lblDynoSettings.Text = "Enter the axle diameter in mm.  This value is IMPORTANT for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your axles and therefore impacts the torque and power results to some extent.  " + "Additionally, based on the assumed design of your rollers, the axle diameter used to calculate the end cap wall thickness.";
        }
        private void txtAxleDiameter_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                AxleDiameter = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = AxleDiameter.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtAxleMass_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("AxelMass");
            lblDynoSettings.Text = "Enter the axle mass in grams.  This value is IMPORTANT for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your axles and therefore impacts the torque and power results.  " + "Note: Only enter a mass for your axle(s) if the axles rotate witht the roller.  If you are using a fixed axle that does not rotate with the roller, enter zero for its mass";
        }
        private void txtAxleMass_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                AxleMass = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = AxleMass.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtEndCapMass_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("EndCapMass");
            lblDynoSettings.Text = "Enter the total mass of the end caps in grams.  This value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your end caps and therefore impacts the torque and power results.";
        }
        private void txtEndCapMass_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                EndCapMass = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = EndCapMass.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtExtraDiameter_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("ExtraDiameter");
            lblDynoSettings.Text = "Enter the extra diameter in mm.  If you are using extra components this value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your extra dyno components.  These components are typically disks " + "that you can add to the ends of your rollers to increase the overall moment of inertia.  Extras can be mounted on the axle " + "or attached directly to the ends of the roller / end cap components.";
        }
        private void txtExtraDiameter_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                ExtraDiameter = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = ExtraDiameter.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtExtraWallThickness_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("ExtraWallThickness");
            lblDynoSettings.Text = "Enter the extra wall thickness in mm.  If you are using extra components this value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your extra dyno components.  These components are typically disks " + "that you can add to the ends of your rollers to increase the overall moment of inertia.  Extras can be mounted on the axle " + "or attached directly to the ends of the roller / end cap components.";
        }
        private void txtExtraWallThickness_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                ExtraWallThickness = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = ExtraWallThickness.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtExtraMass_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("ExtraMass");
            lblDynoSettings.Text = "Enter the masses of all the extra components in grams.  If you are using extra components this value is CRITICAL for accurate dyno results.  " + "This value is used to calculate the moment of inertia of your extra dyno components.  These components are typicall disks " + "that you can add to the ends of your rollers to increase the overall moment of inertia.  Extras can be mounted on the axle " + "or attached directly to the ends of the roller / end cap components.";
        }
        private void txtExtraMass_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                ExtraMass = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = ExtraMass.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtSignalsPerRPM_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("SignalsPerRPM");
            lblDynoSettings.Text = "Enter the number of signals that are produced with each revolution of the roller.  " + "For example, if you are using a coil and magnet system to detect the roller RPM, each magnet attached to the roller " + "will represent one signal per RPM.  Make sure that if you are using multiple magnets that they are are spaced evenly around the " + "circumference of the roller.  Slight positional differences will lead to noise in the RPM values.";
        }
        private void txtSignalsPerRPM_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0.1d;
            double LocalMax = 50d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                SignalsPerRPM = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = SignalsPerRPM.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void txtSignalsPerRPM2_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("SignalsPerRPM");
            lblDynoSettings.Text = "Enter the number of signals that are produced with each revolution of the component being monitored by the second channel.  " + "This channel can be used to monitor the RPM of an IC engine or some other component of the drivetrain." + "If you are using a spark signal for a four stroke IC engine you can enter 0.5 for this value.";


        }
        private void txtSignalsPerRPM2_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0.1d;
            double LocalMax = 50d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                SignalsPerRPM2 = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = SignalsPerRPM2.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void RPM1RPM2TextBox_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                RPM1RPM2Ratio = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = RPM1RPM2Ratio.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void RawMOITextBox_Leave(object sender, EventArgs e)
        {
            double LocalMin = 0d;
            double LocalMax = 999d;
            if (double.TryParse(((TextBox)sender).Text, out TempDouble) && Program.MainI.CheckNumericalLimits(LocalMin, LocalMax, TempDouble))
            {
                RawMOI = TempDouble;
                UpdateMomentOfInertias();
            }
            else
            {
                Interaction.MsgBox(((TextBox)sender).Name + " : Value must be between " + LocalMin + " and " + LocalMax, MsgBoxStyle.Exclamation);
                {
                    var withBlock = (TextBox)sender;
                    withBlock.Text = RawMOI.ToString();
                    withBlock.Focus();
                }
            }
        }
        private void RPM1RPM2TextBox_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("GearRatio");
            lblDynoSettings.Text = "Enter the end-to-end ratio for your drivetrain.  This value has NO impact on the magnitude of the dyno results.  " + "This number is used to back calculate the RPM of the motor from the RPM of the rollers. " + "It can be measured e.g. by keeping constant 2000RPM on engine and measuring actual roller RPM achieved. " + "This value is typically greater than 1.";
        }
        private void RawMOITextBox_Enter(object sender, EventArgs e)
        {
            picDynoSettings.BackgroundImage = (Bitmap?)m_rManager.GetObject("RollerMass");
            lblDynoSettings.Text = "Enter the roller raw (measured) inertia if you know it. This value is CRITICAL for accurate dyno results.  " + "This value impacts the torque and power results.  ";
        }
        internal void UpdateMomentOfInertias()
        {
            double RollerMomentOfInertia;
            double AxleMomentOfInertia;
            double EndCapMomentOfInertia;
            double ExtraMomentOfInertia;

            double r1;
            double r2;
            double m;

            // Using I = 1/2 x m x (r1^2 + r2^2)
            // Roller
            m = RollerMass / 1000.0d;
            r1 = (RollerDiameter / 2.0d - RollerWallThickness) / 1000.0d;
            r2 = RollerDiameter / 2.0d / 1000.0d;
            RollerMomentOfInertia = 1d / 2d * m * (Math.Pow(r1, 2d) + Math.Pow(r2, 2d));
            // Axle
            m = AxleMass / 1000.0d;
            r1 = 0d;
            r2 = AxleDiameter / 2.0d / 1000.0d;
            AxleMomentOfInertia = 1d / 2d * m * (Math.Pow(r1, 2d) + Math.Pow(r2, 2d));
            // End Cap
            m = EndCapMass / 1000.0d;
            r1 = AxleDiameter / 2.0d / 1000.0d;
            r2 = (RollerDiameter / 2.0d - RollerWallThickness) / 1000.0d;
            EndCapMomentOfInertia = 1d / 2d * m * (Math.Pow(r1, 2d) + Math.Pow(r2, 2d));
            // Extras
            m = ExtraMass / 1000.0d;
            r1 = ExtraDiameter / 2.0d / 1000.0d;
            r2 = (ExtraDiameter / 2.0d - ExtraWallThickness) / 1000.0d;
            ExtraMomentOfInertia = 1d / 2d * m * (Math.Pow(r1, 2d) + Math.Pow(r2, 2d));
            // Total
            if (DirectMOICheckBox.Checked)
            {
                SimpleDyno.DynoMomentOfInertia = RawMOI;
            }
            else
            {
                SimpleDyno.DynoMomentOfInertia = RollerMomentOfInertia + AxleMomentOfInertia + EndCapMomentOfInertia + ExtraMomentOfInertia;
            }

            // Ideal Roller Mass
            // Car outputs 1 N force which will give F/m acceleration
            double CarAcceleration = 1d / (CarMass / 1000.0d); // m/s^2
                                                               // This equals the angular acceleration of the roller
            double RollerAcceleration = CarAcceleration / (RollerDiameter / 1000.0d / 2.0d); // radians/s^2
                                                                                             // Torque is the same force through the radius of the roller
            double RollerTorque = 1d * RollerDiameter / 1000.0d / 2.0d;
            // Torque is also the moment of inertia by angular accleration
            // Therefore, Torque / angular acceleration = ideal moment of inertia
            IdealMomentOfInertia = RollerTorque / RollerAcceleration;
            // r1 and r2 are the same as when calculated for the roller moment of inertia
            r1 = (RollerDiameter / 2.0d - RollerWallThickness) / 1000.0d;
            r2 = RollerDiameter / 2.0d / 1000.0d;
            // So (Note - actually not going to use this)
            IdealRollerMass = IdealMomentOfInertia * 2.0d / (Math.Pow(r1, 2d) + Math.Pow(r2, 2d)) * 1000d;
            // For Rollout calculations
            RollerCircumference = RollerDiameter * Math.PI; // circumference in mm
            SimpleDyno.WheelCircumference = WheelDiameter * Math.PI;

            // For Wheel and Motor RPM conversions and speed conversion
            SimpleDyno.RollerRPMtoWheelRPM = RollerDiameter / WheelDiameter;

            if (RPM1RPM2RatioCheckBox.Checked)
            {
                SimpleDyno.RollerRPMtoMotorRPM = RPM1RPM2Ratio;
            }
            else
            {
                SimpleDyno.RollerRPMtoMotorRPM = RollerDiameter / WheelDiameter * SimpleDyno.GearRatio;
            }

            SimpleDyno.RollerRadsPerSecToMetersPerSec = RollerCircumference / 1000d / (2d * Math.PI);

            if (SimpleDyno.DynoMomentOfInertia >= 0.0009d)
            {
                lblActualMomentOfInertia.Text = "Actual Dyno MoI = " + SimpleDyno.DynoMomentOfInertia.ToString("0.000") + " kg.m^2";
            }
            else
            {
                lblActualMomentOfInertia.Text = "Actual Dyno MoI = " + (1000d * SimpleDyno.DynoMomentOfInertia).ToString("0.000") + " g.m^2 ";
            }

            lblTargetMomentOfInertia.Text = "% of Target Dyno MoI = " + (SimpleDyno.DynoMomentOfInertia / IdealMomentOfInertia * 100d).ToString("0.0") + "%";

            lblTargetRollerMass.Text = "Target Roller Mass = " + IdealRollerMass.ToString("0") + " grams";

            // Update the drag coefficient and air resistance
            SimpleDyno.ForceAir = 0.5d * (FrontalArea / 1000000d) * 1.2d * DragCoefficient;

            // Update conversions for time to RPM
            SimpleDyno.ElapsedTimeToRadPerSec = 2d * Math.PI / SignalsPerRPM;
            SimpleDyno.ElapsedTimeToRadPerSec2 = 2d * Math.PI / SignalsPerRPM2;
        }
    }
}
