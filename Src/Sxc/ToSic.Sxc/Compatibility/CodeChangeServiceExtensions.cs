using System;
using ToSic.Eav.CodeChanges;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility
{
    internal static class CodeChangeServiceExtensions
    {
        public static void WarnSxc(this CodeChangeService svc, ICodeChangeInfo change, IBlock block = default)
        {
            if (block != null)
                change = change.UsedAs(more: LogBlockDetails(block));
            svc.Warn(change);
        }

        public static void LogBlockDetails(IBlock block, ILog log)
        {
            if (block == null) return;
            log.A($"Site ({block.Context?.Site?.Id}): {block.Context?.Site?.UrlRoot}");
            log.A($"Page ({block.Context?.Page?.Id}): {block.Context?.Page?.Url}");
            log.A($"App ({block.App?.AppId}) Name: {block.App?.Name}");
            log.A($"View ({block.View?.Id}): {block.View?.Name}");

        }

        public static string[] LogBlockDetails(IBlock block) => block != null
            ? new[]
            {
                $"Site ({block.Context?.Site?.Id}): {block.Context?.Site?.UrlRoot}",
                $"Page ({block.Context?.Page?.Id}): {block.Context?.Page?.Url}",
                $"App ({block.App?.AppId}) Name: {block.App?.Name}",
                $"View ({block.View?.Id}): {block.View?.Name}"
            }
            : Array.Empty<string>();

    }
}
