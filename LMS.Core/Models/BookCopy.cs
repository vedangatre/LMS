using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Models
{
    public class BookCopy
    {
        int CopyId { get; set; } 
        int BookId { get; set; }
        bool IsDamaged { get; set; } = false;
    }
}
