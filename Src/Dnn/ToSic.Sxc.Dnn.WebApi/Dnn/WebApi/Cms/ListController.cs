using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Web.Http;
using ToSic.Eav.WebApi.Cms;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public class ListController: SxcApiControllerBase<ListControllerReal>, IListController
    {
        public ListController() : base(ListControllerReal.LogSuffix) { }

        /// <inheritdoc />
        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        [HttpPost]
        public void Move(Guid? parent, string fields, int index, int toIndex)
            => SysHlp.Real.Move(parent, fields, index, toIndex);


        /// <inheritdoc />
        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        [HttpDelete]
        public void Delete(Guid? parent, string fields, int index)
            => SysHlp.Real.Delete(parent, fields, index);

    }
}
