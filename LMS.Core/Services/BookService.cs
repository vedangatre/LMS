using LMS.Core.Dto;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Core.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookAuthorRepository _bookAuthorRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookCopyRepository _bookCopyRepository;

        public BookService(
            IBookRepository bookRepository,
            IBookAuthorRepository bookAuthorRepository,
            IAuthorRepository authorRepository,
            IBookCopyRepository bookCopyRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _bookAuthorRepository = bookAuthorRepository ?? throw new ArgumentNullException(nameof(bookAuthorRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _bookCopyRepository = bookCopyRepository ?? throw new ArgumentNullException(nameof(bookCopyRepository));
        }

        public Book? AddBook(object? bookName, List<object>? authorNames)
        {
            if (bookName is null) throw new ArgumentNullException(nameof(bookName));
            if (authorNames is null) throw new ArgumentNullException(nameof(authorNames));

            if (bookName is not string bookNameString)
            {
                throw new ArgumentException("Book name must be a string.", nameof(bookName));
            }

            bookNameString = bookNameString.Trim();
            if (string.IsNullOrWhiteSpace(bookNameString))
            {
                throw new ArgumentException("Book name cannot be empty.", nameof(bookName));
            }

            // Normalize + validate author names, then dedupe (set semantics).
            var normalizedAuthorNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var author in authorNames)
            {
                if (author is null) throw new ArgumentNullException(nameof(authorNames));
                if (author is not string authorString)
                {
                    throw new ArgumentException("Author name must be a string.", nameof(authorNames));
                }

                var trimmed = authorString.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    throw new ArgumentException("Author name cannot be empty.", nameof(authorNames));
                }

                normalizedAuthorNames.Add(trimmed);
            }

            // Resolve author IDs for the normalized author set.
            var authorIds = new HashSet<int>();
            foreach (var authorName in normalizedAuthorNames)
            {
                var existingAuthors = _authorRepository.GetByName(authorName);
                var existing = existingAuthors?.FirstOrDefault();
                if (existing is not null)
                {
                    authorIds.Add(existing.AuthorId);
                    continue;
                }

                var added = _authorRepository.Add(new Author { AuthorName = authorName });
                authorIds.Add(added.AuthorId);
            }

            // Duplication rule:
            // - same bookName AND same AUTHOR SET (order-insensitive) => blocked
            var existingBooks = _bookRepository.SearchByName(bookNameString) ?? new List<Book>();
            foreach (var existingBook in existingBooks)
            {
                var existingAuthorIds = _bookAuthorRepository.GetAuthorIdsByBookId(existingBook.BookId) ?? new List<int>();
                if (new HashSet<int>(existingAuthorIds).SetEquals(authorIds))
                {
                    return null;
                }
            }

            var createdBook = _bookRepository.Add(new Book { BookName = bookNameString });
            foreach (var authorId in authorIds)
            {
                _bookAuthorRepository.Add(createdBook.BookId, authorId);
            }

            _bookCopyRepository.AddCopies(createdBook.BookId, 1);

            return createdBook;
        }

        public void DeleteBook(int bookId)
        {
            if (_bookCopyRepository.GetIssuedCount(bookId) > 0)
            {
                throw new InvalidOperationException("Cannot delete book when any copy is issued.");
            }

            _bookCopyRepository.DeleteAllByBookId(bookId);
            _bookAuthorRepository.DeleteByBookId(bookId);
            _bookRepository.Delete(bookId);
        }

        public void Delete(int bookId)
        {
            if (bookId <= 0) throw new ArgumentOutOfRangeException(nameof(bookId));

            if (_bookCopyRepository.GetIssuedCount(bookId) > 0)
            {
                throw new InvalidOperationException("Cannot delete book when any copy is issued.");
            }

            _bookCopyRepository.DeleteAllByBookId(bookId);
            _bookAuthorRepository.DeleteByBookId(bookId);
            _bookRepository.Delete(bookId);
        }

        public void AddCopies(int bookId, int count)
        {
            if (bookId <= 0) throw new ArgumentOutOfRangeException(nameof(bookId));
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            _bookCopyRepository.AddCopies(bookId, count);
        }

        public void RemoveCopies(int bookId, int count)
        {
            if (bookId <= 0) throw new ArgumentOutOfRangeException(nameof(bookId));
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            if (_bookCopyRepository.GetIssuedCount(bookId) > 0)
            {
                throw new InvalidOperationException("Cannot remove copies because one or more copies are issued.");
            }

            _bookCopyRepository.RemoveCopies(bookId, count);
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public List<Book> SearchByBookName(string name)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            var normalizedName = name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentException("Book name cannot be empty.", nameof(name));
            }

            return _bookRepository.SearchByName(normalizedName);
        }

        public List<Book> SearchByAuthorName(string authorName)
        {
            if (authorName is null) throw new ArgumentNullException(nameof(authorName));

            var normalizedAuthorName = authorName.Trim();
            if (string.IsNullOrWhiteSpace(normalizedAuthorName))
            {
                throw new ArgumentException("Author name cannot be empty.", nameof(authorName));
            }

            var authors = _authorRepository.GetByName(normalizedAuthorName);
            if (authors is null || authors.Count == 0)
            {
                return new List<Book>();
            }

            var bookIds = new HashSet<int>();
            foreach (var author in authors)
            {
                var linkedBookIds = _bookAuthorRepository.GetBookIdsByAuthorId(author.AuthorId);
                foreach (var bookId in linkedBookIds)
                {
                    bookIds.Add(bookId);
                }
            }

            if (bookIds.Count == 0)
            {
                return new List<Book>();
            }

            return _bookRepository.GetByIds(bookIds.ToList());
        }

        public List<BookReportItem> GetBookReport()
        {
            var books = _bookRepository.GetAll();
            var results = new List<BookReportItem>();

            foreach (var book in books)
            {
                var authorIds = _bookAuthorRepository.GetAuthorIdsByBookId(book.BookId);
                var authors = _authorRepository.GetByIds(authorIds);
                var authorNames = string.Join(", ", authors.Select(a => a.AuthorName));

                var total = _bookCopyRepository.GetTotalCount(book.BookId);
                var issued = _bookCopyRepository.GetIssuedCount(book.BookId);

                results.Add(new BookReportItem
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    Authors = authorNames,
                    IssuedCount = issued,
                    AvailableCount = Math.Max(0, total - issued)
                });
            }

            return results;
        }
    }
}
