using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            var webRes = WebResources;

            // TODO: CONTINUE HERE - NOW wEBrESOURCES IS A dYNAMIC AND WE SHOULD EASILY WORK WITH IT
            if (webRes != null)
            {
                var siteContext = CtxResolver.Site();
                var languages = siteContext.Site.SafeLanguagePriorityCodes();
                var keysToRemove = new List<string>();
                foreach (var key in keys)
                {
                    var resSettingsProperty = webRes.FindPropertyInternal(key, languages, null);
                    if (!resSettingsProperty.IsFinal || !(resSettingsProperty.Result is IEnumerable<IEntity> listSettings)) continue;
                    
                    var resSettings = listSettings.FirstOrDefault();
                    if (resSettings == null) continue;

                    var enabledProp = resSettings.FindPropertyInternal("Enabled", languages, null);
                    if (!enabledProp.IsFinal || enabledProp.FieldType != DataTypes.Boolean) continue;

                    var enabled = enabledProp.Result as bool?;
                    if (enabled == false) continue;

                    var htmlProp = resSettings.FindPropertyInternal("Html", languages, null);
                    if (!htmlProp.IsFinal || enabledProp.FieldType != DataTypes.String) continue;

                    var html = htmlProp.Result as string;
                    if (html == null) continue;

                    // all ok so far
                    keysToRemove.Add(key);
                    (PageServiceShared.Features as PageFeatures.PageFeatures).ManualFeatures.Add(new PageFeature(
                        key, 
                        "manual",
                        "manual-description",
                        html: html));
                }

                // drop keys which were already taken care of
                keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
            }

            PageServiceShared.Activate(keys);
        }

        private DynamicEntity WebResources
        {
            get
            {
                if (_alreadyTriedToFindWebResources) return _webResources;
                var settings = SettingsStack;
                if (settings != null)
                {
                    var resources = settings.Get("WebResources"); // .FindPropertyInternal("WebResources", new string[] { null }, null);
                    if(resources != null)
                        _webResources = resources as DynamicEntity;
                    //if (resources.FieldType == DataTypes.Entity && resources.Result is IEnumerable<IEntity> webResEntities)
                    //    _webResources = webResEntities.FirstOrDefault();
                }
                _alreadyTriedToFindWebResources = true;
                return _webResources;
            }
        }
        private DynamicEntity _webResources;
        private bool _alreadyTriedToFindWebResources;

        private DynamicStack SettingsStack => _settingsStack ?? (_settingsStack = LoadSettings());
        private DynamicStack _settingsStack;

        private DynamicStack LoadSettings()
        {
            return CodeRoot?.Settings as DynamicStack;

            // Since the IPageService is used in templates, this should always return something real
            // But just to be sure, we'll go for the null-check
            //var maybeBlock = CtxResolver.BlockOrNull();
            //var appState = maybeBlock?.AppState;
            //var sources = appState?.SettingsInApp.GetStack(true, null);
            //if (sources == null) return null;
            //var settings = new PropertyStack();
            //settings.Init(AppConstants.RootNameSettings, sources.ToArray());

            //return settings;
        }
    }
}
