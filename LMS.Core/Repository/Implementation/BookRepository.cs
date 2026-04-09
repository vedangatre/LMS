using System.Data;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Book Add(Book book)
        {
            const string sql = @"
                INSERT INTO Book (BookName)
                VALUES (@bookName);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@bookName";
            nameParameter.Value = book.BookName;
            command.Parameters.Add(nameParameter);

            connection.Open();
            var newId = (int)command.ExecuteScalar()!;

            return new Book
            {
                BookId = newId,
                BookName = book.BookName
            };
        }

        public void Delete(int bookId)
        {
            const string sql = @"DELETE FROM Book WHERE BookId = @bookId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@bookId";
            idParameter.Value = bookId;
            command.Parameters.Add(idParameter);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Book> GetAll()
        {
            const string sql = @"SELECT BookId, BookName FROM Book";
            var books = new List<Book>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(MapBook(reader));
            }

            return books;
        }

        public List<Book> SearchByName(string name)
        {
            const string sql = @"
                SELECT BookId, BookName
                FROM Book
                WHERE BookName LIKE @bookName";

            var books = new List<Book>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@bookName";
            nameParameter.Value = $"%{name}%";
            command.Parameters.Add(nameParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(MapBook(reader));
            }

            return books;
        }

        public List<Book> GetByIds(List<int> bookIds)
        {
            if (bookIds.Count == 0)
            {
                return new List<Book>();
            }

            var parameterNames = new List<string>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();

            for (var i = 0; i < bookIds.Count; i++)
            {
                var parameterName = $"@id{i}";
                parameterNames.Add(parameterName);

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = parameterName;
                idParameter.Value = bookIds[i];
                command.Parameters.Add(idParameter);
            }

            command.CommandText = $"SELECT BookId, BookName FROM Book WHERE BookId IN ({string.Join(", ", parameterNames)})";

            var books = new List<Book>();
            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(MapBook(reader));
            }

            return books;
        }

        private static Book MapBook(IDataRecord record)
        {
            return new Book
            {
                BookId = record.GetInt32(0),
                BookName = record.GetString(1)
            };
        }
    }
}
