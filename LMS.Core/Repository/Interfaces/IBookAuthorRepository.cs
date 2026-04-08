using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Interfaces
{
    public interface IBookAuthorRepository
    {
        void Add(int bookId, int authorId);

        List<int> GetBookIdsByAuthorId(int authorId);

        List<int> GetAuthorIdsByBookId(int bookId); // ADD THIS (needed)
    }
}
