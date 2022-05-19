using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.LightSpeed
{
    public class LightSpeedDecorator: EntityBasedType
    {
        public static string TypeName = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";
        public const string FieldIsEnabled = "IsEnabled";
        public const string FieldDuration = "Duration";
        public const string FieldDurationUser = "DurationUsers";
        public const string FieldDurationEditor = "DurationEditors";
        public const string FieldDurationSysAdmin = "DurationSystemAdmin";
        public const string FieldByUrlParameters = "ByUrlParameters";
        public const string FieldUrlCaseSensitive = "UrlParametersCaseSensitive";
        public const string FieldAdvanced = "Advanced";

        public LightSpeedDecorator(IEntity entity) : base(entity)
        {
        }

        public bool IsEnabled => Get(FieldIsEnabled, false);

        public int Duration => Get(FieldDuration, 0);

        public int DurationUser => Get(FieldDurationUser, 0);

        public int DurationEditor => Get(FieldDurationEditor, 0);

        public int DurationSystemAdmin => Get(FieldDurationSysAdmin, 0);

        public bool ByUrlParam => Get(FieldByUrlParameters, false);

        public bool UrlParamCaseSensitive => Get(FieldUrlCaseSensitive, false);

        public string Advanced => Get(FieldAdvanced, "");

        public static LightSpeedDecorator GetFromAppStatePiggyBack(AppState appState, ILog log)
        {
            var decoFromPiggyBack = appState?.PiggyBack.GetOrGenerate(appState, $"decorator-{TypeName}", () =>
            {
                log.A("Debug WIP - remove once this has proven to work; get LightSpeed PiggyBack - recreate");
                var decoEntityOrNullPb = appState?.Metadata?.FirstOrDefaultOfType(TypeName);
                return new LightSpeedDecorator(decoEntityOrNullPb);
            });
            return decoFromPiggyBack ?? new LightSpeedDecorator(null);
        }
    }
}
