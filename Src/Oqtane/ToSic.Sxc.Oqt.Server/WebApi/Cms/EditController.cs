using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using JsonOptions = ToSic.Eav.Serialization.JsonOptions;
using RealController = ToSic.Sxc.WebApi.Cms.EditControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]

    [ApiController]
    public class EditController: OqtStatefulControllerBase, IEditController
    {
        public EditController() : base(RealController.LogSuffix) { }

        private RealController Real => GetService<RealController>();


        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => Real.Load(items, appId);

        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [Authorize(Roles = RoleNames.Admin)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => Real.Save(package, appId, partOfPage);

        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public async Task<IEnumerable<EntityForPickerDto>> EntityPicker(
            int appId, 
            /*[FromBody] string[] items,*/ string contentTypeName = null  // [FromBody] is commented because it is sometimes null and options.AllowEmptyInputInBodyModelBinding = true is not working
            // 2dm 2023-01-22 #maybeSupportIncludeParentApps
            // bool? includeParentApps = null
        )
        {
            // get body as json (using this complicated way to read body because it is sometimes null and
            // options.AllowEmptyInputInBodyModelBinding = true or similar solutions are not working)
            // it is expected to be fixed in .NET 7
            // https://github.com/dotnet/aspnetcore/issues/29570
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var items = string.IsNullOrEmpty(body) ? null : JsonNode.Parse(body, JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions)?.AsArray().Select(n => n.ToString()).ToArray();
            return Real.EntityPicker(appId, items, contentTypeName/*, includeParentApps*/);
        }

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
        // TODO: 2DM please check permissions
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => Real.LinkInfo(link, appId, contentType, guid, field);

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Publish(int id)
            => Real.Publish(id);
    }
}
