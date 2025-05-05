SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

PRINT '*** Starting migration script.';
GO

-- Preflight check 1: Ensure no AttributeID appears more than once in ToSIC_EAV_AttributesInSets
PRINT 'Running Preflight check 1: Checking for duplicate AttributeIDs in ToSIC_EAV_AttributesInSets';
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
PRINT 'Running Preflight check 2: Removing orphaned Attributes and related data';
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    -- 2.1 Find orphaned AttributeIDs
    PRINT '... Finding orphaned AttributeIDs';
    DECLARE @OrphanedAttributes TABLE (AttributeID INT);
    INSERT INTO @OrphanedAttributes (AttributeID)
    SELECT a.AttributeID
    FROM [dbo].[ToSIC_EAV_Attributes] a
    LEFT JOIN [dbo].[ToSIC_EAV_AttributesInSets] ais ON a.AttributeID = ais.AttributeID
    WHERE ais.AttributeID IS NULL;

    -- 2.2 Find ValueIDs related to orphaned AttributeIDs
    PRINT '... Finding related ValueIDs';
    DECLARE @OrphanedValueIDs TABLE (ValueID INT);
    INSERT INTO @OrphanedValueIDs (ValueID)
    SELECT v.ValueID
    FROM [dbo].[ToSIC_EAV_Values] v
    INNER JOIN @OrphanedAttributes oa ON v.AttributeID = oa.AttributeID;

    -- 2.3 Delete from ToSIC_EAV_ValuesDimensions (child of Values)
    PRINT '... Deleting orphaned ValuesDimensions';
    DELETE vd
    FROM [dbo].[ToSIC_EAV_ValuesDimensions] vd
    INNER JOIN @OrphanedValueIDs ov ON vd.ValueID = ov.ValueID;

    -- 2.4 Delete from ToSIC_EAV_Values (child of Attributes)
    PRINT '... Deleting orphaned Values';
    DELETE v
    FROM [dbo].[ToSIC_EAV_Values] v
    INNER JOIN @OrphanedValueIDs ov ON v.ValueID = ov.ValueID;

    -- 2.5 Delete from ToSIC_EAV_EntityRelationships (child of Attributes)
    PRINT '... Deleting orphaned EntityRelationships';
    DELETE er
    FROM [dbo].[ToSIC_EAV_EntityRelationships] er
    INNER JOIN @OrphanedAttributes oa ON er.AttributeID = oa.AttributeID;

    -- 2.6 Delete orphaned Attributes
    PRINT '... Deleting orphaned Attributes';
    DELETE a
    FROM [dbo].[ToSIC_EAV_Attributes] a
    INNER JOIN @OrphanedAttributes oa ON a.AttributeID = oa.AttributeID;
END
GO

-- Add new columns to ToSIC_EAV_Attributes if they do not exist yet.
PRINT 'Adding new columns (ContentTypeId, SortOrder, IsTitle) to ToSIC_EAV_Attributes';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'ContentTypeId' AND Object_ID = OBJECT_ID('ToSIC_EAV_Attributes'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] ADD
        [ContentTypeId] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_ContentTypeId DEFAULT (0),
        [SortOrder] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_SortOrder DEFAULT (0),
        [IsTitle] [bit] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_IsTitle DEFAULT (0);
END
GO

-- Migrate data from ToSIC_EAV_AttributesInSets to new columns in ToSIC_EAV_Attributes
PRINT 'Migrating data from ToSIC_EAV_AttributesInSets to ToSIC_EAV_Attributes';
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

-- Add Index on ContentTypeId if it doesn't exist
PRINT '... Adding index IX_ToSIC_EAV_Attributes_ContentTypeId';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_ContentTypeId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_ContentTypeId] ON [dbo].[ToSIC_EAV_Attributes] ([ContentTypeId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- Add foreign key constraint from ContentTypeId to AttributeSets.AttributeSetID if it does not exist
PRINT 'Adding foreign key FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets';
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes]
    ADD CONSTRAINT FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets
    FOREIGN KEY ([ContentTypeId]) REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID]);
END
GO

-- Remove Stored Procedures
PRINT 'Removing obsolete Stored Procedures';
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogSet]
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogGet]
GO
DROP PROCEDURE IF EXISTS [dbo].[ToSIC_EAV_ChangeLogAdd]
GO

-- Remove Tables
PRINT 'Removing obsolete Tables';
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_ContextInfo]
GO
DROP TABLE IF EXISTS [dbo].[ToSIC_EAV_AttributesInSets]
GO

