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
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    PRINT 'Running Preflight check 1: Checking for duplicate AttributeIDs in ToSIC_EAV_AttributesInSets';

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
    PRINT 'Running Preflight check 2: Removing orphaned Attributes and related data';

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

-- 1. Add new columns to ToSIC_EAV_Attributes if they do not exist yet.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'ContentTypeId' AND Object_ID = OBJECT_ID('ToSIC_EAV_Attributes'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding new columns (ContentTypeId, SortOrder, IsTitle) to ToSIC_EAV_Attributes';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] ADD
        [ContentTypeId] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_ContentTypeId DEFAULT (0),
        [SortOrder] [int] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_SortOrder DEFAULT (0),
        [IsTitle] [bit] NOT NULL CONSTRAINT DF_ToSIC_EAV_Attributes_IsTitle DEFAULT (0);
END
GO

-- 2. Migrate data from ToSIC_EAV_AttributesInSets to new columns in ToSIC_EAV_Attributes
IF OBJECT_ID('[dbo].[ToSIC_EAV_AttributesInSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Migrating data from ToSIC_EAV_AttributesInSets to ToSIC_EAV_Attributes';
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

-- 3. Add Index on ContentTypeId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_ContentTypeId' AND Object_ID = OBJECT_ID('ToSIC_EAV_Attributes'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attributes_ContentTypeId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_ContentTypeId] ON [dbo].[ToSIC_EAV_Attributes] ([ContentTypeId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 4. Add foreign key constraint from ContentTypeId to AttributeSets.AttributeSetID
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets' AND Object_ID = OBJECT_ID('ToSIC_EAV_AttributeSets'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding foreign key FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets';
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



-- *** Rename AssignmentObjectTypes to TsDynDataTargetType
PRINT 'Renaming table ToSIC_EAV_AssignmentObjectTypes to TsDynDataTargetType and related objects';

-- 1. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_AssignmentObjectTypes' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_AssignmentObjectTypes to TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]', N'TsDynDataTargetType';
END
GO

-- 2. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTargetType]'))
BEGIN
    PRINT '... Renaming column AssignmentObjectTypeID to TargetTypeId in TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[TsDynDataTargetType].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_AssignmentObjectTypes' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataTargetType]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_AssignmentObjectTypes to PK_TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_AssignmentObjectTypes]', N'PK_TsDynDataTargetType', N'OBJECT';
END
GO

-- 4. Rename the index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AssignmentObjectTypes')
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_AssignmentObjectTypes to IX_TsDynDataTargetType_Name';
    EXEC sp_rename N'[dbo].[TsDynDataTargetType].[IX_ToSIC_EAV_AssignmentObjectTypes]', N'IX_TsDynDataTargetType_Name', N'INDEX';
END
GO

-- 5. Rename the foreign key column in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignmentObjectTypeID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming column AssignmentObjectTypeID to TargetTypeId in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[AssignmentObjectTypeID]', N'TargetTypeId', N'COLUMN';
END
GO

-- 6. Rename foreign key constraint on ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]', N'FK_ToSIC_EAV_Entities_TsDynDataTargetType', N'OBJECT';
END
GO

-- 7. Add Index on TargetTypeId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TargetTypeId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TargetTypeId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TargetTypeId] ON [dbo].[ToSIC_EAV_Entities] ([TargetTypeId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 8. Recreate the foreign key constraint on ToSIC_EAV_Entities with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_ToSIC_EAV_Entities_TsDynDataTargetType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType]
    FOREIGN KEY([TargetTypeId])
    REFERENCES [dbo].[TsDynDataTargetType] ([TargetTypeId]);
END
GO

-- 9. Check the constraint FK_ToSIC_EAV_Entities_TsDynDataTargetType
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTargetType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTargetType];
END
GO



-- *** Rename ToSIC_EAV_ChangeLog to TsDynDataTransaction
PRINT 'Renaming table ToSIC_EAV_ChangeLog to TsDynDataTransaction and related objects';

-- 1. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_ChangeLog' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_ChangeLog to TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_ChangeLog]', N'TsDynDataTransaction';
END
GO

-- 2. Rename the primary key column in the renamed table
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTransaction]'))
BEGIN
    PRINT '... Renaming column ChangeID to TransactionId in TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[TsDynDataTransaction].[ChangeID]', N'TransactionId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_ChangeLog' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataTransaction]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_ChangeLog to PK_TsDynDataTransaction';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_ChangeLog]', N'PK_TsDynDataTransaction', N'OBJECT';
END
GO

-- 4. Rename the default constraint for the Timestamp column
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_ChangeLog_Timestamp' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataTransaction]'))
BEGIN
    PRINT '... Renaming default constraint DF_ToSIC_EAV_ChangeLog_Timestamp';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_ChangeLog_Timestamp]', N'DF_TsDynDataTransaction_Timestamp', N'OBJECT';
END
GO

-- 5. Rename the foreign key column ChangeLogCreated in the referencing table (ToSIC_EAV_Attributes)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

-- 6. Rename the foreign key column ChangeLogDeleted in the referencing table (ToSIC_EAV_Attributes)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 7. Rename the foreign key column ChangeLogCreated in the referencing table (ToSIC_EAV_AttributeSets)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

-- 8. Rename the foreign key column ChangeLogDeleted in the referencing table (ToSIC_EAV_AttributeSets)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 9. Rename the foreign key column ChangeLogCreated in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

-- 10. Rename the foreign key column ChangeLogModified in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogModified columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

-- 11. Rename the foreign key column ChangeLogDeleted in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 12. Rename the foreign key column ChangeLogCreated in the referencing table (ToSIC_EAV_Values)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogCreated columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO

-- 12b. Rename the foreign key column ChangeLogModified in the referencing table (ToSIC_EAV_Values)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogModified columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

-- 14. Rename the foreign key column ChangeLogDeleted in the referencing table (ToSIC_EAV_Values)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming ChangeLogDeleted columns in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 15. Rename the foreign key constraint FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Rename the foreign key constraint FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]', N'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 16. Add Index on IX_ToSIC_EAV_Attributes_TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attributes_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Attributes] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 17. Create the foreign key constraint FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 18. Check constraint FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated];
END
GO

