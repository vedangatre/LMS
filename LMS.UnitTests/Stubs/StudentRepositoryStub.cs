using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;

namespace LMS.UnitTests.Stubs
{
    public class StudentRepositoryStub : IStudentRepository
    {
        public int ExistingId { get; set; } = 1;
        public string ExistingName { get; set; } = "John";
        public bool ExistingHasActiveIssues { get; set; } = false;

        public Student AddStudent(string name)
        {

            return new Student { StudentId = ExistingId, StudentName = name };
        }

        public bool DeleteStudent(int studentId)
        {
            if (studentId != ExistingId) return false;

            if (ExistingHasActiveIssues) return false;

            return true;
        }

        public Student? GetStudentById(int studentId)
        {
            if (studentId == ExistingId) return new Student { StudentId = studentId, StudentName = ExistingName };
            return null;
        }

        public List<Student> GetAllStudents()
        {
            return new List<Student> 
            { 
                new Student { StudentId = ExistingId, StudentName = ExistingName } 
            };
        }
    }
}
