/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_LogToTimeline] ******/
DROP PROCEDURE [dbo].[ToSIC_EAV_LogToTimeline]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_DeleteApp] ******/
DROP PROCEDURE [dbo].[ToSIC_EAV_DeleteApp]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogSet] ******/
DROP PROCEDURE [dbo].[ToSIC_EAV_ChangeLogSet]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogGet] ******/
DROP PROCEDURE [dbo].[ToSIC_EAV_ChangeLogGet]
GO


/****** Object:  StoredProcedure [dbo].[ToSIC_EAV_ChangeLogAdd] ******/
DROP PROCEDURE [dbo].[ToSIC_EAV_ChangeLogAdd]
GO


ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] DROP CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values]
GO


ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] DROP CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Entities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] DROP CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] DROP CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] DROP CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]
GO


ALTER TABLE [dbo].[ToSIC_EAV_AttributeGroups] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets]
GO


ALTER TABLE [dbo].[ToSIC_EAV_Apps] DROP CONSTRAINT [FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Zones] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Zones]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Zones]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_ValuesDimensions] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_ValuesDimensions]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Values] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Values]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_EntityRelationships] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_EntityRelationships]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Entities] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Entities]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Dimensions] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Dimensions]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_DataTimeline] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_DataTimeline]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_DataTimeline]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_ContextInfo] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ContextInfo]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_ContextInfo]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_ChangeLog] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ChangeLog]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_ChangeLog]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_AttributeTypes] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeTypes]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_AttributeTypes]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_AttributesInSets] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_AttributesInSets]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_AttributeSets] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_AttributeSets]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Attributes] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Attributes]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_AttributeGroups] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeGroups]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_AttributeGroups]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_AssignmentObjectTypes] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_AssignmentObjectTypes]
GO


/****** Object:  Table [dbo].[ToSIC_EAV_Apps] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Apps]') AND type in (N'U'))
DROP TABLE [dbo].[ToSIC_EAV_Apps]
GO
