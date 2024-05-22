using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SimpleDyno
{
    public partial class Correction : Form
    {
        public static OpenFileDialog RunDownOpenFileDialog = new();
        public static StreamReader RunDownFileInput;
        public static bool blnUsingLoadedRunDownFile = false;
        public Correction()
        {
            InitializeComponent();
        }

        private void chkUseRunDown_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseRunDown.Checked == true)
            {
                grpRunDown.Enabled = true;
                // Main.frmFit.cmbWhichRDFit.Enabled = True
                SimpleDyno.frmFit.rdoRunDown.Enabled = true;
            }
            else
            {
                grpRunDown.Enabled = false;
                SimpleDyno.frmFit.cmbWhichRDFit.Enabled = false;
                SimpleDyno.frmFit.rdoRunDown.Enabled = false;
                blnUsingLoadedRunDownFile = false;
                lblCoastDownFile.Text = "No file loaded";
            }
        }
        private void chkUseCoastDownFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseCoastDownFile.Checked == true)
            {
                btnLoadCoastDown.Enabled = true;
                rdoFreeRoller.Enabled = false;
                rdoRollerAndWheel.Enabled = false;
                rdoRollerAndDrivetrain.Enabled = false;
            }
            else
            {
                btnLoadCoastDown.Enabled = false;
                blnUsingLoadedRunDownFile = false;
                rdoFreeRoller.Enabled = true;
                rdoRollerAndWheel.Enabled = true;
                rdoRollerAndDrivetrain.Enabled = true;
                chkUseRunDown_CheckedChanged(this, EventArgs.Empty);
            }
        }
        private void Correction_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevents form from actually closing, rather it hides
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                if (chkUseCoastDownFile.Checked && !blnUsingLoadedRunDownFile)
                {
                    Interaction.MsgBox("No coast down file was selected. Please confirm your coast down selection", Constants.vbOKOnly);
                    e.Cancel = true;
                }
                else
                {
                    this.Hide();
                    e.Cancel = true;
                    Program.MainI.btnShow_Click(this, EventArgs.Empty);
                }
            }
        }

        private void btnLoadCoastDown_Click(object sender, EventArgs e)
        {

            var RunDownOpenFileDialog = new OpenFileDialog();

            {
                ref var withBlock = ref RunDownOpenFileDialog;
                withBlock.Reset();
                withBlock.Filter = "Power Run files v6.3 (*.sdp)|*.sdp";
                withBlock.ShowDialog();
            }

            string Temp = "";
            string[] TempSplit;

            if (RunDownOpenFileDialog.FileName != "")
            {
                var RunDownFileInput = new StreamReader(RunDownOpenFileDialog.FileName);
                {
                    ref var withBlock1 = ref RunDownFileInput;
                    while (!(Temp.StartsWith("Coast_Down?_Roller?_Wheel?_Drivetrain?:") | RunDownFileInput.EndOfStream))
                        Temp = withBlock1.ReadLine();
                    if (RunDownFileInput.EndOfStream)
                    {
                        Interaction.MsgBox("No coast down data found.", MsgBoxStyle.OkOnly);
                        blnUsingLoadedRunDownFile = false;
                    }
                    else
                    {
                        TempSplit = Strings.Split(Temp, " ");
                        if (Strings.UCase(TempSplit[1]) == "FALSE")
                        {
                            Interaction.MsgBox("Coast Down not applied to this run", MsgBoxStyle.OkOnly);
                            blnUsingLoadedRunDownFile = false;
                        }
                        else
                        {
                            if (Strings.UCase(TempSplit[2]) == "TRUE")
                            {
                                rdoFreeRoller.Checked = true;
                                Interaction.MsgBox("Free Roller Coast Down data found.", MsgBoxStyle.OkOnly);
                                blnUsingLoadedRunDownFile = true;
                                SimpleDyno.frmFit.rdoRunDown.Enabled = false;
                                SimpleDyno.frmFit.cmbWhichRDFit.Enabled = false;
                            }
                            else if (Strings.UCase(TempSplit[3]) == "TRUE")
                            {
                                rdoRollerAndWheel.Checked = true;
                                Interaction.MsgBox("Free Roller + Wheel Coast Down data found.", MsgBoxStyle.OkOnly);
                                blnUsingLoadedRunDownFile = true;
                                SimpleDyno.frmFit.rdoRunDown.Enabled = false;
                                SimpleDyno.frmFit.cmbWhichRDFit.Enabled = false;
                            }
                            else
                            {
                                rdoRollerAndDrivetrain.Checked = true;
                                Interaction.MsgBox("Roller + Drivetrain Coast Down data found", MsgBoxStyle.OkOnly);
                                blnUsingLoadedRunDownFile = true;
                                SimpleDyno.frmFit.rdoRunDown.Enabled = false;
                                SimpleDyno.frmFit.cmbWhichRDFit.Enabled = false;
                            }
                            lblCoastDownFile.Text = "Using " + RunDownOpenFileDialog.FileName.ToString().Substring(RunDownOpenFileDialog.FileName.ToString().LastIndexOf(@"\") + 1);
                        }
                    }
                }
                RunDownFileInput.Close();
            }
            else
            {
                blnUsingLoadedRunDownFile = false;
            }
        }
    }
}
