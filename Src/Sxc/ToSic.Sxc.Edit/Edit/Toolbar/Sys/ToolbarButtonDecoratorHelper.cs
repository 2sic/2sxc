using ToSic.Eav.Apps;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ToolbarButtonDecoratorHelper(IAppReaderFactory appReaders) : ServiceBase($"{SxcLogName}.TbdHlp", connect: [appReaders])
{
    public IAppIdentity? MainAppIdentity { get; set; }

    internal ToolbarButtonDecorator? GetDecorator(IAppIdentity? appIdentity, string? typeName, string? command)
    {
        // If no special context was given, use the main one from the current context
        appIdentity ??= MainAppIdentity;

        if (appIdentity == null || !typeName.HasValue() || !command.HasValue())
            return null;

        var appReader = appReaders.Get(appIdentity);

        var type = appReader.TryGetContentType(typeName);
        if (type == null)
            return null;

        var md = type.Metadata
            .OfType(ToolbarButtonDecorator.TypeName)
            .ToList();

        return md
            .Select(m => new ToolbarButtonDecorator(m))
            .FirstOrDefault(d => d.Command.EqualsInsensitive(command));

    }
}