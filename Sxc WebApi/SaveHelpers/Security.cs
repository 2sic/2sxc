using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.SaveHelpers
{
    internal class Security:SaveHelperBase
    {
        public Security(SxcInstance sxcInstance, Log parentLog) : base(sxcInstance, parentLog, "Api.SavSec") { }


        public AppAndPermissions DoSaveSecurityCheck(/*SxcInstance SxcInstance,*/ int appId, IEnumerable<BundleWithHeader> items/*, Log Log*/)
        {
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, items.Select(i => i.Header).ToList(), out var exp))
                throw exp;
            if (!permCheck.UserUnrestrictedAndFeatureEnabled(out exp))
                throw exp;
            Log.Add("passed security checks");
            return permCheck;

        }
    }
}
