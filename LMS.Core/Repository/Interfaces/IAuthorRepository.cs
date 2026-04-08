using LMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Repository.Interfaces
{
    public interface IAuthorRepository
    {
        Author Add(Author author);

        List<Author> GetByName(string name);
    }
}
