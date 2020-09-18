using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.ContentBlocks;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {
        private ContentBlockBackend Backend => Factory.Resolve<ContentBlockBackend>().Init(GetContext(), GetBlock(), Log);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            var entityId = Backend.NewBlock(parentId, field, sortOrder, app, guid);

            // now return a rendered instance
            var newContentBlock = new BlockFromEntity().Init(GetBlock(), entityId, Log);
            return newContentBlock.BlockBuilder.Render();

        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? index = null)
            => Backend.AddItem(index);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void MoveItemInList(int parentId, string field, int indexFrom, int indexTo, [FromUri] bool partOfPage = false) 
            => Backend.MoveInList(parentId, field, indexFrom, indexTo, partOfPage);

        /// <summary>
        /// Delete a content-block inside a list of content-blocks
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="partOfPage"></param>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveItemInList(int parentId, string field, int index, [FromUri] bool partOfPage = false) 
            => Backend.RemoveBlockInList(parentId, field, index, partOfPage);
    }
}
