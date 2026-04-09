using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.UnitTests.Stubs.BookServiceStubs
{
    internal class IAuthorRepositoryStub : IAuthorRepository
    {
        private readonly List<Author> _authors = new();
        private int _nextAuthorId = 1;

        public int AddCallCount { get; private set; }

        public Author Add(Author author)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));

            AddCallCount++;
            var created = new Author
            {
                AuthorId = _nextAuthorId++,
                AuthorName = author.AuthorName
            };

            _authors.Add(created);
            return created;
        }

        public List<Author> GetByName(string name)
        {
            return _authors.Where(a => a.AuthorName == name).ToList();
        }

        public List<Author> GetByIds(List<int> authorIds)
        {
            var set = new HashSet<int>(authorIds);
            return _authors.Where(a => set.Contains(a.AuthorId)).ToList();
        }

        public int TotalAuthors => _authors.Count;
    }
}
