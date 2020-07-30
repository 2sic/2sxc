using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public partial class ContentGroupController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sCoGr");
        }

        private BlockConfiguration GetContentGroup(Guid contentGroupGuid)
        {
            Log.Add($"get group:{contentGroupGuid}");
            var cms = new CmsRuntime(BlockBuilder.App, Log, true, false);
            var contentGroup = cms.Blocks.GetBlockConfig(contentGroupGuid);

            if (contentGroup == null)
                throw new Exception("BlockConfiguration with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
        }



        private List<IEntity> FindItemAndFieldTypeName(Guid guid, string part, out string attributeSetName)
        {
            var parent = BlockBuilder.App.Data.List.One(guid);
            if (parent == null) throw new Exception($"No item found for {guid}");
            if (!parent.Attributes.ContainsKey(part)) throw new Exception($"Could not find field {part} in item {guid}");
            var itemList = parent.Children(part);
            
            // find attribute-type-name
            var attribute = parent.Type[part];
            if (attribute == null) throw new Exception($"Attribute definition for '{part}' not found on the item {guid}");
            attributeSetName = attribute.EntityFieldItemTypePrimary();
            return itemList;
        }

        private List<IEntity> FindContentGroupAndTypeName(Guid guid, string part, out string attributeSetName)
        {
            var wrapLog = Log.Call<List<IEntity>>($"{guid}, {part}");
            var contentGroup = GetContentGroup(guid);
            attributeSetName = null;
            var partIsContent = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase);
            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var itemList = partIsContent ? contentGroup.Content : contentGroup.Header;

            if (itemList == null) return wrapLog(null, null);

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (contentGroup.View == null)
            {
                Log.Add("Something found, but doesn't seem to be a content-group. Cancel.");
                return wrapLog(null, null);
            }

            attributeSetName = partIsContent ? contentGroup.View.ContentType : contentGroup.View.HeaderType;
            return wrapLog(null, itemList);
        }





        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public SortedEntityItem Header(Guid guid)
        {
            Log.Add($"header for:{guid}");
            var cg = GetContentGroup(guid);

            // new in v11 - this call might be run on a non-content-block, in which case we return null
            if (cg.Entity.Type.Name != BlocksRuntime.BlockTypeName) return null;

            var header = cg.Header.FirstOrDefault();

            return new SortedEntityItem
            {
                Index = 0,
                Id = header?.EntityId ?? 0,
                Guid = header?.EntityGuid ?? Guid.Empty,
                Title = header?.GetBestTitle() ?? "",
                Type = header?.Type.StaticName?? cg.View.HeaderType
            };
        }


        #region helper classes for data transport / json interface

        public class SortedEntityItem
        {
            public int Index;
            public int Id;
            public Guid Guid;
            public string Title;
            public string Type;
        }

        #endregion
    }
}