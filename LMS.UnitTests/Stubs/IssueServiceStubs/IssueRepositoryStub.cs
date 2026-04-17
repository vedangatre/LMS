using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
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

        public List<IssueRecord> GetIssuedBooks()
        {
            return _issueRecords.FindAll(r => r.ReturnDate == null);
        }

        public IssueRecord? GetIssueRecordById(int issueId)
        {
            return _issueRecords.Find(r => r.IssueId == issueId);
        }
    }
}