-- Rename AssignmentObjectTypes to TsDynDataTargetType
PRINT 'Renaming table ToSIC_EAV_AssignmentObjectTypes to TsDynDataTargetType and related objects';

-- 1. Drop the referencing foreign key constraint on ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes')
BEGIN
    PRINT '... Dropping FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities]
    DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes];
END
GO

-- 2. Rename the index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AssignmentObjectTypes')
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_AssignmentObjectTypes to IX_TsDynDataTargetType_Name';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AssignmentObjectTypes].[IX_ToSIC_EAV_AssignmentObjectTypes]', N'IX_TsDynDataTargetType_Name', N'INDEX';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_AssignmentObjectTypes' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AssignmentObjectTypes]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_AssignmentObjectTypes to PK_TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_AssignmentObjectTypes]', N'PK_TsDynDataTargetType', N'OBJECT';
END
GO

-- 4. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_AssignmentObjectTypes' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_AssignmentObjectTypes to TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]', N'TsDynDataTargetType';
END
GO

-- 5. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTargetType]'))
BEGIN
    PRINT '... Renaming column AssignmentObjectTypeID to TargetTypeId in TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[TsDynDataTargetType].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 6. Rename the foreign key column in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming column AssignmentObjectTypeID to TargetTypeId in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 7. Recreate the foreign key constraint on ToSIC_EAV_Entities with the new names

-- Add Index on TargetTypeId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TargetTypeId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TargetTypeId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TargetTypeId] ON [dbo].[ToSIC_EAV_Entities] ([TargetTypeId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Entities_TsDynDataTargetType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType]
    FOREIGN KEY([TargetTypeId]) -- Use the new column name here
    REFERENCES [dbo].[TsDynDataTargetType] ([TargetTypeId]);
END
GO

-- 8. Check the constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
BEGIN
    PRINT '... Checking constraint FK_ToSIC_EAV_Entities_TsDynDataTargetType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType];
END
GO

-- Rename ToSIC_EAV_ChangeLog to TsDynDataTransaction
PRINT 'Renaming table ToSIC_EAV_ChangeLog to TsDynDataTransaction and related objects';

/*
-- 1. Drop referencing foreign keys on ToSIC_EAV_Attachments
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Dropping ChangeLogCreated foreign keys from ToSIC_EAV_Attachments';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] DROP CONSTRAINT [FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogCreated];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Dropping ChangeLogDeleted foreign keys from ToSIC_EAV_Attachments';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] DROP CONSTRAINT [FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogDeleted];
END
GO
*/

-- 2. Drop referencing foreign keys on ToSIC_EAV_Attributes
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Dropping ChangeLogCreated foreign keys from ToSIC_EAV_Attributes';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Dropping ChangeLogDeleted foreign keys from ToSIC_EAV_Attributes';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 3. Drop referencing foreign keys on ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Dropping ChangeLogCreated foreign keys from ToSIC_EAV_AttributeSets';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Dropping ChangeLogDeleted foreign keys from ToSIC_EAV_AttributeSets';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 4. Drop referencing foreign keys on ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Dropping ChangeLogCreated foreign keys from ToSIC_EAV_Entities';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified')
BEGIN
    PRINT '... Dropping ChangeLogModified foreign keys from ToSIC_EAV_Entities';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Dropping ChangeLogDeleted foreign keys from ToSIC_EAV_Entities';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 5. Drop referencing foreign keys on ToSIC_EAV_Values
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Dropping ChangeLogCreated foreign keys from ToSIC_EAV_Values';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified')
BEGIN
    PRINT '... Dropping ChangeLogModified foreign keys from ToSIC_EAV_Values';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Dropping ChangeLogDeleted foreign keys from ToSIC_EAV_Values';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 6. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_ChangeLog' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_ChangeLog]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_ChangeLog to PK_TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_ChangeLog]', N'PK_TsDynDataTransaction', N'OBJECT';
END
GO

-- 7. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_ChangeLog' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_ChangeLog to TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_ChangeLog]', N'TsDynDataTransaction';
END
GO

-- 8. Rename the primary key column in the renamed table
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTransaction]'))
BEGIN
    PRINT '... Renaming column ChangeID to TransactionId in TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[TsDynDataTransaction].[ChangeID]', N'TransactionId', N'COLUMN';
END
GO

