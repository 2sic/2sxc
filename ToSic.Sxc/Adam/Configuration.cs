using ToSic.Eav.Configuration;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam
{
    public class Configuration
    {
        public const string TypeName = "AdamConfiguration";

        public static string AdamAppRootFolder =>
            _adamAppRootFolder ?? (_adamAppRootFolder =
                Global.GetOverride(TypeName, "OverrideAppRootFolder",
                    "adam/[AppFolder]/"));

        private static string _adamAppRootFolder;
    }
}
