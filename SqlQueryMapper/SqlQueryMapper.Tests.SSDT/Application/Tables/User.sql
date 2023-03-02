CREATE TABLE [Application].[User](
    [UserID]                INT                                         CONSTRAINT [DF_Application_User_UserID] DEFAULT (NEXT VALUE FOR [Sequences].[UserID]) NOT NULL,
    [FullName]              NVARCHAR(255)                               NOT NULL,
    [PreferredName]         NVARCHAR(255)                               NOT NULL,
    [IsPermittedToLogon]    BIT                                         NOT NULL,
    [LogonName]             NVARCHAR (255)                              NULL,
    [HashedPassword]        NVARCHAR (MAX)                              NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserHistory]));