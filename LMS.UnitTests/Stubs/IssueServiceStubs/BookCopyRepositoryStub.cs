using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.UnitTests.Stubs.IssueServiceStubs
{
    internal class BookCopyRepositoryStub : IBookCopyRepository
    {
        private List<BookCopy> _bookCopies = new List<BookCopy>();
        private int _nextCopyId = 1;

        public BookCopyRepositoryStub()
        {
            _bookCopies.Add(new BookCopy { CopyId = 1, BookId = 1, IsDamaged = false });
            _bookCopies.Add(new BookCopy { CopyId = 2, BookId = 2, IsDamaged = false });
            _nextCopyId = 3;
        }

        public void AddCopies(int bookId, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _bookCopies.Add(new BookCopy { CopyId = _nextCopyId++, BookId = bookId, IsDamaged = false });
            }
        }

        public void RemoveCopies(int bookId, int count)
        {
            var copiesToRemove = _bookCopies.FindAll(c => c.BookId == bookId && !c.IsDamaged).Take(count).ToList();
            foreach (var copy in copiesToRemove)
            {
                _bookCopies.Remove(copy);
            }
        }

        public void DeleteByBookId(int bookId)
        {
            _bookCopies.RemoveAll(c => c.BookId == bookId);
        }

        public bool HasIssuedCopies(int bookId)
        {
            return false;
        }

        public int GetIssuedCount(int bookId)
        {
            return 0;
        }

        public int GetAvailableCount(int bookId)
        {
            return _bookCopies.FindAll(c => c.BookId == bookId && !c.IsDamaged).Count;
        }

        public BookCopy? GetAvailableCopy(int bookId)
        {
            return _bookCopies.Find(c => c.BookId == bookId && !c.IsDamaged);
        }

        public BookCopy? GetByCopyId(int copyId)
        {
            return _bookCopies.Find(c => c.CopyId == copyId);
        }
    }
}
