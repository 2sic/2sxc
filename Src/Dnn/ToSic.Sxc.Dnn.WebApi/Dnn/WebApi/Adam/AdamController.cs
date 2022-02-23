using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Dnn.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
    [ValidateAntiForgeryToken]
    public class AdamController : SxcApiControllerBase, IAdamController<int>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public AdamController() : base("Adam") { }

        private AdamControllerReal<int> Real => GetService<AdamControllerReal<int>>().Init(Log);


        [HttpPost]
        [HttpPut]
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false) 
            => Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request), appId, contentType, guid, field, subFolder, usePortalRoot);


        // test method to provide a public API for accessing adam items easily
        // not sure if it is ever used
        // 2022-02-22 2dm - disabled
        //[HttpGet]
        //public IEnumerable<AdamItemDto> Items(string contentType, Guid guid, string field, string folder = "")
        //{
        //    // if app-path specified, use that app, otherwise use from context
        //    const int AutoDetect = -1;
        //    return Items(AutoDetect, contentType, guid, field, folder);
        //}

        [HttpGet]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
            => Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot);


        [HttpPost]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);

        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}