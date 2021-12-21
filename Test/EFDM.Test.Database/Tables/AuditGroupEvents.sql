CREATE TABLE [dbo].[AuditGroupEvents] (
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Created] DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedById] INT NOT NULL DEFAULT 1,
    [ObjectType] NVARCHAR (255)NOT NULL,
    [ObjectId] NVARCHAR (255) NOT NULL,
    [ActionId] INT NOT NULL,
    CONSTRAINT [PK_AuditGroupEvents] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_AuditGroupEvents_CreatedBy_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([Id]),
);