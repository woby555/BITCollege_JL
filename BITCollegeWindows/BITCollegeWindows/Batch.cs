using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using BITCollege_JL.Data;
using BITCollege_JL.Models;
using Utility;
using System.Globalization;
using BITCollegeWindows.CollegeRegistrationService;

namespace BITCollegeWindows
{
    /// <summary>
    /// Batch:  This class provides functionality that will validate
    /// and process incoming xml files.
    /// </summary>
    public class Batch
    {

        private String inputFileName;
        private String logFileName;
        private String logData;

        BITCollege_JLContext db = new BITCollege_JLContext();
        CollegeRegistrationClient registrationClient = new CollegeRegistrationClient();

        /// <summary>
        /// Processes and displays error for each query iteration
        /// </summary>
        /// <param name="beforeQuery">The previous query</param>
        /// <param name="afterQuery">The current query being validated</param>
        /// <param name="message">The error message</param>
        private void ProcessErrors(IEnumerable<XElement> beforeQuery, IEnumerable<XElement> afterQuery, String message)
        {
            IEnumerable<XElement> errors = beforeQuery.Except(afterQuery);

            foreach (XElement item in errors)
            {
                logData += "\r\n----------ERROR----------";
                logData += "\r\nFile: " + inputFileName;
                logData += "\r\nProgram: " + item.Element("program");
                logData += "\r\nStudent Number: " + item.Element("student_no");
                logData += "\r\nCourse Number: " + item.Element("course_no");
                logData += "\r\nRegistration Number: " + item.Element("registration_no");
                logData += "\r\nType: " + item.Element("type");
                logData += "\r\nGrade: " + item.Element("grade");
                logData += "\r\nNotes: " + item.Element("notes");
                logData += "\r\nNode Count:" + item.Nodes().Count();
                logData += "\r\nError Message: " + message;
                logData += "\r\n------------------------------";
            }
        }

        /// <summary>
        /// Processes the header of the file before going into inner elements
        /// </summary>
        /// <exception cref="Exception">Incorrect number of root files, date, checksum, or program acronym</exception>
        private void ProcessHeader()
        {
            XDocument xDoc = XDocument.Load(inputFileName);

            XElement rootElement = xDoc.Element("student_update");

            DateTime dateAttribute = DateTime.Parse(rootElement.Attribute("date").Value);

            string programAcronym = rootElement.Attribute("program").Value;
            AcademicProgram matchProgram = db.AcademicPrograms.FirstOrDefault(x => x.ProgramAcronym == programAcronym);

            XAttribute checksum = rootElement.Attribute("checksum");

            int expectedChecksum = int.Parse(checksum.Value);

            int actualChecksum = rootElement.Elements("transaction")
                .Elements("student_no")
                .Select(y => int.Parse(y.Value))
                .Sum();

            if (rootElement.Attributes().Count() != 3)
            {
                throw new Exception($"\r\nIncorrect number of root attributes for file {inputFileName}");
            }

            if (dateAttribute.Date != DateTime.Today.Date)
            {
                throw new Exception($"\r\nIncorrect date for file {inputFileName}");
            }

            if(matchProgram == null)
            {
                throw new Exception($"\r\nProgram acronym not found for file {inputFileName}");
            }

            if(actualChecksum != expectedChecksum)
            {
                throw new Exception($"\r\nIncorrect checksum value for file {inputFileName}");
            }


        }

        /// <summary>
        /// Validates each attribute and removes transaction from being processed if errors are found.
        /// </summary>
        private void ProcessDetails()
        {
            XDocument xDoc = XDocument.Load(inputFileName);

            IEnumerable<XElement> first = xDoc.Descendants().Elements("transaction");

            IEnumerable<XElement> second = first.Where(x => x.Nodes().Count() == 7);

            ProcessErrors(first, second, "Node count is not 7");

            IEnumerable<XElement> third = second.Where(x => x.Element("program").Value == xDoc.Root.Attribute("program").Value);

            ProcessErrors(second, third, "Transaction program does not match root program");

            IEnumerable<XElement> fourth = third.Where(x => Numeric.IsNumeric(x.Element("type").Value, NumberStyles.Number));
            ProcessErrors(third, fourth, "Type element is not numeric.");

            IEnumerable<XElement> fifth = fourth.Where(x => Numeric.IsNumeric(x.Element("grade").Value, NumberStyles.Number) || x.Element("grade").Value == "*");
            ProcessErrors(fourth, fifth, "Grade element is not number or '*'");

            IEnumerable<XElement> sixth = fifth.Where(x => x.Element("type").Value == "1" || x.Element("type").Value == "2");
            ProcessErrors(fifth, sixth, "Type element must have a value of 1 or 2");

            IEnumerable<XElement> seventh = sixth.Where(x =>
                (x.Element("type").Value == "1" && x.Element("grade").Value == "*") ||
                (x.Element("type").Value == "2" &&
                    Numeric.IsNumeric(x.Element("grade").Value, NumberStyles.Number) &&
                    double.TryParse(x.Element("grade").Value, out double gradeValue) &&
                    gradeValue >= 0 && gradeValue <= 100)
            );

            ProcessErrors(sixth, seventh, "Invalid grade value for the type");

            IEnumerable<long> studentNumbers = db.Students.Select(s => s.StudentNumber).ToList();
            IEnumerable<XElement> eighth = seventh.Where(x => studentNumbers.Contains(long.Parse(x.Element("student_no").Value)));
            ProcessErrors(seventh, eighth, "Invalid student_no");

            IEnumerable<string> courseNumbers = db.Courses.Select(c => c.CourseNumber).ToList();
            IEnumerable<XElement> ninth = eighth.Where(x =>
                x.Element("type").Value == "2" ||
                (x.Element("type").Value == "1" && courseNumbers.Contains(x.Element("course_no").Value))
            );
            ProcessErrors(eighth, ninth, "Invalid course_no");

            IEnumerable<long> registrationNumbers = db.Registrations.Select(r => r.RegistrationNumber).ToList();
            IEnumerable<XElement> tenth = ninth.Where(x =>
                (x.Element("type").Value == "1" && x.Element("registration_no").Value == "*") ||
                (x.Element("type").Value == "2" && registrationNumbers.Contains(long.Parse(x.Element("registration_no").Value)))
            );
            ProcessErrors(ninth, tenth, "Invalid registration_no");

            ProcessTransactions(tenth);
        }

