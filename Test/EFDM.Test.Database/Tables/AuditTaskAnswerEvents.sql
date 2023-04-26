CREATE TABLE [dbo].[AuditTaskAnswerEvents] (
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Created] DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedById] INT NOT NULL DEFAULT 1,
    [ObjectType] NVARCHAR (255)NOT NULL,
    [ObjectId] NVARCHAR (255) NOT NULL,
    [ActionId] INT NOT NULL,
    CONSTRAINT [PK_AuditTaskAnswerEvents] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_AuditTaskAnswerEvents_CreatedBy_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([Id]),
);