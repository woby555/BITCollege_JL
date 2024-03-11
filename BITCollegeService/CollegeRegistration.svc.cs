using BITCollege_JL.Data;
using BITCollege_JL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class CollegeRegistration : ICollegeRegistration
    {
        private static BITCollege_JLContext db = new BITCollege_JLContext();

        /// <summary>
        /// Drops the specified course.
        /// </summary>
        /// <param name="registrationId">The registration ID of the course of which to drop.</param>
        /// <returns>A bool of true or false if the course is successfully dropped.</returns>
        public bool DropCourse(int registrationId)
        {
            try
            {
                Registration registration = db.Registrations.Find(registrationId);

                if (registration != null)
                {

                    db.Registrations.Remove(registration);

                    db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a course.
        /// </summary>
        /// <param name="studentId">The student ID to register to a course.</param>
        /// <param name="courseId">The course ID to register to.</param>
        /// <param name="notes">Any additional notes for registration.</param>
        /// <returns>An int which determines if a course is successfully registered or not.</returns>
        public int RegisterCourse(int studentId, int courseId, string notes)
        {
            int returnvalue = 0;
            IQueryable<Registration> allRecords = db.Registrations.Where(x => x.StudentId == studentId && x.CourseId == courseId);

            Course course = db.Courses.Find(courseId);
            Student student = db.Students.Find(studentId);

            IEnumerable<Registration> nullRecords = allRecords.Where(x => x.Grade == null);

            if (nullRecords.Count() > 0)
            {
                returnvalue = -100;
            }

            if (BusinessRules.CourseTypeLookup(course.CourseType) == CourseType.MASTERY)
            {
                MasteryCourse masteryCourse = (MasteryCourse)course;

                int maximumAttempts = masteryCourse.MaximumAttempts;

                IEnumerable<Registration> completeRecords = allRecords.Where(x => x.Grade != null);

                if (completeRecords.Count() >= maximumAttempts)
                {
                    returnvalue = -200;
                }
            }

            if (returnvalue == 0)
            {
                try
                {
                    Registration registration = new Registration();

                    registration.StudentId = studentId;
                    registration.CourseId = courseId;
                    registration.Notes = notes;
                    registration.RegistrationDate = DateTime.Today;
                    registration.SetNextRegistrationNumber();

                    double tuitionAmount = course.TuitionAmount;
                    double adjustedTuitionAmount = student.GradePointState.TuitionRateAdjustment(student) * tuitionAmount;

                    student.OutstandingFees += adjustedTuitionAmount;

                    db.Registrations.Add(registration);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    returnvalue = -300;
                }
            }

            return returnvalue;

        }

        /// <summary>
        /// Updates the grade of a registration.
        /// </summary>
        /// <param name="grade">The grade value.</param>
        /// <param name="registrationId">The course registration ID.</param>
        /// <param name="notes">Any additional notes.</param>
        /// <returns>The updated grade point average after updating a grade.</returns>
        public double? UpdateGrade(double grade, int registrationId, string notes)
        {
            Registration registration = db.Registrations.Find(registrationId);

            registration.Grade = grade;
            registration.Notes = notes;

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                    }
                }
            }

            double? gradePointAverage = CalculateGradePointAverage(registration.StudentId);

            return gradePointAverage;
        }


        /// <summary>
        /// Calculates the grade point average.
        /// </summary>
        /// <param name="studentId">The student Id to update the GPA.</param>
        /// <returns>The calculated grade point average.</returns>
        private double? CalculateGradePointAverage(int studentId)
        {
            double totalGradePointValue = 0;
            double totalCreditHours = 0;
            double? calculatedGradePointAverage = null;

            IQueryable<Registration> registrations = db.Registrations.Where(x => x.StudentId == studentId && x.Grade != null);

            foreach (Registration record in registrations.ToList())
            {
                double grade = record.Grade.Value;

                CourseType courseType = BusinessRules.CourseTypeLookup(record.Course.CourseType);

                if (courseType != CourseType.AUDIT)
                {
                    double gradePointValue = BusinessRules.GradeLookup(grade, courseType);
                    double creditHours = record.Course.CreditHours;

                    totalGradePointValue += gradePointValue * creditHours;
                    totalCreditHours += creditHours;
                }
            }

            if(totalCreditHours == 0)
            {
                calculatedGradePointAverage = null;
            }
            else 
            {
                calculatedGradePointAverage = totalGradePointValue / totalCreditHours;
            }

            Student student = db.Students.Find(studentId);

            student.GradePointAverage = calculatedGradePointAverage;

            db.SaveChanges();

            student.ChangeState();

            return calculatedGradePointAverage;
        }
    }
}
