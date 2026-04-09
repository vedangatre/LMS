using System.Data;
using LMS.Core.Models;
using LMS.Core.Repository.Interfaces;

namespace LMS.Core.Repository.Implementation
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StudentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Student AddStudent(string name)
        {
            const string sql = @"
                INSERT INTO Student (StudentName)
                VALUES (@studentName);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@studentName";
            nameParameter.Value = name;
            command.Parameters.Add(nameParameter);

            connection.Open();
            var newId = (int)command.ExecuteScalar()!;

            return new Student { StudentId = newId, StudentName = name };
        }

        public bool DeleteStudent(int studentId)
        {
            const string sql = @"DELETE FROM Student WHERE StudentId = @studentId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@studentId";
            idParameter.Value = studentId;
            command.Parameters.Add(idParameter);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public Student? GetStudentById(int studentId)
        {
            const string sql = @"
                SELECT StudentId, StudentName
                FROM Student
                WHERE StudentId = @studentId";

            using var connection = _connectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@studentId";
            idParameter.Value = studentId;
            command.Parameters.Add(idParameter);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Student
            {
                StudentId = reader.GetInt32(0),
                StudentName = reader.GetString(1)
            };
        }
    }
}
