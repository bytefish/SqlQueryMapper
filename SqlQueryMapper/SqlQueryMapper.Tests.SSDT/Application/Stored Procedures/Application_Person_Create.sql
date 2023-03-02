CREATE PROCEDURE [Application].[Person_Create]
    @FullName               NVARCHAR(255)
    ,@PreferredName         NVARCHAR(255)
    ,@UserID                INT
    ,@LastEditedBy          INT
AS
BEGIN TRY
BEGIN TRANSACTION

    SET NOCOUNT ON;

    IF @LastEditedBy IS NULL
        THROW 50000, 'LastEditedBy is required', 16;
        
    IF @FullName IS NULL
        THROW 50000, 'FullName is required', 16;
        
    IF @PreferredName IS NULL
        THROW 50000, 'PreferredName is required', 16;
        
    -- Inserts the Address into the [Application].[Address] table and returns 
    -- the entire entity with computed values included (RowVersion, ...).
    INSERT INTO [Application].[Person](FullName, PreferredName, UserID, LastEditedBy)
    OUTPUT inserted.*
    VALUES (@FullName, @PreferredName, @UserID, @LastEditedBy);

COMMIT TRANSACTION
END TRY
BEGIN CATCH
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;

    THROW;
END CATCH