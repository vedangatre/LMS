using LMS.Core.Models;
using LMS.Core.Services;
using LMS.UnitTests.Stubs.BookServiceStubs;
using NUnit.Framework;
using System.Collections.Generic;
using static NUnit.Framework.Is;

namespace LMS.UnitTests
{
    [TestFixture]
    public class BookServiceTests
    {
        [Test]
        public void AddBook_WithMultipleAuthors_AddsBookAndLinksAuthors()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            var result = service.AddBook("onepiece", new List<object> { "oda", "Tejas" });

            Assert.That(result, Not.Null);
            Assert.That(bookRepo.AddCallCount, EqualTo(1));
            Assert.That(bookAuthorRepo.AddCallCount, EqualTo(2));
            Assert.That(authorRepo.TotalAuthors, EqualTo(2));
        }

        [Test]
        public void AddBook_WithWhitespace_TrimsAndBlocksDuplicateAuthorSetIgnoringOrder()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            var first = service.AddBook(" onepiece ", new List<object> { " oda ", " Tejas " });
            Assert.That(first, Not.Null);

            // Same book name + same author set => blocked (order can differ too).
            var second = service.AddBook("onepiece", new List<object> { "Tejas", "oda" });
            Assert.That(second, Is.Null);

            Assert.That(bookRepo.AddCallCount, EqualTo(1));
            Assert.That(bookAuthorRepo.AddCallCount, EqualTo(2));
            Assert.That(authorRepo.TotalAuthors, EqualTo(2));
        }

        [Test]
        public void AddBook_InvalidData_ThrowsForNullDatatypeAndWhitespace()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            Assert.Throws<ArgumentNullException>(() => service.AddBook(null, new List<object> { "oda" }));
            Assert.Throws<ArgumentException>(() => service.AddBook(42, new List<object> { "oda" }));

            Assert.Throws<ArgumentNullException>(() => service.AddBook("onepiece", null));
            Assert.Throws<ArgumentNullException>(() => service.AddBook("onepiece", new List<object> { null!, "oda" }));

            Assert.Throws<ArgumentException>(() => service.AddBook("   ", new List<object> { "oda" }));
            Assert.Throws<ArgumentException>(() => service.AddBook("onepiece", new List<object> { "   " }));

            Assert.Throws<ArgumentException>(() => service.AddBook("onepiece", new List<object> { 123, "oda" }));
        }

        [Test]
        public void AddBook_DuplicateBlockedByBookNameAndAuthorSet()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            // onepiece : oda, Tejas // add
            var b1 = service.AddBook("onepiece", new List<object> { "oda", "Tejas" });
            Assert.That(b1, Not.Null);

            // onepiece : harsh, Omkar // add
            var b2 = service.AddBook("onepiece", new List<object> { "harsh", "Omkar" });
            Assert.That(b2, Not.Null);

            // onepiece: oda, Tejas // blocked
            var b3 = service.AddBook("onepiece", new List<object> { "oda", "Tejas" });
            Assert.That(b3, Is.Null);

            // onepiece: harsh, Omkar, Vedang // allowed
            var b4 = service.AddBook("onepiece", new List<object> { "harsh", "Omkar", "Vedang" });
            Assert.That(b4, Not.Null);

            // Naruto: Prajwal, arnav // allowed
            var b5 = service.AddBook("Naruto", new List<object> { "Prajwal", "arnav" });
            Assert.That(b5, Not.Null);

            // onepiece: Vedang, harsh, Omkar // blocked (same author set, different order)
            var b6 = service.AddBook("onepiece", new List<object> { "Vedang", "harsh", "Omkar" });
            Assert.That(b6, Is.Null);

            Assert.That(bookRepo.GetAll().Count, EqualTo(4));     // b1, b2, b4, b5
            Assert.That(bookAuthorRepo.AddCallCount, EqualTo(9)); // 2 + 2 + 3 + 2
            Assert.That(authorRepo.TotalAuthors, EqualTo(7));      // oda, Tejas, harsh, Omkar, Vedang, Prajwal, arnav
        }

        [Test]
        public void DeleteBook_DeletesFromBookAndBookAuthor()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            var onepiece = service.AddBook("onepiece", new List<object> { "oda", "Tejas" });
            var naruto = service.AddBook("naruto", new List<object> { "kishimoto" });

            Assert.That(onepiece, Not.Null);
            Assert.That(naruto, Not.Null);
            Assert.That(bookRepo.GetAll().Count, EqualTo(2));
            Assert.That(bookAuthorRepo.GetAuthorIdsByBookId(onepiece!.BookId).Count, EqualTo(2));
            Assert.That(bookAuthorRepo.GetAuthorIdsByBookId(naruto!.BookId).Count, EqualTo(1));

            service.DeleteBook(onepiece.BookId);

            Assert.That(bookRepo.GetAll().Count, EqualTo(1));
            Assert.That(bookRepo.GetAll()[0].BookId, EqualTo(naruto.BookId));
            Assert.That(bookAuthorRepo.GetAuthorIdsByBookId(onepiece.BookId), Is.Empty);
            Assert.That(bookAuthorRepo.GetAuthorIdsByBookId(naruto.BookId).Count, EqualTo(1));
            Assert.That(bookAuthorRepo.DeleteByBookIdCallCount, EqualTo(1));
        }

        [Test]
        public void AddCopies_DelegatesToBookCopyRepository()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            service.AddCopies(10, 3);

            Assert.That(bookCopyRepo.AddCopiesCallCount, EqualTo(1));
            Assert.That(bookCopyRepo.LastBookId, EqualTo(10));
            Assert.That(bookCopyRepo.LastCount, EqualTo(3));
        }

        [Test]
        public void GetAllBooks_ReturnsAllBooks()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            service.AddBook("onepiece", new List<object> { "oda", "tejas" });
            service.AddBook("naruto", new List<object> { "kishimoto" });

            var result = service.GetAllBooks();

            Assert.That(result.Count, EqualTo(2));
        }

        [Test]
        public void SearchByBookName_ReturnsMatchingBooks()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            service.AddBook("onepiece", new List<object> { "oda", "tejas" });
            service.AddBook("onepiece", new List<object> { "harsh", "omkar" });
            service.AddBook("naruto", new List<object> { "kishimoto" });

            var result = service.SearchByBookName(" onepiece ");

            Assert.That(result.Count, EqualTo(2));
            Assert.That(result.TrueForAll(b => b.BookName == "onepiece"), Is.True);
        }

        [Test]
        public void SearchByAuthorName_ReturnsBooksLinkedToAuthor()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            service.AddBook("onepiece", new List<object> { "oda", "tejas" });
            service.AddBook("monster", new List<object> { "oda", "naoki" });
            service.AddBook("naruto", new List<object> { "kishimoto" });

            var result = service.SearchByAuthorName(" oda ");

            Assert.That(result.Count, EqualTo(2));
            Assert.That(result.Exists(b => b.BookName == "onepiece"), Is.True);
            Assert.That(result.Exists(b => b.BookName == "monster"), Is.True);
        }

        [Test]
        public void RemoveCopies_WhenCopiesAreNotIssued_RemovesRequestedCopies()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            bookCopyRepo.SeedAvailableCopies(7, 3);

            service.RemoveCopies(7, 2);

            Assert.That(bookCopyRepo.RemoveCopiesCallCount, EqualTo(1));
            Assert.That(bookCopyRepo.LastBookId, EqualTo(7));
            Assert.That(bookCopyRepo.LastCount, EqualTo(2));
        }

        [Test]
        public void Delete_WhenAnyCopyIsNotIssued_DeletesBookAndBookAuthorMappings()
        {
            var bookRepo = new IBookRepositoryStub();
            var bookAuthorRepo = new IBookAuthorRepositoryStub();
            var authorRepo = new IAuthorRepositoryStub();
            var bookCopyRepo = new IBookCopyRepositoryStub();
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo, bookCopyRepo);

            var book = service.AddBook("bleach", new List<object> { "kubo" });
            Assert.That(book, Not.Null);

            bookCopyRepo.SeedAvailableCopies(book!.BookId, 1);

            service.Delete(book.BookId);

            Assert.That(bookRepo.GetAll().Count, EqualTo(0));
            Assert.That(bookAuthorRepo.GetAuthorIdsByBookId(book.BookId), Is.Empty);
            Assert.That(bookAuthorRepo.DeleteByBookIdCallCount, EqualTo(1));
        }
    }
}
