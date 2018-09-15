using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class PermissionsForAppAndTypes: PermissionsForApp, IContextPermissionCheck
    {
        public Dictionary<string, IPermissionCheck> PermissionCheckers { get; protected set; }

        public PermissionsForAppAndTypes(SxcInstance sxcInstance, int appId, string contentType, Log parentLog) 
            : this(sxcInstance, appId, new []{contentType}, parentLog)
        {
        }

        public PermissionsForAppAndTypes(SxcInstance sxcInstance, int appId, string[] contentTypes, Log parentLog) : base(sxcInstance, appId, parentLog)
        {
            BuildPermissionCheckers(contentTypes);
        }

        public PermissionsForAppAndTypes(SxcInstance sxcInstance, int appId, List<ItemIdentifier> items, Log parentLog) : base(sxcInstance, appId, parentLog)
        {
            var contentTypes = ExtractTypeNamesFromItems(items);
            BuildPermissionCheckers(contentTypes);
        }


        private void BuildPermissionCheckers(IEnumerable<string> contentTypes) 
            => PermissionCheckers = contentTypes.ToDictionary(t => t, TypePermissionChecker);


        public new bool Ensure(List<Grants> grants, out HttpResponseException preparedException)
        {
            foreach (var set in PermissionCheckers)
                if (!Ensure(grants, set.Value, set.Key, out preparedException))
                    return false;

            preparedException = null;
            return true;
        }

        protected bool Ensure(List<Grants> grants, IPermissionCheck permChecker, string typeName, out HttpResponseException preparedException)
        {
            var wrapLog = Log.Call("Ensure", () => $"[{string.Join(",", grants)}], {typeName}", () => "or throw");
            // temp!!!
            //_lastChecker = permChecker;
            //var permChecker = _lastChecker = TypePermissionChecker(typeName);

            if (!permChecker.UserMay(grants))
            {
                Log.Add("permissions not ok");
                preparedException = Http.PermissionDenied("required permissions for this type are not given");
                throw preparedException;
            }
            wrapLog("ok");
            preparedException = null;
            return true;
        }

        protected List<string> ExtractTypeNamesFromItems(List<ItemIdentifier> items)
        {
            var appMan = new AppRuntime(App, Log);

            // build list of type names
            var typeNames = items.Select(item =>
            {
                var typeName = item.ContentTypeName;
                return !string.IsNullOrEmpty(typeName) || item.EntityId == 0
                    ? typeName
                    : appMan.Entities.Get(item.EntityId).Type.StaticName;
            }).ToList();

            // make sure we have at least one entry, so the checks will work
            if (typeNames.Count == 0)
                typeNames.Add(null);
            return typeNames;
        }
        

        /// <summary>
        /// Creates a permission checker for an app
        /// Optionally you can provide a type-name, which will be 
        /// included in the permission check
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal IPermissionCheck TypePermissionChecker(string typeName)
        {
            Log.Call("TypePermissionChecker", $"{typeName}");
            // now do relevant security checks
            var type = typeName == null
                ? null
                : new AppRuntime(App, Log).ContentTypes.Get(typeName);

            // user has edit permissions on this app, and it's the same app as the user is coming from
            return new DnnPermissionCheck(Log,
                instance: SxcInstance.EnvInstance,
                app: App,
                portal: PortalForSecurityCheck,
                targetType: type);
        }


    }
}
