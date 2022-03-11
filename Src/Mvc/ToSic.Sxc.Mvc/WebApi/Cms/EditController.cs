using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [Route(WebApiConstants.WebApiRoot + "/cms/edit/[action]")]
    [ApiController]
    public class EditController: SxcStatefulControllerBase, IEditController
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public EditController(MvcContextBuilder contextBuilder, 
            Lazy<EntityPickerBackend> entityBackend, 
            Lazy<EditLoadBackend> loadBackend,
            Lazy<EditSaveBackend> saveBackendLazy,
            Lazy<HyperlinkBackend<int, int>> linkBackendLazy)
        {
            _contextBuilder = contextBuilder;
            _entityBackend = entityBackend;
            _loadBackend = loadBackend;
            _saveBackendLazy = saveBackendLazy;
            _linkBackendLazy = linkBackendLazy;
        }

        private readonly MvcContextBuilder _contextBuilder;
        private readonly Lazy<EntityPickerBackend> _entityBackend;
        private readonly Lazy<EditLoadBackend> _loadBackend;
        private readonly Lazy<EditSaveBackend> _saveBackendLazy;
        private readonly Lazy<HyperlinkBackend<int, int>> _linkBackendLazy;
        private EntityPickerBackend EntityBackend => _entityBackend.Value;

        #endregion


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public string Ping() => "test ping";

        [HttpPost]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            var context = GetContext();
            var block = GetBlock();
            var result = _loadBackend.Value
                .Init(Log)
                .Load(appId, items);
            return result;
        }

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => _saveBackendLazy.Value.Init(appId, Log).Save(package, partOfPage);

        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(int appId, [FromBody] string[] items, string contentTypeName = null)
            => EntityBackend.Init(Log).GetAvailableEntities(appId, items, contentTypeName);

        // 2021-04-13 2dm should be unused now
        ///// <inheritdoc />
        //[HttpGet]
        //// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //public string LookupLink(string link, int appId, string contentType = default, Guid guid = default, string field = default)
        //    => _linkBackendLazy.Value.Init(Log).ResolveHyperlink(appId, link, contentType, guid, field);

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _linkBackendLazy.Value.Init(Log).LookupHyperlink(appId, link, contentType, guid, field);

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
            => false /*Real.Publish(id)*/; // TODO

    }
}
