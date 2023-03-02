CREATE PROCEDURE [Application].[PersonAddress_Create]
    @PersonID           INT
    ,@AddressID         INT
    ,@AddressTypeID     INT
    ,@LastEditedBy      INT
AS
BEGIN TRY
BEGIN TRANSACTION

    SET NOCOUNT ON;

    IF @PersonID IS NULL
        THROW 50000, 'PersonID is required', 16;
    
    IF @AddressID IS NULL
        THROW 50000, 'AddressID is required', 16;

    IF @AddressTypeID IS NULL
        THROW 50000, 'AddressTypeID is required', 16;

    IF @LastEditedBy IS NULL
        THROW 50000, 'LastEditedBy is required', 16;

    IF (SELECT 1 FROM [Application].[Person] WHERE [PersonID] = @PersonID) IS NULL
        THROW 50000, 'Person does not exist', 16;
    
    IF (SELECT 1 FROM [Application].[Address] WHERE [AddressID] = @AddressID) IS NULL
        THROW 50000, 'Address does not exist', 16;

    IF (SELECT 1 FROM [Application].[AddressType] WHERE [AddressTypeID] = @AddressTypeID) IS NULL
        THROW 50000, 'AddressType does not exist', 16;
        
    IF (SELECT 1 
            FROM [Application].[PersonAddress] 
            WHERE [PersonID] = @PersonID AND [AddressID] = @AddressID AND [AddressTypeID] = @AddressTypeID) IS NOT NULL
        THROW 50000, 'Person is already assigned to the given Address and AddressType', 16;
        

    -- Inserts the PersonAddress into the [Application].[PersonAddress] table and returns 
    -- the entire entity with computed values included (RowVersion, ...).
    INSERT INTO [Application].[PersonAddress](PersonID, AddressID, AddressTypeID, LastEditedBy)
    OUTPUT inserted.*
    VALUES (@PersonID, @AddressID, @AddressTypeID, @LastEditedBy);

COMMIT TRANSACTION
END TRY
BEGIN CATCH
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;

    THROW;
END CATCH