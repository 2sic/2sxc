using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
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
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw Http.PermissionDenied(error);
            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw Http.PermissionDenied(error);

            Log.Add("passed security checks");
            return permCheck;

        }
    }
}
