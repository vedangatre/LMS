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

        public int GetTotalCount(int bookId)
        {
            _availableCopiesByBookId.TryGetValue(bookId, out var available);
            _issuedCopiesByBookId.TryGetValue(bookId, out var issued);
            return available + issued;
        }

        public int GetIssuedCount(int bookId)
        {
            _issuedCopiesByBookId.TryGetValue(bookId, out var issued);
            return issued;
        }

        public void DeleteAllByBookId(int bookId)
        {
            _availableCopiesByBookId.Remove(bookId);
            _issuedCopiesByBookId.Remove(bookId);
        }
    }
}
