SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- 1. remove sql trigger functionality that store data in 'ToSIC_EAV_DataTimeline' table 
-- min Sql Server 2016
DROP TRIGGER IF EXISTS [dbo].[AutoLogAllChangesToTimeline_EntityRelationships];
DROP TRIGGER IF EXISTS [dbo].[AutoLogAllChangesToTimeline_Values];
DROP TRIGGER IF EXISTS [dbo].[AutoLogAllChangesToTimeline_ValuesDimensions];
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_LogToTimeline]
GO

-- 2. remove trigger generated data from 'ToSIC_EAV_DataTimeline' in batches
WHILE (SELECT COUNT(*) FROM [dbo].[ToSIC_EAV_DataTimeline] WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')) > 0
BEGIN
    ;WITH CTE AS
    (
	SELECT TOP 100000 * 
	FROM [dbo].[ToSIC_EAV_DataTimeline] 
	WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')
	)
    DELETE FROM CTE;
END
GO

-- 3. drop NewData column from 'ToSIC_EAV_DataTimeline'
ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] DROP COLUMN IF EXISTS [NewData];
GO

-- 4. add CJson column to 'ToSIC_EAV_DataTimeline'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'CJson' AND Object_ID = OBJECT_ID('ToSIC_EAV_DataTimeline'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] ADD [CJson] varbinary(max) NULL;
END
GO

-- 5. remove columns that are not in use
ALTER TABLE [dbo].[ToSIC_EAV_AttributeGroups] DROP COLUMN IF EXISTS [SortOrder];
ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP COLUMN IF EXISTS [SortOrder];
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP COLUMN IF EXISTS [Description];
GO
