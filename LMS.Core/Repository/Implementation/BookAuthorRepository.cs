using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class BookAuthorRepository : IBookAuthorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookAuthorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(int bookId, int authorId)
        {
            const string sql = @"INSERT INTO BookAuthor (BookId, AuthorId) VALUES (@bookId, @authorId)";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var bookParameter = command.CreateParameter();
            bookParameter.ParameterName = "@bookId";
            bookParameter.Value = bookId;
            command.Parameters.Add(bookParameter);

            var authorParameter = command.CreateParameter();
            authorParameter.ParameterName = "@authorId";
            authorParameter.Value = authorId;
            command.Parameters.Add(authorParameter);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DeleteByBookId(int bookId)
        {
            const string sql = @"DELETE FROM BookAuthor WHERE BookId = @bookId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var bookParameter = command.CreateParameter();
            bookParameter.ParameterName = "@bookId";
            bookParameter.Value = bookId;
            command.Parameters.Add(bookParameter);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<int> GetBookIdsByAuthorId(int authorId)
        {
            const string sql = @"SELECT BookId FROM BookAuthor WHERE AuthorId = @authorId";
            var bookIds = new List<int>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var authorParameter = command.CreateParameter();
            authorParameter.ParameterName = "@authorId";
            authorParameter.Value = authorId;
            command.Parameters.Add(authorParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                bookIds.Add(reader.GetInt32(0));
            }

            return bookIds;
        }

        public List<int> GetAuthorIdsByBookId(int bookId)
        {
            const string sql = @"SELECT AuthorId FROM BookAuthor WHERE BookId = @bookId";
            var authorIds = new List<int>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var bookParameter = command.CreateParameter();
            bookParameter.ParameterName = "@bookId";
            bookParameter.Value = bookId;
            command.Parameters.Add(bookParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                authorIds.Add(reader.GetInt32(0));
            }

            return authorIds;
        }
    }
}
