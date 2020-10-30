using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.ImportExport;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    public class LinkController : SxcStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.LnkCnt";

        public LinkController(StatefulControllerDependencies dependencies) : base(dependencies)
        {
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public void /*object*/ GetFileByPath(string relativePath)
        {
            WipConstants.DontDoAnythingImplementLater();
            //var dnnDynamicCode = new DnnDynamicCodeRoot().Init(GetBlock(), Log);
            //var portal = dnnDynamicCode.Dnn.Portal;
            //relativePath = relativePath.Replace(portal.HomeDirectory, "");
            //var file = FileManager.Instance.GetFile(portal.PortalId, relativePath);
            //if (file == null) return null;
            //var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
            //return FolderPermissionController.CanViewFolder(folder)
            //    ? new
            //    {
            //        file.FileId
            //    }
            //    : null;
        }

        /// <summary>
        /// This overload is only for resolving page-references, which need fewer parameters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public string Resolve(string hyperlink, int appId)
            => Resolve(hyperlink, appId, null, default, null);

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public string Resolve(string hyperlink, int appId, string contentType, Guid guid, string field)
            => new HyperlinkBackend<int, int>().Init(Log).ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);
    }
}