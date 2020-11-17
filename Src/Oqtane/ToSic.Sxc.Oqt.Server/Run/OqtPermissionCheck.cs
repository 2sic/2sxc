using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtPermissionCheck: AppPermissionCheck
    {
        public OqtPermissionCheck(): base($"{OqtConstants.OqtLogPrefix}.PrmChk") { }

        // todo: #permissions
        protected override bool EnvironmentAllows(List<Grants> grants) => true;

        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            if (condition.Equals("SecurityAccessLevel.Anonymous", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

        protected override IUser User => new OqtUser(WipConstants.NullUser);
    }
}
