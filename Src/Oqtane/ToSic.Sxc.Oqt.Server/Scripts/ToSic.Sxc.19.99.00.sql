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


-- Rename ToSIC_EAV_ChangeLog to TsDynDataTransaction
-- 1. Drop referencing foreign keys on ToSIC_EAV_Attachments
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] DROP CONSTRAINT [FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] DROP CONSTRAINT [FK_ToSIC_EAV_Attachments_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 2. Drop referencing foreign keys on ToSIC_EAV_Attributes
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] DROP CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 3. Drop referencing foreign keys on ToSIC_EAV_AttributeSets
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] DROP CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted];
END
GO

-- 4. Drop referencing foreign keys on ToSIC_EAV_Entities
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] DROP CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified];
END
GO

-- 5. Drop referencing foreign keys on ToSIC_EAV_Values
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] DROP CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified];
END
GO

-- 6. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_ChangeLog' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_ChangeLog]'))
BEGIN
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_ChangeLog]', N'PK_TsDynDataTransaction', N'OBJECT';
END
GO

-- 7. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_ChangeLog' AND type = 'U')
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_ChangeLog]', N'TsDynDataTransaction';
END
GO

-- 8. Rename the primary key column in the renamed table
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataTransaction]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataTransaction].[ChangeID]', N'TransactionId', N'COLUMN';
END
GO

-- 9. Rename the default constraint for the Timestamp column
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ToSIC_EAV_ChangeLog_Timestamp' AND parent_object_id = OBJECT_ID('[dbo].[TsDynDataTransaction]'))
BEGIN
    EXEC sp_rename N'[dbo].[DF_ToSIC_EAV_ChangeLog_Timestamp]', N'DF_TsDynDataTransaction_Timestamp', N'OBJECT';
END
GO

-- 10. Rename the foreign key columns in the referencing table (ToSIC_EAV_Attachments)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attachments]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attachments].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attachments]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attachments].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 11. Rename the foreign key columns in the referencing table (ToSIC_EAV_Attributes)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Attributes].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 12. Rename the foreign key columns in the referencing table (ToSIC_EAV_AttributeSets)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_AttributeSets].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO

-- 12b. Rename the foreign key columns in the referencing table (ToSIC_EAV_Entities)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Entities]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Entities].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

-- 14. Rename the foreign key columns in the referencing table (ToSIC_EAV_Values)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogCreated' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogCreated]', N'TransactionIdCreated', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogDeleted' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogDeleted]', N'TransactionIdDeleted', N'COLUMN';
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'ChangeLogModified' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_Values]'))
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_Values].[ChangeLogModified]', N'TransactionIdModified', N'COLUMN';
END
GO

-- 15. Recreate the foreign key constraints on ToSIC_EAV_Attachments with the new names
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_Attachments' AND type = 'U')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated')
    BEGIN
        ALTER TABLE [dbo].[ToSIC_EAV_Attachments] WITH CHECK 
        ADD CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated] 
        FOREIGN KEY([TransactionIdCreated])
        REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
    END
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted')
    BEGIN
        ALTER TABLE [dbo].[ToSIC_EAV_Attachments] WITH CHECK 
        ADD CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted] 
        FOREIGN KEY([TransactionIdDeleted])
        REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
    END
END
GO

-- 16. Recreate the foreign key constraints on ToSIC_EAV_Attributes with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated] 
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted] 
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 17. Recreate the foreign key constraints on ToSIC_EAV_AttributeSets with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated] 
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted] 
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 18. Recreate the foreign key constraints on ToSIC_EAV_Entities with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated] 
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted] 
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified] 
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 19. Recreate the foreign key constraints on ToSIC_EAV_Values with the new names
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated] 
    FOREIGN KEY([TransactionIdCreated])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted] 
    FOREIGN KEY([TransactionIdDeleted])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] WITH CHECK 
    ADD CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified] 
    FOREIGN KEY([TransactionIdModified])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]);
END
GO

-- 20. Check the constraints
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] CHECK CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attachments] CHECK CONSTRAINT [FK_ToSIC_EAV_Attachments_TsDynDataTransactionDeleted];
END
GO
--
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_TsDynDataTransactionDeleted];
END
GO
--
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_TsDynDataTransactionDeleted];
END
GO
--
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionDeleted];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Entities_TsDynDataTransactionModified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_TsDynDataTransactionModified];
END
GO
--
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionCreated')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionCreated];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionDeleted];
END
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ToSIC_EAV_Values_TsDynDataTransactionModified')
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_TsDynDataTransactionModified];
END
GO

-- Rename ToSIC_EAV_DataTimeline to TsDynDataHistory and update related objects
-- 1. Drop unused/obsolete columns
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SourceTextKey' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] DROP COLUMN [SourceTextKey];
END
GO
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'NewData' AND Object_ID = Object_ID(N'[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] DROP COLUMN [NewData];
END
GO

-- 2. Rename the primary key constraint
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ToSIC_EAV_DataTimeline' AND parent_object_id = OBJECT_ID('[dbo].[ToSIC_EAV_DataTimeline]'))
BEGIN
    EXEC sp_rename N'[dbo].[PK_ToSIC_EAV_DataTimeline]', N'PK_TsDynDataHistory', N'OBJECT';
END
GO

-- 3. Rename the table
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'ToSIC_EAV_DataTimeline' AND type = 'U')
BEGIN
    EXEC sp_rename N'[dbo].[ToSIC_EAV_DataTimeline]', N'TsDynDataHistory';
END
GO

-- 4. Rename the primary key column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'Id' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[ID]', N'HistoryId', N'COLUMN';
END
GO

-- 5. Rename SysCreatedDate column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysCreatedDate' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysCreatedDate]', N'Timestamp', N'COLUMN';
END
GO

-- 6. Rename SourceID column
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SourceID' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SourceID]', N'SourceId', N'COLUMN';
END
GO

-- 7. Rename SysLogId column (the foreign key column)
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'SysLogId' AND Object_ID = Object_ID(N'[dbo].[TsDynDataHistory]'))
BEGIN
    EXEC sp_rename N'[dbo].[TsDynDataHistory].[SysLogId]', N'TransactionId', N'COLUMN';
END
GO

-- 8. Clean up orphaned history entries before adding FK constraint
-- Delete rows from TsDynDataHistory where the TransactionId does not exist in TsDynDataTransaction
DELETE hist
FROM [dbo].[TsDynDataHistory] hist
LEFT JOIN [dbo].[TsDynDataTransaction] trans ON hist.TransactionId = trans.TransactionId
WHERE trans.TransactionId IS NULL AND hist.TransactionId IS NOT NULL; -- Only delete if TransactionId was set but is now invalid
GO

-- 9. Recreate the foreign key constraint referencing TsDynDataTransaction
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
BEGIN
    ALTER TABLE [dbo].[TsDynDataHistory] WITH CHECK 
    ADD CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction] 
    FOREIGN KEY([TransactionId])
    REFERENCES [dbo].[TsDynDataTransaction] ([TransactionId]); -- Use new referenced table/column names
END
GO

-- 10. Check the constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TsDynDataHistory_TsDynDataTransaction')
BEGIN
    ALTER TABLE [dbo].[TsDynDataHistory] CHECK CONSTRAINT [FK_TsDynDataHistory_TsDynDataTransaction];
END
GO

-- 11. Add Index on SourceId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_SourceId' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_SourceId] ON [dbo].[TsDynDataHistory]
    (
        [SourceId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

-- 12. Add Index on SourceGuid if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TsDynDataHistory_SourceGuid' AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_SourceGuid] ON [dbo].[TsDynDataHistory]
    (
        [SourceGuid] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

