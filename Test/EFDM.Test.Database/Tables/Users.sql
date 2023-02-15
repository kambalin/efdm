﻿CREATE TABLE [dbo].[Users] (
    [Id]           INT                IDENTITY (100, 1) NOT NULL,    
    [Login]        NVARCHAR (100)     NOT NULL,
    [Title]        NVARCHAR (255)     NOT NULL,
    [Email]        NVARCHAR (255)     NULL,
    [IsDeleted]    BIT                NOT NULL DEFAULT 0,    
    [Phone]        NVARCHAR (255)     NULL,
    [JobTitle]     NVARCHAR (1024)    NULL,
    [Department]   NVARCHAR (1024)    NULL,
    [Created]      DATETIMEOFFSET (7) CONSTRAINT [DF_Users_Created] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [Modified]     DATETIMEOFFSET (7) CONSTRAINT [DF_Users_Modified] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedById]  INT                NOT NULL DEFAULT 1,
    [ModifiedById] INT                NOT NULL DEFAULT 1,

    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] DESC),
    CONSTRAINT [FK_Users_CreatedBy_User] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Users_ModifiedBy_User] FOREIGN KEY ([ModifiedById]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [UI_Users_Login] UNIQUE ([Login]),
);