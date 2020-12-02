using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [Route(WebApiConstants.WebApiRoot + "/cms/edit/[action]")]
    [ApiController]
    public class EditController: SxcStatefulControllerBase
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public EditController(MvcContextBuilder contextBuilder, 
            Lazy<EntityPickerBackend> entityBackend, 
            Lazy<EditLoadBackend> loadBackend,
            Lazy<EditSaveBackend> saveBackendLazy)
        {
            _contextBuilder = contextBuilder;
            _entityBackend = entityBackend;
            _loadBackend = loadBackend;
            _saveBackendLazy = saveBackendLazy;
        }

        private readonly MvcContextBuilder _contextBuilder;
        private readonly Lazy<EntityPickerBackend> _entityBackend;
        private readonly Lazy<EditLoadBackend> _loadBackend;
        private readonly Lazy<EditSaveBackend> _saveBackendLazy;
        private EntityPickerBackend EntityBackend => _entityBackend.Value;

        #endregion


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public string Ping() => "test ping";

        [HttpPost]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            var context = GetContext();
            var block = GetBlock();
            var result = _loadBackend.Value
                .Init(Log)
                .Load(_contextBuilder.Init(block), appId, items);
            return result;
        }

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOneDto package, int appId, bool partOfPage)
            => _saveBackendLazy.Value.Init(appId, Log)
                .Save(package, appId, partOfPage);

        /// <summary>
        /// Used to be GET Ui/GetAvailableEntities
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="items"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="dimensionId"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(int appId, [FromBody] string[] items,
            string contentTypeName = null, int? dimensionId = null)
            => EntityBackend.Init(Log)
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);

    }
}
