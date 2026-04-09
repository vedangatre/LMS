using System.Collections.Generic;

namespace LMS.Core.Models
{
    public class BookDetails
    {
        public int BookId { get; set; }
        public string BookName { get; set; } = string.Empty;
        public List<string> AuthorNames { get; set; } = new();
        public int IssuedCount { get; set; }
        public int AvailableCount { get; set; }
    }
}
