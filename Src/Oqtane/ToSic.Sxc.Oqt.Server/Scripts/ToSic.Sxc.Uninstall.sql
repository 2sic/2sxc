-- Remove Tables

DROP TABLE IF EXISTS [dbo].[TsDynDataRelationship]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataValueDimension]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataDimension]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataValue]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataEntity]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataHistory]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataAttribute]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataAttributeType]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataContentType]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataTargetType]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataApp]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataZone]
GO

DROP TABLE IF EXISTS [dbo].[TsDynDataTransaction]
GO

DELETE FROM ModuleDefinition WHERE ModuleDefinitionName LIKE 'ToSic.Sxc.Oqt.%, ToSic.Sxc.Oqtane.Client'
GO

DELETE FROM __EFMigrationsHistory WHERE MigrationId LIKE 'ToSic.Sxc.%'
GO

DELETE FROM Setting WHERE SettingName IN ('TsDynDataZoneId', 'TsDynDataApp', 'TsDynDataContentGroup', 'TsDynDataPreview', 'EavZone', 'EavApp', 'EavContentGroup', 'EavPreview')
GO
