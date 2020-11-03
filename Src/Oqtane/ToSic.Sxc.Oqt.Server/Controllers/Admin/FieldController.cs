using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/field/[action]")]
    public class FieldController : SxcStatefulControllerBase, IFieldController
    {
        private readonly Lazy<AppRuntime> _appRuntime;
        private readonly Lazy<ContentTypeApi> _ctApiLazy;
        protected override string HistoryLogName => "Api.Fields";
        public FieldController(StatefulControllerDependencies dependencies, Lazy<AppRuntime> appRuntime, Lazy<ContentTypeApi> ctApiLazy) : base(dependencies)
        {
            _appRuntime = appRuntime;
            _ctApiLazy = ctApiLazy;
        }

        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        public IEnumerable<ContentTypeFieldDto> All(int appId, string staticName) => _ctApiLazy.Value.Init(appId, Log).GetFields(appId, staticName);

        /// <summary>
        /// Used to be GET ContentType/DataTypes
        /// </summary>
        [HttpGet]
        public string[] DataTypes(int appId) => _ctApiLazy.Value.Init(appId, Log).DataTypes(appId);

        /// <summary>
        /// Used to be GET ContentType/InputTypes
        /// </summary>
	    [HttpGet]
        public List<InputTypeInfo> InputTypes(int appId) => _appRuntime.Value.Init(State.Identity(null, appId), true, Log).ContentTypes.GetInputTypes();

        /// <summary>
        /// Used to be GET ContentType/AddField
        /// </summary>
        [HttpPost]
        public int Add(int appId, int contentTypeId, string staticName, string type, string inputType, int index)
            => _ctApiLazy.Value.Init(appId, Log).AddField(contentTypeId, staticName, type, inputType, index);

        /// <summary>
        /// Used to be GET ContentType/DeleteField
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int contentTypeId, int attributeId)
            => _ctApiLazy.Value.Init(appId, Log).DeleteField(appId, contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentType/Reorder
        /// </summary>
	    [HttpPost]
        public bool Sort(int appId, int contentTypeId, string order) => _ctApiLazy.Value.Init(appId, Log).Reorder(appId, contentTypeId, order);


        /// <summary>
        /// Used to be GET ContentType/UpdateInputType
        /// </summary>
        [HttpPost]
        public bool InputType(int appId, int attributeId, string inputType) => _ctApiLazy.Value.Init(appId, Log).SetInputType(attributeId, inputType);

        #endregion

        /// <summary>
        /// Used to be GET ContentType/Rename
        /// </summary>
        [HttpPost]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName) => _ctApiLazy.Value.Init(appId, Log).Rename(appId, contentTypeId, attributeId, newName);
    }
}