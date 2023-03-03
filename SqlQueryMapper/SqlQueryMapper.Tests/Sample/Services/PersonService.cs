// Licensed under the MIT license. See LICENSE file in the project root for full license information.


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
        private readonly ISqlConnectionFactory _connectionFactory;

        public PersonService(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #region Person

        public async Task CreatePersonAsync(Person person, int lastEditedBy, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Application].[Person_Create]")
                    .Param("FullName", person.FullName)
                    .Param("PreferredName", person.PreferredName)
                    .Param("UserID", person.UserId)
                    .Param("LastEditedBy", lastEditedBy);

                await query
                    .MapAsync((reader) => ConvertPerson(reader, person), cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static void ConvertPerson(DbDataReader reader, Person address)
        {
            address.Id = reader.GetInt32("PersonID");
            address.FullName = reader.GetString("FullName");
            address.PreferredName = reader.GetString("PreferredName");
            address.UserId = reader.GetNullableInt32("UserID");
            address.LastEditedBy = reader.GetInt32("LastEditedBy");
            address.RowVersion = reader.GetByteArray("RowVersion");
            address.LastEditedBy = reader.GetInt32("LastEditedBy");
            address.ValidFrom = reader.GetDateTime("ValidFrom");
            address.ValidTo = reader.GetDateTime("ValidTo");
        }

        #endregion Person

        #region User

        public async Task CreateUserAsync(User user, int lastEditedBy, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Application].[User_Create]")
                    .Param("FullName", user.FullName)
                    .Param("PreferredName", user.PreferredName)
                    .Param("IsPermittedToLogon", user.IsPermittedToLogon)
                    .Param("LogonName", user.LogonName)
                    .Param("HashedPassword", user.HashedPassword)
                    .Param("LastEditedBy", lastEditedBy);

                await query
                    .MapAsync((reader) => ConvertUser(reader, user), cancellationToken)
                    .ConfigureAwait(false);
            }
        }


        private static void ConvertUser(DbDataReader reader, User user)
        {
            user.Id = reader.GetInt32("UserID");
            user.FullName = reader.GetString("FullName");
            user.PreferredName = reader.GetString("PreferredName");
            user.IsPermittedToLogon = reader.GetBoolean("IsPermittedToLogon");
            user.LogonName = reader.GetString("LogonName");
            user.HashedPassword = reader.GetString("HashedPassword");
            user.LastEditedBy = reader.GetInt32("LastEditedBy");
            user.RowVersion = reader.GetByteArray("RowVersion");
            user.LastEditedBy = reader.GetInt32("LastEditedBy");
            user.ValidFrom = reader.GetDateTime("ValidFrom");
            user.ValidTo = reader.GetDateTime("ValidTo");
        }

        #endregion User

        #region Address 

        public async Task CreateAddressAsync(Address address, int lastEditedBy, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Application].[Address_Create]")
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

        #endregion Address

        #region PersonAddress

        public async Task CreatePersonAddressAsync(PersonAddress personAddress, int lastEditedBy, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Application].[PersonAddress_Create]")
                    .Param("PersonID", personAddress.PersonId)
                    .Param("AddressID", personAddress.AddressId)
                    .Param("AddressTypeID", personAddress.AddressTypeId)
                    .Param("LastEditedBy", lastEditedBy);

                await query
                    .MapAsync((reader) => ConvertPersonAddress(reader, personAddress), cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static void ConvertPersonAddress(DbDataReader reader, PersonAddress address)
        {
            address.Id = reader.GetInt32("AddressID");
            address.PersonId = reader.GetInt32("PersonID");
            address.AddressId = reader.GetInt32("AddressID");
            address.AddressTypeId = reader.GetInt32("AddressTypeID");
            address.LastEditedBy = reader.GetInt32("LastEditedBy");
            address.RowVersion = reader.GetByteArray("RowVersion");
            address.LastEditedBy = reader.GetInt32("LastEditedBy");
            address.ValidFrom = reader.GetDateTime("ValidFrom");
            address.ValidTo = reader.GetDateTime("ValidTo");
        }

        #endregion PersonAddress
    }
}
