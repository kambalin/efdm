CREATE TABLE [dbo].[AuditGroupProperties] (
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [AuditId] BIGINT NOT NULL,
    [Name] NVARCHAR (255) NOT NULL,
    [OldValue] NVARCHAR (max) NULL,
    [NewValue] NVARCHAR (max) NULL,
    CONSTRAINT [PK_AuditGroupProperties] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_AuditGroupProperties_AuditId_AuditGroupEvents] FOREIGN KEY ([AuditId]) REFERENCES [dbo].[AuditGroupEvents] ([Id]),
);