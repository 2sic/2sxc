SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Folder settings
UPDATE [dbo].[Folder] SET [IsSystem] = 0, [Type] = 'Private' WHERE [Path] LIKE 'adam%' AND [IsSystem] = 1
GO

-- Insert missing View permissions
INSERT INTO [dbo].[Permission]
	([SiteId]
	,[EntityName]
	,[EntityId]
	,[PermissionName]
	,[RoleId]
	,[UserId]
	,[IsAuthorized]
	,[CreatedBy]
	,[CreatedOn]
	,[ModifiedBy]
	,[ModifiedOn])
SELECT 
	[SiteId] = COALESCE(p.[SiteId], f.[SiteId])
	,[EntityName] = COALESCE(p.[EntityName], 'Folder')
	,[FolderId] = COALESCE(p.[EntityId], f.[FolderId])
	,[PermissionName] = COALESCE(p.[PermissionName], 'View')
	,[RoleId] = COALESCE(p.[RoleId],pv.[RoleId])
	,[UserId] = COALESCE(p.[UserId],pv.[UserId])
	,[IsAuthorized] = COALESCE(p.[IsAuthorized], pv.[IsAuthorized], 1)
	,[CreatedBy] = COALESCE(p.[CreatedBy], pv.[CreatedBy] ,f.[CreatedBy])
	,[CreatedOn] = COALESCE(p.[CreatedOn], pv.[CreatedOn], f.[CreatedOn])
	,[ModifiedBy] = COALESCE(p.[ModifiedBy], pv.[ModifiedBy], f.[ModifiedBy])
	,[ModifiedOn] = COALESCE(p.[ModifiedOn], pv.[ModifiedOn], f.[ModifiedOn])
FROM 
	[dbo].[Folder] AS f
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'View'
	LEFT JOIN [dbo].[Permission] AS pv ON pv.[EntityId] = f.[FolderId] AND pv.[EntityName] = 'Folder' AND pv.[PermissionName] = 'Browse'
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

-- Inser missing Browse permissions
INSERT INTO [dbo].[Permission]
	([SiteId]
	,[EntityName]
	,[EntityId]
	,[PermissionName]
	,[RoleId]
	,[UserId]
	,[IsAuthorized]
	,[CreatedBy]
	,[CreatedOn]
	,[ModifiedBy]
	,[ModifiedOn])
SELECT 
	[SiteId] = COALESCE(p.[SiteId], f.[SiteId])
	,[EntityName] = COALESCE(p.[EntityName], 'Folder')
	,[FolderId] = COALESCE(p.[EntityId], f.[FolderId])
	,[PermissionName] = COALESCE(p.[PermissionName], 'Browse')
	,[RoleId] = COALESCE(p.[RoleId],pv.[RoleId])
	,[UserId] = COALESCE(p.[UserId],pv.[UserId])
	,[IsAuthorized] = COALESCE(p.[IsAuthorized], pv.[IsAuthorized], 1)
	,[CreatedBy] = COALESCE(p.[CreatedBy], pv.[CreatedBy] ,f.[CreatedBy])
	,[CreatedOn] = COALESCE(p.[CreatedOn], pv.[CreatedOn], f.[CreatedOn])
	,[ModifiedBy] = COALESCE(p.[ModifiedBy], pv.[ModifiedBy], f.[ModifiedBy])
	,[ModifiedOn] = COALESCE(p.[ModifiedOn], pv.[ModifiedOn], f.[ModifiedOn])
FROM 
	[dbo].[Folder] AS f
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'Browse'
	LEFT JOIN [dbo].[Permission] AS pv ON pv.[EntityId] = f.[FolderId] AND pv.[EntityName] = 'Folder' AND pv.[PermissionName] = 'View'
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO
