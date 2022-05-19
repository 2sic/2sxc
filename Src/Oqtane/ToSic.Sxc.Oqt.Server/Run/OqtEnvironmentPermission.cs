using Microsoft.AspNetCore.Http;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEnvironmentPermission : EnvironmentPermission
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IUserPermissions> _userPermissions;
        private readonly Lazy<IUser> _oqtUser;

        public OqtEnvironmentPermission(IHttpContextAccessor httpContextAccessor,
            Lazy<IUserPermissions> userPermissions,
            Lazy<IUser> oqtUser) : base(OqtConstants.OqtLogPrefix)
        {
            _httpContextAccessor = httpContextAccessor;
            _userPermissions = userPermissions;
            _oqtUser = oqtUser;
        }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> for user associated with the executing action.
        /// </summary>
        private ClaimsPrincipal ClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

        private IModule Module => _module ??= (Context as IContextOfBlock)?.Module;
        private IModule _module;

        public override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Call2<bool>(() => $"[{string.Join(",", grants)}]");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesSiteZone())
                ok = UserIsSiteAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = Conditions.EnvironmentGlobal;
            return logWrap.Return(ok, $"{ok} because:{GrantedBecause}");
        }

        public override bool VerifyConditionOfEnvironment(string condition)
        {
            // This terms are historic from DNN
            if (condition.Equals("SecurityAccessLevel.Anonymous", StringComparison.InvariantCultureIgnoreCase))
                return true;

            var m = Module;

            if (condition.Equals("SecurityAccessLevel.View", StringComparison.InvariantCultureIgnoreCase))
                return m != null && _userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.View);

            if (condition.Equals("SecurityAccessLevel.Edit", StringComparison.InvariantCultureIgnoreCase))
                return m != null && _userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, m.Id, PermissionNames.Edit);

            if (condition.Equals("SecurityAccessLevel.Admin", StringComparison.InvariantCultureIgnoreCase))
                return _oqtUser.Value.IsAdmin;

            if (condition.Equals("SecurityAccessLevel.Host", StringComparison.InvariantCultureIgnoreCase))
                return _oqtUser.Value.IsSuperUser;

            return false;
        }

        protected bool UserIsModuleAdmin() => Log.Return(UserIsModuleEditor);

        protected bool UserIsModuleEditor()
            => _userIsModuleEditor ??= Log.Return(() =>
            {
                if (Module == null) return false;
                try
                {
                    return _userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, Module.Id,
                        PermissionNames.Edit);
                }
                catch
                {
                    return false;
                }
            });
        private bool? _userIsModuleEditor;
    }
}
