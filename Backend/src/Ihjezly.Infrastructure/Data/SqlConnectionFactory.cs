using System.Data;
using Ihjezly.Application.Abstractions.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ihjezly.Infrastructure.Data;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Database")!;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}