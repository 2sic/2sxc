using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Model;
using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record LightSpeedDecorator : ModelOfEntity, IOutputCacheSettings
{
    /// <summary>
    /// Nice name. If it ever changes, remember to also update UI as it has references to it.
    /// </summary>
    public static string NiceName = "LightSpeedOutputDecorator";
    public static string TypeNameId = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";

    public LightSpeedDecorator(IEntity? entity) : base(entity!) { }

    public bool IsEnabled => GetThis(false);

    public bool? IsEnabledNullable => Get(nameof(IsEnabled), fallback: null as bool?);

    public int Duration => GetThis(0);

    public int DurationUsers => GetThis(0);

    public int DurationEditors => GetThis(0);

    public int DurationSystemAdmin => GetThis(0);

    public bool ByUrlParameters => GetThis(false);

    public bool UrlParametersCaseSensitive => GetThis(false);

    public string UrlParameterNames => GetThis("");

    public bool UrlParametersOthersDisableCache => GetThis(true);

    public string Advanced => GetThis("");

    public static LightSpeedDecorator GetFromAppStatePiggyBack(IAppReader? appReader/*, ILog log*/)
    {
        var appState = appReader?.GetCache();
        var decoFromPiggyBack = appState?.PiggyBack
            .GetOrGenerate(appState, $"decorator-{TypeNameId}", () =>
            {
                //log.A("Debug WIP - remove once this has proven to work; get LightSpeed PiggyBack - recreate");
                var decoEntityOrNullPb = appState.Metadata.First(typeName: TypeNameId);
                return new LightSpeedDecorator(decoEntityOrNullPb);
            })
            .Value;
        return decoFromPiggyBack ?? new LightSpeedDecorator(null as IEntity);
    }
}