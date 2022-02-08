using System;
using System.Collections.Generic;
using IntegrationSamples.SxcEdit01.Integration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Cms;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    //[AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.DefaultRouteRoot + "/cms" + WebApiConstants.DefaultRouteControllerAction)]
    [ApiController]
    public class EditController: IntStatefulControllerBase, IEditController
    {
        #region DI
        protected override string HistoryLogName => IntConstants.LogPrefix + ".UiCntr";

        public EditController(Dependencies dependencies,
            Lazy<EntityPickerBackend> entityBackend,
            Lazy<EditLoadBackend> loadBackend,
            Lazy<EditSaveBackend> saveBackendLazy,
            Lazy<HyperlinkBackend<int, int>> linkBackendLazy) : base(dependencies)
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
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
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

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _linkBackendLazy.Value.Init(Log).LookupHyperlink(appId, link, contentType, guid, field);

    }
}
