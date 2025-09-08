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
GO


-- =====================================================================
-- 2) Populate values for new columns in TsDynDataZone table
-- =====================================================================

SET NOCOUNT ON;
BEGIN TRAN;

-- Compute a default TenantId fallback (useful if some Site lookups fail)
DECLARE @DefaultTenantId INT;
SELECT TOP (1) @DefaultTenantId = TenantId FROM dbo.Site;

-- 2.1 Create a transaction and capture its id
DECLARE @TransactionId INT;
DECLARE @t TABLE (TransactionId INT);
INSERT INTO dbo.TsDynDataTransaction ([Timestamp],[User])
OUTPUT inserted.TransactionId INTO @t
VALUES (SYSUTCDATETIME(), 'oqt:1');
SELECT @TransactionId = TransactionId FROM @t;

-- 2.2 Fill from Setting (authoritative mapping ZoneId -> SiteId)
UPDATE z
SET
  TenantId          = COALESCE(z.TenantId, s.TenantId, @DefaultTenantId),
  SiteId            = COALESCE(z.SiteId, m.EntityId),
  TransCreatedId    = COALESCE(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
JOIN dbo.Setting m
  ON m.EntityName  = 'Site'
 AND m.SettingName = 'TsDynDataZoneId'
 AND TRY_CAST(m.SettingValue AS INT) = z.ZoneId
LEFT JOIN dbo.Site s
  ON s.SiteId = m.EntityId
WHERE
  z.TenantId IS NULL OR z.SiteId IS NULL;

-- 2.3 Fill remaining gaps by parsing "(Site N)" from Name
UPDATE z
SET
  TenantId          = COALESCE(z.TenantId, s2.TenantId, @DefaultTenantId),
  SiteId            = COALESCE(z.SiteId, p.ParsedSiteId),
  TransCreatedId    = COALESCE(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
OUTER APPLY (
  -- Find "(Site <digits>)" and extract the digits
  SELECT pos = PATINDEX('%(Site [0-9]%)%', z.[Name])
) a
OUTER APPLY (
  SELECT ParsedSiteId =
    CASE WHEN a.pos > 0
         THEN TRY_CONVERT(
                INT,
                SUBSTRING(
                  z.[Name],
                  a.pos + 6,                                       -- after "(Site "
                  CHARINDEX(')', z.[Name], a.pos) - (a.pos + 6)   -- until next ')'
                )
              )
    END
) p
LEFT JOIN dbo.Site s2
  ON s2.SiteId = p.ParsedSiteId
WHERE
  (z.TenantId IS NULL OR z.SiteId IS NULL)
  AND p.ParsedSiteId IS NOT NULL;

-- 2.4 Final touch-up if SiteId exists
UPDATE z
SET
  TenantId          = COALESCE(z.TenantId, s3.TenantId, @DefaultTenantId),
  AppBasePath       = COALESCE(z.AppBasePath, CONCAT('/2sxc/', z.SiteId)),
  AppBaseSharedPath = COALESCE(z.AppBaseSharedPath, '/2sxc/Shared'),
  TransCreatedId    = COALESCE(z.TransCreatedId, @TransactionId),
  TransModifiedId   = @TransactionId
FROM dbo.TsDynDataZone z
LEFT JOIN dbo.Site s3
  ON s3.SiteId = z.SiteId
WHERE
  z.SiteId IS NOT NULL
  AND (z.TenantId IS NULL OR z.AppBasePath IS NULL OR z.AppBaseSharedPath IS NULL);

COMMIT;
GO
