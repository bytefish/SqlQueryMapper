/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
  We need to deactivate all Temporal Tables before the initial data load.
*/

EXEC [Application].[DeactivateTemporalTables]
GO

/* 
    Set the initial data for the [Application] schema
*/
:r ".\Application\pds-100-ins-application-users.sql"
GO

:r ".\Application\pds-110-ins-application-address-type.sql"
GO

EXEC [Application].[ReactivateTemporalTables]
GO
