using Microsoft.AspNetCore.Http;
using Oqtane.Security;
using Oqtane.Shared;
using System.Security.Claims;
using ToSic.Eav.Environment.Sys.Permissions;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Users;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtEnvironmentPermission(
    IHttpContextAccessor httpContextAccessor,
    LazySvc<IUserPermissions> userPermissions,
    LazySvc<IUser> oqtUser)
    : ServiceWithSetup<EnvironmentPermissionOptions>(OqtConstants.OqtLogPrefix, connect: [httpContextAccessor, userPermissions, oqtUser]),
        IEnvironmentPermission
{

    public void Setup(EnvironmentPermissionOptionsBase options)
        => base.Setup(options as EnvironmentPermissionOptions ?? throw new InvalidCastException("Must convert the options, but not compatible."));

    /// <inheritdoc/>
    public PermissionCheckInfo EnvironmentAllows(List<Grants> grants)
    {
        var ok = new EnvironmentPermissionsLogic(parentLog: Log)
            .EnvironmentAllows(grants: grants, options: MyOptions, userIsModuleAdmin: UserIsModuleAdmin, userIsModuleEditor: UserIsModuleEditor);
        return new(ok, ok ? Conditions.EnvironmentGlobal : Conditions.Undefined);
    }

    /// <summary>
    /// Gets the <see cref="ClaimsPrincipal"/> for user associated with the executing action.
    /// </summary>
    private ClaimsPrincipal ClaimsPrincipal => httpContextAccessor.HttpContext?.User;

    private IModule Module => field ??= (MyOptions?.ContextOfSiteOrBlock as IContextOfBlock)?.Module;

    public bool VerifyConditionOfEnvironment(string condition)
    {
        // This terms are historic from DNN
        if (condition.EqualsInsensitive(EnvironmentPermissionConstants.SalAnonymous))
            return true;

        var m = Module;

        if (condition.EqualsInsensitive(EnvironmentPermissionConstants.SalView))
            return m != null && userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.View);

        if (condition.EqualsInsensitive(EnvironmentPermissionConstants.SalEdit))
            return m != null && userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.Edit);

        if (condition.EqualsInsensitive(EnvironmentPermissionConstants.SalSiteAdmin))
            return oqtUser.Value.IsSiteAdmin;

        if (condition.EqualsInsensitive(EnvironmentPermissionConstants.SalSystemUser))
            return oqtUser.Value.IsSystemAdmin;

        return false;
    }

    private bool UserIsModuleAdmin()
    {
        var l = Log.Fn<bool>($"{nameof(Module)}: {Module?.Id}.");
        return l.ReturnAsOk(UserIsModuleEditor());
    }

    private bool UserIsModuleEditor()
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