using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class MultiPermissionsTypes: MultiPermissionsApp
    {
        protected IEnumerable<string> ContentTypes;

        public MultiPermissionsTypes(SxcInstance sxcInstance, int appId, string contentType, Log parentLog) 
            : this(sxcInstance, appId, new []{contentType}, parentLog)
        {
        }

        public MultiPermissionsTypes(SxcInstance sxcInstance, int appId, IEnumerable<string> contentTypes, Log parentLog) 
            : base(sxcInstance, appId, parentLog)
        {
            ContentTypes = contentTypes;
        }

        public MultiPermissionsTypes(SxcInstance sxcInstance, int appId, List<ItemIdentifier> items, Log parentLog) 
            : base(sxcInstance, appId, parentLog)
        {
            ContentTypes = ExtractTypeNamesFromItems(items);
        }


        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => ContentTypes.Distinct().ToDictionary(t => t, BuildTypePermissionChecker);


        private IEnumerable<string> ExtractTypeNamesFromItems(IEnumerable<ItemIdentifier> items)
        {
            // build list of type names
            var typeNames = items.Select(item =>
                !string.IsNullOrEmpty(item.ContentTypeName) || item.EntityId == 0
                    ? item.ContentTypeName
                    : AppRuntime.Entities.Get(item.EntityId).Type.StaticName);

            return typeNames;
        }

        protected AppRuntime AppRuntime => _appRuntime ?? (_appRuntime = new AppRuntime(App, Log));
        private AppRuntime _appRuntime;
        

        /// <summary>
        /// Creates a permission checker for an type in this app
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected IPermissionCheck BuildTypePermissionChecker(string typeName)
        {
            Log.Add($"BuildTypePermissionChecker({typeName})");
            // now do relevant security checks
            return BuildPermissionChecker(AppRuntime.ContentTypes.Get(typeName));
        }


    }
}
