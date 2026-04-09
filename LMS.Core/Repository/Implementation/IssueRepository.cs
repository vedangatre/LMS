using LMS.Core.Dto;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class IssueRepository : IIssueRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IssueRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public int Create(IssueRecord issueRecord)
        {
            const string sql = @"
                INSERT INTO IssueRecord (StudentId, CopyId, IssueDate, ReturnDate)
                VALUES (@studentId, @copyId, @issueDate, @returnDate);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var studentParameter = command.CreateParameter();
            studentParameter.ParameterName = "@studentId";
            studentParameter.Value = issueRecord.StudentId;
            command.Parameters.Add(studentParameter);

            var copyParameter = command.CreateParameter();
            copyParameter.ParameterName = "@copyId";
            copyParameter.Value = issueRecord.CopyId;
            command.Parameters.Add(copyParameter);

            var issueDateParameter = command.CreateParameter();
            issueDateParameter.ParameterName = "@issueDate";
            issueDateParameter.Value = issueRecord.IssueDate;
            command.Parameters.Add(issueDateParameter);

            var returnDateParameter = command.CreateParameter();
            returnDateParameter.ParameterName = "@returnDate";
            returnDateParameter.Value = issueRecord.ReturnDate ?? (object)DBNull.Value;
            command.Parameters.Add(returnDateParameter);

            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public int UpdateReturn(int issueId, DateTime returnDate)
        {
            const string sql = @"
                UPDATE IssueRecord
                SET ReturnDate = @returnDate
                WHERE IssueId = @issueId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var dateParameter = command.CreateParameter();
            dateParameter.ParameterName = "@returnDate";
            dateParameter.Value = returnDate;
            command.Parameters.Add(dateParameter);

            var issueParameter = command.CreateParameter();
            issueParameter.ParameterName = "@issueId";
            issueParameter.Value = issueId;
            command.Parameters.Add(issueParameter);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int GetIssuedBooks()
        {
            const string sql = @"SELECT COUNT(*) FROM IssueRecord WHERE ReturnDate IS NULL";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public bool HasActiveIssuesForStudent(int studentId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM IssueRecord
                WHERE StudentId = @studentId AND ReturnDate IS NULL";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var studentParameter = command.CreateParameter();
            studentParameter.ParameterName = "@studentId";
            studentParameter.Value = studentId;
            command.Parameters.Add(studentParameter);

            connection.Open();
            return (int)command.ExecuteScalar()! > 0;
        }

        public List<IssuedBookReportItem> GetIssuedBookDetails()
        {
            const string sql = @"
                SELECT s.StudentName, b.BookName, ir.IssueDate
                FROM IssueRecord ir
                INNER JOIN Student s ON s.StudentId = ir.StudentId
                INNER JOIN BookCopy bc ON bc.CopyId = ir.CopyId
                INNER JOIN Book b ON b.BookId = bc.BookId
                WHERE ir.ReturnDate IS NULL
                ORDER BY ir.IssueDate";

            var results = new List<IssuedBookReportItem>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new IssuedBookReportItem
                {
                    StudentName = reader.GetString(0),
                    BookName = reader.GetString(1),
                    IssueDate = reader.GetDateTime(2)
                });
            }

            return results;
        }
    }
}
