using LMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Interfaces
{
    public interface IBookRepository
    {
        Book Add(Book book);

        void Delete(int bookId);

        List<Book> GetAll();

        List<Book> SearchByName(string name);

        List<Book> GetByIds(List<int> bookIds);
    }
}
