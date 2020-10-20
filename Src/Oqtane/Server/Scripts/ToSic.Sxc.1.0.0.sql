/*  
Create ToSicSxc table
*/

CREATE TABLE [dbo].[ToSicSxc](
	[SxcId] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
  CONSTRAINT [PK_ToSicSxc] PRIMARY KEY CLUSTERED 
  (
	[SxcId] ASC
  )
)
GO

/*  
Create foreign key relationships
*/
ALTER TABLE [dbo].[ToSicSxc]  WITH CHECK ADD  CONSTRAINT [FK_ToSicSxc_Module] FOREIGN KEY([ModuleId])
REFERENCES [dbo].Module ([ModuleId])
ON DELETE CASCADE
GO