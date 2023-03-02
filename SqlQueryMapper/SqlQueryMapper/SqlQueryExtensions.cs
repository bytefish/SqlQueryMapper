// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SqlQueryMapper
{
    /// <summary>
    /// SQL Server Extensions for the <see cref="SqlQuery"/>.
    /// </summary>
    public static class SqlQueryExtensions
    {
        /// <summary>
        /// Sets the Command Text for the <see cref="SqlQuery"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static SqlQuery Sql(this SqlQuery query, string commandText)
        {
            var cmd = new SqlCommand(commandText);

            return query.SetCommand(cmd);
        }

        /// <summary>
        /// Sets the Command Text for the <see cref="SqlQuery"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static SqlQuery Proc(this SqlQuery query, string storedProcedureName)
        {
            var cmd = new SqlCommand(storedProcedureName);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            return query.SetCommand(cmd);
        }

        /// <summary>
        /// Adds FOR JSON PATH to the Command Text.
        /// </summary>
        /// <param name="cmd">Command to modify</param>
        /// <returns>Command with JSON return value</returns>
        public static SqlCommand AsJson(this SqlCommand cmd)
        {
            cmd.CommandText += " FOR JSON PATH";
            return cmd;
        }

        /// <summary>
        /// Adds FOR JSON PATH, ROOT(...) to the Command Text.
        /// </summary>
        /// <param name="cmd">Command to modify</param>
        /// <param name="root">JSON root node</param>
        /// <returns>Command with JSON return value</returns>
        public static SqlCommand AsJson(this SqlCommand cmd, string root)
        {
            cmd.CommandText += " FOR JSON PATH, ROOT('" + root + "')";
            return cmd;
        }

        /// <summary>
        /// Adds FOR JSON PATH, WITHOUT_ARRAY_WRAPPER to the Command Text.
        /// </summary>
        /// <param name="cmd">Command to modify</param>
        /// <returns>Command with JSON return value</returns>
        public static SqlCommand AsSingleJson(this SqlCommand cmd)
        {
            cmd.CommandText += " FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
            return cmd;
        }

        /// <summary>
        /// Add a parameter with specified value to the mapper.
        /// </summary>
        /// <param name="mapper">Mapper where the parameter will be added.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns>Mapper object.</returns>
        public static SqlQuery Param(this SqlQuery query, string name, object? value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }

            if (query.Command is SqlCommand)
            {
                var p = ((SqlCommand)query.Command).Parameters.AddWithValue(name, value);
                if (p.SqlDbType == System.Data.SqlDbType.NVarChar
                    || p.SqlDbType == System.Data.SqlDbType.VarChar)
                {
                    p.Size = 100 * (value.ToString()!.Length / 100 + 1);
                }
            }

            return query;
        }

        /// <summary>
        /// Maps a <see cref="DbDataReader"/> to a single row.
        /// </summary>
        /// <typeparam name="T">Type of the Entity</typeparam>
        /// <param name="query"><see cref="SqlQuery"/> to execute</param>
        /// <param name="callback">Callback for each row in the <see cref="DbDataReader"/></param>
        /// <param name="cancellationToken">Cancellation Token to cancel asynchronous processing</param>
        /// <returns>The entity of type <typeparamref name="T"/></returns>
        /// <exception cref="InvalidOperationException">Thrown, when 0 or more than one entities found</exception>
        public static async Task<T> MapSingleAsync<T>(this SqlQuery query, Func<DbDataReader, T> callback, CancellationToken cancellationToken)
            where T: class
        {
            var result = await query.MapAsync(callback, cancellationToken);

            if(result.Count != 1)
            {
                throw new InvalidOperationException($"Expected '1' results, but got '{result.Count}'");
            }

            return result.Single();
        }
    }
}