-- 19. Rename the foreign key constraint FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Rename the foreign key constraint FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]', N'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 20. Add Index on IX_ToSIC_EAV_Attributes_TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]'))
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Attributes_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Attributes_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Attributes] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 21. Create the foreign key constraint FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Attributes]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 22. Check constraint FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted];
END
GO

-- 23. Rename the foreign key constraint FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]', N'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 24. Add Index on IX_ToSIC_EAV_AttributeSets_TransactionIdCreated 
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_AttributeSets_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AttributeSets_TransactionIdCreated] ON [dbo].[ToSIC_EAV_AttributeSets] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 25. Create foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 26. Check constraint FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated];
END
GO

-- 27. Rename the foreign key constraint FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]', N'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 28. Add Index on TransactionIdDeleted if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_AttributeSets] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 29. Create foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 30. Check constraint FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted];
END
GO

-- 31. Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]', N'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 32. Add Index IX_ToSIC_EAV_Entities_TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 33. Create foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 34. Check constraint FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated];
END
GO

-- 35. Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified')
BEGIN
    PRINT '... Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]', N'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified', N'OBJECT';
END
GO

-- 36. Add Index IX_ToSIC_EAV_Entities_TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdModified] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 37. Create foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionModified
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 38. Check constraint FK_ToSIC_EAV_Entities_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified];
END
GO

-- 39. Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]', N'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 40. Add Index IX_ToSIC_EAV_Entities_TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Entities] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 41. Create foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 42. Check constraint FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted];
END
GO

-- 43. Rename the foreign key constraint FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated')
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]', N'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 44. Add Index IX_ToSIC_EAV_Values_TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdCreated] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 45. Create foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 46. Check constraint FK_ToSIC_EAV_Values_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated];
END
GO

-- 47. Rename the foreign key constraint FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified')
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]', N'FK_ToSIC_EAV_Values_TsDynDataTransactionModified', N'OBJECT';
END
GO

-- 48. Add Index IX_ToSIC_EAV_Values_TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdModified] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 49. Create foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionModified
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 50. Check constraint FK_ToSIC_EAV_Values_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified];
END
GO

-- 51. Rename the foreign key constraint FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]', N'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 52. Add Index IX_ToSIC_EAV_Values_TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Values_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Values]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Values_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Values_TransactionIdDeleted] ON [dbo].[ToSIC_EAV_Values] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 53. Create foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Values]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 54. Check constraint FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted];
END
GO



-- *** Rename ToSIC_EAV_DataTimeline to TsDynDataHistory and update related objects
PRINT 'Renaming table ToSIC_EAV_DataTimeline to TsDynDataHistory and related objects';

-- 1. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_DataTimeline' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_DataTimeline to TsDynDataHistory';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_DataTimeline]', N'TsDynDataHistory';
END
GO

-- 2. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'Id' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column ID to HistoryId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[ID]', N'HistoryId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_DataTimeline' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_DataTimeline to PK_TsDynDataHistory';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_DataTimeline]', N'PK_TsDynDataHistory', N'OBJECT';
END
GO

-- 4. Drop unused/obsolete column SourceTextKey
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SourceTextKey' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Dropping obsolete column SourceTextKey';
    ALTER TABLE [dbo].[TsDynDataHistory] DROP COLUMN [SourceTextKey];
END
GO

-- 5. Drop unused/obsolete column NewData
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'NewData' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Dropping obsolete column NewData';
    ALTER TABLE [dbo].[TsDynDataHistory] DROP COLUMN [NewData];
END
GO

-- 6. Rename SysCreatedDate column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysCreatedDate' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SysCreatedDate to Timestamp in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysCreatedDate]', N'Timestamp', N'COLUMN';
END
GO

-- 7. Rename SourceID column
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'SourceID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SourceID to SourceId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SourceID]', N'SourceId', N'COLUMN';
END
GO

-- 8. Rename SysLogId column (the foreign key column)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysLogId' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    PRINT '... Renaming column SysLogId to TransactionId in TsDynDataHistory';
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysLogId]', N'TransactionId', N'COLUMN';
END
GO

-- 9. Clean up orphaned history entries before adding FK constraint
PRINT '... Cleaning up orphaned history entries in TsDynDataHistory';
DELETE hist
FROM [dbo].[TsDynDataHistory] hist
LEFT JOIN [dbo].[TsDynDataTransaction] trans ON hist.TransactionId = trans.TransactionId
WHERE trans.TransactionId IS NULL AND hist.TransactionId IS NOT NULL; -- Only delete if TransactionId was set but is now invalid
GO

-- 10. Add Index IX_TsDynDataHistory_TransactionId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_TransactionId' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
    AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_TransactionId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_TransactionId] ON [dbo].[TsDynDataHistory] ([TransactionId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 11. Create the foreign key constraint referencing TsDynDataTransaction
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
   AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataHistory_TsDynDataTransaction';
    ALTER TABLE [dbo].[TsDynDataHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction]
    FOREIGN KEY([TransactionId])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 12. Check the constraint FK_TsDynDataHistory_TsDynDataTransaction
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataHistory_TsDynDataTransaction';
    ALTER TABLE [dbo].[TsDynDataHistory] CHECK CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction];
END
GO

-- 12b. Add Index IX_TsDynDataHistory_SourceId
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

-- 14. Add Index IX_TsDynDataHistory_SourceGuid
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_SourceGuid' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
    AND OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_SourceGuid';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_SourceGuid] ON [dbo].[TsDynDataHistory]
    (
        [SourceGuid] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 15. Rename constraint DF_DataTimeline_Operation
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_DataTimeline_Operation')
BEGIN
    PRINT '... Renaming constraint DF_DataTimeline_Operation';
    EXEC sp_rename N'[dbo].[DF_DataTimeline_Operation]', N'DF_TsDynDataHistory_Operation', N'OBJECT';
END
GO



-- *** Rename ToSIC_EAV_Zones to TsDynDataZone and update related objects
PRINT 'Renaming table ToSIC_EAV_Zones to TsDynDataZone and related objects';

-- 1. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Zones' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_Zones to TsDynDataZone';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Zones]', N'TsDynDataZone';
END
GO

-- 2. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in TsDynDataZone';
    EXEC sp_rename N'[dbo].[TsDynDataZone].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint PK_ToSIC_EAV_Zones
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_Zones' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_Zones to PK_TsDynDataZone';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_Zones]', N'PK_TsDynDataZone', N'OBJECT';
END
GO

