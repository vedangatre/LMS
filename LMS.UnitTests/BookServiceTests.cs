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
        private IBookRepositoryStub _bookRepo = null!;
        private IBookAuthorRepositoryStub _bookAuthorRepo = null!;
        private IAuthorRepositoryStub _authorRepo = null!;
        private IBookCopyRepositoryStub _bookCopyRepo = null!;
        private BookService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _bookRepo = new IBookRepositoryStub();
            _bookAuthorRepo = new IBookAuthorRepositoryStub();
            _authorRepo = new IAuthorRepositoryStub();
            _bookCopyRepo = new IBookCopyRepositoryStub();
            _service = new BookService(_bookRepo, _bookAuthorRepo, _authorRepo, _bookCopyRepo);
        }

        private Book? AddBook(string name, params object[] authors)
        {
            return _service.AddBook(name, new List<object>(authors));
        }

        [Test]
        public void AddBook_WithMultipleAuthors_AddsBookAndLinksAuthors()
        {
            var result = AddBook("onepiece", "oda", "Tejas");

            Assert.That(result, Not.Null);
            Assert.That(_bookRepo.AddCallCount, EqualTo(1));
            Assert.That(_bookAuthorRepo.AddCallCount, EqualTo(2));
            Assert.That(_authorRepo.TotalAuthors, EqualTo(2));
        }

        [Test]
        public void AddBook_WithWhitespace_TrimsAndBlocksDuplicateAuthorSetIgnoringOrder()
        {
            var first = AddBook(" onepiece ", " oda ", " Tejas ");
            Assert.That(first, Not.Null);

            var second = AddBook("onepiece", "Tejas", "oda");
            Assert.That(second, Is.Null);

            Assert.That(_bookRepo.AddCallCount, EqualTo(1));
            Assert.That(_bookAuthorRepo.AddCallCount, EqualTo(2));
            Assert.That(_authorRepo.TotalAuthors, EqualTo(2));
        }

        [Test]
        public void AddBook_InvalidData_ThrowsForNullDatatypeAndWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() => _service.AddBook(null, new List<object> { "oda" }));
            Assert.Throws<ArgumentException>(() => _service.AddBook(42, new List<object> { "oda" }));

            Assert.Throws<ArgumentNullException>(() => _service.AddBook("onepiece", null));
            Assert.Throws<ArgumentNullException>(() => _service.AddBook("onepiece", new List<object> { null!, "oda" }));

            Assert.Throws<ArgumentException>(() => _service.AddBook("   ", new List<object> { "oda" }));
            Assert.Throws<ArgumentException>(() => _service.AddBook("onepiece", new List<object> { "   " }));

            Assert.Throws<ArgumentException>(() => _service.AddBook("onepiece", new List<object> { 123, "oda" }));
        }

        [Test]
        public void AddBook_DuplicateBlockedByBookNameAndAuthorSet()
        {
            var b1 = AddBook("onepiece", "oda", "Tejas");
            Assert.That(b1, Not.Null);

            var b2 = AddBook("onepiece", "harsh", "Omkar");
            Assert.That(b2, Not.Null);

            var b3 = AddBook("onepiece", "oda", "Tejas");
            Assert.That(b3, Is.Null);

            var b4 = AddBook("onepiece", "harsh", "Omkar", "Vedang");
            Assert.That(b4, Not.Null);

            var b5 = AddBook("Naruto", "Prajwal", "arnav");
            Assert.That(b5, Not.Null);

            var b6 = AddBook("onepiece", "Vedang", "harsh", "Omkar");
            Assert.That(b6, Is.Null);

            Assert.That(_bookRepo.GetAll().Count, EqualTo(4));     
            Assert.That(_bookAuthorRepo.AddCallCount, EqualTo(9)); 
            Assert.That(_authorRepo.TotalAuthors, EqualTo(7));      
        }

        [Test]
        public void DeleteBook_DeletesFromBookAndBookAuthor()
        {
            var onepiece = AddBook("onepiece", "oda", "Tejas");
            var naruto = AddBook("naruto", "kishimoto");

            Assert.That(onepiece, Not.Null);
            Assert.That(naruto, Not.Null);
            Assert.That(_bookRepo.GetAll().Count, EqualTo(2));
            Assert.That(_bookAuthorRepo.GetAuthorIdsByBookId(onepiece!.BookId).Count, EqualTo(2));
            Assert.That(_bookAuthorRepo.GetAuthorIdsByBookId(naruto!.BookId).Count, EqualTo(1));

            _service.DeleteBook(onepiece.BookId);

            Assert.That(_bookRepo.GetAll().Count, EqualTo(1));
            Assert.That(_bookRepo.GetAll()[0].BookId, EqualTo(naruto.BookId));
            Assert.That(_bookAuthorRepo.GetAuthorIdsByBookId(onepiece.BookId), Is.Empty);
            Assert.That(_bookAuthorRepo.GetAuthorIdsByBookId(naruto.BookId).Count, EqualTo(1));
            Assert.That(_bookAuthorRepo.DeleteByBookIdCallCount, EqualTo(1));
        }

        [Test]
        public void AddCopies_DelegatesToBookCopyRepository()
        {
            _service.AddCopies(10, 3);

            Assert.That(_bookCopyRepo.AddCopiesCallCount, EqualTo(1));
            Assert.That(_bookCopyRepo.LastBookId, EqualTo(10));
            Assert.That(_bookCopyRepo.LastCount, EqualTo(3));
        }

        [Test]
        public void GetAllBooks_ReturnsAllBooks()
        {
            AddBook("onepiece", "oda", "tejas");
            AddBook("naruto", "kishimoto");

            var result = _service.GetAllBooks();

            Assert.That(result.Count, EqualTo(2));
        }

        [Test]
        public void SearchByBookName_ReturnsMatchingBooks()
        {
            AddBook("onepiece", "oda", "tejas");
            AddBook("onepiece", "harsh", "omkar");
            AddBook("naruto", "kishimoto");

            var result = _service.SearchByBookName(" onepiece ");

            Assert.That(result.Count, EqualTo(2));
            Assert.That(result.TrueForAll(b => b.BookName == "onepiece"), Is.True);
        }

        [Test]
        public void SearchByAuthorName_ReturnsBooksLinkedToAuthor()
        {
            AddBook("onepiece", "oda", "tejas");
            AddBook("monster", "oda", "naoki");
            AddBook("naruto", "kishimoto");

            var result = _service.SearchByAuthorName(" oda ");

            Assert.That(result.Count, EqualTo(2));
            Assert.That(result.Exists(b => b.BookName == "onepiece"), Is.True);
            Assert.That(result.Exists(b => b.BookName == "monster"), Is.True);
        }

        [Test]
        public void RemoveCopies_WhenCopiesAreNotIssued_RemovesRequestedCopies()
        {
            _bookCopyRepo.SeedAvailableCopies(7, 3);

            _service.RemoveCopies(7, 2);

            Assert.That(_bookCopyRepo.RemoveCopiesCallCount, EqualTo(1));
            Assert.That(_bookCopyRepo.LastBookId, EqualTo(7));
            Assert.That(_bookCopyRepo.LastCount, EqualTo(2));
        }

        [Test]
        public void Delete_WhenAnyCopyIsNotIssued_DeletesBookAndBookAuthorMappings()
        {
            var book = AddBook("bleach", "kubo");
            Assert.That(book, Not.Null);

            _bookCopyRepo.SeedAvailableCopies(book!.BookId, 1);

            _service.Delete(book.BookId);

            Assert.That(_bookRepo.GetAll().Count, EqualTo(0));
            Assert.That(_bookAuthorRepo.GetAuthorIdsByBookId(book.BookId), Is.Empty);
            Assert.That(_bookAuthorRepo.DeleteByBookIdCallCount, EqualTo(1));
        }
    }
}
