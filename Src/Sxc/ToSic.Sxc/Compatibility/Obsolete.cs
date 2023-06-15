using System;
using ToSic.Lib.Logging;
using ToSic.Eav.Obsolete;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility
{
    public class Obsolete
    {
        public static void Report(CodeChangeInfo change, string specificId = default, Action<ILog> addMore = null) 
            => new LogObsolete(change, specificId, addMore);

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