-- 4. Add new column TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdCreated' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdCreated column to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdCreated] INT NULL;
END
GO

-- 5. Add new column TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdModified' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdModified column to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdModified] INT NULL;
END
GO

-- 6. Add new column TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdDeleted' AND Object_ID = OBJECT_ID('TsDynDataZone'))
    AND OBJECT_ID('TsDynDataZone', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdDeleted column to TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataZone] ADD [TransactionIdDeleted] INT NULL;
END
GO

-- 7. Rename the foreign key column in the referencing table (ToSIC_EAV_Apps)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Apps]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Apps].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 8. Rename the foreign key column in the referencing table (ToSIC_EAV_Dimensions)
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'ZoneID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
BEGIN
    PRINT '... Renaming column ZoneID to ZoneId in ToSIC_EAV_Dimensions';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Dimensions].[ZoneID]', N'ZoneId', N'COLUMN';
END
GO

-- 9. Rename the foreign key constraint FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones')
BEGIN
    PRINT '... Renaming Zone foreign key from ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones]', N'FK_ToSIC_EAV_Apps_TsDynDataZone', N'OBJECT';
END
GO

-- 10. Rename the foreign key constraint FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones')
BEGIN
    PRINT '... Renaming Zone foreign key from ToSIC_EAV_Dimensions';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]', N'FK_ToSIC_EAV_Dimensions_TsDynDataZone', N'OBJECT';
END
GO

-- 11. Add Index IX_ToSIC_EAV_Dimensions_ZoneId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Dimensions_ZoneId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Dimensions_ZoneId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Dimensions_ZoneId] ON [dbo].[ToSIC_EAV_Dimensions] ([ZoneId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 12. Create the foreign key constraint referencing TsDynDataZone
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_TsDynDataZone')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Dimensions]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Dimensions_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Dimensions_TsDynDataZone]
    FOREIGN KEY([ZoneId])
    REFERENCES [dbo].[TsDynDataZone] ([ZoneId]);
END
GO

-- 12b. Check constraint FK_ToSIC_EAV_Dimensions_TsDynDataZone
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Dimensions_TsDynDataZone')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Dimensions_TsDynDataZone';
    ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_Dimensions_TsDynDataZone];
END
GO

-- 14. Add Index IX_TsDynDataZone_TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdCreated] ON [dbo].[TsDynDataZone] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 15. Create foreign key FK_TsDynDataZone_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataZone_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 16. Check constraint FK_TsDynDataZone_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionCreated';  
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionCreated];
END
GO

-- 17. Add Index IX_TsDynDataZone_TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdModified] ON [dbo].[TsDynDataZone] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 18. Create foreign key FK_TsDynDataZone_TsDynDataTransactionModified
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataZone_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 19. Check constraint FK_TsDynDataZone_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionModified];
END
GO

-- 20. Add Index IX_TsDynDataZone_TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataZone_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataZone]'))
    AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataZone_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataZone_TransactionIdDeleted] ON [dbo].[TsDynDataZone] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 21. Create foreign key FK_TsDynDataZone_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[TsDynDataZone]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataZone_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataZone] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 22. Check the constraints FK_TsDynDataZone_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataZone_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataZone_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataZone] CHECK CONSTRAINT [FK_TsDynDataZone_TsDynDataTransactionDeleted];
END



-- *** Rename ToSIC_EAV_Apps to TsDynDataApp and update related objects
PRINT 'Renaming table ToSIC_EAV_Apps to TsDynDataApp and related objects';

-- 1. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Apps' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_Apps to TsDynDataApp';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Apps]', N'TsDynDataApp';
END
GO

-- 2. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AppID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataApp]'))
BEGIN
    PRINT '... Renaming column AppID to AppId in TsDynDataApp';
    EXEC sp_rename N'[dbo].[TsDynDataApp].[AppID]', N'AppId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_Apps' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_Apps to PK_TsDynDataApp';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_Apps]', N'PK_TsDynDataApp', N'OBJECT';
END
GO

-- 4. Rename the unique constraint ToSIC_EAV_Apps_PreventDuplicates
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Apps_PreventDuplicates' AND type = 'UQ')
BEGIN
    PRINT '... Renaming unique constraint ToSIC_EAV_Apps_PreventDuplicates to UQ_TsDynDataApp_Name_ZoneId';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Apps_PreventDuplicates]', N'UQ_TsDynDataApp_Name_ZoneId', N'OBJECT';
END
GO

-- 5. Add new column TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdCreated' AND Object_ID = OBJECT_ID('TsDynDataApp'))
    AND OBJECT_ID('TsDynDataApp', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdCreated column to TsDynDataApp';
    ALTER TABLE [dbo].[TsDynDataApp] ADD [TransactionIdCreated] INT NULL;
END
GO

-- 6. Add new column TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdModified' AND Object_ID = OBJECT_ID('TsDynDataApp'))
    AND OBJECT_ID('TsDynDataApp', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdModified column to TsDynDataApp';
    ALTER TABLE [dbo].[TsDynDataApp] ADD [TransactionIdModified] INT NULL;
END
GO

-- 7. Add new column TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdDeleted' AND Object_ID = OBJECT_ID('TsDynDataApp'))
    AND OBJECT_ID('TsDynDataApp', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdDeleted column to TsDynDataApp';
    ALTER TABLE [dbo].[TsDynDataApp] ADD [TransactionIdDeleted] INT NULL;
END
GO

-- 8. Rename the foreign key column in the referencing table (ToSIC_EAV_AttributeSets)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AppID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    PRINT '... Renaming column AppID to AppId in ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[AppID]', N'AppId', N'COLUMN';
END
GO

-- 9. Rename the foreign key column in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AppID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming column AppID to AppId in ToSIC_EAV_Entities';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[AppID]', N'AppId', N'COLUMN';
END
GO

-- 10. Rename the foreign key constraint FK_ToSIC_EAV_Apps_TsDynDataZone
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Apps_TsDynDataZone')
BEGIN

    PRINT '... Renaming Zone foreign key from ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Apps_TsDynDataZone]', N'FK_ToSIC_EAV_Apps_TsDynDataApp', N'OBJECT';
END
GO

-- 11. Add Index IX_TsDynDataApp_ZoneId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataApp_ZoneId' AND object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataApp_ZoneId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataApp_ZoneId] ON [dbo].[TsDynDataApp] ([ZoneId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 12. Create the foreign key constraint FK_TsDynDataApp_TsDynDataZone
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataZone')
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataApp_TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataApp] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataApp_TsDynDataZone]
    FOREIGN KEY([ZoneId])
    REFERENCES [dbo].[TsDynDataZone] ([ZoneId]);
END
GO

-- 12b. Check constraint FK_TsDynDataApp_TsDynDataZone
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataZone')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataApp_TsDynDataZone';
    ALTER TABLE [dbo].[TsDynDataApp] CHECK CONSTRAINT [FK_TsDynDataApp_TsDynDataZone];
END
GO

-- 14. Rename the foreign key constraint FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps')
BEGIN
    PRINT '... Renaming foreign key FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]', N'FK_ToSIC_EAV_AttributeSets_TsDynDataApp', N'OBJECT';
END
GO

-- 15. Add Index IX_ToSIC_EAV_AttributeSets_AppId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_AppId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_AttributeSets_AppId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AttributeSets_AppId] ON [dbo].[ToSIC_EAV_AttributeSets] ([AppId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 16. Create the foreign key constraint FK_ToSIC_EAV_AttributeSets_TsDynDataApp
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataApp')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_AttributeSets]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_AttributeSets_TsDynDataApp';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataApp]
    FOREIGN KEY([AppId])
    REFERENCES [dbo].[TsDynDataApp] ([AppId]);
END
GO

-- 17. Check constraint FK_ToSIC_EAV_AttributeSets_TsDynDataApp
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataApp')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_AttributeSets_TsDynDataApp';
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataApp];
END
GO

