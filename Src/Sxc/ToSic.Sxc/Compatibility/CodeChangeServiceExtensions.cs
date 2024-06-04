using ToSic.Eav.Code.Infos;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using CodeInfoService = ToSic.Eav.Code.InfoSystem.CodeInfoService;

namespace ToSic.Sxc.Compatibility;

internal static class CodeChangeServiceExtensions
{
    public static void WarnSxc(this CodeInfoService svc, CodeUse change, IBlock block = default)
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
        ?
        [
            $"Site ({block.Context?.Site?.Id}): {block.Context?.Site?.UrlRoot}",
            $"Page ({block.Context?.Page?.Id}): {block.Context?.Page?.Url}",
            $"App ({block.App?.AppId}) Name: {block.App?.Name}",
            $"View ({block.View?.Id}): {block.View?.Name}"
        ]
        : [];

}