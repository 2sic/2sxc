using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Sys;

namespace ToSic.Sxc.WebApi.Sys
{
    internal class SiteSetupAutoInstallAppsRule: EntityBasedType
    {
        public const string TargetGuid = "guid";
        public const string TargetAll = "all";
        public const string TargetUrl = "url";
        public const string ModeForbidden = "f";
        public const string ModeAllow = "a";
        public const string ModeOptional = "o";
        public const string ModeRequired = "r";

        public SiteSetupAutoInstallAppsRule(IEntity entity) : base(entity)
        {
        }

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
}
