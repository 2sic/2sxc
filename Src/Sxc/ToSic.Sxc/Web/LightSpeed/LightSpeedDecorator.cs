using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Metadata;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Web.LightSpeed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LightSpeedDecorator: EntityBasedType
{
    public static string TypeNameId = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";
    public const string FieldDurationUser = "DurationUsers";
    public const string FieldDurationEditor = "DurationEditors";
    public const string FieldDurationSysAdmin = "DurationSystemAdmin";
    public const string FieldByUrlParameters = "ByUrlParameters";
    public const string FieldUrlCaseSensitive = "UrlParametersCaseSensitive";

    internal LightSpeedDecorator(IEntity entity) : base(entity)
    {
    }

    public bool IsEnabled => GetThis(false);

    public int Duration => GetThis(0);

    public int DurationUser => Get(FieldDurationUser, 0);

    public int DurationEditor => Get(FieldDurationEditor, 0);

    public int DurationSystemAdmin => Get(FieldDurationSysAdmin, 0);

    public bool ByUrlParam => Get(FieldByUrlParameters, false);

    public bool UrlParamCaseSensitive => Get(FieldUrlCaseSensitive, false);

    public string Advanced => GetThis("");

    public static LightSpeedDecorator GetFromAppStatePiggyBack(IAppStateCache appState, ILog log)
    {
        var decoFromPiggyBack = appState?.PiggyBack.GetOrGenerate(appState, $"decorator-{TypeNameId}", () =>
        {
            log.A("Debug WIP - remove once this has proven to work; get LightSpeed PiggyBack - recreate");
            var decoEntityOrNullPb = appState?.Metadata?.FirstOrDefaultOfType(TypeNameId);
            return new LightSpeedDecorator(decoEntityOrNullPb);
        });
        return decoFromPiggyBack ?? new LightSpeedDecorator(null);
    }
}