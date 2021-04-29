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
            
            if(providers.HasSource("querystring"))
                providers.Add(new LookUpInLookUps("query", providers.Sources["querystring"]));

            if (providers.HasSource("module"))
            {
                var original = providers.Sources["module"];
                var mid = original.Get("moduleid");
                var preferred = new LookUpInDictionary("module", new Dictionary<string, string> {{"id", mid}});
                providers.Sources["module"] = new LookUpInLookUps("module", preferred, original);
            }
            
            // add site - id & guid
            
            // add page - id only for now or maybe guid 
            
            
            // Not implemented: Tenant
                
            
            return providers;
        }
    }
}
