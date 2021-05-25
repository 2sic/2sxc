using System.Collections.Generic;
using System.Security.Policy;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Dnn.LookUp
{
    /// <summary>
    /// Retrieves the current engine for a specific module. <br/>
    /// Internally it asks DNN for the current Property-Access objects and prepares them for use in EAV.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnLookUpEngineResolver : HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {

        #region Constructor / Dependency Injection

        public DnnLookUpEngineResolver(IZoneCultureResolver cultureResolver) : base("Dnn.LookUp")
        {
            _cultureResolver = cultureResolver;
        }
        private readonly IZoneCultureResolver _cultureResolver;

        #endregion

        /// <inheritdoc />
        public ILookUpEngine GetLookUpEngine(int instanceId)
        {
            var portalSettings = PortalSettings.Current;
            return portalSettings == null
                ? new LookUpEngine(Log)
                : GenerateDnnBasedLookupEngine(portalSettings, instanceId);
        }

        [PrivateApi]
        public LookUpEngine GenerateDnnBasedLookupEngine(PortalSettings portalSettings, int instanceId)
        {
            var providers = new LookUpEngine(Log);
            var dnnUsr = portalSettings.UserInfo;
            var dnnCult = _cultureResolver.SafeCurrentCultureInfo();
            var dnn = new DnnTokenReplace(instanceId, portalSettings, dnnUsr);
            var stdSources = dnn.PropertySources;
            foreach (var propertyAccess in stdSources)
                providers.Add(new LookUpInDnnPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));

            if (providers.HasSource("module"))
            {
                var original = providers.Sources["module"];
                var id = original.Get("moduleid");
                var preferred = new LookUpInDictionary("module", new Dictionary<string, string> {{"id", id}});
                providers.Sources["module"] = new LookUpInLookUps("module", preferred, original);
            }

            // site - id & guid
            if (providers.HasSource("portal"))
            {
                var original = providers.Sources["portal"];
                var id = original.Get("portalid");
                var guid = DotNetNuke.Common.Globals.GetPortalSettings()?.GUID;
                var preferred = new LookUpInDictionary("site", new Dictionary<string, string> { { "id", id }, { "guid", $"{guid}" } });
                providers.Add(preferred);
            }

            // page - id only for now or maybe guid
            if (providers.HasSource("tab"))
            {
                var original = providers.Sources["tab"];
                var id = original.Get("tabid");
                var guid = DotNetNuke.Common.Globals.GetPortalSettings()?.ActiveTab?.UniqueId;
                var preferred = new LookUpInDictionary("page", new Dictionary<string, string> { { "id", id }, { "guid", $"{guid}" } });
                providers.Add(preferred);
            }

            // Not implemented: Tenant

            return providers;
        }
    }
}
