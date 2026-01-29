using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Model;
using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
[ModelSource(ContentType = ContentTypeNameId)]
public record LightSpeedDecorator : ModelOfEntity, IOutputCacheSettings
{
    /// <summary>
    /// Nice name. If it ever changes, remember to also update UI as it has references to it.
    /// </summary>
    public const string ContentTypeName = "LightSpeedOutputDecorator";
    public const string ContentTypeNameId = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";

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
            .GetOrGenerate(appState, $"decorator-{ContentTypeNameId}", () => appState.Metadata.First<LightSpeedDecorator>())
            .Value;
        return decoFromPiggyBack ?? (null as IEntity).As<LightSpeedDecorator>()!;
    }
}