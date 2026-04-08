using LMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Interfaces
{
    public interface IBookCopyRepository
    {
        void AddCopies(int bookId, int count);

        void RemoveCopies(int bookId, int count);

        BookCopy? GetAvailableCopy(int bookId);
    }
}
