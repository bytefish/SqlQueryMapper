CREATE TABLE [Application].[AddressType](
    [AddressTypeID]         INT                                         CONSTRAINT [DF_Application_AddressType_AddressTypeID] DEFAULT (NEXT VALUE FOR [Sequences].[AddressTypeID]) NOT NULL,
    [Name]                  NVARCHAR(255)                               NOT NULL,
    [Description]           NVARCHAR(1000)                              NOT NULL,       
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_AddressType] PRIMARY KEY ([AddressTypeID]),
    CONSTRAINT [FK_AddressType_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Application].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[AddressTypeHistory]));