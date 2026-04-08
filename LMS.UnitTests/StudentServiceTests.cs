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
            // Act
            var result = _service.AddStudent("Bob");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StudentName, Is.EqualTo("Bob"));
        }

        [Test]
        public void GetStudentById_ShouldReturnStudent_WhenExists()
        {
            // Act
            var result = _service.GetStudentById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StudentName, Is.EqualTo("John"));
        }

        [Test]
        public void GetStudentById_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = _service.GetStudentById(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteStudent_ShouldReturnTrue_WhenStudentExists()
        {
            // Act
            var result = _service.DeleteStudent(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteStudent_ShouldReturnFalse_WhenStudentDoesNotExist()
        {
            // Act
            var result = _service.DeleteStudent(999);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
