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
