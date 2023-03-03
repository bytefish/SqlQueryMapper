# SqlQueryMapper #

[MIT License]: https://opensource.org/licenses/MIT
[Belgrade SqlClient]: https://github.com/JocaPC/Belgrade-SqlClient

SqlQueryMapper is a small wrapper over ADO.NET, that simplifies data access. It draws a 
lot of inspiration from [Belgrade SqlClient], which has a lot more features to offer, so 
please also give [Belgrade SqlClient] a try:

* https://github.com/JocaPC/Belgrade-SqlClient

The idea of the library is to have a very small library for data access and retain 
complete control over the result mapping, without conventions and magic imposed by 
libraries like Dapper.

## Installing SqlQueryMapper ##

You can use [NuGet](https://www.nuget.org) to install SqlQueryMapper. Run the following command 
in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

```
PM> Install-Package SqlQueryMapper
```

## Basic Usage ##

The following example shows the basic usage of an `SqlQuery` to call a Stored Procedure
named `[Application].[Address_Create]` and map its results to an object. It uses the basic 
ADO.NET infrastructure, such as a `DbConnection` and a `DbDataReader`:

```csharp
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Microsoft.Extensions.Logging;
using SqlQueryMapper.Extensions;
using SqlQueryMapper.Tests.Sample.Connection;
using SqlQueryMapper.Tests.Sample.Models;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlQueryMapper.Tests.Sample.Services
{
    public class PersonService
    {
        private readonly ILogger<PersonService> _logger;
        private readonly ISqlConnectionFactory _connectionFactory;
        
        public PersonService(ILogger<PersonService> logger, ISqlConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        
        // ...
        
        public async Task CreateAddressAsync(Address address, int lastEditedBy, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Application].[Address_Create]")
                    .SetLogger(_logger)
                    .Param("AddressLine1", address.AddressLine1)
                    .Param("AddressLine2", address.AddressLine2)
                    .Param("AddressLine3", address.AddressLine3)
                    .Param("AddressLine4", address.AddressLine4)
                    .Param("PostalCode", address.PostalCode)
                    .Param("City", address.City)
                    .Param("Country", address.Country)
                    .Param("LastEditedBy", lastEditedBy);

                await query
                    .MapAsync((reader) => ConvertAddress(reader, address), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        
        private static void ConvertAddress(DbDataReader reader, Address address)
        {
            address.Id = reader.GetInt32("AddressID");
            address.AddressLine1 = reader.GetString("AddressLine1");
            address.AddressLine2 = reader.GetNullableString("AddressLine2");
            address.AddressLine3 = reader.GetNullableString("AddressLine3");
            address.AddressLine4 = reader.GetNullableString("AddressLine4");
            address.City = reader.GetString("City");
            address.PostalCode = reader.GetNullableString("PostalCode");
            address.Country = reader.GetString("Country");
            address.RowVersion = reader.GetByteArray("RowVersion");
            address.LastEditedBy = reader.GetInt32("LastEditedBy");
            address.ValidFrom = reader.GetDateTime("ValidFrom");
            address.ValidTo = reader.GetDateTime("ValidTo");
        }
        
        // ...
    }
}
```

## Reading JSON ##

The following `JsonQueryTests` integration test shows how to use the SqlQuery library to 
query for JSON an deserialize the results. 

```csharp
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SqlQueryMapper.Tests.Sample.Connection;
using SqlQueryMapper.Tests.Sample.Models;
using SqlQueryMapper.Tests.Sample.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace SqlQueryMapper.Tests.Json
{
    /// <summary>
    /// Examples for JSON queries.
    /// </summary>
    public class JsonQueryTests : TransactionalTestBase
    {
        #region Sample Model

        /// <summary>
        /// OData Response model.
        /// </summary>
        /// <typeparam name="T">Returned Entity</typeparam>
        private class ODataResponse<T>
        {
            [JsonPropertyName("@odata.context")]
            public string? Type { get; set; }

            [JsonPropertyName("value")]
            public List<T> Value { get; set; } = new();
        }


        /// <summary>
        /// Models the Address.
        /// </summary>
        private class AddressDto
        {
            [JsonPropertyName("AddressLine1")]
            public string AddressLine1 { get; set; } = null!;

            [JsonPropertyName("AddressLine2")]
            public string? AddressLine2 { get; set; }

            [JsonPropertyName("AddressLine3")]
            public string? AddressLine3 { get; set; }

            [JsonPropertyName("AddressLine4")]
            public string? AddressLine4 { get; set; }

            [JsonPropertyName("AddressType")]
            public string AddressType { get; set; } = null!;
        }

        /// <summary>
        /// Models the Person.
        /// </summary>
        private class PersonDto
        {
            [JsonPropertyName("FullName")]
            public string FullName { get; set; } = null!;

            [JsonPropertyName("PreferredName")]
            public string PreferredName { get; set; } = null!;

            [JsonPropertyName("Addresses")]
            public List<AddressDto> Addresses { get; set; } = new();
        }

        #endregion Sample Model

        #region Test Data

        private async Task InitializeSampleData(ISqlConnectionFactory connectionFactory)
        {
            var logger = CreateLogger<PersonService>();
            var service = new PersonService(logger, connectionFactory);

            var user = new User
            {
                FullName = "Philipp Wagner",
                PreferredName = "Philipp",
                IsPermittedToLogon = true,
                LogonName = "philipp@test.localhost",
                HashedPassword = "ThisIsASuperSecretPasswordUserForTests",
            };

            await service.CreateUserAsync(user, 1, default);

            var person = new Person
            {
                FullName = "Philipp Wagner",
                PreferredName = "Philipp",
                UserId = user.Id,
                LastEditedBy = 1
            };

            await service.CreatePersonAsync(person, 1, default);

            // Create a Billing Address
            {
                var address = new Address
                {
                    AddressLine1 = "Billing Address Street 123",
                    City = "Billing Town",
                    Country = "Billing Country",
                };

                await service.CreateAddressAsync(address, 1, default);

                var personAddress = new PersonAddress
                {
                    PersonId = person.Id,
                    AddressId = address.Id,
                    AddressTypeId = (int)AddressTypeEnum.Billing
                };

                await service.CreatePersonAddressAsync(personAddress, 1, default);
            }

            // Create a Home Address
            {
                var address = new Address
                {
                    AddressLine1 = "Home Address Street 456",
                    City = "Home Town",
                    Country = "Home Country",
                };

                await service.CreateAddressAsync(address, 1, default);

                var personAddress = new PersonAddress
                {
                    PersonId = person.Id,
                    AddressId = address.Id,
                    AddressTypeId = (int)AddressTypeEnum.Home
                };

                await service.CreatePersonAddressAsync(personAddress, 1, default);
            }
        }

        #endregion Test Data

        #region Tests

        [Test]
        public async Task JsonQuery_QueryForJson_Success()
        {
            // An ISqlConnectionFactory to create an opened SqlConnection.
            var connectionFactory = GetSqlServerConnectionFactory();

            await InitializeSampleData(connectionFactory);

            // A JSON Query using "FOR JSON PATH" to return the data exactely the way we need it:
            var sql = @"SELECT person.FullName, person.PreferredName,
                    (SELECT address.AddressLine1, AddressLine2, AddressLine3, AddressLine4, address_type.Name AS [AddressType]
                        FROM [Application].[PersonAddress] person_address
                            INNER JOIN [Application].[Address] address ON person_address.AddressID = address.AddressID
							INNER JOIN [Application].[AddressType] address_type ON person_address.AddressTypeID = address_type.AddressTypeID
                        WHERE 
                            person_address.PersonID = person.PersonID
                        FOR JSON PATH
                    ) AS [Addresses]
                FROM [Application].[Person] person
                FOR JSON PATH";

            // The JSON should be an OData Response, so we wrap it into an 
            // OData JSON response according to the OData specification:
            var options = new SqlQueryStreamOptions
            {
                Encoding = Encoding.UTF8,
                DefaultOutput = "[]",
                Prefix = @"{""@odata.context"":""http://localhost/odata/Person"",""value"":",
                Suffix = @"}"
            };

            // Now read the JSON Response from the SQL Server:
            string jsonResult = string.Empty;

            using (var connection = await connectionFactory.GetDbConnectionAsync(default).ConfigureAwait(false))
            {
                using (var textWriter = new StringWriter())
                {
                    // Fire off the SQL query to get the JSON response:
                    await new SqlQuery(connection).Sql(sql)
                        .StreamAsync(textWriter, options, default)
                        .ConfigureAwait(false);

                    jsonResult = textWriter.ToString();
                }
            }

            // Deserialize the Response into an ODataResponse<T> model:
            ODataResponse<PersonDto> result = System.Text.Json.JsonSerializer.Deserialize<ODataResponse<PersonDto>>(jsonResult)!;

            // And run some sanity tests:
            Assert.IsNotNull(result);

            Assert.AreEqual("http://localhost/odata/Person", result.Type);

            // Check ODataResponse value:
            {
                var value = result.Value;

                Assert.AreEqual(1, value.Count);

                // Check Person:
                {
                    var person = value[0];

                    Assert.AreEqual("Philipp Wagner", person.FullName);
                    Assert.AreEqual("Philipp", person.PreferredName);

                    // Check Addresses:
                    {
                        var addresses = person.Addresses
                            .OrderBy(x => x.AddressType)
                            .ToList();

                        Assert.AreEqual(2, result.Value[0].Addresses.Count);

                        Assert.AreEqual("Billing Address Street 123", result.Value[0].Addresses[0].AddressLine1);
                        Assert.AreEqual("Home Address Street 456", result.Value[0].Addresses[1].AddressLine1);
                    }
                }
            }
        }

        #endregion Tests

        #region Infrastructure 

        /// <summary>
        /// Builds an <see cref="SqlServerConnectionFactory"/>.
        /// </summary>
        /// <returns>An initialized <see cref="SqlServerConnectionFactory"/></returns>
        /// <exception cref="InvalidOperationException">Thrown when no Connection String "ApplicationDatabase" was found</exception>
        private SqlServerConnectionFactory GetSqlServerConnectionFactory()
        {
            var connectionString = _configuration.GetConnectionString("SqlQueryMapperTestDatabase");

            if (connectionString == null)
            {
                throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
            }

            return new SqlServerConnectionFactory(connectionString);
        }

        #endregion Infrastructure 
    }
}
```

## License ##

The library is released under terms of the [MIT License].
