using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security;

namespace IntegrationSamples.SxcEdit01.Integration
{
    public class IntAppPermissionCheck: AppPermissionCheck
    {
        public IntAppPermissionCheck(IAppStates appStates, Dependencies dependencies) : base(appStates, dependencies, IntConstants.LogPrefix)
        {
        }

        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            return true;
        }

        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            return false;
        }
    }
}
