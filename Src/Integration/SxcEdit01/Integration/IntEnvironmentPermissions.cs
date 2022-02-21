using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security;

namespace IntegrationSamples.SxcEdit01.Integration
{
    public class IntEnvironmentPermissions: EnvironmentPermission
    {
        public IntEnvironmentPermissions() : base(IntConstants.LogPrefix)
        {
        }

        public override bool EnvironmentAllows(List<Grants> grants) => true;

        public override bool VerifyConditionOfEnvironment(string condition) => false;
    }
}
