using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Save
{
    internal class SaveSecurity: SaveHelperBase
    {
        public SaveSecurity(IBlock block, ILog parentLog) : base(block, parentLog, "Api.SavSec") { }


        public IMultiPermissionCheck DoPreSaveSecurityCheck(int appId, IEnumerable<BundleWithHeader> items)
        {
            var app = Factory.Resolve<Apps.App>().Init(appId, Log, Block);
            var permCheck = new MultiPermissionsTypes(Block.Context, app, items.Select(i => i.Header).ToList(), Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);
            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);

            Log.Add("passed security checks");
            return permCheck;

        }
    }
}
