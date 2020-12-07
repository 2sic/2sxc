using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcPermissionCheck: AppPermissionCheck
    {
        public MvcPermissionCheck(): base("Mvc") { }

        // todo: #permissions
        protected override bool EnvironmentAllows(List<Grants> grants) => true;

        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            if (condition.Equals("SecurityAccessLevel.Anonymous", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

    }
}
