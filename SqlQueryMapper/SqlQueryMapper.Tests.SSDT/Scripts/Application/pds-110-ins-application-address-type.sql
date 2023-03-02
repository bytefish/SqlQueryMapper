PRINT 'Inserting [Application].[AddressType] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[AddressType]
-----------------------------------------------
MERGE INTO [Application].[AddressType] AS [Target]
USING (VALUES 
      (1,  N'Home',         N'Home Address',        1,      @ValidFrom,     @ValidTo)
    , (2,  N'Work',         N'Work Address',        1,      @ValidFrom,     @ValidTo)	
    , (3,  N'Billing',      N'Billing Address',     1,      @ValidFrom,     @ValidTo)
    , (4,  N'Delivery',     N'Delivery Address',    1,      @ValidFrom,     @ValidTo)
) AS [Source]([AddressTypeID], [Name], [Description], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[AddressTypeID] = [Source].[AddressTypeID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([AddressTypeID], [Name], [Description], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[AddressTypeID], [Source].[Name], [Source].[Description], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);