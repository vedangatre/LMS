using System;
using LMS.Core.Models;
using NUnit.Framework;
using LMS.UnitTests.Stubs;
using LMS.Core.Services;

namespace LMS.UnitTests
{
    [TestFixture]
    public class StudentServiceTests
    {
        private StudentService _service;

        [SetUp]
        public void Setup()
        {
            var stubRepo = new StudentRepositoryStub();
            stubRepo.ExistingId = 1;
            stubRepo.ExistingName = "John";
            _service = new StudentService(stubRepo);
        }

        [Test]
        public void AddStudent_ShouldAddStudentSuccessfully()
        {
            var result = _service.AddStudent("Bob");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StudentName, Is.EqualTo("Bob"));
        }

        [Test]
        public void AddStudent_ShouldThrowArgumentNull_WhenNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.AddStudent(null!));
        }

        [Test]
        public void AddStudent_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => _service.AddStudent(string.Empty));
        }

        [Test]
        public void AddStudent_ShouldThrowArgumentException_WhenNameIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => _service.AddStudent("   "));
        }


        [Test]
        public void DeleteStudent_ShouldReturnTrue_WhenStudentExists()
        {
            var result = _service.DeleteStudent(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteStudent_ShouldReturnFalse_WhenStudentDoesNotExist()
        {
            var result = _service.DeleteStudent(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public void DeleteStudent_ShouldThroughException_WhenStudentIdIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => _service.DeleteStudent(0));
        }

        [Test]
        public void DeleteStudent_ShouldReturnFalse_WhenStudentHasActiveIssuedBooks()
        {
            var stubRepo = new Stubs.StudentRepositoryStub();
            stubRepo.ExistingId = 2;
            stubRepo.ExistingHasActiveIssues = true;
            var service = new LMS.Core.Services.StudentService(stubRepo);

            var result = service.DeleteStudent(2);

            Assert.That(result, Is.False);
        }


        [Test]
        public void GetStudentById_ShouldReturnStudent_WhenExists()
        {
            var result = _service.GetStudentById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StudentName, Is.EqualTo("John"));
        }

        [Test]
        public void GetStudentById_ShouldReturnNull_WhenNotExists()
        {
            var result = _service.GetStudentById(34);

            Assert.That(result, Is.Null);
        }
    }
}
