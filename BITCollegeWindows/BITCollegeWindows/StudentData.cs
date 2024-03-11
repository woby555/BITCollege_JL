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
    /// <summary>
    /// Opens the StudentData Form where a summary of the student's college information is displayed.
    /// </summary>
    public partial class StudentData : Form
    {
        BITCollege_JLContext db = new BITCollege_JLContext();
        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// given:  This constructor will be used when returning to StudentData
        /// from another form.  This constructor will pass back
        /// specific information about the student and registration
        /// based on activites taking place in another form.
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            this.constructorData = constructor;

            studentNumberMaskedTextBox.Text = constructor.Student.StudentNumber.ToString();

            studentNumberMaskedTextBox_Leave(null, null);

            Refresh();
        }

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();

            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();

            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed
            this.Location = new Point(0, 0);


        }

        /// <summary>
        /// Populates the constructor with data of the current student.
        /// </summary>
        private void PopulateConstructorData()
        {
            if (studentBindingSource.Current is Student currentStudent)
            {
                if (registrationBindingSource.Current is Registration currentRegistration)
                {
                    constructorData.Student = currentStudent;
                    constructorData.Registration = currentRegistration;
                }
            }
        }

        /// <summary>
        /// Returns the selected student's data and registrations.
        /// </summary>
        private void studentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            long enteredStudentNumber;

            if (long.TryParse(studentNumberMaskedTextBox.Text, out enteredStudentNumber))
            {
                IQueryable<Student> matchedStudentQuery = db.Students
                    .Where(student => student.StudentNumber == enteredStudentNumber);

                Student matchedStudent = matchedStudentQuery.FirstOrDefault();

                if (matchedStudent == null)
                {
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;

                    studentNumberMaskedTextBox.Focus();

                    studentBindingSource.DataSource = typeof(Student);
                    registrationBindingSource.DataSource = typeof(Registration);

                    MessageBox.Show($"Student {enteredStudentNumber} does not exist.", "Student Not Found", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    IQueryable<Registration> registrationQuery = db.Registrations
                        .Where(registration => registration.StudentId == matchedStudent.StudentId);

                    List<Registration> registrationRecords = registrationQuery.ToList();

                    if (registrationRecords.Count == 0)
                    {
                        lnkUpdateGrade.Enabled = false;
                        lnkViewDetails.Enabled = false;

                        registrationBindingSource.Clear();
                    }
                    else
                    {
                        lnkUpdateGrade.Enabled = true;
                        lnkViewDetails.Enabled = true;

                        registrationBindingSource.DataSource = registrationRecords;
                    }

                    studentBindingSource.DataSource = matchedStudent;

                }
            }
        }

    }
}

