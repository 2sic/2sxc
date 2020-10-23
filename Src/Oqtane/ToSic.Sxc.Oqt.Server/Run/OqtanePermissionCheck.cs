using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtanePermissionCheck: AppPermissionCheck
    {
        public OqtanePermissionCheck(): base("Mvc.PrmChk") { }

        // todo: #permissions
        protected override bool EnvironmentAllows(List<Grants> grants) => true;

        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            if (condition.Equals("SecurityAccessLevel.Anonymous", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

        protected override IUser User => new OqtaneUser(WipConstants.NullUser);
    }
}
