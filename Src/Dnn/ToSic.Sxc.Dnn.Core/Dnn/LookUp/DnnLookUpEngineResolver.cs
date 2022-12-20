using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Dnn.LookUp
{
    /// <summary>
    /// Retrieves the current engine for a specific module. <br/>
    /// Internally it asks DNN for the current Property-Access objects and prepares them for use in EAV.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnLookUpEngineResolver : HasLog, ILookUpEngineResolver
    {

        #region Constructor / Dependency Injection

        public DnnLookUpEngineResolver(IZoneCultureResolver cultureResolver/*, ViewModuleIdHack viewModuleIdProvider*/) : base("Dnn.LookUp")
        {
            _cultureResolver = cultureResolver;
            // 2022-12-21 this was a bug in DNN installations, but it appears it was because updates were done wrong - disable for now #viewModuleHack
            //_viewModuleIdProvider = viewModuleIdProvider;
        }
        private readonly IZoneCultureResolver _cultureResolver;
        // 2022-12-21 this was a bug in DNN installations, but it appears it was because updates were done wrong - disable for now #viewModuleHack
        // private readonly ViewModuleIdHack _viewModuleIdProvider;

        #endregion

        /// <inheritdoc />
        public ILookUpEngine GetLookUpEngine(int moduleId)
        {
            var wrapLog = Log.Fn<ILookUpEngine>("" + moduleId);
            var portalSettings = PortalSettings.Current;
            return portalSettings == null 
                ? wrapLog.Return(new LookUpEngine(Log), "no context") 
                : wrapLog.Return(GenerateDnnBasedLookupEngine(portalSettings, moduleId), "with site");
        }

        [PrivateApi]
        public LookUpEngine GenerateDnnBasedLookupEngine(PortalSettings portalSettings, int moduleId)
        {
            var wrapLog = Log.Fn<LookUpEngine>($"..., {moduleId}");
            // 2022-12-21 this was a bug in DNN installations, but it appears it was because updates were done wrong - disable for now #viewModuleHack
            //if (moduleId < 1) moduleId = _viewModuleIdProvider.ModuleId;
            var providers = new LookUpEngine(Log);
            var dnnUsr = portalSettings.UserInfo;
            var dnnCult = _cultureResolver.SafeCurrentCultureInfo();
            var dnn = new DnnTokenReplace(moduleId, portalSettings, dnnUsr);
            var stdSources = dnn.PropertySources;
            foreach (var propertyAccess in stdSources)
                providers.Add(new LookUpInDnnPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));

            // Expand the Lookup for "module" to also have an "id" property
            if (providers.HasSource(SourceModule))
            {
                var original = providers.Sources[SourceModule];
                var id = original.Get(OldDnnModuleId);
                var preferred = new LookUpInDictionary(SourceModule, new Dictionary<string, string> { { KeyId, id } });
                providers.Sources[SourceModule] = new LookUpInLookUps(SourceModule, preferred, original);
            }

            // Create the lookup for "site" based on the "portal" and only give it "id" & "guid"
            if (providers.HasSource(OldDnnSiteSource))
            {
                var original = providers.Sources[OldDnnSiteSource];
                var id = original.Get(OldDnnSiteId);
                var guid = DotNetNuke.Common.Globals.GetPortalSettings()?.GUID;
                var preferred = new LookUpInDictionary(SourceSite, new Dictionary<string, string> { { KeyId, id }, { KeyGuid, $"{guid}" } });
                providers.Add(preferred);
            }

            // Create the lookup for "page" based on the "tab" and only give it "id" & "guid"
            if (providers.HasSource(OldDnnPageSource))
            {
                var original = providers.Sources[OldDnnPageSource];
                var id = original.Get(OldDnnPageId);
                var guid = DotNetNuke.Common.Globals.GetPortalSettings()?.ActiveTab?.UniqueId;
                var preferred = new LookUpInDictionary(SourcePage, new Dictionary<string, string> { { KeyId, id }, { KeyGuid, $"{guid}" } });
                providers.Add(preferred);
            }

            // Not implemented in Dnn: "Tenant" source

            return wrapLog.Return(providers);
        }
    }
}