-- 9. Rename the default constraint for the Timestamp column
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_ChangeLog_Timestamp' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataTransaction]'))
BEGIN
    PRINT '... Renaming default constraint DF_ToSIC_EAV_ChangeLog_Timestamp';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_ChangeLog_Timestamp]', N'DF_TsDynDataTransaction_Timestamp', N'OBJECT';
END
GO

-- 10. Rename the foreign key columns in the referencing table (ToSIC_EAV_Attachments)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attachments]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Attachments';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attachments].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attachments]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Attachments';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attachments].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 11. Rename the foreign key columns in the referencing table (ToSIC_EAV_Attributes)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 12. Rename the foreign key columns in the referencing table (ToSIC_EAV_AttributeSets)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 12b. Rename the foreign key columns in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogModified columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 14. Rename the foreign key columns in the referencing table (ToSIC_EAV_Values)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogModified columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

/*
-- 15. Recreate the foreign key constraints on ToSIC_EAV_Attachments with the new names
PRINT '... Recreating Transaction foreign keys on ToSIC_EAV_Attachments';

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attachments_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]'))
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attachments_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attachments_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Attachments] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] CHECK CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attachments_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attachments_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attachments_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Attachments] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attachments]', 'U') IS NOT NULL
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] CHECK CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted];
END
GO
*/

-- 16. Recreate the foreign key constraints on ToSIC_EAV_Attributes with the new names
PRINT '... Recreating Transaction foreign keys on ToSIC_EAV_Attributes';

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attributes_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Attributes] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]'))
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attributes_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Attributes] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted];
END
GO

-- 17. Recreate the foreign key constraints on ToSIC_EAV_AttributeSets with the new names
PRINT '... Recreating Transaction foreign keys on ToSIC_EAV_AttributeSets';

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_AttributeSets_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AttributeSets_TransactionIdCreated] ON [dbo].[ToSIC_EAV_AttributeSets] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_AttributeSets] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted];
END

-- 18. Recreate the foreign key constraints on ToSIC_EAV_Entities with the new names
PRINT '... Recreating Transaction foreign keys on ToSIC_EAV_Entities';

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdModified if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdModified] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified];
END

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted];
END
GO

-- 19. Recreate the foreign key constraints on ToSIC_EAV_Values with the new names
PRINT '... Recreating Transaction foreign keys on ToSIC_EAV_Values';

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdModified if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdModified] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified];
END
GO

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted];
END
GO

-- Rename ToSIC_EAV_DataTimeline to TsDynDataHistory and update related objects
PRINT 'Renaming table ToSIC_EAV_DataTimeline to TsDynDataHistory and related objects';

-- 1. Drop unused/obsolete columns
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SourceTextKey' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    PRINT '... Dropping obsolete column SourceTextKey from ToSIC_EAV_DataTimeline';
    ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] DROP COLUMN [SourceTextKey];
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'NewData' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    PRINT '... Dropping obsolete column NewData from ToSIC_EAV_DataTimeline';
    ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] DROP COLUMN [NewData];
END
GO

-- 2. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_DataTimeline' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_DataTimeline to PK_TsDynDataHistory';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_DataTimeline]', N'PK_TsDynDataHistory', N'OBJECT';
END
GO

-- 3. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_DataTimeline' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_DataTimeline to TsDynDataHistory';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_DataTimeline]', N'TsDynDataHistory';
END
GO

-- 4. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'Id' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column ID to HistoryId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[ID]', N'HistoryId', N'COLUMN';
END
GO

-- 5. Rename SysCreatedDate column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysCreatedDate' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SysCreatedDate to Timestamp in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysCreatedDate]', N'Timestamp', N'COLUMN';
END
GO

-- 6. Rename SourceID column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SourceID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SourceID to SourceId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SourceID]', N'SourceId', N'COLUMN';
END
GO

-- 7. Rename SysLogId column (the foreign key column)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysLogId' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SysLogId to TransactionId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysLogId]', N'TransactionId', N'COLUMN';
END
GO

-- 8. Clean up orphaned history entries before adding FK constraint
PRINT '... Cleaning up orphaned history entries in TsDynDataHistory';
DELETE hist
FROM [dbo].[TsDynDataHistory] hist
LEFT JOIN [dbo].[TsDynDataTransaction] trans ON hist.TransactionId = trans.TransactionId
WHERE trans.TransactionId IS NULL AND hist.TransactionId IS NOT NULL; -- Only delete if TransactionId was set but is now invalid
GO

-- 9. Recreate the foreign key constraint referencing TsDynDataTransaction

