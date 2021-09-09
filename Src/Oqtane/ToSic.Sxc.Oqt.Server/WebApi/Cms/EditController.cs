using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/edit/[action]")]

    [ValidateAntiForgeryToken]

    [ApiController]
    public class EditController: OqtStatefulControllerBase, IEditController
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public EditController(
            Lazy<EntityPickerBackend> entityBackend,
            Lazy<EditLoadBackend> loadBackend,
            Lazy<EditSaveBackend> saveBackendLazy,
            Lazy<HyperlinkBackend<int, int>> linkBackendLazy)
        {
            _entityBackend = entityBackend;
            _loadBackend = loadBackend;
            _saveBackendLazy = saveBackendLazy;
            _linkBackendLazy = linkBackendLazy;
        }

        private readonly Lazy<EntityPickerBackend> _entityBackend;
        private readonly Lazy<EditLoadBackend> _loadBackend;
        private readonly Lazy<EditSaveBackend> _saveBackendLazy;
        private readonly Lazy<HyperlinkBackend<int, int>> _linkBackendLazy;
        private EntityPickerBackend EntityBackend => _entityBackend.Value;

        #endregion

        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => _loadBackend.Value.Init(Log).Load(appId, items);

        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [Authorize(Roles = RoleNames.Admin)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => _saveBackendLazy.Value.Init(appId, Log).Save(package, partOfPage);

        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public async Task<IEnumerable<EntityForPickerDto>> EntityPicker(int appId, 
            /*[FromBody] string[] items,*/ string contentTypeName = null) // [FromBody] is commented because it is sometimes null and options.AllowEmptyInputInBodyModelBinding = true is not working
        {
            // get body as json (using this complicated way to read body because it is sometimes null and
            // options.AllowEmptyInputInBodyModelBinding = true or similar solutions are not working)
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var items = JsonConvert.DeserializeObject<string[]>(body);

            return EntityBackend.Init(Log)
                .GetAvailableEntities(appId, items, contentTypeName);
        }

        // 2021-04-13 2dm should be unused now
        ///// <inheritdoc />
        //[HttpGet]
        //// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //public string LookupLink(string link, int appId, string contentType = default, Guid guid = default, string field = default)
        //    => _linkBackendLazy.Value.Init(Log).ResolveHyperlink(appId, link, contentType, guid, field);

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
        // TODO: 2DM please check permissions
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _linkBackendLazy.Value.Init(Log).LookupHyperlink(appId, link, contentType, guid, field);

    }
}
