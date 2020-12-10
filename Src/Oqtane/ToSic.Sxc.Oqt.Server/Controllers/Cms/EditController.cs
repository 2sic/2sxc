using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.WebApiStateRoot + "/cms/edit/[action]")]
    [ApiController]
    public class EditController: OqtStatefulControllerBase
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public EditController(OqtUiContextBuilder uiContextBuilder,
            StatefulControllerDependencies dependencies,
            Lazy<EntityPickerBackend> entityBackend,
            Lazy<EditLoadBackend> loadBackend,
            Lazy<EditSaveBackend> saveBackendLazy) : base(dependencies)
        {
            _uiContextBuilder = uiContextBuilder;
            _entityBackend = entityBackend;
            _loadBackend = loadBackend;
            _saveBackendLazy = saveBackendLazy;
        }

        private readonly OqtUiContextBuilder _uiContextBuilder;
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
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            var context = GetContext();
            var result = _loadBackend.Value
                .Init(Log)
                .Load(appId, items);
            return result;
        }

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
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
