using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.SaveHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SaveSecurity(Generator<MultiPermissionsTypes, MultiPermissionsTypes.Options> multiPermissionsTypesGen, LazySvc<ISysFeaturesService> sysFeatSvc)
    : ServiceBase("Api.SavSec", connect: [multiPermissionsTypesGen, sysFeatSvc])
{

    public IMultiPermissionCheck DoPreSaveSecurityCheck(IContextOfApp context, IEnumerable<BundleWithHeader> items)
    {
        var list = items.ToListOpt();
        var l = Log.Fn<IMultiPermissionCheck>($"{list.Count} items");

        var appReader = context.AppReaderRequired;
        var permCheck = multiPermissionsTypesGen.New(new()
        {
            SiteContext = context,
            App = appReader,
            ContentTypes = new SavePermissionDataHelper(Log).ExtractTypeNamesFromItems(
                appReader,
                list.Select(i => i.Header).ToList()
            )
        });
        
        if (!permCheck.EnsureAllWithError(GrantSets.WriteSomething, out var error))
            throw HttpException.PermissionDenied(error);
        
        if (!permCheck.UserCanWriteAndPublicFormsEnabled(sysFeatSvc, out _, out error))
            throw HttpException.PermissionDenied(error);

        return l.Return(permCheck, "passed security checks");
    }
}