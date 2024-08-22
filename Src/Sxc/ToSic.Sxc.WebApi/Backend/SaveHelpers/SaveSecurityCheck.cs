using ToSic.Eav.Security;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.Backend.SaveHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SaveSecurity: SaveHelperBase
{
    private readonly Generator<MultiPermissionsTypes> _multiPermissionsTypesGen;

    public SaveSecurity(Generator<MultiPermissionsTypes> multiPermissionsTypesGen) : base("Api.SavSec")
    {
        ConnectLogs([
            _multiPermissionsTypesGen = multiPermissionsTypesGen
        ]);
    }

    internal new SaveSecurity Init(IContextOfApp context)
    {
        base.Init(context);
        return this;
    }


    public IMultiPermissionCheck DoPreSaveSecurityCheck(IEnumerable<BundleWithHeader> items)
    {
        var l = Log.Fn<IMultiPermissionCheck>($"{items.Count()} items");

        var permCheck = _multiPermissionsTypesGen.New()
            .Init(Context, Context.AppReader, items.Select(i => i.Header).ToList());
        if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
            throw HttpException.PermissionDenied(error);
        if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
            throw HttpException.PermissionDenied(error);

        return l.Return(permCheck, "passed security checks");
    }
}