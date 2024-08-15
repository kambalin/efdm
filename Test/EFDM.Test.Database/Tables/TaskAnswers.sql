﻿CREATE TABLE [dbo].[TaskAnswers]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AnswerValue] DECIMAL(18, 2) NULL,
	[TextField1] NVARCHAR (150) NULL,
    [TextField2] NVARCHAR (150) NULL,
	[ValidFrom] DATETIMEOFFSET (0) NULL,
	[ValidTill] DATETIMEOFFSET (0) NULL,
)