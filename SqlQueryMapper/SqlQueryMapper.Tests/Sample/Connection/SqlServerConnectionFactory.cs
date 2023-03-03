// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlQueryMapper.Tests.Sample.Connection
{
    /// <summary>
    /// SQL Server implementation for a <see cref="ISqlConnectionFactory"/>.
    /// </summary>
    public class SqlServerConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken)
        {
            var dbConnection = new SqlConnection(_connectionString);

            await dbConnection.OpenAsync(cancellationToken);

            return dbConnection;
        }
    }
}
