using System;

namespace ToSic.Sxc.Oqt.Server.Context;

internal readonly record struct OqtTenantContextInfo(int TenantId, int SiteId, string ConnectionStringName, string ConnectionString);