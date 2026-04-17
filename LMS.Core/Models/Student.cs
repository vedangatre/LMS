using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
    }
}