-- 18. Rename the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps')
BEGIN
    PRINT '... Renaming foreign key FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]', N'FK_ToSIC_EAV_Entities_TsDynDataApp', N'OBJECT';
END
GO

-- 19. Add Index IX_ToSIC_EAV_Entities_AppId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_AppId' AND object_id = OBJECT_ID('[dbo].[ToSIC_EAV_Entities]'))
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_AppId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_AppId] ON [dbo].[ToSIC_EAV_Entities] ([AppId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 20. Create the foreign key constraint FK_ToSIC_EAV_Entities_TsDynDataApp
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataApp')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Entities_TsDynDataApp';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataApp]
    FOREIGN KEY([AppId])
    REFERENCES [dbo].[TsDynDataApp] ([AppId]);
END
GO

-- 21. Check constraint FK_ToSIC_EAV_Entities_TsDynDataApp
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataApp')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataApp';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataApp];
END
GO

-- 22. Add Index IX_TsDynDataApp_TransactionIdCreated
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataApp_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataApp_TransactionIdCreated';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataApp_TransactionIdCreated] ON [dbo].[TsDynDataApp] ([TransactionIdCreated] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 23. Create foreign key FK_TsDynDataApp_TsDynDataTransactionCreated
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionCreated')
   AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataApp_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[TsDynDataApp] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionCreated]
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 24. Check constraint FK_TsDynDataApp_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataApp_TsDynDataTransactionCreated';
    ALTER TABLE [dbo].[TsDynDataApp] CHECK CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionCreated];
END
GO

-- 25. Add Index IX_TsDynDataApp_TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataApp_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataApp_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataApp_TransactionIdModified] ON [dbo].[TsDynDataApp] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 26. Create foreign key FK_TsDynDataApp_TsDynDataTransactionModified
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataApp_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataApp] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 27. Check constraint FK_TsDynDataApp_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataApp_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataApp] CHECK CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionModified];
END
GO

-- 28. Add Index IX_TsDynDataApp_TransactionIdDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataApp_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataApp_TransactionIdDeleted';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataApp_TransactionIdDeleted] ON [dbo].[TsDynDataApp] ([TransactionIdDeleted] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 29. Create foreign key FK_TsDynDataApp_TsDynDataTransactionDeleted
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionDeleted')
   AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Recreating foreign key FK_TsDynDataApp_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataApp] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionDeleted]
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 30. Check the constraints FK_TsDynDataApp_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataApp_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataApp_TsDynDataTransactionDeleted';
    ALTER TABLE [dbo].[TsDynDataApp] CHECK CONSTRAINT [FK_TsDynDataApp_TsDynDataTransactionDeleted];
END
GO

-- 31. Rename index IX_ToSIC_EAV_Apps_ZoneId to IX_TsDynDataApp_ZoneId if exists
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Apps_ZoneId' AND object_id = OBJECT_ID('[dbo].[TsDynDataApp]'))
    AND OBJECT_ID('[dbo].[TsDynDataApp]', 'U') IS NOT NULL
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_Apps_ZoneId to IX_TsDynDataApp_ZoneId';
    EXEC sp_rename N'[dbo].[TsDynDataApp].[IX_ToSIC_EAV_Apps_ZoneId]', N'IX_TsDynDataApp_ZoneId', N'INDEX';
END
GO



-- *** Renaming table ToSIC_EAV_AttributeSets to TsDynDataContentType and update related objects
PRINT 'Renaming table ToSIC_EAV_AttributeSets to TsDynDataContentType and related objects';

-- 1. Rename table ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_AttributeSets' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_AttributeSets to TsDynDataContentType';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets]', N'TsDynDataContentType';
END
GO

-- 2. Rename PK column AttributeSetID → ContentTypeId
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AttributeSetID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming column AttributeSetID to ContentTypeId';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[AttributeSetID]', N'ContentTypeId', N'COLUMN';
END
GO

-- 3. Rename PK_ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_AttributeSets' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Rename PK_ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_AttributeSets]', N'PK_TsDynDataContentType', N'OBJECT';
END
GO

