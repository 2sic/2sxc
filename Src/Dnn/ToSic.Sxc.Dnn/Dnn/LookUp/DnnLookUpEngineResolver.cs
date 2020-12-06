using System.Threading;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;

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
            return providers;
        }
    }
}
