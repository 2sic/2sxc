SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

BEGIN TRANSACTION SexyContentUpdate;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ToSIC_EAV_Apps' AND COLUMN_NAME = 'SysSettings')
BEGIN
  ALTER TABLE ToSIC_EAV_Apps
    ADD SysSettings nvarchar(max) NULL
END

COMMIT TRANSACTION SexyContentUpdate;
--ROLLBACK TRANSACTION SexyContentUpdate;
GO

/* TargetTypes Metadata for ... */
UPDATE [dbo].[ToSIC_EAV_AssignmentObjectTypes] SET [Name] = 'Scope' ,[Description] = 'Metadata for Scope' WHERE [AssignmentObjectTypeID] = 7
UPDATE [dbo].[ToSIC_EAV_AssignmentObjectTypes] SET [Name] = 'Dimension' ,[Description] = 'Metadata for Dimension' WHERE [AssignmentObjectTypeID] = 8
GO
