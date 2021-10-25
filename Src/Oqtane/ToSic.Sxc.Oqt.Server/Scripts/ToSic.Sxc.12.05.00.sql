SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Folder settings
UPDATE [dbo].[Folder] SET [IsSystem] = 0, [Type] = 'Private' WHERE [Path] LIKE 'adam%' AND [IsSystem] = 1
GO

-- Inser missing Browse permissions for All Users
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
	[SiteId] = f.[SiteId]
	,[EntityName] = 'Folder'
	,[FolderId] = f.[FolderId]
	,[PermissionName] = 'Browse'
	,[RoleId] = 1 -- All Users
	,[UserId] = null
	,[IsAuthorized] = 1
	,[CreatedBy] = f.[CreatedBy]
	,[CreatedOn] = f.[CreatedOn]
	,[ModifiedBy] = f.[ModifiedBy]
	,[ModifiedOn] = f.[ModifiedOn]
FROM 
	[dbo].[Folder] AS f
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'Browse'
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

-- Insert missing View permissions for All Users
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
	[SiteId] = f.[SiteId]
	,[EntityName] = 'Folder'
	,[FolderId] = f.[FolderId]
	,[PermissionName] = 'View'
	,[RoleId] = 1 -- All Users
	,[UserId] = null
	,[IsAuthorized] = 1
	,[CreatedBy] = f.[CreatedBy]
	,[CreatedOn] = f.[CreatedOn]
	,[ModifiedBy] = f.[ModifiedBy]
	,[ModifiedOn] = f.[ModifiedOn]
FROM 
	[dbo].[Folder] AS f
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'View'
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

-- Inser missing Browse permissions for Administrators
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
	[SiteId] = f.[SiteId]
	,[EntityName] = 'Folder'
	,[FolderId] = f.[FolderId]
	,[PermissionName] = 'Browse'
	,[RoleId] = r.RoleId
	,[UserId] = null
	,[IsAuthorized] = 1
	,[CreatedBy] = f.[CreatedBy]
	,[CreatedOn] = f.[CreatedOn]
	,[ModifiedBy] = f.[ModifiedBy]
	,[ModifiedOn] = f.[ModifiedOn]
FROM 
	[dbo].[Folder] AS f
	INNER JOIN [dbo].[Role] AS r ON f.SiteId = r.SiteId AND r.[Name] = 'Administrators'
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'Browse' AND r.RoleId = p.RoleId
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

-- Inser missing View permissions for Administrators
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
	[SiteId] = f.[SiteId]
	,[EntityName] = 'Folder'
	,[FolderId] = f.[FolderId]
	,[PermissionName] = 'View'
	,[RoleId] = r.RoleId
	,[UserId] = null
	,[IsAuthorized] = 1
	,[CreatedBy] = f.[CreatedBy]
	,[CreatedOn] = f.[CreatedOn]
	,[ModifiedBy] = f.[ModifiedBy]
	,[ModifiedOn] = f.[ModifiedOn]
FROM 
	[dbo].[Folder] AS f
	INNER JOIN [dbo].[Role] AS r ON f.SiteId = r.SiteId AND r.[Name] = 'Administrators'
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'View' AND r.RoleId = p.RoleId
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

-- Inser missing Edit permissions for Administrators
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
	[SiteId] = f.[SiteId]
	,[EntityName] = 'Folder'
	,[FolderId] = f.[FolderId]
	,[PermissionName] = 'Edit'
	,[RoleId] = r.RoleId
	,[UserId] = null
	,[IsAuthorized] = 1
	,[CreatedBy] = f.[CreatedBy]
	,[CreatedOn] = f.[CreatedOn]
	,[ModifiedBy] = f.[ModifiedBy]
	,[ModifiedOn] = f.[ModifiedOn]
FROM 
	[dbo].[Folder] AS f
	INNER JOIN [dbo].[Role] AS r ON f.SiteId = r.SiteId AND r.[Name] = 'Administrators'
	LEFT JOIN [dbo].[Permission] AS p ON p.[EntityId] = f.[FolderId] AND p.[EntityName] = 'Folder' AND p.[PermissionName] = 'Edit' AND r.RoleId = p.RoleId
WHERE
	f.[Path] LIKE 'adam%'
	AND p.[PermissionId] IS NULL
GO

