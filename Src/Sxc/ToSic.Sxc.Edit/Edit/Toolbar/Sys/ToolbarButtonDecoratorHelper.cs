using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Models;
using ToSic.Sys.Caching;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ToolbarButtonDecoratorHelper(IAppReaderFactory appReaders, ScopedCache<Dictionary<string, ToolbarButtonDecorator?>> cache, Ctx<IAppIdentity> exCtx)
    : ServiceBase($"{SxcLogName}.TbdHlp", connect: [appReaders])
{
    internal ToolbarButtonDecorator? GetDecorator(IAppIdentity? appIdentity, string? typeName, string? command)
    {
        // If no special context was given, use the main one from the current context
        appIdentity ??= exCtx.Value;

        if (typeName.IsEmpty() || command.IsEmpty())
            return null;

        var cacheKey = $"{appIdentity.Show()}/{typeName}/{command}";

        if (cache.Cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var appReader = appReaders.Get(appIdentity);

        var type = appReader.TryGetContentType(typeName);
        if (type == null)
            return null;

        var result = type.Metadata
            .GetModels<ToolbarButtonDecorator>()
            .OfType<ToolbarButtonDecorator>()
            .FirstOrDefault(d => d.Command.EqualsInsensitive(command));

        cache.Cache[cacheKey] = result;

        return result;
    }
}
