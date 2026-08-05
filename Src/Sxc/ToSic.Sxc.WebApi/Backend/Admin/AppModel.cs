//using ToSic.Eav.Data.Raw.Sys;
//using ToSic.Eav.Data.ContentTypes;
//using ToSic.Eav.Data.Raw;

//namespace ToSic.Eav.WebApi.Sys.Admin;

//// TODO: @2rb
//// - Merge this with AppDto, doesn't make sense to have 2 identical objects
//// - the final object should be AppRaw (not Model)
//// - Should probably just implement IRawModelAutoConvert, not inherit from RawEntity

//[ContentType(
//    Name = "App",
//    Guid = "53b3fe9b-d689-4b1f-bed1-503cbc898ffc",
//    Description = "App information",
//    Scope = "System"
//)]
//public record AppModel(AppRaw  app) : RawEntity
//{
//    public bool IsApp => app.IsApp;
//    [ContentTypeField(IsTitle = true)]  // TODO: @2rb remember to move
//    public string Name => app.Name;

//    public string Folder => app.Folder;
//    public string AppRoot => app.AppRoot;
//    public bool IsHidden => app.IsHidden;
//    public int? ConfigurationId => app.ConfigurationId;
//    public int Items => app.Items;
//    public string? Thumbnail => app.Thumbnail;
//    public string Version => app.Version;
//    public bool IsGlobal => app.IsGlobal;
//    public bool IsInherited => app.IsInherited;
//    public AppMetadataDto? Lightspeed => app.Lightspeed;
//    public bool HasCodeWarnings => app.HasCodeWarnings;

//    protected override IDictionary<string, object?> GetValues() =>
//        new Dictionary<string, object?>
//        {
//            { nameof(IsApp), IsApp },
//            { nameof(AppRaw.Guid), app.Guid },
//            { nameof(Name), Name },
//            { nameof(Folder), Folder },
//            { nameof(AppRoot), AppRoot },
//            { nameof(IsHidden), IsHidden },
//            { nameof(ConfigurationId), ConfigurationId },
//            { nameof(Items), Items },
//            { nameof(Thumbnail), Thumbnail },
//            { nameof(Version), Version },
//            { nameof(IsGlobal), IsGlobal },
//            { nameof(IsInherited), IsInherited },
//            { nameof(Lightspeed), Lightspeed },
//            { nameof(HasCodeWarnings), HasCodeWarnings },
//        };
//}