-- Add Index on TransactionId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_TransactionId' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
    AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_TransactionId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_TransactionId] ON [dbo].[TsDynDataHistory] ([TransactionId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
   AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataHistory_TsDynDataTransaction';
    ALTER TABLE [dbo].[TsDynDataHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction]
    FOREIGN KEY([TransactionId])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]); -- Use new referenced table/column names
END
GO

-- 10. Check the constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
BEGIN
    PRINT '... Checking constraint FK_TsDynDataHistory_TsDynDataTransaction';
    ALTER TABLE [dbo].[TsDynDataHistory] CHECK CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction];
END
GO

-- 11. Add Index on SourceId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_SourceId' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
    AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_SourceId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_SourceId] ON [dbo].[TsDynDataHistory]
    (
        [SourceId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 12. Add Index on SourceGuid if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_SourceGuid' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_SourceGuid';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_SourceGuid] ON [dbo].[TsDynDataHistory]
    (
        [SourceGuid] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- Rename ToSIC_EAV_Zones to TsDynDataZone and update related objects
PRINT 'Renaming table ToSIC_EAV_Zones to TsDynDataZone and related objects';

-- 1. Drop referencing foreign keys
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones')
BEGIN
    PRINT '... Dropping Zone foreign key from ToSIC_EAV_Apps';
    ALTER TABLE [dbo].[ToSIC_EAV_Apps] DROP CONSTRAINT [FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones')
BEGIN
    PRINT '... Dropping Zone foreign key from ToSIC_EAV_Dimensions';
    ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] DROP CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones];
END
GO

-- 2. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_Zones' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Zones]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_Zones to PK_TsDynDataZone';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_Zones]', N'PK_TsDynDataZone', N'OBJECT';
END
GO

-- 3. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Zones' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_Zones to TsDynDataZone';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Zones]', N'TsDynDataZone';
END
GO

-- 4. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in TsDynDataZone';
    EXEC sp_rename N'[dbo].[TsDynDataZone].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 5. Add new Transaction ID columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdCreated' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionId columns to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdCreated] INT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdModified' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdModified column to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdModified] INT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdDeleted' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdDeleted column to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdDeleted] INT NULL;
END
GO

-- 6. Rename the foreign key column in the referencing table (ToSIC_EAV_Apps)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Apps]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Apps].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 7. Rename the foreign key column in the referencing table (ToSIC_EAV_Dimensions)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in ToSIC_EAV_Dimensions';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Dimensions].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 8. Recreate the foreign key constraint on ToSIC_EAV_Apps with the new names

-- Add Index on ZoneId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Apps_ZoneId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Apps]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Apps]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Apps_ZoneId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Apps_ZoneId] ON [dbo].[ToSIC_EAV_Apps] ([ZoneId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Apps_TsDynDataZone')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Apps]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Apps_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Apps] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Apps_TsDynDataZone]
    FOREIGN KEY([ZoneId]) -- Use the new column name here
    REFERENCES [dbo].[TsDynDataZone] ([ZoneId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Apps_TsDynDataZone')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Apps_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Apps] CHECK CONSTRAINT [FK_ToSIC_EAV_Apps_TsDynDataZone];
END

-- 9. Recreate the foreign key constraint on ToSIC_EAV_Dimensions with the new names

-- Add Index on ZoneId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Dimensions_ZoneId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Dimensions_ZoneId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Dimensions_ZoneId] ON [dbo].[ToSIC_EAV_Dimensions] ([ZoneId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_TsDynDataZone')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Dimensions_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Dimensions_TsDynDataZone]
    FOREIGN KEY([ZoneId]) -- Use the new column name here
    REFERENCES [dbo].[TsDynDataZone] ([ZoneId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_TsDynDataZone')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_ToSIC_EAV_Dimensions_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_Dimensions_TsDynDataZone];
END
GO

-- 10. Recreate the foreign key constraints on TsDynDataZone with the new names

-- Add Index on TransactionIdCreated if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdCreated] ON [dbo].[TsDynDataZone] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataZone_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionCreated';  
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionCreated];
END
GO

-- Add Index on TransactionIdModified if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdModified] ON [dbo].[TsDynDataZone] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataZone_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionModified];
END
GO

-- Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdDeleted] ON [dbo].[TsDynDataZone] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataZone_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- Check the constraints
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking Transaction foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionDeleted];
END

PRINT '*** Finished migration script.';
GO
