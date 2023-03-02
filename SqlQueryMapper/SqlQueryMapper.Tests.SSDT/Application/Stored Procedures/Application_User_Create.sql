CREATE PROCEDURE [Application].[User_Create]
    @FullName                   NVARCHAR(255)
    ,@PreferredName             NVARCHAR(255)
    ,@IsPermittedToLogon        BIT
    ,@LogonName                 NVARCHAR(255)
    ,@HashedPassword            NVARCHAR(MAX)
    ,@LastEditedBy              INT
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
        
    IF @IsPermittedToLogon IS NULL
        THROW 50000, 'IsPermittedToLogon is required', 16;

    IF @IsPermittedToLogon = 1 AND @LogonName IS NULL
        THROW 50000, 'User is permitted to logon. LogonName is required', 16;

    IF @IsPermittedToLogon = 1 AND @HashedPassword IS NULL
        THROW 50000, 'User is permitted to logon. HashedPassword is required', 16;

    IF (SELECT 1 FROM [Application].[User] WHERE LogonName = @LogonName) IS NOT NULL
        THROW 50000, 'LogonName already exists', 16;

    -- Inserts the User into the [Application].[User] table and returns 
    -- the entire entity with computed values included (RowVersion, ...).
    INSERT INTO [Application].[User](FullName, PreferredName, IsPermittedToLogon, LogonName, HashedPassword, LastEditedBy)
    OUTPUT inserted.*
    VALUES (@FullName, @PreferredName, @IsPermittedToLogon, @LogonName, @HashedPassword, @LastEditedBy);

COMMIT TRANSACTION
END TRY
BEGIN CATCH
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;

    THROW;
END CATCH