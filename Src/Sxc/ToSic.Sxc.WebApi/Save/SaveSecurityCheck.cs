using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;

namespace ToSic.Sxc.WebApi.Save
{
    internal class SaveSecurity: SaveHelperBase<SaveSecurity>
    {
        private readonly Generator<Apps.App> _appGen;
        private readonly Generator<MultiPermissionsTypes> _multiPermissionsTypesGen;

        public SaveSecurity(IContextOfApp context, 
            Generator<Apps.App> appGen,
            Generator<MultiPermissionsTypes> multiPermissionsTypesGen, 
            ILog parentLog) : base("Api.SavSec")
        {
            _appGen = appGen;
            _multiPermissionsTypesGen = multiPermissionsTypesGen;
            base.Init(context, parentLog);
        }


        public IMultiPermissionCheck DoPreSaveSecurityCheck(int appId, IEnumerable<BundleWithHeader> items)
        {
            var app = _appGen.New().Init(appId, Log, null, Context.UserMayEdit);
            var permCheck = _multiPermissionsTypesGen.New().Init(Context, app, items.Select(i => i.Header).ToList(), Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);
            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);

            Log.A("passed security checks");
            return permCheck;
        }
    }
}
