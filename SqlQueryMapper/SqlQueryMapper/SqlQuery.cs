// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SqlQueryMapper
{
    /// <summary>
    /// Provides a thin wrapper over ADO.NET.
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        /// Connection to the database.
        /// </summary
        public DbTransaction? Transaction { get; private set; }

        /// <summary>
        /// Command to be executed.
        /// </summary>
        public DbCommand? Command { get; private set; }

        /// <summary>
        /// Logger to use.
        /// </summary>
        public ILogger? Logger { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SqlQuery"/> with a connection.
        /// </summary>
        /// <param name="connection">Connection to use</param>
        public SqlQuery(DbConnection connection)
            : this(connection, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SqlQuery"/> with a connection.
        /// </summary>
        /// <param name="connection">Connection to use</param>
        /// <param name="transaction">Transaction to use</param>
        public SqlQuery(DbConnection connection, DbTransaction? transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Sets the <see cref="ILogger"/> to use.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> to user</param>
        /// <returns>SqlQuery with the given <see cref="ILogger"/></returns>
        public SqlQuery SetCommand(ILogger logger)
        {
            Logger = logger;

            return this;
        }

        /// <summary>
        /// Sets the <see cref="DbCommand"/> to execute.
        /// </summary>
        /// <param name="command"><see cref="DbCommand"/> to execute</param>
        /// <returns>SqlQuery with <see cref="DbCommand"/></returns>
        public SqlQuery SetCommand(DbCommand command)
        {
            Command = command;

            return this;
        }

        /// <summary>
        /// Sets the Command Type.
        /// </summary>
        /// <param name="commandType">Command Type</param>
        /// <returns>SqlQuery with Command Type</returns>
        public SqlQuery SetCommandType(CommandType commandType)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.CommandType = commandType;

            return this;
        }

        /// <summary>
        /// Sets the Command Timeout.
        /// </summary>
        /// <param name="commandTimeout">Command Timeout</param>
        /// <returns>SqlQuery with Command Timeout</returns>
        public SqlQuery SetCommandTimeout(TimeSpan commandTimeout)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.CommandTimeout = (int)commandTimeout.TotalSeconds;

            return this;
        }

        /// <summary>
        /// Adds a Parameter to the Query.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        /// <param name="size">Size</param>
        /// <returns><see cref="SqlQuery"/> with Parameter added</returns>
        public SqlQuery AddParameter(string name, DbType type, object? value, int size = 0)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            var parameter = Command.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value;
            parameter.Size = size;

            if (size == 0)
            {
                bool isTextType = type == DbType.AnsiString || type == DbType.String;

                if (value != null && isTextType)
                {
                    parameter.Size = 100 * (value.ToString()!.Length / 100 + 1);
                }
            }
            else
            {
                parameter.Size = size;
            }

            Command.Parameters.Add(parameter);

            return this;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<DataSet> ExecuteDataSetAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            DataSet dataset = new DataSet();

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                while (!reader.IsClosed)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataset.Tables.Add(dataTable);
                }
            }

            return dataset;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataTable"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<DataTable> ExecuteDataTableAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            DataTable dataTable = new DataTable();

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            int numberOfRowsAffected = await Command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            return numberOfRowsAffected;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            object? scalarValue = await Command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return scalarValue;
        }


        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and executes a Callback on each row.
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="callback">Callback for each row</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task.</returns>
        public async Task MapAsync(Action<DbDataReader> callback, CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                do
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        callback(reader);
                    }
                } while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and executes a Callback on each row.
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="callback">Callback for each row</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task.</returns>
        public async Task<List<T>> MapAsync<T>(Func<DbDataReader, T> callback, CancellationToken cancellationToken)
            where T : class
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            List<T> results = new List<T>();

            using (var reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (reader.Read())
                {
                    var entity = callback(reader);

                    results.Add(entity);
                }

                await reader.NextResultAsync();
            }

            return results;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and executes a Callback on each row.
        /// </summary>
        /// <param name="callback">Callback for each row</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task.</returns>
        public async Task<List<T>> MapAsync<T>(Func<DbDataReader, Task<T>> callback, CancellationToken cancellationToken)
            where T : class
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            List<T> results = new List<T>();

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var entity = await callback(reader);

                    results.Add(entity);
                }

                await reader.NextResultAsync(cancellationToken);
            }

            return results;
        }

        /// <summary>
        /// Helper function that flushes results of SQL query into the stream.
        /// </summary>
        /// <param name="stream">Stream to write the JSON Response to</param>
        /// <param name="defaultOutput">The default output, e.g. "[]"</param>
        /// <returns>An awaitable Task</returns>
        public async Task StreamAsync(TextWriter stream, SqlQueryStreamOptions options, CancellationToken cancellationToken)
        {
            bool outputIsGenerated = false;
            bool isErrorDetected = false;

            bool isFirstChunk = true;

            try
            {
                await MapAsync(async reader =>
                {
                    try
                    {
                        if (isFirstChunk && options.Prefix != null)
                        {
                            await FlushContentAsync(stream, options.Prefix, outputEncoding: options.Encoding);

                            isFirstChunk = false;
                        }

                        if (reader.HasRows)
                        {
                            if (reader.FieldCount != 1)
                            {
                                throw new ArgumentException("SELECT query should not have " + reader.FieldCount + " columns (expected 1).", "reader");
                            }

                            string? buffer = null;

                            if (reader[0].GetType().Name == "String")
                            {
                                buffer = reader.GetString(0);
                                await FlushContentAsync(stream, buffer, outputEncoding: options.Encoding);
                                outputIsGenerated = true;
                            }
                            else if (reader[0].GetType().Name == "Byte[]")
                            {
                                byte[] binary = new byte[2048];
                                int amount = (int)reader.GetBytes(0, 0, binary, 0, 2048);
                                int pos = amount;
                                do
                                {
                                    await FlushContentAsync(stream, binary, outputEncoding: options.Encoding, amount: amount);
                                    outputIsGenerated = true;
                                    amount = (int)reader.GetBytes(0, pos, binary, 0, 2048);
                                    pos += amount;
                                }
                                while (amount > 0);
                            }
                            else
                            {
                                Logger?.LogError("Column type " + reader[0].GetType().Name + " cannot be sent to the streamed.");

                                throw new ArgumentException("The column type returned by the query cannot be sent to the stream.", reader[0].GetType().Name);
                            }
                        }
                        else
                        {
                            var amount = options.DefaultOutput is byte[]? ((byte[])options.DefaultOutput).Length : -1;

                            await FlushContentAsync(stream, options.DefaultOutput, options.Encoding, amount: amount);

                            outputIsGenerated = true;
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger?.LogError(exception, "An Error has occured");

                        isErrorDetected = true;
                    }
                }, cancellationToken);
            }
            finally
            {
                if (!isErrorDetected && isFirstChunk && options.Prefix != null)
                {
                    await FlushContentAsync(stream, options.Prefix, outputEncoding: options.Encoding);

                    isFirstChunk = false;
                }

                /// If the output is not generated by DataReader we need to generate default value.
                if (!outputIsGenerated && options.DefaultOutput != null)
                {
                    var amount = options.DefaultOutput is byte[]? ((byte[])options.DefaultOutput).Length : -1;

                    await FlushContentAsync(stream, options.DefaultOutput, outputEncoding: options.Encoding, amount: amount);
                }

                // Add suffix if there was no error.
                if (!isErrorDetected && options.Suffix != null)
                {
                    await FlushContentAsync(stream, options.Suffix, options.Encoding);
                }
            }
        }

        /// <summary>
        /// Helper function that flushes content into stream or writer.
        /// </summary>
        /// <param name="content">Content that will be flushed into stream or text writer.</param>
        /// <param name="amount">Lengt of the bytes to be writted (-1 for string).</param>
        /// <returns>An awaitable Task</returns>
        private async Task FlushContentAsync(TextWriter writer, object content, Encoding outputEncoding, int amount = -1)
        {
            try
            {
                if (amount > -1)
                {
                    await writer
                        .WriteAsync(outputEncoding.GetString((byte[])content, 0, amount))
                        .ConfigureAwait(false);

                    await writer.FlushAsync().ConfigureAwait(false);
                }
                else
                {
                    await writer
                        .WriteAsync(content as string)
                        .ConfigureAwait(false);

                    await writer.FlushAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "Error occured while trying to flush content to output");

                throw;
            }
        }
    }
}