namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class LoadSettingsProviderParameters
{
    public required IContextOfApp ContextOfApp { get; init; }

    public  required List<IContentType> ContentTypes { get; init; }

    public required List<string> InputTypes { get; init; }
}