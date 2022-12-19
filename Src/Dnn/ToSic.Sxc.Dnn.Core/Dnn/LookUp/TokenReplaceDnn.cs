using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.LookUp
{
    /// <summary>
    /// This class is mainly here to deliver all standard DNN-token lists to 2sxc. 
    /// So it mainly initializes the normal DNN Token provider and offers a property called Property-Access which then contains all value-resolvers
    /// </summary>
    [PrivateApi("not for public use, it's an internal class just needed to retrieve DNN stuff")]
    internal sealed class DnnTokenReplace : TokenReplace
    {
        /// <param name="instanceId"></param>
        /// <param name="ps"></param>
        /// <param name="userInfo"></param>
        public DnnTokenReplace(int instanceId, PortalSettings ps, UserInfo userInfo)
            : base(Scope.DefaultSettings, "", ps, userInfo, instanceId)
        {
            ModuleId = instanceId;
            PortalSettings = ps;  
            ReplaceTokens("InitializePropertySources"); // must be executed, otherwise the list doesn't get built
        }

        public Dictionary<string, IPropertyAccess> PropertySources => PropertySource;
    }
}