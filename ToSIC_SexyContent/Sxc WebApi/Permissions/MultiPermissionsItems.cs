using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class MultiPermissionsItems: MultiPermissionsApp
    {
        protected List<IEntity> Items;

        public MultiPermissionsItems(SxcInstance sxcInstance, int appId, IEntity item, Log parentLog) 
            : base(sxcInstance, appId, parentLog)
        {
            Items = new List<IEntity> {item};
        }

        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => Items.ToDictionary(i => i.EntityId.ToString(), BuildItemPermissionChecker);

        /// <summary>
        /// Creates a permission checker for an type in this app
        /// </summary>
        /// <returns></returns>
        protected IPermissionCheck BuildItemPermissionChecker(IEntity item)
        {
            Log.Call("BuildItemPermissionChecker", $"{item.EntityId}");
            // now do relevant security checks
            return BuildPermissionChecker(item.Type, item);
        }


    }
}
