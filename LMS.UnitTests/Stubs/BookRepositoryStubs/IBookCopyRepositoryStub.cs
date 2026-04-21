using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.UnitTests.Stubs.BookServiceStubs
{
    internal class IBookCopyRepositoryStub : IBookCopyRepository
    {
        private readonly Dictionary<int, int> _availableCopiesByBookId = new();
        private readonly Dictionary<int, int> _issuedCopiesByBookId = new();

        public int AddCopiesCallCount { get; private set; }
        public int RemoveCopiesCallCount { get; private set; }
        public int LastBookId { get; private set; }
        public int LastCount { get; private set; }
        public bool HasIssuedCopiesResult { get; set; }
        public int DeleteByBookIdCallCount { get; private set; }

        public void SeedAvailableCopies(int bookId, int count)
        {
            _availableCopiesByBookId[bookId] = count;
        }

        public void AddCopies(int bookId, int count)
        {
            AddCopiesCallCount++;
            LastBookId = bookId;
            LastCount = count;
            _availableCopiesByBookId[bookId] = _availableCopiesByBookId.TryGetValue(bookId, out var existing)
                ? existing + count
                : count;
        }

        public BookCopy? GetAvailableCopy(int bookId)
        {
            if (_availableCopiesByBookId.TryGetValue(bookId, out var count) && count > 0)
            {
                _availableCopiesByBookId[bookId] = count - 1;
                return new BookCopy();
            }

            return null;
        }

        public void RemoveCopies(int bookId, int count)
        {
            RemoveCopiesCallCount++;
            LastBookId = bookId;
            LastCount = count;
        }

        public void DeleteByBookId(int bookId)
        {
            DeleteByBookIdCallCount++;
            LastBookId = bookId;
            _availableCopiesByBookId.Remove(bookId);
        }

        public bool HasIssuedCopies(int bookId)
        {
            return HasIssuedCopiesResult;
        }

        public int GetIssuedCount(int bookId)
        {
            return _issuedCopiesByBookId.TryGetValue(bookId, out var count) ? count : 0;
        }

        public int GetAvailableCount(int bookId)
        {
            return _availableCopiesByBookId.TryGetValue(bookId, out var count) ? count : 0;
        }

        public void SeedIssuedCopies(int bookId, int count)
        {
            _issuedCopiesByBookId[bookId] = count;
        }

        public BookCopy? GetByCopyId(int copyId)
        {
            return null;
        }
    }
}
