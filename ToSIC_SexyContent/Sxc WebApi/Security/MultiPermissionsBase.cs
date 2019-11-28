using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Security
{
    /// <summary>
    /// A permission checker which is initialized with various items which must be checked
    /// Calling Ensure or similar will verify that all permission checks succeed
    /// </summary>
    internal abstract class MultiPermissionsBase: HasLog, IMultiPermissionCheck
    {
        /// <summary>
        /// All the permission checks that will be used
        /// </summary>
        public Dictionary<string, IPermissionCheck> PermissionCheckers
            => _permissionCheckers ?? (_permissionCheckers = InitializePermissionChecks());
        private Dictionary<string, IPermissionCheck> _permissionCheckers;

        protected MultiPermissionsBase(string logName, ILog parentLog) : base(logName, parentLog)
        {}

        #region abstract methods

        protected abstract Dictionary<string, IPermissionCheck> InitializePermissionChecks();

        public abstract bool ZoneIsOfCurrentContextOrUserIsSuper(out HttpResponseException exp);

        #endregion


        /// <summary>
        /// Verify that all checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool UserMayOnAll(List<Grants> grants) => EnsureAll(grants, out var _);

        /// <summary>
        /// Ensure that all checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="preparedException">Out variable to use to throw upstream</param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool EnsureAll(List<Grants> grants, out HttpResponseException preparedException)
        {
            var wrap = Log.Call("EnsureAll");
            foreach (var set in PermissionCheckers)
                if (!set.Value.Ensure(grants, out preparedException))
                {
                    wrap(false.ToString());
                    return false;
                }

            preparedException = null;
            wrap(true.ToString());
            return true;
        }

        //2018-09-22 2dm removed again, as all internal checks actually already do this
        //public bool SameAppOrIsSuperUserAndEnsure(List<Grants> grants, out HttpResponseException preparedException)
        //{
        //    if (!ZoneIsOfCurrentContextOrUserIsSuper(out preparedException))
        //        return false;
        //    if (!EnsureAll(grants, out preparedException))
        //        return false;
        //    preparedException = null;
        //    return true;
        //}




    }
}

