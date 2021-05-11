using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Save
{
    internal class SaveSecurity: SaveHelperBase<SaveSecurity>
    {

        public SaveSecurity(IContextOfApp context, ILog parentLog) : base("Api.SavSec")
        {
            base.Init(context, parentLog);
        }


        public IMultiPermissionCheck DoPreSaveSecurityCheck(int appId, IEnumerable<BundleWithHeader> items)
        {
            var sp = Context.ServiceProvider;
            var app = sp.Build<Apps.App>().Init(sp, appId, Log, null, Context.UserMayEdit);
            var permCheck = sp.Build<MultiPermissionsTypes>().Init(Context, app, items.Select(i => i.Header).ToList(), Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);
            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);

            Log.Add("passed security checks");
            return permCheck;

        }
    }
}
