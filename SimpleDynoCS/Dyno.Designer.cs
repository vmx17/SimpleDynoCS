namespace SimpleDyno
{
    partial class Dyno : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dyno));
            Label12 = new Label();
            txtCarMass = new TextBox();
            Label2 = new Label();
            Label20 = new Label();
            txtWheelDiameter = new TextBox();
            Label5 = new Label();
            txtGearRatio = new TextBox();
            Label6 = new Label();
            txtFrontalArea = new TextBox();
            Label8 = new Label();
            txtDragCoefficient = new TextBox();
            Label4 = new Label();
            txtSignalsPerRPM2 = new TextBox();
            Label24 = new Label();
            txtSignalsPerRPM1 = new TextBox();
            Label23 = new Label();
            txtExtraWallThickness = new TextBox();
            txtRollerDiameter = new TextBox();
            txtExtraMass = new TextBox();
            Label22 = new Label();
            Label31 = new Label();
            Label19 = new Label();
            Label32 = new Label();
            txtRollerWallThickness = new TextBox();
            Label33 = new Label();
            Label21 = new Label();
            txtExtraDiameter = new TextBox();
            txtRollerMass = new TextBox();
            txtEndCapMass = new TextBox();
            Label15 = new Label();
            txtAxleMass = new TextBox();
            Label27 = new Label();
            Label28 = new Label();
            txtAxleDiameter = new TextBox();
            picDynoSettings = new PictureBox();
            lblTargetRollerMass = new Label();
            lblActualMomentOfInertia = new Label();
            lblTargetMomentOfInertia = new Label();
            lblDynoSettings = new Label();
            Label1 = new Label();
            RPM1RPM2TextBox = new TextBox();
            RPM1RPM2RatioCheckBox = new CheckBox();
            RawMOITextBox = new TextBox();
            DirectMOICheckBox = new CheckBox();
            FakeFocusTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)picDynoSettings).BeginInit();
            SuspendLayout();
            // 
            // Label12
            // 
            Label12.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label12.Location = new Point(8, 20);
            Label12.Name = "Label12";
            Label12.Size = new Size(160, 20);
            Label12.TabIndex = 245;
            Label12.Text = "Vehicle Mass (g)";
            Label12.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtCarMass
            // 
            txtCarMass.CausesValidation = false;
            txtCarMass.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCarMass.Location = new Point(176, 20);
            txtCarMass.Name = "txtCarMass";
            txtCarMass.Size = new Size(72, 22);
            txtCarMass.TabIndex = 244;
            txtCarMass.Tag = "";
            txtCarMass.Text = "1000";
            txtCarMass.TextAlign = HorizontalAlignment.Center;
            txtCarMass.Enter += txtCarMass_Enter;
            txtCarMass.Leave += txtCarMass_Leave;
            // 
            // Label2
            // 
            Label2.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label2.Location = new Point(88, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(160, 20);
            Label2.TabIndex = 275;
            Label2.Text = "Non Critical Parameters";
            Label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label20
            // 
            Label20.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label20.Location = new Point(8, 121);
            Label20.Name = "Label20";
            Label20.Size = new Size(160, 20);
            Label20.TabIndex = 271;
            Label20.Text = "Wheel Diameter (mm)";
            Label20.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtWheelDiameter
            // 
            txtWheelDiameter.CausesValidation = false;
            txtWheelDiameter.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtWheelDiameter.Location = new Point(176, 121);
            txtWheelDiameter.Name = "txtWheelDiameter";
            txtWheelDiameter.Size = new Size(72, 22);
            txtWheelDiameter.TabIndex = 270;
            txtWheelDiameter.Tag = "";
            txtWheelDiameter.Text = "100";
            txtWheelDiameter.TextAlign = HorizontalAlignment.Center;
            txtWheelDiameter.Enter += txtWheelDiameter_Enter;
            txtWheelDiameter.Leave += txtWheelDiameter_Leave;
            // 
            // Label5
            // 
            Label5.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label5.Location = new Point(8, 97);
            Label5.Name = "Label5";
            Label5.Size = new Size(160, 20);
            Label5.TabIndex = 272;
            Label5.Text = "Gear Ratio";
            Label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtGearRatio
            // 
            txtGearRatio.CausesValidation = false;
            txtGearRatio.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtGearRatio.Location = new Point(176, 97);
            txtGearRatio.Name = "txtGearRatio";
            txtGearRatio.Size = new Size(72, 22);
            txtGearRatio.TabIndex = 269;
            txtGearRatio.Tag = "";
            txtGearRatio.Text = "1";
            txtGearRatio.TextAlign = HorizontalAlignment.Center;
            txtGearRatio.Enter += txtGearRatio_Enter;
            txtGearRatio.Leave += txtGearRatio_Leave;
            // 
            // Label6
            // 
            Label6.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label6.Location = new Point(8, 49);
            Label6.Name = "Label6";
            Label6.Size = new Size(160, 20);
            Label6.TabIndex = 273;
            Label6.Text = "Frontal Area (mm2)";
            Label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFrontalArea
            // 
            txtFrontalArea.CausesValidation = false;
            txtFrontalArea.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFrontalArea.Location = new Point(176, 49);
            txtFrontalArea.Name = "txtFrontalArea";
            txtFrontalArea.Size = new Size(72, 22);
            txtFrontalArea.TabIndex = 267;
            txtFrontalArea.Tag = "";
            txtFrontalArea.Text = "1000";
            txtFrontalArea.TextAlign = HorizontalAlignment.Center;
            txtFrontalArea.Enter += txtFrontalArea_Enter;
            txtFrontalArea.Leave += txtFrontalArea_Leave;
            // 
            // Label8
            // 
            Label8.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label8.Location = new Point(8, 73);
            Label8.Name = "Label8";
            Label8.Size = new Size(160, 20);
            Label8.TabIndex = 274;
            Label8.Text = "Drag Coeff.";
            Label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDragCoefficient
            // 
            txtDragCoefficient.CausesValidation = false;
            txtDragCoefficient.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtDragCoefficient.Location = new Point(176, 73);
            txtDragCoefficient.Name = "txtDragCoefficient";
            txtDragCoefficient.Size = new Size(72, 22);
            txtDragCoefficient.TabIndex = 268;
            txtDragCoefficient.Tag = "";
            txtDragCoefficient.Text = "1";
            txtDragCoefficient.TextAlign = HorizontalAlignment.Center;
            txtDragCoefficient.Enter += txtDragCoefficient_Enter;
            txtDragCoefficient.Leave += txtDragCoefficient_Leave;
            // 
            // Label4
            // 
            Label4.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label4.Location = new Point(88, 178);
            Label4.Name = "Label4";
            Label4.Size = new Size(160, 20);
            Label4.TabIndex = 298;
            Label4.Text = "Critical Parameters";
            Label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSignalsPerRPM2
            // 
            txtSignalsPerRPM2.CausesValidation = false;
            txtSignalsPerRPM2.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSignalsPerRPM2.Location = new Point(176, 441);
            txtSignalsPerRPM2.Name = "txtSignalsPerRPM2";
            txtSignalsPerRPM2.Size = new Size(72, 22);
            txtSignalsPerRPM2.TabIndex = 296;
            txtSignalsPerRPM2.Tag = "";
            txtSignalsPerRPM2.Text = "1";
            txtSignalsPerRPM2.TextAlign = HorizontalAlignment.Center;
            txtSignalsPerRPM2.Enter += txtSignalsPerRPM2_Enter;
            txtSignalsPerRPM2.Leave += txtSignalsPerRPM2_Leave;
            // 
            // Label24
            // 
            Label24.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label24.Location = new Point(37, 441);
            Label24.Name = "Label24";
            Label24.Size = new Size(131, 20);
            Label24.TabIndex = 297;
            Label24.Text = "Signals per RPM2";
            Label24.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSignalsPerRPM1
            // 
            txtSignalsPerRPM1.CausesValidation = false;
            txtSignalsPerRPM1.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSignalsPerRPM1.Location = new Point(176, 417);
            txtSignalsPerRPM1.Name = "txtSignalsPerRPM1";
            txtSignalsPerRPM1.Size = new Size(72, 22);
            txtSignalsPerRPM1.TabIndex = 285;
            txtSignalsPerRPM1.Tag = "";
            txtSignalsPerRPM1.Text = "1";
            txtSignalsPerRPM1.TextAlign = HorizontalAlignment.Center;
            txtSignalsPerRPM1.Enter += txtSignalsPerRPM_Enter;
            txtSignalsPerRPM1.Leave += txtSignalsPerRPM_Leave;
            // 
            // Label23
            // 
            Label23.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label23.Location = new Point(72, 417);
            Label23.Name = "Label23";
            Label23.Size = new Size(96, 20);
            Label23.TabIndex = 289;
            Label23.Text = "Signals per RPM";
            Label23.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtExtraWallThickness
            // 
            txtExtraWallThickness.CausesValidation = false;
            txtExtraWallThickness.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtExtraWallThickness.Location = new Point(176, 369);
            txtExtraWallThickness.Name = "txtExtraWallThickness";
            txtExtraWallThickness.Size = new Size(72, 22);
            txtExtraWallThickness.TabIndex = 283;
            txtExtraWallThickness.Tag = "";
            txtExtraWallThickness.Text = "0";
            txtExtraWallThickness.TextAlign = HorizontalAlignment.Center;
            txtExtraWallThickness.Enter += txtExtraWallThickness_Enter;
            txtExtraWallThickness.Leave += txtExtraWallThickness_Leave;
            // 
            // txtRollerDiameter
            // 
            txtRollerDiameter.CausesValidation = false;
            txtRollerDiameter.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtRollerDiameter.Location = new Point(176, 201);
            txtRollerDiameter.Name = "txtRollerDiameter";
            txtRollerDiameter.Size = new Size(72, 22);
            txtRollerDiameter.TabIndex = 276;
            txtRollerDiameter.Tag = "";
            txtRollerDiameter.Text = "100";
            txtRollerDiameter.TextAlign = HorizontalAlignment.Center;
            txtRollerDiameter.Enter += txtRollerDiameter_Enter;
            txtRollerDiameter.Leave += txtRollerDiameter_Leave;
            // 
            // txtExtraMass
            // 
            txtExtraMass.CausesValidation = false;
            txtExtraMass.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtExtraMass.Location = new Point(176, 393);
            txtExtraMass.Name = "txtExtraMass";
            txtExtraMass.Size = new Size(72, 22);
            txtExtraMass.TabIndex = 284;
            txtExtraMass.Tag = "";
            txtExtraMass.Text = "0";
            txtExtraMass.TextAlign = HorizontalAlignment.Center;
            txtExtraMass.Enter += txtExtraMass_Enter;
            txtExtraMass.Leave += txtExtraMass_Leave;
            // 
            // Label22
            // 
            Label22.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label22.Location = new Point(8, 249);
            Label22.Name = "Label22";
            Label22.Size = new Size(160, 20);
            Label22.TabIndex = 288;
            Label22.Text = "Roller Mass (g)";
            Label22.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label31
            // 
            Label31.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label31.Location = new Point(8, 371);
            Label31.Name = "Label31";
            Label31.Size = new Size(160, 20);
            Label31.TabIndex = 294;
            Label31.Text = "Extra Wall Thickness (mm)";
            Label31.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label19
            // 
            Label19.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label19.Location = new Point(8, 201);
            Label19.Name = "Label19";
            Label19.Size = new Size(160, 20);
            Label19.TabIndex = 286;
            Label19.Text = "Roller Diameter (mm)";
            Label19.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label32
            // 
            Label32.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label32.Location = new Point(8, 347);
            Label32.Name = "Label32";
            Label32.Size = new Size(160, 20);
            Label32.TabIndex = 293;
            Label32.Text = "Extra Diameter (mm)";
            Label32.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtRollerWallThickness
            // 
            txtRollerWallThickness.CausesValidation = false;
            txtRollerWallThickness.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtRollerWallThickness.Location = new Point(176, 225);
            txtRollerWallThickness.Name = "txtRollerWallThickness";
            txtRollerWallThickness.Size = new Size(72, 22);
            txtRollerWallThickness.TabIndex = 277;
            txtRollerWallThickness.Tag = "";
            txtRollerWallThickness.Text = "50";
            txtRollerWallThickness.TextAlign = HorizontalAlignment.Center;
            txtRollerWallThickness.Enter += txtRollerWallThickness_Enter;
            txtRollerWallThickness.Leave += txtRollerWallThickness_Leave;
            // 
            // Label33
            // 
            Label33.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label33.Location = new Point(8, 395);
            Label33.Name = "Label33";
            Label33.Size = new Size(160, 20);
            Label33.TabIndex = 295;
            Label33.Text = "Extra Mass (g)";
            Label33.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label21
            // 
            Label21.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label21.Location = new Point(8, 225);
            Label21.Name = "Label21";
            Label21.Size = new Size(160, 20);
            Label21.TabIndex = 287;
            Label21.Text = "Roller Wall Thickness (mm)";
            Label21.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtExtraDiameter
            // 
            txtExtraDiameter.CausesValidation = false;
            txtExtraDiameter.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtExtraDiameter.Location = new Point(176, 345);
            txtExtraDiameter.Name = "txtExtraDiameter";
            txtExtraDiameter.Size = new Size(72, 22);
            txtExtraDiameter.TabIndex = 282;
            txtExtraDiameter.Tag = "";
            txtExtraDiameter.Text = "0";
            txtExtraDiameter.TextAlign = HorizontalAlignment.Center;
            txtExtraDiameter.Enter += txtExtraDiameter_Enter;
            txtExtraDiameter.Leave += txtExtraDiameter_Leave;
            // 
            // txtRollerMass
            // 
            txtRollerMass.CausesValidation = false;
            txtRollerMass.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtRollerMass.Location = new Point(176, 249);
            txtRollerMass.Name = "txtRollerMass";
            txtRollerMass.Size = new Size(72, 22);
            txtRollerMass.TabIndex = 278;
            txtRollerMass.Tag = "";
            txtRollerMass.Text = "1000";
            txtRollerMass.TextAlign = HorizontalAlignment.Center;
            txtRollerMass.Enter += txtRollerMass_Enter;
            txtRollerMass.Leave += txtRollerMass_Leave;
            // 
            // txtEndCapMass
            // 
            txtEndCapMass.CausesValidation = false;
            txtEndCapMass.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtEndCapMass.Location = new Point(176, 321);
            txtEndCapMass.Name = "txtEndCapMass";
            txtEndCapMass.Size = new Size(72, 22);
            txtEndCapMass.TabIndex = 281;
            txtEndCapMass.Tag = "";
            txtEndCapMass.Text = "0";
            txtEndCapMass.TextAlign = HorizontalAlignment.Center;
            txtEndCapMass.Enter += txtEndCapMass_Enter;
            txtEndCapMass.Leave += txtEndCapMass_Leave;
            // 
            // Label15
            // 
            Label15.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label15.Location = new Point(8, 297);
            Label15.Name = "Label15";
            Label15.Size = new Size(160, 20);
            Label15.TabIndex = 291;
            Label15.Text = "Axle Mass (g)";
            Label15.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAxleMass
            // 
            txtAxleMass.CausesValidation = false;
            txtAxleMass.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAxleMass.Location = new Point(176, 297);
            txtAxleMass.Name = "txtAxleMass";
            txtAxleMass.Size = new Size(72, 22);
            txtAxleMass.TabIndex = 280;
            txtAxleMass.Tag = "";
            txtAxleMass.Text = "0";
            txtAxleMass.TextAlign = HorizontalAlignment.Center;
            txtAxleMass.Enter += txtAxleMass_Enter;
            txtAxleMass.Leave += txtAxleMass_Leave;
            // 
            // Label27
            // 
            Label27.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label27.Location = new Point(8, 273);
            Label27.Name = "Label27";
            Label27.Size = new Size(160, 20);
            Label27.TabIndex = 290;
            Label27.Text = "Axle Diameter (mm)";
            Label27.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Label28
            // 
            Label28.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label28.Location = new Point(8, 321);
            Label28.Name = "Label28";
            Label28.Size = new Size(160, 20);
            Label28.TabIndex = 292;
            Label28.Text = "End Cap Mass (g)";
            Label28.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAxleDiameter
            // 
            txtAxleDiameter.CausesValidation = false;
            txtAxleDiameter.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAxleDiameter.Location = new Point(176, 273);
            txtAxleDiameter.Name = "txtAxleDiameter";
            txtAxleDiameter.Size = new Size(72, 22);
            txtAxleDiameter.TabIndex = 279;
            txtAxleDiameter.Tag = "";
            txtAxleDiameter.Text = "10";
            txtAxleDiameter.TextAlign = HorizontalAlignment.Center;
            txtAxleDiameter.Enter += txtAxleMass_Enter;
            txtAxleDiameter.Leave += txtAxleMass_Leave;
            // 
            // picDynoSettings
            // 
            picDynoSettings.BackgroundImageLayout = ImageLayout.Zoom;
            picDynoSettings.InitialImage = null;
            picDynoSettings.Location = new Point(263, 20);
            picDynoSettings.Name = "picDynoSettings";
            picDynoSettings.Size = new Size(669, 383);
            picDynoSettings.TabIndex = 299;
            picDynoSettings.TabStop = false;
            // 
            // lblTargetRollerMass
            // 
            lblTargetRollerMass.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTargetRollerMass.Location = new Point(8, 558);
            lblTargetRollerMass.Name = "lblTargetRollerMass";
            lblTargetRollerMass.Size = new Size(241, 25);
            lblTargetRollerMass.TabIndex = 302;
            lblTargetRollerMass.Text = "Target Roller Mass";
            lblTargetRollerMass.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblActualMomentOfInertia
            // 
            lblActualMomentOfInertia.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblActualMomentOfInertia.Location = new Point(8, 509);
            lblActualMomentOfInertia.Name = "lblActualMomentOfInertia";
            lblActualMomentOfInertia.Size = new Size(241, 24);
            lblActualMomentOfInertia.TabIndex = 301;
            lblActualMomentOfInertia.Text = "Actual MOI";
            lblActualMomentOfInertia.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTargetMomentOfInertia
            // 
            lblTargetMomentOfInertia.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTargetMomentOfInertia.Location = new Point(8, 533);
            lblTargetMomentOfInertia.Name = "lblTargetMomentOfInertia";
            lblTargetMomentOfInertia.Size = new Size(241, 25);
            lblTargetMomentOfInertia.TabIndex = 300;
            lblTargetMomentOfInertia.Text = "Target MOI";
            lblTargetMomentOfInertia.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblDynoSettings
            // 
            lblDynoSettings.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDynoSettings.Location = new Point(260, 409);
            lblDynoSettings.Name = "lblDynoSettings";
            lblDynoSettings.Size = new Size(672, 131);
            lblDynoSettings.TabIndex = 303;
            lblDynoSettings.Text = "Dyno Settings Information";
            // 
            // Label1
            // 
            Label1.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label1.Location = new Point(89, 489);
            Label1.Name = "Label1";
            Label1.Size = new Size(160, 20);
            Label1.TabIndex = 304;
            Label1.Text = "Moment of Inertia (MOI)";
            Label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // RPM1RPM2TextBox
            // 
            RPM1RPM2TextBox.CausesValidation = false;
            RPM1RPM2TextBox.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RPM1RPM2TextBox.Location = new Point(184, 145);
            RPM1RPM2TextBox.Name = "RPM1RPM2TextBox";
            RPM1RPM2TextBox.Size = new Size(64, 22);
            RPM1RPM2TextBox.TabIndex = 305;
            RPM1RPM2TextBox.Tag = "";
            RPM1RPM2TextBox.Text = "1";
            RPM1RPM2TextBox.TextAlign = HorizontalAlignment.Center;
            RPM1RPM2TextBox.Enter += RPM1RPM2TextBox_Enter;
            RPM1RPM2TextBox.Leave += RPM1RPM2TextBox_Leave;
            // 
            // RPM1RPM2RatioCheckBox
            // 
            RPM1RPM2RatioCheckBox.AutoSize = true;
            RPM1RPM2RatioCheckBox.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RPM1RPM2RatioCheckBox.Location = new Point(4, 148);
            RPM1RPM2RatioCheckBox.Name = "RPM1RPM2RatioCheckBox";
            RPM1RPM2RatioCheckBox.Size = new Size(174, 18);
            RPM1RPM2RatioCheckBox.TabIndex = 306;
            RPM1RPM2RatioCheckBox.Text = "MotorRPM / RollerRPM ratio";
            RPM1RPM2RatioCheckBox.UseVisualStyleBackColor = true;
            // 
            // RawMOITextBox
            // 
            RawMOITextBox.CausesValidation = false;
            RawMOITextBox.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RawMOITextBox.Location = new Point(176, 465);
            RawMOITextBox.Name = "RawMOITextBox";
            RawMOITextBox.Size = new Size(72, 22);
            RawMOITextBox.TabIndex = 307;
            RawMOITextBox.Tag = "";
            RawMOITextBox.Text = "0";
            RawMOITextBox.TextAlign = HorizontalAlignment.Center;
            RawMOITextBox.Enter += RawMOITextBox_Enter;
            RawMOITextBox.Leave += RawMOITextBox_Leave;
            // 
            // DirectMOICheckBox
            // 
            DirectMOICheckBox.AutoSize = true;
            DirectMOICheckBox.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DirectMOICheckBox.Location = new Point(7, 467);
            DirectMOICheckBox.Name = "DirectMOICheckBox";
            DirectMOICheckBox.Size = new Size(166, 18);
            DirectMOICheckBox.TabIndex = 308;
            DirectMOICheckBox.Text = "Input raw MOI (kg*m^2)";
            DirectMOICheckBox.UseVisualStyleBackColor = true;
            // 
            // FakeFocusTextBox
            // 
            FakeFocusTextBox.Location = new Point(876, 558);
            FakeFocusTextBox.Name = "FakeFocusTextBox";
            FakeFocusTextBox.Size = new Size(56, 20);
            FakeFocusTextBox.TabIndex = 309;
            FakeFocusTextBox.Visible = false;
            // 
            // Dyno
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(944, 587);
            Controls.Add(FakeFocusTextBox);
            Controls.Add(DirectMOICheckBox);
            Controls.Add(RawMOITextBox);
            Controls.Add(RPM1RPM2RatioCheckBox);
            Controls.Add(RPM1RPM2TextBox);
            Controls.Add(Label1);
            Controls.Add(lblDynoSettings);
            Controls.Add(lblTargetRollerMass);
            Controls.Add(lblActualMomentOfInertia);
            Controls.Add(lblTargetMomentOfInertia);
            Controls.Add(picDynoSettings);
            Controls.Add(Label4);
            Controls.Add(txtSignalsPerRPM2);
            Controls.Add(Label24);
            Controls.Add(txtSignalsPerRPM1);
            Controls.Add(Label23);
            Controls.Add(txtExtraWallThickness);
            Controls.Add(txtRollerDiameter);
            Controls.Add(txtExtraMass);
            Controls.Add(Label22);
            Controls.Add(Label31);
            Controls.Add(Label19);
            Controls.Add(Label32);
            Controls.Add(txtRollerWallThickness);
            Controls.Add(Label33);
            Controls.Add(Label21);
            Controls.Add(txtExtraDiameter);
            Controls.Add(txtRollerMass);
            Controls.Add(txtEndCapMass);
            Controls.Add(Label15);
            Controls.Add(txtAxleMass);
            Controls.Add(Label27);
            Controls.Add(Label28);
            Controls.Add(txtAxleDiameter);
            Controls.Add(Label2);
            Controls.Add(Label20);
            Controls.Add(txtWheelDiameter);
            Controls.Add(Label5);
            Controls.Add(txtGearRatio);
            Controls.Add(Label6);
            Controls.Add(txtFrontalArea);
            Controls.Add(Label8);
            Controls.Add(txtDragCoefficient);
            Controls.Add(Label12);
            Controls.Add(txtCarMass);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Dyno";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dyno Setup";
            FormClosing += Dyno_FormClosing;
            ((System.ComponentModel.ISupportInitialize)picDynoSettings).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label Label12;
        private TextBox txtCarMass;
        private Label Label2;
        private Label Label20;
        private TextBox txtWheelDiameter;
        private Label Label5;
        private TextBox txtGearRatio;
        private Label Label6;
        private TextBox txtFrontalArea;
        private Label Label8;
        private TextBox txtDragCoefficient;
        private Label Label4;
        private TextBox txtSignalsPerRPM2;
        private Label Label24;
        private TextBox txtSignalsPerRPM1;
        private Label Label23;
        private TextBox txtExtraWallThickness;
        private TextBox txtRollerDiameter;
        private TextBox txtExtraMass;
        private Label Label22;
        private Label Label31;
        private Label Label19;
        private Label Label32;
        private TextBox txtRollerWallThickness;
        private Label Label33;
        private Label Label21;
        private TextBox txtExtraDiameter;
        private TextBox txtRollerMass;
        private TextBox txtEndCapMass;
        private Label Label15;
        private TextBox txtAxleMass;
        private Label Label27;
        private Label Label28;
        private TextBox txtAxleDiameter;
        private PictureBox picDynoSettings;
        private Label lblTargetRollerMass;
        private Label lblActualMomentOfInertia;
        private Label lblTargetMomentOfInertia;
        private Label lblDynoSettings;
        private Label Label1;
        private TextBox RPM1RPM2TextBox;
        private CheckBox RPM1RPM2RatioCheckBox;
        private TextBox RawMOITextBox;
        private CheckBox DirectMOICheckBox;
        private TextBox FakeFocusTextBox;
        #endregion
    }
}