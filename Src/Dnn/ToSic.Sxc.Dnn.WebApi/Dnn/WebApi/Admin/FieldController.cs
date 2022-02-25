using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi;
using ContentTypeApi = ToSic.Eav.WebApi.ContentTypeApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class FieldController : SxcApiControllerBase<DummyControllerReal>, IFieldController
    {
        public FieldController() : base("Fields") { }

        private ContentTypeApi Backend => GetService<ContentTypeApi>();

        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        public IEnumerable<ContentTypeFieldDto> All(int appId, string staticName) => Backend.Init(appId, Log).GetFields(staticName);

        /// <summary>
        /// Used to be GET ContentType/DataTypes
        /// </summary>
        [HttpGet]
        public string[] DataTypes(int appId) => Backend.Init(appId, Log).DataTypes();

        /// <summary>
        /// Used to be GET ContentType/InputTypes
        /// </summary>
	    [HttpGet]
        public List<InputTypeInfo> InputTypes(int appId) => GetService<AppRuntime>().Init(appId, true, Log).ContentTypes.GetInputTypes();

        /// <inheritdoc />
        [HttpGet]
        public Dictionary<string, string> ReservedNames() => Attributes.ReservedNames;
        
        /// <summary>
        /// Used to be GET ContentType/AddField
        /// </summary>
        [HttpPost]
        public int Add(int appId, int contentTypeId, string staticName, string type, string inputType, int index) 
            => Backend.Init(appId, Log).AddField(contentTypeId, staticName, type, inputType, index);

        /// <summary>
        /// Used to be GET ContentType/DeleteField
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int contentTypeId, int attributeId) 
            => Backend.Init(appId, Log).DeleteField(contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentType/Reorder
        /// </summary>
	    [HttpPost]
        public bool Sort(int appId, int contentTypeId, string order) => Backend.Init(appId, Log).Reorder(contentTypeId, order);


        /// <summary>
        /// Used to be GET ContentType/UpdateInputType
        /// </summary>
        [HttpPost]
        public bool InputType(int appId, int attributeId, string inputType) => Backend.Init(appId, Log).SetInputType(attributeId, inputType);

        #endregion

        /// <summary>
        /// Used to be GET ContentType/Rename
        /// </summary>
        [HttpPost]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName) => Backend.Init(appId, Log).Rename(contentTypeId, attributeId, newName);

	}
}