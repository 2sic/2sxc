SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- Preflight check 1: Ensure no AttributeID appears more than once in ToSIC_EAV_AttributesInSets
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    IF EXISTS (
        SELECT AttributeID
        FROM [dbo].[ToSIC_EAV_AttributesInSets]
        GROUP BY AttributeID
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 50001, 'Preflight check failed: Duplicate AttributeID found in ToSIC_EAV_AttributesInSets. Migration stopped.', 1;
        RETURN;
    END
END
GO

-- Preflight check 2: Remove orphaned Attributes and related data
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    -- 2.1 Find orphaned AttributeIDs
    DECLARE @OrphanedAttributes TABLE (AttributeID INT);
    INSERT INTO @OrphanedAttributes (AttributeID)
    SELECT a.AttributeID
    FROM [dbo].[ToSIC_EAV_Attributes] a
    LEFT JOIN [dbo].[ToSIC_EAV_AttributesInSets] ais ON a.AttributeID = ais.AttributeID
    WHERE ais.AttributeID IS NULL;

    -- 2.2 Find ValueIDs related to orphaned AttributeIDs
    DECLARE @OrphanedValueIDs TABLE (ValueID INT);
    INSERT INTO @OrphanedValueIDs (ValueID)
    SELECT v.ValueID
    FROM [dbo].[ToSIC_EAV_Values] v
    INNER JOIN @OrphanedAttributes oa ON v.AttributeID = oa.AttributeID;

    -- 2.3 Delete from ToSIC_EAV_ValuesDimensions (child of Values)
    DELETE vd
    FROM [dbo].[ToSIC_EAV_ValuesDimensions] vd
    INNER JOIN @OrphanedValueIDs ov ON vd.ValueID = ov.ValueID;

    -- 2.4 Delete from ToSIC_EAV_Values (child of Attributes)
    DELETE v
    FROM [dbo].[ToSIC_EAV_Values] v
    INNER JOIN @OrphanedValueIDs ov ON v.ValueID = ov.ValueID;

    -- 2.5 Delete from ToSIC_EAV_EntityRelationships (child of Attributes)
    DELETE er
    FROM [dbo].[ToSIC_EAV_EntityRelationships] er
    INNER JOIN @OrphanedAttributes oa ON er.AttributeID = oa.AttributeID;

    -- 2.6 Delete orphaned Attributes
    DELETE a
    FROM [dbo].[ToSIC_EAV_Attributes] a
    INNER JOIN @OrphanedAttributes oa ON a.AttributeID = oa.AttributeID;
END
GO


-- Add new columns to ToSIC_EAV_Attributes if they do not exist yet.
-- These columns will store data previously held in ToSIC_EAV_AttributesInSets.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'ContentTypeId' AND Object_ID = OBJECT_ID('ToSIC_EAV_Attributes'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] ADD 
        [ContentTypeId] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_ContentTypeId DEFAULT (0),
        [SortOrder] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_SortOrder DEFAULT (0),
        [IsTitle] [bit] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_IsTitle DEFAULT (0);
END
GO

-- Migrate data from ToSIC_EAV_AttributesInSets to new columns in ToSIC_EAV_Attributes
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    UPDATE a
    SET 
        a.ContentTypeId = ais.AttributeSetID,
        a.SortOrder = ais.SortOrder,
        a.IsTitle = ais.IsTitle
    FROM [dbo].[ToSIC_EAV_Attributes] a
    INNER JOIN [dbo].[ToSIC_EAV_AttributesInSets] ais
        ON a.AttributeID = ais.AttributeID;
END
GO

-- Add foreign key constraint from ContentTypeId to AttributeSets.AttributeSetID if it does not exist
IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets'
)
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes]
    ADD CONSTRAINT FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets
    FOREIGN KEY ([ContentTypeId]) REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID]);
END
GO

-- Remove Stored Procedures
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogSet]
GO

DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogGet]
GO

DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogAdd]
GO

-- Remove Tables
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ContextInfo]
GO

DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributesInSets]
GO

-- Rename AssignmentObjectTypes to TsDynDataTargetType
-- 1. Drop the referencing foreign key constraint on ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] 
    DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes];
END
GO

-- 2. Rename the index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AssignmentObjectTypes' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AssignmentObjectTypes]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AssignmentObjectTypes].[IX_ToSIC_EAV_AssignmentObjectTypes]', N'IX_TsDynDataTargetType_Name', N'INDEX';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_AssignmentObjectTypes' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AssignmentObjectTypes]'))
BEGIN
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_AssignmentObjectTypes]', N'PK_TsDynDataTargetType', N'OBJECT';
END
GO

-- 4. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_AssignmentObjectTypes' AND type = 'U')
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]', N'TsDynDataTargetType';
END
GO

-- 5. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTargetType]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataTargetType].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 6. Rename the foreign key column in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 7. Recreate the foreign key constraint on ToSIC_EAV_Entities with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType] 
    FOREIGN KEY([TargetTypeId]) -- Use the new column name here
    REFERENCES [dbo].[TsDynDataTargetType] ([TargetTypeId]);
END
GO

-- 8. Check the constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType];
END
GO
