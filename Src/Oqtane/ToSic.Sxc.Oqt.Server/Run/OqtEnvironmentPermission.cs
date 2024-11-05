using Microsoft.AspNetCore.Http;
using Oqtane.Security;
using Oqtane.Shared;
using System.Security.Claims;
using ToSic.Eav.Context;
using ToSic.Eav.Integration.Security;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Shared;
using static System.StringComparison;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtEnvironmentPermission(
    IHttpContextAccessor httpContextAccessor,
    LazySvc<IUserPermissions> userPermissions,
    LazySvc<IUser> oqtUser)
    : EnvironmentPermission(OqtConstants.OqtLogPrefix, connect: [httpContextAccessor, userPermissions, oqtUser])
{
    /// <summary>
    /// Gets the <see cref="ClaimsPrincipal"/> for user associated with the executing action.
    /// </summary>
    private ClaimsPrincipal ClaimsPrincipal => httpContextAccessor.HttpContext?.User;

    private IModule Module => _module ??= (Context as IContextOfBlock)?.Module;
    private IModule _module;

    public override bool VerifyConditionOfEnvironment(string condition)
    {
        // This terms are historic from DNN
        if (condition.Equals(SalAnonymous, InvariantCultureIgnoreCase))
            return true;

        var m = Module;

        if (condition.Equals(SalView, InvariantCultureIgnoreCase))
            return m != null && userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.View);

        if (condition.Equals(SalEdit, InvariantCultureIgnoreCase))
            return m != null && userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.Edit);

        if (condition.Equals(SalSiteAdmin, InvariantCultureIgnoreCase))
            return oqtUser.Value.IsSiteAdmin;

        if (condition.Equals(SalSystemUser, InvariantCultureIgnoreCase))
            return oqtUser.Value.IsSystemAdmin;

        return false;
    }

    protected override bool UserIsModuleAdmin()
    {
        var l = Log.Fn<bool>($"{nameof(Module)}: {Module?.Id}.");
        return l.ReturnAsOk(UserIsModuleEditor());
    }

    protected override bool UserIsModuleEditor()
    {
        return _userIsModuleEditor ??= IsModuleEditor();
        bool IsModuleEditor()
        {
            var l = Log.Fn<bool>();
            if (Module == null)
                return l.ReturnFalse();
            try
            {
                return l.Return(userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, Module.Id, PermissionNames.Edit));
            }
            catch
            {
                return l.ReturnFalse();
            }
        }
    }
    private bool? _userIsModuleEditor;
}