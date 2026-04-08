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
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo);

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
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo);

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
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo);

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
            var service = new BookService(bookRepo, bookAuthorRepo, authorRepo);

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
    }
}
