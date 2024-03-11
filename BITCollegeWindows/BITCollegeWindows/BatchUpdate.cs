using BITCollege_JL.Data;
using BITCollege_JL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class BatchUpdate : Form
    {
        private BITCollege_JLContext db = new BITCollege_JLContext();
        private Batch batch = new Batch();

        public BatchUpdate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Batch processing
        /// Further code to be added.
        /// </summary>
        private void lnkProcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if(radSelect.Checked)
            {
                batch.ProcessTransmission(academicProgramComboBox.SelectedValue.ToString());
            }
            else if (radAll.Checked)
            {
                foreach(AcademicProgram program in academicProgramComboBox.Items)
                {
                    string programAcronym = program.ProgramAcronym;

                    batch.ProcessTransmission(programAcronym.ToString());
                }
            }

            string result = batch.WriteLogData();

            rtxtLog.Text += result;

        }

        /// <summary>
        /// given:  Always open this form in top right of frame.
        /// Further code to be added.
        /// </summary>
        private void BatchUpdate_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            IQueryable<AcademicProgram> academicPrograms = db.AcademicPrograms;
            List<AcademicProgram> programList = academicPrograms.ToList();

            academicProgramBindingSource.DataSource = programList;
        }

        /// <summary>
        /// Handles the CheckChanged event
        /// </summary>
        private void radAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radAll.Checked)
            {
                academicProgramComboBox.Enabled = false;
            }
            else
            {
                academicProgramComboBox.Enabled = true;
            }
        }

        /// <summary>
        /// Test button to try out one transmission. Not used.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Batch batch = new Batch();

            batch.ProcessTransmission("VT");
            batch.WriteLogData();
        }
    }
}
