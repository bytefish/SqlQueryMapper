CREATE PROCEDURE [Application].[Address_Create]
    @AddressLine1          NVARCHAR(2000)
    ,@AddressLine2          NVARCHAR(2000)
    ,@AddressLine3          NVARCHAR(2000)
    ,@AddressLine4          NVARCHAR(2000)
    ,@PostalCode            NVARCHAR(255)
    ,@City                  NVARCHAR(255)
    ,@Country               NVARCHAR(255)
    ,@LastEditedBy          INT
AS
BEGIN TRY
BEGIN TRANSACTION

    SET NOCOUNT ON;

    IF @LastEditedBy IS NULL
        THROW 50000, 'LastEditedBy is required', 16;
        
    IF @AddressLine1 IS NULL
        THROW 50000, 'AddressLine 1 is required', 16;
        
    IF @City IS NULL
        THROW 50000, 'City is required', 16;

    IF @Country IS NULL
        THROW 50000, 'Country is required', 16;

    -- Inserts the Address into the [Application].[Address] table and returns 
    -- the entire entity with computed values included (RowVersion, ...).
    INSERT INTO [Application].[Address](AddressLine1, AddressLine2, AddressLine3, AddressLine4, PostalCode, City, Country, LastEditedBy)
    OUTPUT inserted.*
    VALUES (@AddressLine1, @AddressLine2, @AddressLine3, @AddressLine4, @PostalCode, @City, @Country, @LastEditedBy);

COMMIT TRANSACTION
END TRY
BEGIN CATCH
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;

    THROW;
END CATCH