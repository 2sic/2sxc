using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            //var settings = SettingsStack;
            //if (settings != null)
            //{
            //    var resources = settings.FindPropertyInternal("WebResources", null, null);
            //}

            PageServiceShared.Activate(keys);
        }

        private PropertyStack SettingsStack => _settingsStack ?? (_settingsStack = LoadSettings());
        private PropertyStack _settingsStack;

        private PropertyStack LoadSettings()
        {
            // Since the IPageService is used in templates, this should always return something real
            // But just to be sure, we'll go for the null-check
            var maybeBlock = CtxResolver.BlockOrNull();
            var appState = maybeBlock?.AppState;
            var siteContext = CtxResolver.Site();
            var languages = siteContext.Site.SafeLanguagePriorityCodes();
            var sources = appState?.SettingsInApp.GetStack(true, null);
            if (sources == null) return null;
            var settings = new PropertyStack();
            settings.Init(AppConstants.RootNameSettings, sources.ToArray());

            return settings;
        }
    }
}
