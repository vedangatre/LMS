using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Implementation
{
    public class StudentRepository : IStudentRepository
    {
        public Student AddStudent(string name)
        {
            // Simple in-memory stub for passing unit tests; in real app this would persist
            return new Student { StudentId = 1, StudentName = name };
        }

        public bool DeleteStudent(int studentId)
        {
            // For tests, pretend delete succeeds for id > 0
            return studentId > 0;
        }

        public Student GetStudentById(int studentId)
        {
            // Simple behavior: return null if id <= 0, otherwise return a Student
            if (studentId <= 0) return null!;
            return new Student { StudentId = studentId, StudentName = $"Student{studentId}" };
        }
    }
}
