using BITCollege_JL.Data;
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
    /// <summary>
    /// Opens the History form where the student's registrations are displayed.
    /// </summary>
    public partial class History : Form
    {
        BITCollege_JLContext db = new BITCollege_JLContext();

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
        public History(ConstructorData constructorData)
        {
            InitializeComponent();

            this.constructorData = constructorData;

            studentNumberMaskedLabel.Text = constructorData.Student.StudentNumber.ToString();
            descriptionLabel1.Text = constructorData.Student.AcademicProgram.Description;
            fullNameLabel1.Text = constructorData.Student.FullName;
            

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
        /// given:  Open this form in top right corner of the frame.
        /// Loads a DataGridView of the selected student's registrations.
        /// </summary>
        private void History_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            try
            {
                var query = from registration in db.Registrations
                            join course in db.Courses on registration.CourseId equals course.CourseId
                            where registration.StudentId == constructorData.Student.StudentId
                            select new
                            {
                                RegistrationNumber = registration.RegistrationNumber,
                                RegistrationDate = registration.RegistrationDate,
                                Course = registration.Course.Title,
                                Grade = registration.Grade,
                                Notes = registration.Notes
                            };
                registrationBindingSource.DataSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
