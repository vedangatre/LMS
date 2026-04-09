using System;
using System.Collections.Generic;
using System.Text;
using LMS.Core.Models;

namespace LMS.Core.Repository.Interfaces
{
    public interface IIssueRepository
    {
        int Create(IssueRecord issueRecord);
        int UpdateReturn(int issueId, DateTime returnDate);
        int GetIssuedBooks();
    }
}
