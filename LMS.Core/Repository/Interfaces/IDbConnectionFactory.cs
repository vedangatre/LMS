using System.Data;

namespace LMS.Core.Repository.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