-- 4. Rename columns UsesConfigurationOfAttributeSet → InheritContentTypeId
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'UsesConfigurationOfAttributeSet' AND Object_ID = Object_ID(N'[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming column UsesConfigurationOfAttributeSet to InheritContentTypeId';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[UsesConfigurationOfAttributeSet]', N'InheritContentTypeId', N'COLUMN';
END
GO

-- 5. Rename columns AlwaysShareConfiguration → IsGlobal
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AlwaysShareConfiguration' AND Object_ID = Object_ID(N'[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming column AlwaysShareConfiguration to IsGlobal';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[AlwaysShareConfiguration]', N'IsGlobal', N'COLUMN';
END
GO

-- 6. Add column TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'TransactionIdModified' AND Object_ID = OBJECT_ID('TsDynDataContentType'))
    AND OBJECT_ID('TsDynDataContentType', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding TransactionIdModified column to TsDynDataContentType';
    ALTER TABLE [dbo].[TsDynDataContentType] ADD [TransactionIdModified] INT NULL;
END
GO

-- 7. Rename columns AttributeSetID → InheritContentTypeId
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'AttributeSetID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    PRINT '... Renaming column AttributeSetID to ContentTypeId';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[AttributeSetID]', N'ContentTypeId', N'COLUMN';
END
GO

-- 4. Rename FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]', N'FK_TsDynDataContentType_TsDynDataContentType' , N'OBJECT';
END
GO

-- 5. Rename FK_ToSIC_EAV_AttributeSets_TsDynDataApp
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataApp')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_AttributeSets_TsDynDataApp';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_TsDynDataApp]', N'FK_TsDynDataContentType_TsDynDataApp' , N'OBJECT';
END
GO

-- 6. Rename FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated]', N'FK_TsDynDataContentType_TsDynDataTransactionCreated' , N'OBJECT';
END
GO

-- 7. Rename FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted]', N'FK_TsDynDataContentType_TsDynDataTransactionDeleted' , N'OBJECT';
END
GO

-- 8. Rename FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_ContentTypeId_ToSIC_EAV_AttributeSets]', N'FK_ToSIC_EAV_Attributes_TsDynDataContentType' , N'OBJECT';
END
GO

-- 9. Rename FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]', N'FK_ToSIC_EAV_Entities_TsDynDataContentType' , N'OBJECT';
END
GO

-- 11. Add Index on ContentTypeId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_ContentTypeId')
    AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_ToSIC_EAV_Entities_ContentTypeId';
    CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_Entities_ContentTypeId] ON [dbo].[ToSIC_EAV_Entities] ([ContentTypeId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 12. Create foreign key FK_ToSIC_EAV_Entities_TsDynDataContentType
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataContentType')
   AND OBJECT_ID('[dbo].[ToSIC_EAV_Entities]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_ToSIC_EAV_Entities_TsDynDataContentType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH NOCHECK
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataContentType]
    FOREIGN KEY([ContentTypeId])
    REFERENCES [dbo].[TsDynDataContentType] ([ContentTypeId]);
END
GO

-- 12b. Check the constraints FK_ToSIC_EAV_Entities_TsDynDataContentType
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataContentType')
BEGIN
    PRINT '... Checking foreign key constraints on FK_ToSIC_EAV_Entities_TsDynDataContentType';
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataContentType];
END
GO

-- 14. Add Index IX_TsDynDataContentType_TransactionIdModified
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataContentType_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
    AND OBJECT_ID('[dbo].[TsDynDataContentType]', 'U') IS NOT NULL
BEGIN
    PRINT '... Adding index IX_TsDynDataContentType_TransactionIdModified';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataContentType_TransactionIdModified] ON [dbo].[TsDynDataContentType] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 15. Create foreign key FK_TsDynDataContentType_TsDynDataTransactionModified
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataContentType_TsDynDataTransactionModified')
   AND OBJECT_ID('[dbo].[TsDynDataContentType]', 'U') IS NOT NULL
BEGIN
    PRINT '... Creating foreign key FK_TsDynDataContentType_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataContentType] WITH NOCHECK
    ADD CONSTRAINT [FK_TsDynDataContentType_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 16. Check constraint FK_TsDynDataContentType_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataContentType_TsDynDataTransactionModified')
BEGIN
    PRINT '... Checking foreign key constraints on FK_TsDynDataContentType_TsDynDataTransactionModified';
    ALTER TABLE [dbo].[TsDynDataContentType] CHECK CONSTRAINT [FK_TsDynDataContentType_TsDynDataTransactionModified];
END
GO

-- 17. Rename index IX_ToSIC_EAV_AttributeSets_TransactionIdCreated
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_AttributeSets_TransactionIdCreated';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[IX_ToSIC_EAV_AttributeSets_TransactionIdCreated]', N'IX_TsDynDataContentType_TransactionIdCreated', N'INDEX';
END
GO

-- 18. Rename index IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[IX_ToSIC_EAV_AttributeSets_TransactionIdDeleted]', N'IX_TsDynDataContentType_TransactionIdDeleted', N'INDEX';
END
GO

-- 19. Rename index IX_ToSIC_EAV_AttributeSets_AppId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_AttributeSets_AppId' AND object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_AttributeSets_AppId';
    EXEC sp_rename N'[dbo].[TsDynDataContentType].[IX_ToSIC_EAV_AttributeSets_AppId]', N'IX_TsDynDataContentType_AppId', N'INDEX';
END
GO

-- 20. Add Index IX_TsDynDataContentType_AppId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataContentType_AppId' AND object_id = OBJECT_ID('[dbo].[TsDynDataContentType]'))
    AND OBJECT_ID('[dbo].[TsDynDataContentType]', 'U') IS NOT NULL 
BEGIN
    PRINT '... Adding index IX_TsDynDataContentType_AppId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataContentType_AppId] ON [dbo].[TsDynDataContentType] ([AppId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 21. Rename constraint DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration')
BEGIN
    PRINT '... Renaming constraint DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration]', N'DF_TsDynDataContentType_IsGlobal', N'OBJECT';
END
GO

-- 22. Rename constraint DF_ToSIC_EAV_AttributeSets_StaticName
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_AttributeSets_StaticName')
BEGIN
    PRINT '... Renaming constraint DF_ToSIC_EAV_AttributeSets_StaticName';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_AttributeSets_StaticName]', N'DF_TsDynDataContentType_StaticName', N'OBJECT';
END
GO



-- *** Rename table ToSIC_EAV_Attributes to TsDynDataAttribute and related objects
PRINT 'Renaming table ToSIC_EAV_Attributes to TsDynDataAttribute and related objects';

-- 1. Rename the table ToSIC_EAV_Attributes to TsDynDataAttribute
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Attributes' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_Attributes to TsDynDataAttribute';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes]', N'TsDynDataAttribute';
END
GO

-- 2. Rename the primary key column AttributeID to AttributeId in TsDynDataAttribute
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AttributeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming column AttributeID to AttributeId in TsDynDataAttribute';
    EXEC sp_rename N'[dbo].[TsDynDataAttribute].[AttributeID]', N'AttributeId', N'COLUMN';
