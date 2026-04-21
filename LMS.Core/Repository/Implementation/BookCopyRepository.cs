using System.Data;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class BookCopyRepository : IBookCopyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookCopyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void AddCopies(int bookId, int count)
        {
            const string sql = @"INSERT INTO BookCopy (BookId, IsDamaged) VALUES (@bookId, @isDamaged)";

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@bookId";
            idParameter.Value = bookId;
            command.Parameters.Add(idParameter);

            var damagedParameter = command.CreateParameter();
            damagedParameter.ParameterName = "@isDamaged";
            damagedParameter.Value = false;
            command.Parameters.Add(damagedParameter);

            for (var i = 0; i < count; i++)
            {
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public void DeleteByBookId(int bookId)
        {
            const string sql = @"DELETE FROM BookCopy WHERE BookId = @bookId";

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

        public bool HasIssuedCopies(int bookId)
        {
            return GetIssuedCount(bookId) > 0;
        }

        public int GetIssuedCount(int bookId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM IssueRecord ir
                INNER JOIN BookCopy bc ON bc.CopyId = ir.CopyId
                WHERE bc.BookId = @bookId
                  AND ir.ReturnDate IS NULL";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@bookId";
            idParameter.Value = bookId;
            command.Parameters.Add(idParameter);

            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public int GetAvailableCount(int bookId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM BookCopy bc
                LEFT JOIN IssueRecord ir
                  ON ir.CopyId = bc.CopyId AND ir.ReturnDate IS NULL
                WHERE bc.BookId = @bookId
                  AND ir.IssueId IS NULL";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@bookId";
            idParameter.Value = bookId;
            command.Parameters.Add(idParameter);

            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public void RemoveCopies(int bookId, int count)
        {
            const string sql = @"
                DELETE TOP (@count)
                FROM BookCopy
                WHERE BookId = @bookId
                  AND CopyId IN (
                      SELECT TOP (@count) bc.CopyId
                      FROM BookCopy bc
                      LEFT JOIN IssueRecord ir
                        ON ir.CopyId = bc.CopyId AND ir.ReturnDate IS NULL
                      WHERE bc.BookId = @bookId
                        AND ir.IssueId IS NULL
                      ORDER BY bc.CopyId
                  )";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var countParameter = command.CreateParameter();
            countParameter.ParameterName = "@count";
            countParameter.Value = count;
            command.Parameters.Add(countParameter);

            var bookParameter = command.CreateParameter();
            bookParameter.ParameterName = "@bookId";
            bookParameter.Value = bookId;
            command.Parameters.Add(bookParameter);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public BookCopy? GetAvailableCopy(int bookId)
        {
            const string sql = @"
                SELECT TOP 1 bc.CopyId, bc.BookId
                FROM BookCopy bc
                LEFT JOIN IssueRecord ir
                  ON ir.CopyId = bc.CopyId AND ir.ReturnDate IS NULL
                WHERE bc.BookId = @bookId
                  AND ir.IssueId IS NULL
                ORDER BY bc.CopyId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@bookId";
            idParameter.Value = bookId;
            command.Parameters.Add(idParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new BookCopy
            {
                CopyId = reader.GetInt32(0),
                BookId = reader.GetInt32(1)
            };
        }

        public BookCopy? GetByCopyId(int copyId)
        {
            const string sql = @"
                SELECT CopyId, BookId
                FROM BookCopy
                WHERE CopyId = @copyId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var copyIdParameter = command.CreateParameter();
            copyIdParameter.ParameterName = "@copyId";
            copyIdParameter.Value = copyId;
            command.Parameters.Add(copyIdParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new BookCopy
            {
                CopyId = reader.GetInt32(0),
                BookId = reader.GetInt32(1)
            };
        }
    }
}
