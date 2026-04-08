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
        private IssueService _issueService;

        [SetUp]
        public void Setup()
        {
            _issueRepository = new IssueRepositoryStub();
            _issueService = new IssueService(_issueRepository);
        }

        [Test]
        public void Create_WithValidIssueRecord_ReturnsIssueId()
        {
            // Arrange
            var issueRecord = new IssueRecord
            {
                StudentId = 1,
                CopyId = 1,
                IssueDate = DateTime.Now,
                ReturnDate = null
            };

            // Act
            var result = _issueService.Create(issueRecord);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void UpdateReturn_WithValidIssueIdAndReturnDate_ReturnsSuccessCount()
        {
            // Arrange
            var issueRecord = new IssueRecord
            {
                StudentId = 1,
                CopyId = 1,
                IssueDate = DateTime.Now,
                ReturnDate = null
            };
            int issueId = _issueService.Create(issueRecord);
            DateTime returnDate = DateTime.Now;

            // Act
            var result = _issueService.UpdateReturn(issueId, returnDate);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        // Test 3: GetIssuedBooks - Retrieves count of all issued books
        [Test]
        public void GetIssuedBooks_WithAllIssues_ReturnsIssuedBooksCount()
        {
            // Arrange
            var issueRecord1 = new IssueRecord
            {
                StudentId = 1,
                CopyId = 1,
                IssueDate = DateTime.Now,
                ReturnDate = null
            };
            var issueRecord2 = new IssueRecord
            {
                StudentId = 2,
                CopyId = 2,
                IssueDate = DateTime.Now,
                ReturnDate = null
            };

            _issueService.Create(issueRecord1);
            _issueService.Create(issueRecord2);

            // Act
            var result = _issueService.GetIssuedBooks();

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }
    }
}
