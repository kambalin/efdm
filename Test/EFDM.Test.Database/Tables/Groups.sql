CREATE TABLE [dbo].[Groups] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (150)    NULL,
    [IsDeleted]    BIT                NULL,
    [Created]      DATETIMEOFFSET (7) CONSTRAINT [DF_Group_Created] DEFAULT (sysdatetimeoffset()) NULL,
    [Modified]     DATETIMEOFFSET (7) CONSTRAINT [DF_Group_Modified] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedById]  INT                NULL,
    [ModifiedById] INT                NULL,
    [TypeId]       INT                NULL,
    [TextField1] NVARCHAR (150) NULL,
    [TextField2] NVARCHAR (150) NULL,
    [SubTypeId] TINYINT NOT NULL DEFAULT 255,	
    CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_Groups_GroupTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[GroupTypes] ([Id]),
    CONSTRAINT [FK_Groups_CreatedById_Users_Id] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([Id]),    
    CONSTRAINT [FK_Groups_ModifiedById_Users_Id] FOREIGN KEY ([ModifiedById]) REFERENCES [dbo].[Users] ([Id])
);