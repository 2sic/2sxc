using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.Environment.Dnn7.ValueProviders
{
    public sealed class TokenReplaceDnn : TokenReplace
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