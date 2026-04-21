using LMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Interfaces
{
    public interface IStudentRepository
    {
        public Student AddStudent(string name);
        public bool DeleteStudent(int studentId);
        public Student? GetStudentById(int studentId);
        public List<Student> GetAllStudents();
    }
}
