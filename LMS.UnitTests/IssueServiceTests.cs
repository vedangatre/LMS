using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LMS.Core.Services;
using LMS.Core.Repository.Interfaces;
using LMS.Core.Models;
using LMS.UnitTests.Stubs.IssueServiceStubs;

namespace LMS.UnitTests
{
    public class IssueServiceTests
    {
        private IIssueRepository _issueRepository;
        private IBookCopyRepository _bookCopyRepository;
        private IssueService _issueService;

        [SetUp]
        public void Setup()
        {
            _issueRepository = new IssueRepositoryStub();
            _bookCopyRepository = new BookCopyRepositoryStub();
            _issueService = new IssueService(_issueRepository, _bookCopyRepository);
        }

        [Test]
        public void IssueBook_WithValidStudentIdAndBookId_ReturnsIssueId()
        {
            int studentId = 1;
            int bookId = 1;

            var result = _issueService.IssueBook(studentId, bookId);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void ReturnBook_WithValidIssueId_ReturnsSuccessCount()
        {
            int studentId = 1;
            int bookId = 1;
            int issueId = _issueService.IssueBook(studentId, bookId);

            var result = _issueService.ReturnBook(issueId);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void GetIssuedBooks_WithAllIssues_ReturnsIssuedBooksList()
        {
            int studentId1 = 1;
            int bookId1 = 1;
            int studentId2 = 2;
            int bookId2 = 2;

            _issueService.IssueBook(studentId1, bookId1);
            _issueService.IssueBook(studentId2, bookId2);

            var result = _issueService.GetIssuedBooks();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
