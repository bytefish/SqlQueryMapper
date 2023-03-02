// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Transactions;

namespace SqlQueryMapper.Tests
{
    /// <summary>
    /// Implements Tests, that will be used by all tests, that need an <see cref="ApplicationDbContext"/>.
    /// </summary>
    public class TransactionalTestBase
    {
        /// <summary>
        /// We can assume the Configuration has been initialized, when the Tests 
        /// are run. So we inform the compiler, that this field is intentionally 
        /// left uninitialized.
        /// </summary>
        protected IConfiguration _configuration = null!;

        /// <summary>
        /// TransactionScope to run all Tests within a Transaction.
        /// </summary>
        protected TransactionScope _transactionScope = null!;

        /// <summary>
        /// Initializes the Test.
        /// </summary>
        public TransactionalTestBase()
        {
            _configuration = ReadConfiguration();
        }

        /// <summary>
        /// Read the appsettings.json for the Test, e.g. ConnectionStrings.
        /// </summary>
        /// <returns>Configurations read from the AppSettings</returns>
        private IConfiguration ReadConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// The SetUp method creates a new <see cref="TransactionScope"/>.
        /// </summary>
        [SetUp]
        protected void Setup()
        {
            OnSetupBeforeTransaction();

            _transactionScope = CreateTransactionScope();
            OnSetupInTransaction();
        }
        /// <summary>
        /// The TearDown method disposes the <see cref="TransactionScope"/> after each test.
        /// </summary>
        [TearDown]
        protected void Teardown()
        {
            OnTearDownInTransaction();
            _transactionScope.Dispose();
            OnTearDownAfterTransaction();
        }

        /// <summary>
        /// Called before the Transaction is started.
        /// </summary>
        public virtual void OnSetupBeforeTransaction()
        {
        }

        /// <summary>
        /// Called inside a Transaction.
        /// </summary>
        public virtual void OnSetupInTransaction()
        {
        }

        /// <summary>
        /// Called inside a Transaction.
        /// </summary>
        public virtual void OnTearDownInTransaction()
        {
        }

        /// <summary>
        /// Called after the Transaction has been rolled back.
        /// </summary>
        public virtual void OnTearDownAfterTransaction()
        {
        }

        /// <summary>
        /// Creates a TransactionScope with sane default values.
        /// </summary>
        /// <returns>A <see cref="TransactionScope"/></returns>
        public static TransactionScope CreateTransactionScope(
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Enabled,
            TimeSpan? timeout = null)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = timeout ?? TransactionManager.DefaultTimeout
            };

            return new TransactionScope(transactionScopeOption, options, asyncFlowOption);
        }
    }
}