END
GO

-- 3. Rename the primary key constraint PK_ToSIC_EAV_Attributes to PK_TsDynDataAttribute
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_Attributes' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_Attributes to PK_TsDynDataAttribute';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_Attributes]', N'PK_TsDynDataAttribute', N'OBJECT';
END
GO

-- 4. Add new column TransactionIdModified to TsDynDataAttribute
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'TransactionIdModified' AND Object_ID = Object_ID(N'[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Adding column TransactionIdModified to TsDynDataAttribute';
    ALTER TABLE [dbo].[TsDynDataAttribute] ADD [TransactionIdModified] INT NULL;
END
GO

-- 5. Rename FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types to FK_TsDynDataAttribute_ToSIC_EAV_AttributeTypes';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]', N'FK_TsDynDataAttribute_ToSIC_EAV_AttributeTypes', N'OBJECT';
END
GO

-- 6. Rename FK_ToSIC_EAV_Attributes_TsDynDataContentTyp
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataContentType' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Attributes_TsDynDataContentType to FK_TsDynDataAttribute_TsDynDataContentType';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_TsDynDataContentType]', N'FK_TsDynDataAttribute_TsDynDataContentType', N'OBJECT';
END
GO

-- 7. Rename FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated to FK_TsDynDataAttribute_TsDynDataTransactionCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated]', N'FK_TsDynDataAttribute_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 8. Rename FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted to FK_TsDynDataAttribute_TsDynDataTransactionDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted]', N'FK_TsDynDataAttribute_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 9. Add new Foreign Key for TransactionIdModified on TsDynDataAttribute
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataAttribute_TsDynDataTransactionModified' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Adding FK_TsDynDataAttribute_TsDynDataTransactionModified to TsDynDataAttribute';
    ALTER TABLE [dbo].[TsDynDataAttribute] WITH CHECK
    ADD CONSTRAINT [FK_TsDynDataAttribute_TsDynDataTransactionModified]
    FOREIGN KEY([TransactionIdModified]) REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 10. Renaming index IX_ToSIC_EAV_Attributes_ContentTypeId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_ContentTypeId' AND object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_Attributes_ContentTypeId to IX_TsDynDataAttribute_ContentTypeId';
    EXEC sp_rename N'[dbo].[TsDynDataAttribute].[IX_ToSIC_EAV_Attributes_ContentTypeId]', N'IX_TsDynDataAttribute_ContentTypeId', N'INDEX';
END
GO

-- 11. Renaming index IX_ToSIC_EAV_Attributes_TransactionIdCreated
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_Attributes_TransactionIdCreated to IX_TsDynDataAttribute_TransactionIdCreated';
    EXEC sp_rename N'[dbo].[TsDynDataAttribute].[IX_ToSIC_EAV_Attributes_TransactionIdCreated]', N'IX_TsDynDataAttribute_TransactionIdCreated', N'INDEX';
END
GO

-- 12. Renaming index IX_ToSIC_EAV_Attributes_TransactionIdDeleted
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Attributes_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming index IX_ToSIC_EAV_Attributes_TransactionIdDeleted to IX_TsDynDataAttribute_TransactionIdDeleted';
    EXEC sp_rename N'[dbo].[TsDynDataAttribute].[IX_ToSIC_EAV_Attributes_TransactionIdDeleted]', N'IX_TsDynDataAttribute_TransactionIdDeleted', N'INDEX';
END
GO

-- 12b. Add new Index for TransactionIdModified on TsDynDataAttribute
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataAttribute_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Adding index IX_TsDynDataAttribute_TransactionIdModified on TsDynDataAttribute';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataAttribute_TransactionIdModified] ON [dbo].[TsDynDataAttribute] ([TransactionIdModified] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 14. Rename Default Constraints DF_ToSIC_EAV_Attributes_ContentTypeId
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_Attributes_ContentTypeId' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming default constraint DF_ToSIC_EAV_Attributes_ContentTypeId to DF_TsDynDataAttribute_ContentTypeId';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_Attributes_ContentTypeId]', N'DF_TsDynDataAttribute_ContentTypeId', N'OBJECT';
END
GO

-- 15. Rename Default Constraints DF_ToSIC_EAV_Attributes_SortOrder
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_Attributes_SortOrder' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming default constraint DF_ToSIC_EAV_Attributes_SortOrder to DF_TsDynDataAttribute_SortOrder';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_Attributes_SortOrder]', N'DF_TsDynDataAttribute_SortOrder', N'OBJECT';
END
GO

-- 16. Rename Default Constraints DF_ToSIC_EAV_Attributes_IsTitle
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_Attributes_IsTitle' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming default constraint DF_ToSIC_EAV_Attributes_IsTitle to DF_TsDynDataAttribute_IsTitle';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_Attributes_IsTitle]', N'DF_TsDynDataAttribute_IsTitle', N'OBJECT';
END
GO

-- 17. Renaming column AttributeID to AttributeId in ToSIC_EAV_EntityRelationships
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AttributeID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
BEGIN
    PRINT '... Renaming column AttributeID to AttributeId in ToSIC_EAV_EntityRelationships';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_EntityRelationships].[AttributeID]', N'AttributeId', N'COLUMN';
END
GO

-- 18. Renaming column AttributeID to AttributeId in ToSIC_EAV_Values
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AttributeID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming column AttributeID to AttributeId in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[AttributeID]', N'AttributeId', N'COLUMN';
END
GO

-- 19. Rename FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]', N'FK_ToSIC_EAV_EntityRelationships_TsDynDataAttribute' , N'OBJECT';
END
GO

-- 20. Rename FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes')
BEGIN
    PRINT '... Rename FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]', N'FK_ToSIC_EAV_Values_TsDynDataAttribute' , N'OBJECT';
END
GO



-- *** Renaming table ToSIC_EAV_AttributeTypes to TsDynDataAttributeType and related objects 
PRINT 'Renaming table ToSIC_EAV_AttributeTypes to TsDynDataAttributeType and related objects';
GO

-- 1. Rename the table ToSIC_EAV_AttributeTypes to TsDynDataAttributeType
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_AttributeTypes' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_AttributeTypes to TsDynDataAttributeType';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeTypes]', N'TsDynDataAttributeType';
END
GO

