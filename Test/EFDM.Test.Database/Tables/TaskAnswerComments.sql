CREATE TABLE [dbo].[TaskAnswerComments]
(	
	[Id] INT NOT NULL PRIMARY KEY,
	[Comment] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_TaskAnswerComments_Id_TaskAnswers_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[TaskAnswers]([Id])
)
