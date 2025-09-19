-- TODO: @stv

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- make sure sql rolls back automatically in case of error.
SET XACT_ABORT ON
GO

-- =====================================================================
-- 1) Add TenantId, SiteId, AppBasePath, AppBaseSharedPath columns to TsDynDataZone table
-- =====================================================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'TenantId' AND Object_ID = OBJECT_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
  PRINT '... Adding TenantId';
  ALTER TABLE dbo.TsDynDataZone ADD TenantId INT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'SiteId' AND Object_ID = OBJECT_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
  PRINT '... Adding SiteId';
  ALTER TABLE dbo.TsDynDataZone ADD SiteId INT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AppBasePath' AND Object_ID = OBJECT_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
  PRINT '... Adding AppBasePath';
  ALTER TABLE dbo.TsDynDataZone ADD AppBasePath NVARCHAR(512) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AppBaseSharedPath' AND Object_ID = OBJECT_ID(N'[dbo].[TsDynDataZone]'))
BEGIN
  PRINT '... Adding AppBaseSharedPath';
  ALTER TABLE dbo.TsDynDataZone ADD AppBaseSharedPath NVARCHAR(512) NULL;
END

-- =====================================================================
-- 2) Populate values for new columns in TsDynDataZone table (in two passes)
-- =====================================================================

SET NOCOUNT ON;
BEGIN TRAN;

DECLARE @TenantId INT;
SET @TenantId = 1;

-- 2.1 Create a transaction and capture its id
DECLARE @TransactionId INT;
DECLARE @t TABLE (TransactionId INT);
INSERT INTO dbo.TsDynDataTransaction ([Timestamp],[User])
OUTPUT inserted.TransactionId INTO @t
VALUES (SYSUTCDATETIME(), 'dnn:userid=1');
SELECT @TransactionId = TransactionId FROM @t;

-- 2.2 Fill from PortalSettings (authoritative mapping ZoneId -> SiteId)
UPDATE z
SET
  TenantId          = ISNULL(z.TenantId, @TenantId),
  SiteId            = ISNULL(z.SiteId, m.PortalID),
  TransCreatedId    = ISNULL(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
JOIN dbo.PortalSettings m
  ON m.SettingName = 'TsDynDataZoneId'
 AND TRY_CAST(m.SettingValue AS INT) = z.ZoneId
WHERE
  z.TenantId IS NULL OR z.SiteId IS NULL;

-- 2.3 Fill remaining gaps by parsing "(Portal N)" from Name
UPDATE z
SET
  TenantId          = ISNULL(z.TenantId, @TenantId),
  SiteId            = ISNULL(z.SiteId, p.ParsedSiteId),
  TransCreatedId    = ISNULL(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
OUTER APPLY (
  -- Find "(Portal <digits>)" and extract the digits
  SELECT pos = PATINDEX('%(Portal [0-9]%)%', z.[Name])
) a
OUTER APPLY (
  SELECT ParsedSiteId =
    CASE WHEN a.pos > 0
         THEN TRY_CONVERT(
                INT,
                SUBSTRING(
                  z.[Name],
                  a.pos + 8,                                       -- after "(Portal "
                  CHARINDEX(')', z.[Name], a.pos) - (a.pos + 8)   -- until next ')'
                )
              )
    END
) p
WHERE
  (z.TenantId IS NULL OR z.SiteId IS NULL)
  AND p.ParsedSiteId IS NOT NULL;

-- 2.4 Final touch-up if SiteId exists
UPDATE z
SET
  TenantId          = ISNULL(z.TenantId, @TenantId),
  AppBasePath       = ISNULL(z.AppBasePath, CONCAT('/', ISNULL(p.HomeDirectory, CONCAT(N'/Portals/', z.SiteId)), '/2sxc')),
  AppBaseSharedPath = ISNULL(z.AppBaseSharedPath, '/Portals/_default/2sxc'),
  TransCreatedId    = ISNULL(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
LEFT JOIN dbo.Portals p
  ON p.PortalID = z.SiteId
WHERE
  z.SiteId IS NOT NULL
  AND (z.TenantId IS NULL OR z.AppBasePath IS NULL OR z.AppBaseSharedPath IS NULL);

COMMIT;
GO
