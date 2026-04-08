using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.UnitTests.Stubs.BookServiceStubs
{
    internal class IBookAuthorRepositoryStub : IBookAuthorRepository
    {
        private readonly Dictionary<int, HashSet<int>> _bookToAuthorIds = new();

        public int AddCallCount { get; private set; }

        public void Add(int bookId, int authorId)
        {
            AddCallCount++;

            if (!_bookToAuthorIds.TryGetValue(bookId, out var set))
            {
                set = new HashSet<int>();
                _bookToAuthorIds[bookId] = set;
            }

            set.Add(authorId);
        }

        public List<int> GetBookIdsByAuthorId(int authorId)
        {
            return _bookToAuthorIds
                .Where(kvp => kvp.Value.Contains(authorId))
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<int> GetAuthorIdsByBookId(int bookId)
        {
            return _bookToAuthorIds.TryGetValue(bookId, out var set) ? set.ToList() : new List<int>();
        }
    }
}
