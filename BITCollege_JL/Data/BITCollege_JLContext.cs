using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_JL.Data
{
    public class BITCollege_JLContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_JLContext() : base("name=BITCollege_JLContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.SuspendedState> SuspendedStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.HonoursState> HonoursStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_JL.Models.NextStudent> NextStudents { get; set; }
    }
}
