using System;
using System.Collections.Generic;
using System.Text;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Services
{
    public class IssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IBookCopyRepository _bookCopyRepository;

        public IssueService(IIssueRepository issueRepository, IBookCopyRepository bookCopyRepository)
        {
            _issueRepository = issueRepository;
            _bookCopyRepository = bookCopyRepository;
        }

        public int IssueBook(int studentId, int bookId)
        {
            var availableCopy = _bookCopyRepository.GetAvailableCopy(bookId);
            if (availableCopy == null)
            {
                throw new InvalidOperationException("No available copy for this book.");
            }

            var issueRecord = new IssueRecord
            {
                StudentId = studentId,
                CopyId = availableCopy.CopyId,
                IssueDate = DateTime.Now,
                ReturnDate = null
            };

            return _issueRepository.Create(issueRecord);
        }

        public int ReturnBook(int issueId)
        {
            return _issueRepository.UpdateReturn(issueId, DateTime.Now);
        }

        public List<IssueRecord> GetIssuedBooks()
        {
            return _issueRepository.GetIssuedBooks();
        }
    }
}
