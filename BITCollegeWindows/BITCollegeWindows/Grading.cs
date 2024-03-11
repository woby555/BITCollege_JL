using BITCollege_JL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using Utility;
using BITCollegeWindows.CollegeRegistrationService;

namespace BITCollegeWindows
{
    /// <summary>
    /// Opens the Grading form where a student's registration can be graded.
    /// </summary>
    public partial class Grading : Form
    {
        BITCollege_JLContext db = new BITCollege_JLContext();
        CollegeRegistrationClient registrationClient = new CollegeRegistrationClient();

        ///given:  student and registration data will passed throughout 
        ///application. This object will be used to store the current
        ///student and selected registration
        ConstructorData constructorData;


        /// <summary>
        /// given:  This constructor will be used when called from the
        /// Student form.  This constructor will receive 
        /// specific information about the student and registration
        /// further code required:  
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();

            this.constructorData = constructor;

            studentNumberMaskedLabel.Text = constructor.Student.StudentNumber.ToString();
            programLabel1.Text = constructor.Registration.Course.Title;
            fullNameLabel1.Text = constructor.Student.FullName;

            courseNumberMaskedLabel.Text = constructor.Registration.Course.CourseNumber.ToString();
            courseTypeLabel1.Text = constructor.Registration.Course.CourseType;
            titleLabel1.Text = constructor.Registration.Course.Title;
            gradeTextBox.Text = constructor.Registration.Grade.ToString();
        }

        /// <summary>
        /// given: This code will navigate back to the Student form with
        /// the specific student and registration data that launched
        /// this form.
        /// </summary>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Always open in this form in the top right corner of the frame.
        /// further code required:
        /// </summary>
        private void Grading_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            string courseType = constructorData.Registration.Course.CourseType;
            string courseMask = BusinessRules.CourseFormat(courseType);
            courseNumberMaskedLabel.Mask = courseMask;


            bool gradeEntered = constructorData.Registration.Grade.HasValue;

            if(gradeEntered)
            {
                gradeTextBox.Enabled = false;
                lnkUpdate.Visible = false;
                lblExisting.Visible = true;
            }
            else
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Visible = true;
                lblExisting.Visible = false;
            }
        }

        /// <summary>
        /// Handles the logic for updating a student grade
        /// </summary>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string rawGrade = Numeric.ClearFormatting(gradeTextBox.Text, "%");

            if(Numeric.IsNumeric(rawGrade, NumberStyles.Number))
            {
                double numericGrade = double.Parse(rawGrade);

                if(numericGrade >=0 && numericGrade <= 1)
                {
                    double? updatedGPA = registrationClient.UpdateGrade
                        (numericGrade, constructorData.Registration.RegistrationId, " ");

                    gradeTextBox.Text = (numericGrade * 100).ToString("F2") + "%";

                    gradeTextBox.Enabled = false;
                }

                else
                {
                    MessageBox.Show("Grade must be a decimal value between 0 and 1.", "Invalid Grade", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("An invalid numeric format was entered.", "Invalid Numeric Format",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