        /// <summary>
        /// Processes validated transactions and either updates a grade or registers for a class.
        /// </summary>
        /// <param name="transactionRecords">Transactions that are validated.</param>
        private void ProcessTransactions(IEnumerable<XElement> transactionRecords)
        {
            foreach(XElement transaction in transactionRecords)
            {
                int type = int.Parse(transaction.Element("type").Value);
                string notes = transaction.Element("notes").Value;

                long studentNumber = long.Parse(transaction.Element("student_no").Value);
                int studentId = db.Students
                                .Where(x => x.StudentNumber == studentNumber)
                                .Select(x => x.StudentId)
                                .FirstOrDefault();

                string courseNumber = transaction.Element("course_no").Value;
                int courseId = db.Courses
                                .Where(x => x.CourseNumber == courseNumber)
                                .Select(x => x.CourseId)
                                .FirstOrDefault();

                if (type == 1) 
                {
                    int returnCode = registrationClient.RegisterCourse(studentId, courseId, notes);
                    switch (returnCode)
                    {
                        case 0:
                            logData += $"\r\nStudent: {studentNumber} has successfully registered for course: {courseNumber}";
                        break;

                        case -100:
                            logData += $"\r\nREGISTRATION ERROR:{BusinessRules.RegisterError(-100)}";
                            break;
                        case -200:
                            logData += $"\r\nREGISTRATION ERROR:{BusinessRules.RegisterError(-200)}";
                            break;
                        case -300:
                            logData += $"\r\nREGISTRATION ERROR:{BusinessRules.RegisterError(-300)}";
                            break;
                    }
                }
                if (type == 2) 
                {
                    double gradeInput = double.Parse(transaction.Element("grade").Value);
                    double validGrade = gradeInput / 100;
                    long registrationNumber = long.Parse(transaction.Element("registration_no").Value);
                    int registrationId = db.Registrations
                                       .Where(x=> x.RegistrationNumber == registrationNumber)
                                       .Select(x=> x.RegistrationId)
                                       .FirstOrDefault();
                    try
                    {
                        double? calculatedGPA = registrationClient.UpdateGrade(validGrade, registrationId, notes);
                        if (calculatedGPA.HasValue)
                        {
                            logData += $"\r\nA grade of {gradeInput} has been successfully applied to the registration {registrationId}.";
                        }
                        else
                        {
                            logData += $"\r\n Grade update failed for Registration ID: {registrationId}.";
                        }
                    }
                    catch (Exception ex) 
                    {
                        logData += $"\r\nGrade update failed for Registration Id: {registrationId}. Exception: {ex.Message}";
                    }
                }
            }
        }

        /// <summary>
        /// Writes a log file on the actions taken when processing an XML file
        /// </summary>
        /// <returns>A processed transmission file</returns>
        public string WriteLogData()
        {
            if (!string.IsNullOrEmpty(logFileName))
            {
                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.Write(logData);
                    writer.Close();
                }
            }

            string capturedLogData = logData;

            logData = string.Empty;

            logFileName = string.Empty;

            return capturedLogData;
        }

        /// <summary>
        /// Processes a transmission XML file
        /// </summary>
        /// <param name="programAcronym">The acronym of the program of the XML file</param>
        public void ProcessTransmission(String programAcronym)
        {
            inputFileName = DateTime.Now.Year + "-" + DateTime.Now.DayOfYear + "-" + programAcronym + ".xml";

            logFileName = "LOG " + inputFileName.Replace("xml", "txt");

            if(!File.Exists(inputFileName))
            {
                logData += $"\r\nFile does not exist: {inputFileName}\n";
            }
            else
            {
                try
                {
                    ProcessHeader();
                    ProcessDetails();
                }
                catch (Exception ex)
                {
                    logData += $"\r\nException occurred: {ex.Message}\n";
                }
            }
        }
    }
}
