using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Apps
{
    public class OqtPermissionCheck: AppPermissionCheck
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IUserPermissions> _userPermissions;
        private readonly Lazy<IUser> _oqtUser;

        public OqtPermissionCheck(IHttpContextAccessor httpContextAccessor, 
            Lazy<IUserPermissions> userPermissions, 
            Lazy<IUser> oqtUser,
            IAppStates appStates,
            Dependencies dependencies
            ) : base(appStates, dependencies, OqtConstants.OqtLogPrefix)
        {
            _httpContextAccessor = httpContextAccessor;
            _userPermissions = userPermissions;
            _oqtUser = oqtUser;
        }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> for user associated with the executing action.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

        protected IModule Module => _module ??= (Context as IContextOfBlock)?.Module;
        private IModule _module;

        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Call(() => $"[{string.Join(",", grants)}]");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesSiteZone())
                ok = UserIsSiteAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = Conditions.EnvironmentGlobal;
            logWrap($"{ok} because:{GrantedBecause}");
            return ok;
        }

        protected override bool VerifyConditionOfEnvironment(string condition)
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

        /// <summary>
        /// Verify that we're in the same zone, allowing admin/module checks
        /// </summary>
        /// <returns></returns>
        private bool CurrentZoneMatchesSiteZone()
        {
            var wrapLog = Log.Call<bool>();
            // but is the current portal also the one we're asking about?
            if (Context.Site == null || Context.Site.Id == Eav.Constants.NullId) return wrapLog("no", false); // this is the case when running out-of http-context
            if (AppIdentity == null) return wrapLog("yes", true); // this is the case when an app hasn't been selected yet, so it's an empty module, must be on current portal
            var pZone = Context.Site.ZoneId;
            var result = pZone == AppIdentity.ZoneId; // must match, to accept user as admin
            return wrapLog($"{result}", result);
        }

        private bool UserIsModuleEditor()
            => _userIsModuleEditor ??= Log.Intercept(nameof(UserIsModuleEditor),
                () =>
                {
                    if (Module == null) return false;
                    try
                    {
                        return _userPermissions.Value.IsAuthorized(ClaimsPrincipal, EntityNames.Module, Module.Id, PermissionNames.Edit);
                    }
                    catch
                    {
                        return false;
                    }
                });

        private bool? _userIsModuleEditor;

        private bool UserIsModuleAdmin()
            => Log.Intercept(nameof(UserIsModuleAdmin),
                UserIsModuleEditor);

    }
}
