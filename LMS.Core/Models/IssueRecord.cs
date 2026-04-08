using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Models
{
    public class IssueRecord
    {
        int IssueId { get; set; }
        int StudentId { get; set; }
        int CopyId { get; set; }
        DateTime IssueDate { get; set; }
        DateTime? ReturnDate { get; set; }
    }
}
