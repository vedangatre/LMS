using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AuthorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Author Add(Author author)
        {
            const string sql = @"
                INSERT INTO Author (AuthorName)
                VALUES (@authorName);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@authorName";
            nameParameter.Value = author.AuthorName;
            command.Parameters.Add(nameParameter);

            connection.Open();
            var newId = (int)command.ExecuteScalar()!;

            return new Author { AuthorId = newId, AuthorName = author.AuthorName };
        }

        public List<Author> GetByName(string name)
        {
            const string sql = @"
                SELECT AuthorId, AuthorName
                FROM Author
                WHERE AuthorName = @authorName";

            var authors = new List<Author>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@authorName";
            nameParameter.Value = name;
            command.Parameters.Add(nameParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                authors.Add(new Author
                {
                    AuthorId = reader.GetInt32(0),
                    AuthorName = reader.GetString(1)
                });
            }

            return authors;
        }

        public List<Author> GetByIds(List<int> authorIds)
        {
            if (authorIds.Count == 0)
            {
                return new List<Author>();
            }

            var parameterNames = new List<string>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();

            for (var i = 0; i < authorIds.Count; i++)
            {
                var parameterName = $"@id{i}";
                parameterNames.Add(parameterName);

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = parameterName;
                idParameter.Value = authorIds[i];
                command.Parameters.Add(idParameter);
            }

            command.CommandText = $"SELECT AuthorId, AuthorName FROM Author WHERE AuthorId IN ({string.Join(", ", parameterNames)})";

            var authors = new List<Author>();
            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                authors.Add(new Author
                {
                    AuthorId = reader.GetInt32(0),
                    AuthorName = reader.GetString(1)
                });
            }

            return authors;
        }
    }
}
