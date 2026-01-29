using ToSic.Eav.Model;
using ToSic.Eav.WebApi.Sys.Install;

namespace ToSic.Sxc.Backend.Sys;

[ModelSource(ContentType = ContentTypeNameId)]
internal record SiteSetupAutoInstallAppsRule : ModelOfEntity
{
    public const string ContentTypeNameId = "833baa25-899b-4242-ade7-323a319bcf71";
    public const string ContentTypeName = "⚙️SiteSetupAutoInstallApps";

    public const string TargetGuid = "guid";
    public const string TargetAll = "all";
    public const string TargetUrl = "url";
    public const string ModeForbidden = "f";
    public const string ModeAllow = "a";
    public const string ModeOptional = "o";
    public const string ModeRequired = "r";

    public string Target => GetThis(TargetGuid);

    public string Mode => GetThis(ModeAllow);

    public string AppGuid => GetThis("");

    public string Url => GetThis("");

    public AppInstallRuleDto GetRuleDto() => new()
    {
        name = Title,
        appGuid = AppGuid,
        mode = Mode,
        target = Target,
        url = Url,
    };
}