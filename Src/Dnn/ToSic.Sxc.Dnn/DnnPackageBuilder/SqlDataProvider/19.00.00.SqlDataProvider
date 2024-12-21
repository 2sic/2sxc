SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- Drop the existing foreign key constraint
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships]
    DROP CONSTRAINT IF EXISTS [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes];
GO

-- Add the foreign key constraint with ON DELETE CASCADE
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships]
    ADD CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]
    FOREIGN KEY ([AttributeID])
    REFERENCES [dbo].[ToSIC_EAV_Attributes] ([AttributeID])
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
GO
