using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Security
{
    internal class Security:SaveHelperBase
    {
        public Security(IBlockBuilder blockBuilder, ILog parentLog) : base(blockBuilder, parentLog, "Api.SavSec") { }


        public IMultiPermissionCheck DoPreSaveSecurityCheck(int appId, IEnumerable<BundleWithHeader> items)
        {
            var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, items.Select(i => i.Header).ToList(), Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething,  out var exp))
                throw exp;
            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out exp))
                throw exp;

            // 2018-09-26 2dm
            // add test to verify that saving existing items is allowed

            Log.Add("passed security checks");
            return permCheck;

        }
    }
}
