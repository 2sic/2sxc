SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- =====================================================================
-- TsDynDataHistory: add ParentRef so history can be grouped by app
-- ParentRef format example: "app-42"
-- =====================================================================

-- Add ParentRef column on existing installations
IF OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
    AND NOT EXISTS (
        SELECT *
        FROM sys.columns
        WHERE Name = N'ParentRef'
          AND Object_ID = OBJECT_ID(N'[dbo].[TsDynDataHistory]')
    )
BEGIN
    PRINT '... Adding column [ParentRef] to [dbo].[TsDynDataHistory]';
    ALTER TABLE [dbo].[TsDynDataHistory] ADD [ParentRef] [nvarchar](250) NULL;
END
GO

-- Add index for ParentRef
IF OBJECT_ID('[dbo].[TsDynDataHistory]', 'U') IS NOT NULL
    AND NOT EXISTS (
        SELECT *
        FROM sys.indexes
        WHERE name = 'IX_TsDynDataHistory_ParentRef'
          AND object_id = OBJECT_ID('[dbo].[TsDynDataHistory]')
    )
BEGIN
    PRINT '... Adding index IX_TsDynDataHistory_ParentRef';
    CREATE NONCLUSTERED INDEX [IX_TsDynDataHistory_ParentRef] ON [dbo].[TsDynDataHistory] ([ParentRef] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
GO
