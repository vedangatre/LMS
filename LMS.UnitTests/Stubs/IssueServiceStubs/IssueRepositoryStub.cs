using LMS.Core.Dto;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.UnitTests.Stubs.IssueServiceStubs
{
    // Stub implementation of IIssueRepository
    internal class IssueRepositoryStub : IIssueRepository
    {
        private List<IssueRecord> _issueRecords = new List<IssueRecord>();
        private int _nextIssueId = 1;

        public int Create(IssueRecord issueRecord)
        {
            issueRecord.IssueId = _nextIssueId;
            _issueRecords.Add(issueRecord);
            return _nextIssueId++;
        }

        public int UpdateReturn(int issueId, DateTime returnDate)
        {
            var issueRecord = _issueRecords.Find(r => r.IssueId == issueId);
            if (issueRecord != null)
            {
                issueRecord.ReturnDate = returnDate;
                return 1; // Return 1 to indicate 1 record was updated
            }

            return 0; // Return 0 if no record was found
        }

        public int GetIssuedBooks()
        {
            return _issueRecords.FindAll(r => r.ReturnDate == null).Count;
        }

        public bool HasActiveIssuesForStudent(int studentId)
        {
            return _issueRecords.Any(r => r.StudentId == studentId && r.ReturnDate == null);
        }

        public List<IssuedBookReportItem> GetIssuedBookDetails()
        {
            return new List<IssuedBookReportItem>();
        }
    }
}

