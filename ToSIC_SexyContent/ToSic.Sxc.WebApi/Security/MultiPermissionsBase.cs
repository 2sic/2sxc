using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;

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

        #endregion


        /// <summary>
        /// Verify that all checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool UserMayOnAll(List<Grants> grants) => EnsureAll(grants, out _);

        /// <summary>
        /// Ensure that all! checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="error">Out error message to use to throw upstream</param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool EnsureAll(List<Grants> grants, out string error)
        {
            var wrap = Log.Call<bool>();
            foreach (var set in PermissionCheckers)
                if (!set.Value.Ensure(grants, out error))
                    return wrap(false.ToString(), false);

            error = null;
            return wrap(true.ToString(), true);
        }

        /// <summary>
        /// Ensure that any! checks pass
        /// </summary>
        /// <param name="grants"></param>
        /// <returns>True if all pass, false if any one fails</returns>
        public bool EnsureAny(List<Grants> grants)
        {
            var wrap = Log.Call<bool>();
            return PermissionCheckers.Any(set => set.Value.Ensure(grants, out _)) 
                ? wrap(true.ToString(), true) 
                : wrap(false.ToString(), false);
        }

    }
}

