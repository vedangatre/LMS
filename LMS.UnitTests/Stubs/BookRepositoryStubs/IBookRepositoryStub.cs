using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.UnitTests.Stubs.BookServiceStubs
{
    internal class IBookRepositoryStub : IBookRepository
    {
        private readonly List<Book> _books = new();
        private int _nextBookId = 1;

        public int AddCallCount { get; private set; }

        public Book Add(Book book)
        {
            if (book is null) throw new ArgumentNullException(nameof(book));

            AddCallCount++;
            var created = new Book
            {
                BookId = _nextBookId++,
                BookName = book.BookName
            };

            _books.Add(created);
            return created;
        }

        public void Delete(int bookId)
        {
            _books.RemoveAll(b => b.BookId == bookId);
        }

        public List<Book> GetAll()
        {
            return _books.ToList();
        }

        public List<Book> SearchByName(string name)
        {
            return _books.Where(b => b.BookName == name).ToList();
        }

        public List<Book> GetByIds(List<int> bookIds)
        {
            if (bookIds is null) throw new ArgumentNullException(nameof(bookIds));
            var set = new HashSet<int>(bookIds);
            return _books.Where(b => set.Contains(b.BookId)).ToList();
        }
    }
}
