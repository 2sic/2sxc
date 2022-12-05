/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_LogToTimeline] ******/
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_LogToTimeline]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_DeleteApp] ******/
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_DeleteApp]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogSet] ******/
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogSet]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogGet] ******/
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogGet]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogAdd] ******/
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogAdd]
GO


ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values]
GO


ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Values_ToSIC_EAV_Entities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeGroups] DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Zones]
GO

DROP TABLE IF EXISTS  [dbo].[ToSIC_EAV_ValuesDimensions]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Values]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_EntityRelationships]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Entities]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Dimensions]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_DataTimeline]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ContextInfo]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ChangeLog]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeTypes]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributesInSets]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeSets]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Attributes]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeGroups]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AssignmentObjectTypes]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Apps]
GO
