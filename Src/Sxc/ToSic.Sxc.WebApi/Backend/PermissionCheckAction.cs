using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sys.HookUp;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend;

public class PermissionCheckAction(Generator<MultiPermissionsApp, MultiPermissionsApp.Options> permCheckGen)
    : ServiceBase("Prm.ChkAct", connect: [permCheckGen]),
        IWork<(IContextOfBlock context, List<Grants> grants), bool>
{
    public Task<Package<bool>> Handle(WorkContext mainCtx, Package<(IContextOfBlock context, List<Grants> grants)> package)
    {
        var (context, requiredGrants) = package.Data;
        var permCheck = permCheckGen.New(new() { SiteContext = context, App = /*alternateApp ??*/ context.AppReaderRequired });
        if (permCheck.EnsureAll(requiredGrants) is { Allowed: false } pcResult)
            throw HttpException.PermissionDenied(pcResult.ErrorMessage);
        return Task.FromResult(true.ToPackage());
    }
}