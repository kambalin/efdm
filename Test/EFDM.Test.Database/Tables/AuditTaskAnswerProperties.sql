CREATE TABLE [dbo].[AuditTaskAnswerProperties] (
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [AuditId] BIGINT NOT NULL,
    [Name] NVARCHAR (255) NOT NULL,
    [OldValue] NVARCHAR (max) NULL,
    [NewValue] NVARCHAR (max) NULL,
    CONSTRAINT [PK_AuditTaskAnswerProperties] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_AuditTaskAnswerProperties_AuditId_AuditTaskAnswerEvents] FOREIGN KEY ([AuditId]) REFERENCES [dbo].[AuditTaskAnswerEvents] ([Id]),
);