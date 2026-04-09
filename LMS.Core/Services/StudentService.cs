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
        private readonly IIssueRepository _issueRepository;

        public StudentService(IStudentRepository studentRepository)
            : this(studentRepository, new NoIssueRepository())
        {
        }

        public StudentService(IStudentRepository studentRepository, IIssueRepository issueRepository)
        {
            _studentRepository = studentRepository;
            _issueRepository = issueRepository;
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

            if (_issueRepository.HasActiveIssuesForStudent(studentId))
            {
                throw new InvalidOperationException("Cannot delete student with issued books.");
            }

            return _studentRepository.DeleteStudent(studentId);
        }

        public Student? GetStudentById(int studentId)
        {
            return _studentRepository.GetStudentById(studentId);
        }

        private sealed class NoIssueRepository : IIssueRepository
        {
            public int Create(IssueRecord issueRecord) => 0;
            public int UpdateReturn(int issueId, DateTime returnDate) => 0;
            public int GetIssuedBooks() => 0;
            public bool HasActiveIssuesForStudent(int studentId) => false;
            public List<Dto.IssuedBookReportItem> GetIssuedBookDetails() => new();
        }
    }
}
