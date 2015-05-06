using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenReplaceDnn : DotNetNuke.Services.Tokens.TokenReplace
    {
        /// <summary>
        /// This class is mainly here to deliver all standard DNN-token lists to 2sxc. 
        /// So it mainly initializes the normal DNN-Tokenprovider and offers a property called Property-Access which then contains all value-resolvers
        /// </summary>
        /// <param name="app"></param>
        /// <param name="moduleId"></param>
        /// <param name="ps"></param>
        /// <param name="uinfo"></param>
        public TokenReplaceDnn(App app, int moduleId, PortalSettings ps, UserInfo uinfo)
            : base(Scope.DefaultSettings, "", ps, uinfo, moduleId)
        {
            ModuleId = moduleId;
            PortalSettings = ps;  
            ReplaceTokens("InitializePropertySources"); // must be executed, otherwise the list doesn't get built
        }

        public Dictionary<string, IPropertyAccess> PropertySources
        {
            get
            {
                return PropertySource; 
            }
        }
    }
}