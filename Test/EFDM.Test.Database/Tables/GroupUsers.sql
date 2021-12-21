CREATE TABLE [dbo].[GroupUsers] (
    [GroupId] INT NOT NULL,
    [UserId]  INT NOT NULL,
    CONSTRAINT [PK_GroupUsers] PRIMARY KEY CLUSTERED ([GroupId] ASC, [UserId] ASC),
    CONSTRAINT [FK_GroupUsers_GroupId_Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupUsers_UserId_Users_Id] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);