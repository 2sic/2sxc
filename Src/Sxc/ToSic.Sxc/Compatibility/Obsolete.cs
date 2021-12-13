using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Obsolete;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility
{
    public class Obsolete
    {
        public static ILog Warning12To14(string obsoleteId, string specificId, string link, Action<ILog> addMore = null)
        {
            var obsolete = new LogObsolete(obsoleteId, specificId, "v12",
                "v14 ca. middle of 2022", link, addMore);
            return obsolete.Log;
        }

        public static void LogBlockDetails(IBlock block, ILog log)
        {
            if (block == null) return;
            log.Add($"Site ({block.Context?.Site?.Id}): {block.Context?.Site?.UrlRoot}");
            log.Add($"Page ({block.Context?.Page?.Id}): {block.Context?.Page?.Url}");
            log.Add($"App ({block.App?.AppId}) Name: {block.App?.Name}");
            log.Add($"View ({block.View?.Id}): {block.View?.Name}");

        }
    }
}
