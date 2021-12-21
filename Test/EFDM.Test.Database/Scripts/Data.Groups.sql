﻿Set IDENTITY_INSERT [dbo].[Groups] ON

MERGE INTO [dbo].[Groups] AS [Target]
	USING (VALUES
		(1, N'Пользователи', GETDATE(), GETDATE(), 1, 1, 1, 0),		
		(2, N'Администраторы', GETDATE(), GETDATE(), 2, 1, 1, 0)
	) AS Source ([Id], [Title], [Created], [Modified], [TypeId], [CreatedById], [ModifiedById], [IsDeleted])
								
ON ([Target].[Id] = [Source].[Id])
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([Id], [Title], [Created], [Modified], [TypeId], [CreatedById], [ModifiedById], [IsDeleted])
		VALUES([Source].[Id], [Source].[Title], [Source].[Created], [Source].[Modified],  [Source].[TypeId], [Source].[CreatedById], [Source].[ModifiedById], [Source].[IsDeleted])
	--WHEN NOT MATCHED BY SOURCE THEN 
	--	DELETE
	WHEN MATCHED THEN UPDATE SET		
		[Target].[Title] = [Source].[Title],
		[Target].[TypeId] = [Source].[TypeId], 
		[Target].[IsDeleted] = [Source].[IsDeleted], 
		[Target].[Modified] = [Source].[Modified],
		[Target].[ModifiedById] = [Source].[ModifiedById];


Set IDENTITY_INSERT [dbo].[Groups] OFF