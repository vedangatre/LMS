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

        public BookService(
            IBookRepository bookRepository,
            IBookAuthorRepository bookAuthorRepository,
            IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _bookAuthorRepository = bookAuthorRepository ?? throw new ArgumentNullException(nameof(bookAuthorRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
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

            return createdBook;
        }
    }
}
