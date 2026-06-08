using System.Data;

namespace Providers.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
