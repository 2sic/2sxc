using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;

namespace ToSic.Eav.WebApi.Sys.Admin;

[ContentTypeSpecs(
    Name = "App",
    Guid = "53b3fe9b-d689-4b1f-bed1-503cbc898ffc",
    Description = "App information",
    Scope = "System"
)]
public class AppModel(AppDto app) : RawEntity
{
    public bool IsApp => app.IsApp;
    public new string Guid => app.Guid;

    [ContentTypeAttributeSpecs(IsTitle = true)]
    public string Name => app.Name;

    public string Folder => app.Folder;
    public string AppRoot => app.AppRoot;
    public bool IsHidden => app.IsHidden;
    public int? ConfigurationId => app.ConfigurationId;
    public int Items => app.Items;
    public string? Thumbnail => app.Thumbnail;
    public string Version => app.Version;
    public bool IsGlobal => app.IsGlobal;
    public bool IsInherited => app.IsInherited;
    public AppMetadataDto? Lightspeed => app.Lightspeed;
    public bool HasCodeWarnings => app.HasCodeWarnings;

    public override IDictionary<string, object?> Attributes(RawConvertOptions options) =>
        new Dictionary<string, object?>
        {
            { nameof(IsApp), IsApp },
            { nameof(Guid), Guid },
            { nameof(Name), Name },
            { nameof(Folder), Folder },
            { nameof(AppRoot), AppRoot },
            { nameof(IsHidden), IsHidden },
            { nameof(ConfigurationId), ConfigurationId },
            { nameof(Items), Items },
            { nameof(Thumbnail), Thumbnail },
            { nameof(Version), Version },
            { nameof(IsGlobal), IsGlobal },
            { nameof(IsInherited), IsInherited },
            { nameof(Lightspeed), Lightspeed },
            { nameof(HasCodeWarnings), HasCodeWarnings },
        };
}
