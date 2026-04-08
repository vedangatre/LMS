using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Models
{
    public class IssueRecord
    {
        public int IssueId { get; set; }
        public int StudentId { get; set; }
        public int CopyId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
