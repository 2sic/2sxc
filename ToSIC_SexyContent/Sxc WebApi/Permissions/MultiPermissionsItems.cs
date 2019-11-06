using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class MultiPermissionsItems: MultiPermissionsApp
    {
        protected List<IEntity> Items;

        public MultiPermissionsItems(SxcInstance sxcInstance, int appId, IEntity item, ILog parentLog) 
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
