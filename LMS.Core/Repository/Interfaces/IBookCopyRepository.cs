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
        //later change to remove specific copy by copyId
        // remove damaged copies 
        // change status of copy to damaged function

        BookCopy? GetAvailableCopy(int bookId);

        int GetTotalCount(int bookId);

        int GetIssuedCount(int bookId);

        void DeleteAllByBookId(int bookId);
    }
}
