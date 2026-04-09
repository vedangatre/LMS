using System.Data;
using LMS.Core.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace LMS.Core.Repository.Implementation
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
