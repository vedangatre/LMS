using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Models
{
    public class BookCopy
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public bool IsDamaged { get; set; } = false; 
    }
}
