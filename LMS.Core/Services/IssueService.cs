using System;
using System.Collections.Generic;
using System.Text;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Services
{
    public class IssueService
    {
        private IIssueRepository _issueRepository;

        public IssueService(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public int Create(IssueRecord issueRecord)
        {
            return _issueRepository.Create(issueRecord);
        }

        public int UpdateReturn(int issueId, DateTime returnDate)
        {
            return _issueRepository.UpdateReturn(issueId, returnDate);
        }

        public int GetIssuedBooks()
        {
            return _issueRepository.GetIssuedBooks();
        }
    }
}
