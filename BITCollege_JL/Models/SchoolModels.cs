/*
 * Name: Jake Licmo
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-09-02
 * Updated: 2023-09-02
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;
using System.Web.Mvc;
using BITCollege_JL.Controllers;
using BITCollege_JL.Data;
using System.Linq.Expressions;
using System.Diagnostics.Eventing.Reader;
using System.Data.Entity;

namespace BITCollege_JL.Models
{
    /// <summary>
    /// Student model. Describes the Student table in the database.
    /// </summary>
    public class Student
    {
        protected static BITCollege_JLContext db = new BITCollege_JLContext();

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }


        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)", ErrorMessage = "A valid Canadian province code must be entered.")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Grade Point\nAverage")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(0, 4.5)]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get { return String.Format("{0} {1} {2}", Address, City, Province); }
        }

        /// <summary>
        /// Updates the student's GradePointState.
        /// </summary>
        public void ChangeState()
        {
            int currentGradePointStateId = this.GradePointStateId;
            GradePointState before;

            do
            {
                before = db.GradePointStates.Find(currentGradePointStateId);

                before.StateChangeCheck(this);

                currentGradePointStateId = this.GradePointStateId;

            } while (currentGradePointStateId != before.GradePointStateId);
        }

        /// <summary>
        /// Sets the next student number for the new data entry.
        /// </summary>
        /// <exception cref="Exception">Thrown when the number returns null.</exception>
        public void SetNextStudentNumber()
        {
            string table = "NextStudent";
            long? nextNumber = StoredProcedures.NextNumber(table);


            if (nextNumber.HasValue)
            {
                long validatedNextNumber = nextNumber.Value;
                StudentNumber = validatedNextNumber;
            }
            else
            {
                throw new Exception();
            }
        }

        //Navigation Properties

        public virtual GradePointState GradePointState { get; set; }

        public virtual AcademicProgram AcademicProgram { get; set; }

        public virtual ICollection<Registration> Registrations { get; set; }
    }

    /// <summary>
    /// AcademicProgram model. Describes the Academic Program table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name = "Program\nName")]
        public string Description { get; set; }

        //Navigation Properties
        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }

    /// <summary>
    /// GradePointState model. Describes the Grade Point State table in the database.
    /// </summary>
    public abstract class GradePointState
    {
        protected static BITCollege_JLContext db = new BITCollege_JLContext();

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Tuition Rate\nFactor")]
        public double TuitionRateFactor { get; set; }

        [Display(Name = "State")]
        public string Description { get { return BusinessRules.ParseString(GetType().Name, "State"); } }

        public abstract void StateChangeCheck(Student student);

        public abstract double TuitionRateAdjustment(Student student);

        //Navigation Property
        public virtual ICollection<Student> Students { get; set; }

    }

    /// <summary>
    /// SuspendedState model. Represents the suspended state of the grade point.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState suspendedState;
        private const double lowerLimit = 0.00;
        private const double upperLimit = 1.00;
        private double suspendedTuitionRateFactor = 1.1;

        private SuspendedState()
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            TuitionRateFactor = suspendedTuitionRateFactor;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of SuspendedState
        /// </summary>
        /// <returns>The existing or new instance of SuspendedState</returns>
        public static SuspendedState GetInstance()
        {
            if (suspendedState == null)
            {
                suspendedState = db.SuspendedStates.SingleOrDefault();

                    if (suspendedState == null)
                    {
                        suspendedState = new SuspendedState();
                        db.SuspendedStates.Add(suspendedState);
                        db.SaveChanges();
                    }
            }
            
            return suspendedState;
        }

        /// <summary>
        /// Adjusts the student's tuition based on their current Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        /// <returns>The adjusted tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localvalue = suspendedTuitionRateFactor;

            if (student.GradePointAverage < 0.50)
            {
                localvalue += 0.05;
            }
            else if (student.GradePointAverage < 0.75)
            {
                localvalue += 0.02;
            }
            else
            {
                return localvalue;
            }
            return localvalue;
        }

        /// <summary>
        /// Moves the student between grade point states based on their Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        public override void StateChangeCheck(Student student)
        {
            if(student.GradePointAverage > upperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
        }

    }

    /// <summary>
    /// ProbationState model. Represents the probation state of the grade point.
    /// </summary>
    public class ProbationState : GradePointState
    {
        private static ProbationState probationState;
        private const double lowerLimit = 1.00;
        private const double upperLimit = 2.00;
        private double probationTuitionRateFactor = 1.075;

        private ProbationState()
        {
            LowerLimit = 1.00;
            UpperLimit = 2.00;
            TuitionRateFactor = probationTuitionRateFactor;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of ProbationState
        /// </summary>
        /// <returns>The existing or new instance of ProbationState</returns>
        public static ProbationState GetInstance()
        {
            if (probationState == null)
            {
                probationState = db.ProbationStates.SingleOrDefault();

                if(probationState == null)
                {
                    probationState = new ProbationState();
                    db.ProbationStates.Add(probationState);
                    db.SaveChanges();
                }
            }

            return probationState;
        }

        /// <summary>
        /// Adjusts the student's tuition based on their current Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        /// <returns>The adjusted tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localvalue = probationTuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                       && x.Grade != null);

            int completedCourses = studentCourses.Count();

            if (completedCourses >= 5)
            {
                localvalue += 0.035;
            }

            return localvalue;
        }

        /// <summary>
        /// Moves the student between grade point states based on their Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < lowerLimit)
            {
                student.GradePointStateId = SuspendedState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
            else if (student.GradePointAverage > upperLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
        }

    }

    /// <summary>
    /// RegularState model. Represents the regular state of the grade point.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;
        private const double lowerLimit = 2;
        private const double upperLimit = 3.7;
        private double regularTuitionRateFactor = 1.0;
        private RegularState()
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            TuitionRateFactor = regularTuitionRateFactor;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of RegularState
        /// </summary>
        /// <returns>The existing or new instance of RegularState</returns>
        public static RegularState GetInstance()
        {
            if(regularState == null)
            {
                regularState = db.RegularStates.SingleOrDefault();

                if(regularState == null)
                {
                    regularState= new RegularState();
                    db.RegularStates.Add(regularState);
                    db.SaveChanges();
                }
            }

            return regularState;
        }

        /// <summary>
        /// Adjusts the student's tuition based on their current Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        /// <returns>The adjusted tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            return regularTuitionRateFactor;
        }

        /// <summary>
        /// Moves the student between grade point states based on their Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < lowerLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
            else if (student.GradePointAverage > upperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// HonoursState model. Represents the honours state of the grade point.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;
        private const double lowerLimit = 3.7;
        private const double upperLimit = 4.50;
        private double honoursTuitionRateFactor = 0.9;

        private HonoursState()
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            TuitionRateFactor = honoursTuitionRateFactor;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of HonoursState
        /// </summary>
        /// <returns>The existing or new instance of HonoursState</returns>
        public static HonoursState GetInstance()
        {
            if (honoursState == null)
            {
                honoursState = db.HonoursStates.SingleOrDefault();

                if(honoursState == null)
                {
                    honoursState= new HonoursState();
                    db.HonoursStates.Add(honoursState);
                    db.SaveChanges();

                }
            }

            return honoursState;
        }

        /// <summary>
        /// Adjusts the student's tuition based on their current Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        /// <returns>The adjusted tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localvalue = honoursTuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                      && x.Grade != null);

            int completedCourses = studentCourses.Count();

            if(completedCourses >= 5)
            {
                localvalue -= 0.05;
            }
            if (student.GradePointAverage > 4.25)
            {
                localvalue -= 0.02;
            }


            return localvalue;
        }

        /// <summary>
        /// Moves the student between grade point states based on their Grade Point Average.
        /// </summary>
        /// <param name="student">The student to be evaluated.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < lowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Course model. Describes the Course table in the database.
    /// </summary>
    public abstract class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }


        [Display(Name = "Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Credit\nHours")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double CreditHours { get; set; }

        [Required]
        [Display(Name = "Tuition\nAmount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public string CourseType { get { return BusinessRules.ParseString(GetType().Name, "Course"); } }

        public string Notes { get; set; }

        public abstract void SetNextCourseNumber();

        //Navigation Property
        public virtual AcademicProgram AcademicProgram { get; set; }

        public virtual ICollection<Registration> Registration { get; set; }

    }

    /// <summary>
    /// GradedCourse model. Describes the Graded Course table in the database.
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [Display(Name = "Assignments")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [Display(Name = "Exams")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Sets the next Graded Course number for the new data entry.
        /// </summary>
        /// <exception cref="Exception">Thrown when the number returns null.</exception>
        public override void SetNextCourseNumber()
        {
            string table = "NextGradedCourse";
            long? nextNumber = StoredProcedures.NextNumber(table);

            if(nextNumber.HasValue)
            {
                long validatedNextNumber = nextNumber.Value;
                CourseNumber = "G-" + validatedNextNumber;
            }
            else
            {
                throw new Exception();
            }
        }
    }

    /// <summary>
    /// AuditCourse model. Describes the Audit Course table in the database.
    /// </summary>
    public class AuditCourse : Course 
    {
        /// <summary>
        /// Sets the next audit course number for the new data entry.
        /// </summary>
        /// <exception cref="Exception">Thrown when the number returns null.</exception>
        public override void SetNextCourseNumber()
        {
            string table = "NextAuditCourse";
            long? nextNumber = StoredProcedures.NextNumber(table);

            if (nextNumber.HasValue)
            {
                long validatedNextNumber = nextNumber.Value;
                CourseNumber = "A-" + validatedNextNumber;
            }
            else
            {
                throw new Exception();
            }
        }
    }

    /// <summary>
    /// MasteryCourse. Describes the Mastery Course table in the database.
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name = "Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Sets the next Mastery Course number for the new data entry.
        /// </summary>
        /// <exception cref="Exception">Thrown when the number returns null.</exception>
        public override void SetNextCourseNumber()
        {
            string table = "NextMasteryCourse";
            long? nextNumber = StoredProcedures.NextNumber(table);

            if (nextNumber.HasValue)
            {
                long validatedNextNumber = nextNumber.Value;
                CourseNumber = "M-" + validatedNextNumber;
            }
            else
            {
                throw new Exception();
            }
        }
    }

    /// <summary>
    /// Registration Model. Describes the Registration table in the database.
    /// </summary>
    public class Registration
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }


        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText = "Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Sets the next registration number for the new data entry.
        /// </summary>
        /// <exception cref="Exception">Thrown when the number returns null.</exception>
        public void SetNextRegistrationNumber()
        {
            string table = "NextRegistration";
            long? nextNumber = StoredProcedures.NextNumber(table);

            if (nextNumber.HasValue)
            {
                long validatedNextNumber = nextNumber.Value;
                RegistrationNumber = validatedNextNumber;
            }
            else
            {
                throw new Exception();
            }
        }

        //Navigation Properties
        public virtual Student Student { get; set; }

        public virtual Course Course { get; set; }
    }

    /// <summary>
    /// NextUniqueNumber Model. Highlights the next unique number for its subclasses.
    /// </summary>
    public abstract class NextUniqueNumber
    {
        protected static BITCollege_JLContext db = new BITCollege_JLContext();

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber { get; set; }
    }

    /// <summary>
    /// NextStudent Model. Describes the NextStudent table in the database.
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        /// <summary>
        /// Creates an instance of NextStudent with the NextAvailable Number of 20000000.
        /// </summary>
        private NextStudent()
        {
            NextAvailableNumber = 20000000;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of NextStudent
        /// </summary>
        /// <returns>The existing or new instance of NextStudent</returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null) 
            {
                nextStudent = db.NextStudents.SingleOrDefault();

                if (nextStudent == null)
                {
                    nextStudent = new NextStudent();
                    db.NextStudents.Add(nextStudent);
                    db.SaveChanges();
                }
            }

            return nextStudent;
        }
        
    }

    /// <summary>
    /// NextGradedCourse Model. Represents the NextGradedCourse in the database.
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        /// <summary>
        /// Creates an instance of NextGradedCourse with a NextAvailableNumber of 200000.
        /// </summary>
        private NextGradedCourse()
        {
            NextAvailableNumber = 200000;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of NextGradedCourse.
        /// </summary>
        /// <returns>The existing or new instance of NextGradedCourse</returns>
        public static NextGradedCourse GetInstance()
        {
            if (nextGradedCourse == null)
            {
                nextGradedCourse = db.NextGradedCourses.SingleOrDefault();

                if(nextGradedCourse == null)
                {
                    nextGradedCourse = new NextGradedCourse();
                    db.NextGradedCourses.Add(nextGradedCourse);
                    db.SaveChanges();
                }
            }

            return nextGradedCourse;
        }
    }

    /// <summary>
    /// NextAuditCourse Model. Represents the NextAuditCourse in the database.
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber 
    {
        private static NextAuditCourse nextAuditCourse;

        /// <summary>
        /// Creates an instance of NextAuditCourse with a NextAvailableNumber of 2000.
        /// </summary>
        private NextAuditCourse()
        {
            NextAvailableNumber = 2000;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of NextAuditCourse.
        /// </summary>
        /// <returns>The existing or new instance of NextAuditCourse</returns>
        public static NextAuditCourse GetInstance()
        {
            if (nextAuditCourse == null)
            {
                nextAuditCourse = db.NextAuditCourses.SingleOrDefault();

                if (nextAuditCourse == null)
                {
                    nextAuditCourse = new NextAuditCourse();
                    db.NextAuditCourses.Add(nextAuditCourse);
                    db.SaveChanges();
                }
            }

            return nextAuditCourse;
        }
    }

    /// <summary>
    /// NextMasteryCourse Model. Represents the NextMasteryCourse in the database.
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        /// <summary>
        /// Creates an instance of NextMasteryCourse with a NextAvailableNumber of 20000.
        /// </summary>
        private NextMasteryCourse()
        {
            NextAvailableNumber = 20000;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of NextMasteryCourse.
        /// </summary>
        /// <returns>The existing or new instance of NextMasteryCourse</returns>
        public static NextMasteryCourse GetInstance()
        {
            if (nextMasteryCourse == null)
            {
                nextMasteryCourse = db.NextMasteryCourses.SingleOrDefault();

                if(nextMasteryCourse == null)
                {
                    nextMasteryCourse = new NextMasteryCourse();
                    db.NextMasteryCourses.Add(nextMasteryCourse);
                    db.SaveChanges();
                }
            }
            return nextMasteryCourse;
        }
    }

    /// <summary>
    /// NextRegistration Model. Represents the NextRegistration in the database.
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        /// <summary>
        /// Creates an instance of NextRegistration with a NextAvailableNumber of 700.
        /// </summary>
        private NextRegistration()
        {
            NextAvailableNumber = 700;
        }

        /// <summary>
        /// Returns an existing instance or creates a new instance of NextRegistration.
        /// </summary>
        /// <returns>The existing or new instance of NextRegistration</returns>
        public static NextRegistration GetInstance()
        {
            if(nextRegistration == null)
            {
                nextRegistration = db.NextRegistrations.SingleOrDefault();

                if( nextRegistration == null ) 
                {
                    nextRegistration = new NextRegistration();
                    db.NextRegistrations.Add(nextRegistration);
                    db.SaveChanges();
                }
            }
            return nextRegistration;
        }
    }
}