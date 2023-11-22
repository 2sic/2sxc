SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- add [Guid], [SysSettings] columns to 'ToSIC_EAV_Attributes'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'Guid' AND Object_ID = OBJECT_ID('ToSIC_EAV_Attributes'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_Attributes] ADD [Guid] uniqueidentifier NULL,
    [SysSettings] nvarchar(MAX) NULL;
END
GO

-- add [SysSettings] column to 'ToSIC_EAV_AttributeSets'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'SysSettings' AND Object_ID = OBJECT_ID('ToSIC_EAV_AttributeSets'))
BEGIN
    ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] ADD [SysSettings] nvarchar(MAX) NULL;
END
GO
