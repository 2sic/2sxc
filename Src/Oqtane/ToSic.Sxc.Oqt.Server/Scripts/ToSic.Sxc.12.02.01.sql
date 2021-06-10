/* related to https://github.com/oqtane/oqtane.framework/issues/1269
Update column sizes to values that are introduced in Oqt 2.1.0
because our initil core modification fix in ToSic.Sxc.0.0.1.sql 
had different values in Oqt 2.0.0 - Oqt 2.0.2 */
BEGIN TRANSACTION
GO
IF COLUMNPROPERTY(OBJECT_ID('Folder', 'U'), 'Path', 'charmaxlen')=1000
ALTER TABLE dbo.Folder ALTER COLUMN [Path] nvarchar(512) NOT NULL
GO
IF COLUMNPROPERTY(OBJECT_ID('Folder', 'U'), 'Name', 'charmaxlen')=250
ALTER TABLE dbo.Folder ALTER COLUMN [Name] nvarchar(256) NOT NULL
GO
COMMIT
