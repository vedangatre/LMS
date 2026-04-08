using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;

namespace LMS.UnitTests.Stubs
{
    public class StudentRepositoryStub : IStudentRepository
    {
        // Simple, explicit fields instead of Func delegates so it's easy to understand
        public int ExistingId { get; set; } = 1;
        public string ExistingName { get; set; } = "John";

        // AddStudent: always returns a new Student with the provided name.
        public Student AddStudent(string name)
        {
            return new Student { StudentId = ExistingId, StudentName = name };
        }

        // DeleteStudent: returns true only when the id matches ExistingId
        public bool DeleteStudent(int studentId)
        {
            return studentId == ExistingId;
        }

        // GetStudentById: returns a Student when id matches ExistingId, otherwise null
        public Student? GetStudentById(int studentId)
        {
            if (studentId == ExistingId) return new Student { StudentId = studentId, StudentName = ExistingName };
            return null;
        }
    }
}
