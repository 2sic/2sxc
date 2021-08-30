using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/field/[action]")]
    public class FieldController : OqtStatefulControllerBase, IFieldController
    {
        private readonly Lazy<AppRuntime> _appRuntime;
        private readonly Lazy<ContentTypeApi> _ctApiLazy;
        protected override string HistoryLogName => "Api.Fields";
        public FieldController(Lazy<AppRuntime> appRuntime, Lazy<ContentTypeApi> ctApiLazy)
        {
            _appRuntime = appRuntime;
            _ctApiLazy = ctApiLazy;
        }

        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        public IEnumerable<ContentTypeFieldDto> All(int appId, string staticName) => _ctApiLazy.Value.Init(appId, Log).GetFields(staticName);

        /// <summary>
        /// Used to be GET ContentType/DataTypes
        /// </summary>
        [HttpGet]
        public string[] DataTypes(int appId) => _ctApiLazy.Value.Init(appId, Log).DataTypes();

        /// <summary>
        /// Used to be GET ContentType/InputTypes
        /// </summary>
	    [HttpGet]
        public List<InputTypeInfo> InputTypes(int appId) => _appRuntime.Value.Init(appId, true, Log).ContentTypes.GetInputTypes();

        /// <inheritdoc />
        [HttpGet]
        public Dictionary<string, string> ReservedNames() => Attributes.ReservedNames;

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
            => _ctApiLazy.Value.Init(appId, Log).DeleteField(contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentType/Reorder
        /// </summary>
	    [HttpPost]
        public bool Sort(int appId, int contentTypeId, string order) => _ctApiLazy.Value.Init(appId, Log).Reorder(contentTypeId, order);


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
        public void Rename(int appId, int contentTypeId, int attributeId, string newName) => _ctApiLazy.Value.Init(appId, Log).Rename(contentTypeId, attributeId, newName);
    }
}