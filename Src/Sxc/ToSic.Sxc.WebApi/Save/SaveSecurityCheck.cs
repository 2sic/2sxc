using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;

namespace ToSic.Sxc.WebApi.Save
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class SaveSecurity: SaveHelperBase
    {
        private readonly Generator<Apps.App> _appGen;
        private readonly Generator<MultiPermissionsTypes> _multiPermissionsTypesGen;

        public SaveSecurity(Generator<Apps.App> appGen, Generator<MultiPermissionsTypes> multiPermissionsTypesGen) : base("Api.SavSec") =>
            ConnectServices(
                _appGen = appGen,
                _multiPermissionsTypesGen = multiPermissionsTypesGen
            );

        internal new SaveSecurity Init(IContextOfApp context)
        {
            base.Init(context);
            return this;
        }


        public IMultiPermissionCheck DoPreSaveSecurityCheck(int appId, IEnumerable<BundleWithHeader> items) =>
            Log.Func<IMultiPermissionCheck>(() =>
            {
                var app = _appGen.New().InitWithOptionalBlock(appId);
                var permCheck = _multiPermissionsTypesGen.New()
                    .Init(Context, app, items.Select(i => i.Header).ToList());
                if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                    throw HttpException.PermissionDenied(error);
                if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                    throw HttpException.PermissionDenied(error);

                Log.A("passed security checks");
                return permCheck;
            });
    }
}
