# SqlQueryMapper #

[MIT License]: https://opensource.org/licenses/MIT
[Belgrade SqlClient]: https://github.com/JocaPC/Belgrade-SqlClient

SqlQueryMapper is a small wrapper over ADO.NET, that simplifies data access. It draws a 
lot of inspiration from [Belgrade SqlClient], which has a lot more features to offer, so 
please also give [Belgrade SqlClient] a try:

* https://github.com/JocaPC/Belgrade-SqlClient

## Example ##

```csharp

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
```



## Installing SqlQueryMapper ##

You can use [NuGet](https://www.nuget.org) to install SqlQueryMapper. Run the following command 
in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

```
PM> Install-Package SqlQueryMapper
```

## License ##

The library is released under terms of the [MIT License].