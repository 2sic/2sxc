namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class LoadSettingsProviderParameters
{
    public IContextOfApp ContextOfApp { get; set; }

    public  List<IContentType> ContentTypes { get; set; }

    public List<string> InputTypes { get; set; }
}