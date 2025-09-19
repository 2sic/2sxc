using System.Text;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Caching;

/// <summary>
/// Default cache key builder for Oqtane adapter. Deterministic and includes TenantSiteKey when provided.
/// </summary>
public class OqtCacheKeyBuilder : ICacheKeyBuilder
{
    public string Build(string @namespace, string[] parts, TenantSiteKey? key = null)
    {
        var sb = new StringBuilder();
        sb.Append(@namespace);
        sb.Append('|');

        if (key is TenantSiteKey k)
        {
            sb.Append(k.ToString()); // uses t:{TenantId}/s:{SiteId}
            sb.Append('|');
        }

        if (parts != null && parts.Length > 0)
        {
            for (var i = 0; i < parts.Length; i++)
            {
                if (i > 0) sb.Append('~');
                sb.Append(parts[i]);
            }
        }

        return sb.ToString();
    }
}
