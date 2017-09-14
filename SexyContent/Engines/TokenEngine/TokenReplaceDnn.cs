using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using System.Collections.Generic;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenReplaceDnn : TokenReplace
    {
        /// <summary>
        /// This class is mainly here to deliver all standard DNN-token lists to 2sxc. 
        /// So it mainly initializes the normal DNN-Tokenprovider and offers a property called Property-Access which then contains all value-resolvers
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="ps"></param>
        /// <param name="uinfo"></param>
        public TokenReplaceDnn(int moduleId, PortalSettings ps, UserInfo uinfo)
            : base(Scope.DefaultSettings, "", ps, uinfo, moduleId)
        {
            ModuleId = moduleId;
            PortalSettings = ps;  
            ReplaceTokens("InitializePropertySources"); // must be executed, otherwise the list doesn't get built
        }

        public Dictionary<string, IPropertyAccess> PropertySources => PropertySource;
    }
}