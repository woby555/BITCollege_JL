using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        /// <summary>
        /// Drops the specified course.
        /// </summary>
        /// <param name="registrationId">The registration ID of the course of which to drop.</param>
        /// <returns>A bool of true or false if the course is successfully dropped.</returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Registers a course.
        /// </summary>
        /// <param name="studentId">The student ID to register to a course.</param>
        /// <param name="courseId">The course ID to register to.</param>
        /// <param name="notes">Any additional notes for registration.</param>
        /// <returns>An int which determines if a course is successfully registered or not.</returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, string notes);


        /// <summary>
        /// Updates the grade of a registration.
        /// </summary>
        /// <param name="grade">The grade value.</param>
        /// <param name="registrationId">The course registration ID.</param>
        /// <param name="notes">Any additional notes.</param>
        /// <returns>The updated grade point average after updating a grade.</returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, string notes);
    }
}
