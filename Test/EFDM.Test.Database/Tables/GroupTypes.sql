CREATE TABLE [dbo].[GroupTypes]
(
	[Id] INT IDENTITY(1,1)  NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(150) NOT NULL,
	[IsDeleted] BIT NOT NULL DEFAULT 0,
	[Created] DATETIMEOFFSET (7) CONSTRAINT [DF_GroupTypes_Created] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [Modified] DATETIMEOFFSET (7) CONSTRAINT [DF_GroupTypes_Modified] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedById]  INT NOT NULL DEFAULT 1,
    [ModifiedById] INT NOT NULL DEFAULT 1,

	CONSTRAINT [FK_GroupTypes_CreatedById_Users_Id] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users]([Id]),
	CONSTRAINT [FK_GroupTypes_ModifiedById_Users_Id] FOREIGN KEY ([ModifiedById]) REFERENCES [dbo].[Users]([Id])
)
