using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Obsolete;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility
{
    public class Obsolete
    {
        public static void Warning13To15(string obsoleteId, string specificId, string link, Action<ILog> addMore = null) 
            => new LogObsolete(obsoleteId, specificId, "v13", "v15 ca. end of 2022", link, addMore);

        public static void Warning13To15(string obsoleteId, string specificId, string link, IBlock block) 
            => new LogObsolete(obsoleteId, specificId, "v13", "v15 ca. end of 2022", link, (log) => LogBlockDetails(block, log));

        public static void Killed13(string obsoleteId, string specificId, string link, Action<ILog> addMore = null) 
            => new LogObsolete(obsoleteId, specificId, "v13 EOY 2021", null, link, addMore);

        public static void LogBlockDetails(IBlock block, ILog log)
        {
            if (block == null) return;
            log.A($"Site ({block.Context?.Site?.Id}): {block.Context?.Site?.UrlRoot}");
            log.A($"Page ({block.Context?.Page?.Id}): {block.Context?.Page?.Url}");
            log.A($"App ({block.App?.AppId}) Name: {block.App?.Name}");
            log.A($"View ({block.View?.Id}): {block.View?.Name}");

        }
    }
}
