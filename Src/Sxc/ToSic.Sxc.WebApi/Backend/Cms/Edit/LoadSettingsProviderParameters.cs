namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LoadSettingsProviderParameters
{
    public IContextOfApp ContextOfApp { get; set; }

    public  List<IContentType> ContentTypes { get; set; }

    public List<string> InputTypes { get; set; }
}