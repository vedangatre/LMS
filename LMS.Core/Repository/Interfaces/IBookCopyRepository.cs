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
        void DeleteByBookId(int bookId);
        bool HasIssuedCopies(int bookId);
        int GetIssuedCount(int bookId);
        int GetAvailableCount(int bookId);
        //later change to remove specific copy by copyId
        // remove damaged copies 
        // change status of copy to damaged function

        BookCopy? GetAvailableCopy(int bookId);
        BookCopy? GetByCopyId(int copyId);
    }
}
