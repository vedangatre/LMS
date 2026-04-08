using LMS.Core.Models;
using LMS.Core.Repository.Implementation;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Services
{
    public class StudentService
    {
        IStudentRepository _studentRepository;
        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Student AddStudent(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

            return _studentRepository.AddStudent(name);
        }

        public bool DeleteStudent(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("Student ID must be a positive integer.", nameof(studentId));

            return _studentRepository.DeleteStudent(studentId);
        }

        public Student? GetStudentById(int studentId)
        {
            return _studentRepository.GetStudentById(studentId);
        }
    }
}
