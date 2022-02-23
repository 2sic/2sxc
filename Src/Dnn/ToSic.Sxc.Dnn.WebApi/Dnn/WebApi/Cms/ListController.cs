using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public class ListController: SxcApiControllerBase
    {
        public ListController() : base("List") { }

        private FieldListBackend FieldBacked => GetService<FieldListBackend>().Init(Log);

        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="toIndex"></param>
        [HttpPost]
        public void Move(Guid? parent, string fields, int index, int toIndex)
            => FieldBacked.ChangeOrder(parent, fields, index, toIndex);


        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        [HttpDelete]
        public void Delete(Guid? parent, string fields, int index)
            => FieldBacked.Remove(parent, fields, index);

    }
}
