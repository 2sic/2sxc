-- =====================================================================
-- Update legacy SettingName values (Eav*) to new names (TsDynData*)
-- =====================================================================

UPDATE Setting
SET SettingName = 'TsDynDataZoneId'
WHERE SettingName = 'EavZone';
GO

UPDATE Setting
SET SettingName = 'TsDynDataApp'
WHERE SettingName = 'EavApp';
GO

UPDATE Setting
SET SettingName = 'TsDynDataContentGroup'
WHERE SettingName = 'EavContentGroup';
GO

UPDATE Setting
SET SettingName = 'TsDynDataPreview'
WHERE SettingName = 'EavPreview';
GO


-- =====================================================================
-- Drop all tables and stored procedures in the old ToSIC EAV schema
-- =====================================================================

-- Stored procedures (no dependencies)
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_LogToTimeline];
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_DeleteApp];
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogSet];
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogGet];
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogAdd];
GO

/* 
   Tables drop order:
   - Drop most dependent children first.
   - Core parents (Apps, Zones, AttributeTypes, ChangeLog) last.
*/

-- Child of Values & Dimensions
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ValuesDimensions];      -- FK -> Values, Dimensions
GO

-- Child of Entities, Attributes, ChangeLog
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Values];                 -- FK -> Entities, Attributes, ChangeLog
GO

-- Child of Entities (parent/child) and Attributes
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_EntityRelationships];    -- FK -> Entities, Attributes
GO

-- Child of AttributeSets, Attributes, AttributeGroups
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributesInSets];       -- FK -> AttributeSets, Attributes, AttributeGroups
GO

-- Child of AttributeSets
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeGroups];        -- FK -> AttributeSets
GO

-- Child of AttributeSets, AssignmentObjectTypes, Apps, ChangeLog (also self-FK)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Entities];               -- FK -> AttributeSets, AssignmentObjectTypes, Apps, ChangeLog
GO

-- Child of Zones (also self-FK)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Dimensions];             -- FK -> Zones
GO

-- Not referenced in your FK list (drop early to be safe)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_DataTimeline];
GO

-- Not referenced in your FK list (drop early to be safe)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ContextInfo];
GO

-- Child of AttributeTypes and ChangeLog
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Attributes];             -- FK -> AttributeTypes, ChangeLog
GO

-- Child of Apps and ChangeLog (also self-FK)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeSets];          -- FK -> Apps, ChangeLog
GO

-- Parent of Entities
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AssignmentObjectTypes];  -- parent only
GO

-- Parent of Entities & AttributeSets
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Apps];                   -- parent only
GO

-- Parent of Dimensions
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_Zones];                  -- parent only
GO

-- Parent of Attributes
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributeTypes];         -- parent only
GO

-- Root parent of many (Values, Entities, AttributeSets, Attributes, …)
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ChangeLog];              -- drop last
GO
