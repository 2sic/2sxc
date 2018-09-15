using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    /// <summary>
    /// A permission checker which is initialized with various items which must be checked
    /// Calling Ensure or similar will verify that all permission checks succeed
    /// </summary>
    internal abstract class MultiPermissionCheck: HasLog, IMultiPermissionCheck
    {
        /// <summary>
        /// All the permission checks that will be used
        /// </summary>
        public Dictionary<string, IPermissionCheck> PermissionCheckers
            => _permissionCheckers ?? (_permissionCheckers = InitializePermissionChecks());

        private Dictionary<string, IPermissionCheck> _permissionCheckers;

        protected MultiPermissionCheck(string logName, Log parentLog) : base(logName, parentLog)
        {}

        #region abstract methods

        protected abstract Dictionary<string, IPermissionCheck> InitializePermissionChecks();

        public abstract bool ZoneChangedAndNotSuperUser(out HttpResponseException exp);

        #endregion

        /// <summary>
        /// Ensure that all checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="preparedException">Out variable to use to throw upstream</param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool Ensure(List<Grants> grants, out HttpResponseException preparedException)
        {
            Log.Call("Ensure");
            foreach (var set in PermissionCheckers)
                if (!Ensure(grants, set, out preparedException))
                    return false;

            preparedException = null;
            return true;
        }

        /// <summary>
        /// Run a single permission check
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="set"></param>
        /// <param name="preparedException"></param>
        /// <returns></returns>
        protected bool Ensure(List<Grants> grants, KeyValuePair<string, IPermissionCheck> set, out HttpResponseException preparedException)
        {
            var wrapLog = Log.Call("Ensure", () => $"[{string.Join(",", grants)}], {set.Key}", () => "or throw");

            if (!set.Value.UserMay(grants))
            {
                Log.Add("permissions not ok");
                preparedException = Http.PermissionDenied("required permissions for this type are not given");
                return false;
            }
            wrapLog("ok");
            preparedException = null;
            return true;
        }



        public bool SameAppOrIsSuperUserAndEnsure(List<Grants> grants, out HttpResponseException preparedException)
        {
            if (!ZoneChangedAndNotSuperUser(out preparedException))
                return false;
            if (!Ensure(grants, out preparedException))
                return false;
            preparedException = null;
            return true;
        }



    }
}

