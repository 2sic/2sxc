using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

namespace ToSic.Sxc.Dnn.LookUp
{
    /// <summary>
    /// This class is mainly here to deliver all standard DNN-token lists to 2sxc. 
    /// So it mainly initializes the normal DNN Token provider and offers a property called Property-Access which then contains all value-resolvers
    /// </summary>
    public sealed class TokenReplaceDnn : TokenReplace
    {

        /// <param name="moduleId"></param>
        /// <param name="ps"></param>
        /// <param name="userInfo"></param>
        public TokenReplaceDnn(int moduleId, PortalSettings ps, UserInfo userInfo)
            : base(Scope.DefaultSettings, "", ps, userInfo, moduleId)
        {
            ModuleId = moduleId;
            PortalSettings = ps;  
            ReplaceTokens("InitializePropertySources"); // must be executed, otherwise the list doesn't get built
        }

        public Dictionary<string, IPropertyAccess> PropertySources => PropertySource;
    }
}