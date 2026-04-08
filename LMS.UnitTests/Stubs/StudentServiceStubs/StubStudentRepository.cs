using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;

namespace LMS.UnitTests.Stubs.StudentServiceStubs
{
    public class StubStudentRepository : IStudentRepository
    {
        public Func<string, Student>? AddStudentFunc { get; set; }
        public Func<int, bool>? DeleteStudentFunc { get; set; }
        public Func<int, Student?>? GetStudentByIdFunc { get; set; }

        public Student AddStudent(string name)
        {
            if (AddStudentFunc == null) throw new InvalidOperationException("AddStudentFunc not set");
            return AddStudentFunc(name);
        }

        public bool DeleteStudent(int studentId)
        {
            if (DeleteStudentFunc == null) throw new InvalidOperationException("DeleteStudentFunc not set");
            return DeleteStudentFunc(studentId);
        }

        public Student? GetStudentById(int studentId)
        {
            if (GetStudentByIdFunc == null) return null;
            return GetStudentByIdFunc(studentId);
        }
    }
}
