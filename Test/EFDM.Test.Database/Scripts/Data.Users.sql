SET IDENTITY_INSERT [Users] ON

MERGE INTO [Users] AS [Target]
	USING (VALUES
		(1, N'efdm\system', N'SYSTEM')
	) AS Source ([Id], [Login], [Title])

ON ([Target].[Id] = [Source].[Id])
	-- adding
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT ([Id], [Login], [Title], [IsDeleted], [Created], [Modified], [CreatedById], [ModifiedById])
		VALUES([Source].[Id], [Source].[Login], [Source].[Title], 0, GETDATE(), GETDATE(), 1, 1)
	-- deleting
	WHEN NOT MATCHED BY SOURCE and [Target].[Id] < 100 THEN UPDATE SET
		[Target].[IsDeleted] = 1,
		[Target].[Modified] = GETDATE()
	-- updating
	WHEN MATCHED THEN UPDATE SET 	
		[Target].[Login] = [Source].[Login],
		[Target].[Title] = [Source].[Title],
		[Target].[Modified] = GETDATE();

SET IDENTITY_INSERT [Users] OFF