-- 2. Rename the primary key constraint PK_ToSIC_EAV_AttributeTypes to PK_TsDynDataAttributeType
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_AttributeTypes' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttributeType]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_AttributeTypes to PK_TsDynDataAttributeType';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_AttributeTypes]', N'PK_TsDynDataAttributeType', N'OBJECT';
END
GO

-- 3. Rename the foreign key constraint in TsDynDataAttribute that references this table.
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataAttribute_ToSIC_EAV_AttributeTypes' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataAttribute]'))
BEGIN
    PRINT '... Renaming FK_TsDynDataAttribute_ToSIC_EAV_AttributeTypes to FK_TsDynDataAttribute_TsDynDataAttributeType';
    EXEC sp_rename N'[dbo].[FK_TsDynDataAttribute_ToSIC_EAV_AttributeTypes]', N'FK_TsDynDataAttribute_TsDynDataAttributeType', N'OBJECT';
END
GO



-- *** Renaming table ToSIC_EAV_Entities to TsDynDataEntity and related objects
PRINT '*** Renaming table ToSIC_EAV_Entities to TsDynDataEntity and related objects';

-- 1. Rename table ToSIC_EAV_Entities to TsDynDataEntity
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Entities' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_Entities to TsDynDataEntity';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities]', N'TsDynDataEntity';
END
GO

-- 2. Rename the primary key column EntityID to EntityId
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'EntityID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming PK column EntityID to EntityId in TsDynDataEntity';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[EntityID]', N'EntityId', N'COLUMN';
END
GO

-- 3. Rename column EntityGUID to EntityGuid
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'EntityGUID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming column EntityGUID to EntityGuid in TsDynDataEntity';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[EntityGUID]', N'EntityGuid', N'COLUMN';
END
GO

-- 4. Rename the primary key constraint PK_ToSIC_EAV_Entities to PK_TsDynDataEntity
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_Entities' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_Entities to PK_TsDynDataEntity';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_Entities]', N'PK_TsDynDataEntity', N'OBJECT';
END
GO

-- 5. Drop the foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities')
BEGIN
    PRINT '... Dropping foreign key constraint FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities';
    ALTER TABLE [dbo].[TsDynDataEntity] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities];
END
GO

-- 6. Drop the ConfigurationSet column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ConfigurationSet' AND Object_ID = Object_ID(N'[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Dropping column ConfigurationSet from TsDynDataEntity';
    ALTER TABLE [dbo].[TsDynDataEntity] DROP COLUMN [ConfigurationSet];
END
GO

-- 7. Rename the column EntityID to EntityId in ToSIC_EAV_Values
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'EntityID' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    PRINT '... Renaming column EntityID to EntityId in ToSIC_EAV_Values';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[EntityID]', N'EntityId', N'COLUMN';
END
GO

-- 8. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataApp to FK_TsDynDataEntity_TsDynDataApp
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataApp' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataApp to FK_TsDynDataEntity_TsDynDataApp';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataApp]', N'FK_TsDynDataEntity_TsDynDataApp', N'OBJECT';
END
GO

-- 9. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataContentType to FK_TsDynDataEntity_TsDynDataContentType
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataContentType' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataContentType to FK_TsDynDataEntity_TsDynDataContentType';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataContentType]', N'FK_TsDynDataEntity_TsDynDataContentType', N'OBJECT';
END
GO

-- 10. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTargetType to FK_TsDynDataEntity_TsDynDataTargetType
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTargetType' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTargetType to FK_TsDynDataEntity_TsDynDataTargetType';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataTargetType]', N'FK_TsDynDataEntity_TsDynDataTargetType', N'OBJECT';
END
GO

-- 11. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated to FK_TsDynDataEntity_TsDynDataTransactionCreated
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated to FK_TsDynDataEntity_TsDynDataTransactionCreated';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated]', N'FK_TsDynDataEntity_TsDynDataTransactionCreated', N'OBJECT';
END
GO

-- 12. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionModified to FK_TsDynDataEntity_TsDynDataTransactionModified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionModified to FK_TsDynDataEntity_TsDynDataTransactionModified';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataTransactionModified]', N'FK_TsDynDataEntity_TsDynDataTransactionModified', N'OBJECT';
END
GO

-- 12b. Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted to FK_TsDynDataEntity_TsDynDataTransactionDeleted
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted to FK_TsDynDataEntity_TsDynDataTransactionDeleted';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted]', N'FK_TsDynDataEntity_TsDynDataTransactionDeleted', N'OBJECT';
END
GO

-- 14. Renaming Default Constraint DF_ToSIC_EAV_Entities_EntityGUID to DF_TsDynDataEntity_EntityGuid
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_Entities_EntityGUID' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Default Constraint DF_ToSIC_EAV_Entities_EntityGUID to DF_TsDynDataEntity_EntityGuid';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_Entities_EntityGUID]', N'DF_TsDynDataEntity_EntityGuid', N'OBJECT';
END
GO

-- 15. Renaming Default Constraint DF_ToSIC_EAV_Entities_IsPublished to DF_TsDynDataEntity_IsPublished
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_Entities_IsPublished' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Default Constraint DF_ToSIC_EAV_Entities_IsPublished to DF_TsDynDataEntity_IsPublished';
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_Entities_IsPublished]', N'DF_TsDynDataEntity_IsPublished', N'OBJECT';
END
GO

-- 16. Renaming Index IX_KeyNumber to IX_TsDynDataEntity_KeyNumber
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KeyNumber' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_KeyNumber to IX_TsDynDataEntity_KeyNumber';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_KeyNumber]', N'IX_TsDynDataEntity_KeyNumber', N'INDEX';
END
GO

-- 17. Renaming Index IX_ToSIC_EAV_Entities_AppId to IX_TsDynDataEntity_AppId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_AppId' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_AppId to IX_TsDynDataEntity_AppId';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_AppId]', N'IX_TsDynDataEntity_AppId', N'INDEX';
END
GO

-- 18. Renaming Index IX_ToSIC_EAV_Entities_ContentTypeId to IX_TsDynDataEntity_ContentTypeId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_ContentTypeId' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_ContentTypeId to IX_TsDynDataEntity_ContentTypeId';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_ContentTypeId]', N'IX_TsDynDataEntity_ContentTypeId', N'INDEX';
END
GO

