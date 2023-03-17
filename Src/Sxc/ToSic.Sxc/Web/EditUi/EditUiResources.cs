using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.WebResources;

namespace ToSic.Sxc.Web.EditUi
{
    public class EditUiResources
    {
        #region Constructor

        private readonly IAppStates _appStates;
        private readonly AppSettingsStack _stackHelper;

        public EditUiResources(IAppStates appStates, AppSettingsStack stackHelper)
        {
            _appStates = appStates;
            _stackHelper = stackHelper;
        }

        #endregion

        #region Resources / Constants

        public const string LinkTagTemplate = "<link rel=\"stylesheet\" href=\"{0}\">";
        public const string RobotoFromGoogle = "https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap";
        public const string RobotoFromAltCdn = "/google-fonts/roboto/fonts.css";
        public const string MaterialIconsGoogle = "";
        public const string MaterialIconsAltCdn = "/google-fonts/material-symbols-outlined/fonts.css";

        #endregion

        public EditUiResourcesSpecs GetResources(bool enabled, bool icons, bool roboto)
        {
            if (!enabled) return new EditUiResourcesSpecs();
            var appPreset = _appStates.GetPresetApp();
            var stack = _stackHelper.Init(appPreset).GetStack(ConfigurationConstants.RootNameSettings);
            var getResult =
                stack.InternalGetPath(
                    $"{WebResourceConstants.WebResourcesNode}.{WebResourceConstants.CdnSourceEditField}");
            var cdnSettings = getResult.Result as string;
            var useAltCdn = cdnSettings.HasValue();
            cdnSettings += WebResourceProcessor.VersionSuffix;
            var html = $"<!-- 2dm: cdn settings {getResult.IsFinal}, '{getResult.Result}', '{getResult.Result?.GetType()}' '{cdnSettings}', {cdnSettings?.Length} -->";
            if (icons)
            {
                var url = useAltCdn ? cdnSettings + MaterialIconsAltCdn : MaterialIconsGoogle;
                html += "\n" + string.Format(LinkTagTemplate, url);
            }

            if (roboto)
            {
                var url = useAltCdn ? cdnSettings + RobotoFromAltCdn : RobotoFromGoogle;
                html += "\n" + string.Format(LinkTagTemplate, url);
            }
            html += "\n";
            return new EditUiResourcesSpecs { Html = html };
        }

        public class EditUiResourcesSpecs
        {
            public string Html { get; set; } = "";

            // later we'll also add CSP specs here
        }

    }
}