-- 19. Renaming Index IX_ToSIC_EAV_Entities_TargetTypeId to IX_TsDynDataEntity_TargetTypeId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TargetTypeId' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_TargetTypeId to IX_TsDynDataEntity_TargetTypeId';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_TargetTypeId]', N'IX_TsDynDataEntity_TargetTypeId', N'INDEX';
END
GO

-- 20. Renaming Index IX_ToSIC_EAV_Entities_TransactionIdCreated to IX_TsDynDataEntity_TransactionIdCreated
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdCreated' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_TransactionIdCreated to IX_TsDynDataEntity_TransactionIdCreated';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_TransactionIdCreated]', N'IX_TsDynDataEntity_TransactionIdCreated', N'INDEX';
END
GO

-- 21. Renaming Index IX_ToSIC_EAV_Entities_TransactionIdModified to IX_TsDynDataEntity_TransactionIdModified
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdModified' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_TransactionIdModified to IX_TsDynDataEntity_TransactionIdModified';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_TransactionIdModified]', N'IX_TsDynDataEntity_TransactionIdModified', N'INDEX';
END
GO

-- 22. Renaming Index IX_ToSIC_EAV_Entities_TransactionIdDeleted to IX_TsDynDataEntity_TransactionIdDeleted
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ToSIC_EAV_Entities_TransactionIdDeleted' AND object_id = OBJECT_ID('[dbo].[TsDynDataEntity]'))
BEGIN
    PRINT '... Renaming Index IX_ToSIC_EAV_Entities_TransactionIdDeleted to IX_TsDynDataEntity_TransactionIdDeleted';
    EXEC sp_rename N'[dbo].[TsDynDataEntity].[IX_ToSIC_EAV_Entities_TransactionIdDeleted]', N'IX_TsDynDataEntity_TransactionIdDeleted', N'INDEX';
END
GO



-- *** Rename ToSIC_EAV_EntityRelationships to TsDynDataRelationship and related objects
PRINT '*** Renaming table ToSIC_EAV_EntityRelationships to TsDynDataRelationship and related objects';

-- 1. Rename table ToSIC_EAV_EntityRelationships to TsDynDataRelationship
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_EntityRelationships' AND type = 'U')
BEGIN
    PRINT '... Renaming table ToSIC_EAV_EntityRelationships to TsDynDataRelationship';
    EXEC sp_rename N'[dbo].[ToSIC_EAV_EntityRelationships]', N'TsDynDataRelationship';
END
GO

-- 2. Rename column AttributeID to AttributeId
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'AttributeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming column AttributeID to AttributeId in TsDynDataRelationship';
    EXEC sp_rename N'[dbo].[TsDynDataRelationship].[AttributeID]', N'AttributeId', N'COLUMN';
END
GO

-- 3. Rename column ParentEntityID to ParentEntityId
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'ParentEntityID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming column ParentEntityID to ParentEntityId in TsDynDataRelationship';
    EXEC sp_rename N'[dbo].[TsDynDataRelationship].[ParentEntityID]', N'ParentEntityId', N'COLUMN';
END
GO

-- 4. Rename column ChildEntityID to ChildEntityId
IF EXISTS (SELECT * FROM sys.columns WHERE Name COLLATE Latin1_General_CS_AS = N'ChildEntityID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming column ChildEntityID to ChildEntityId in TsDynDataRelationship';
    EXEC sp_rename N'[dbo].[TsDynDataRelationship].[ChildEntityID]', N'ChildEntityId', N'COLUMN';
END
GO

-- 5. Rename the primary key constraint PK_ToSIC_EAV_EntityRelationships to PK_TsDynDataRelationship
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_EntityRelationships' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming PK_ToSIC_EAV_EntityRelationships to PK_TsDynDataRelationship';
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_EntityRelationships]', N'PK_TsDynDataRelationship', N'OBJECT';
END
GO

-- 6. Renaming FK FK_ToSIC_EAV_EntityRelationships_TsDynDataAttribute to FK_TsDynDataRelationship_TsDynDataAttribute
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_EntityRelationships_TsDynDataAttribute' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_EntityRelationships_TsDynDataAttribute to FK_TsDynDataRelationship_TsDynDataAttribute';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_EntityRelationships_TsDynDataAttribute]', N'FK_TsDynDataRelationship_TsDynDataAttribute', N'OBJECT';
END
GO

-- 7. Renaming FK FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities to FK_TsDynDataRelationship_TsDynDataEntityParent
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities to FK_TsDynDataRelationship_TsDynDataEntityParent';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]', N'FK_TsDynDataRelationship_TsDynDataEntityParent', N'OBJECT';
END
GO

-- 8. Renaming FK FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities to FK_TsDynDataRelationship_TsDynDataEntityChild
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
BEGIN
    PRINT '... Renaming FK FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities to FK_TsDynDataRelationship_TsDynDataEntityChild';
    EXEC sp_rename N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]', N'FK_TsDynDataRelationship_TsDynDataEntityChild', N'OBJECT';
END
GO

-- 9. Adding index IX_TsDynDataRelationship_ParentEntityId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataRelationship_ParentEntityId' AND object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
    AND OBJECT_ID('[dbo].[TsDynDataRelationship]', 'U') IS NOT NULL
    AND EXISTS (SELECT * FROM sys.columns WHERE Name = 'ParentEntityId' AND Object_ID = Object_ID(N'[dbo].[TsDynDataRelationship]')) -- Ensure column exists
BEGIN
    PRINT '... Adding index IX_TsDynDataRelationship_ParentEntityId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataRelationship_ParentEntityId] ON [dbo].[TsDynDataRelationship] ([ParentEntityId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO

-- 10. Adding index IX_TsDynDataRelationship_ChildEntityId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataRelationship_ChildEntityId' AND object_id = OBJECT_ID('[dbo].[TsDynDataRelationship]'))
    AND OBJECT_ID('[dbo].[TsDynDataRelationship]', 'U') IS NOT NULL
    AND EXISTS (SELECT * FROM sys.columns WHERE Name = 'ChildEntityId' AND Object_ID = Object_ID(N'[dbo].[TsDynDataRelationship]')) -- Ensure column exists
BEGIN
    PRINT '... Adding index IX_TsDynDataRelationship_ChildEntityId';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataRelationship_ChildEntityId] ON [dbo].[TsDynDataRelationship] ([ChildEntityId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO





PRINT '*** Finished migration script.';
GO
