using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Microsoft.Practices.Unity;
using ToSic.Eav;
using ToSic.Eav.Serializers;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAV.FormlyEditUI
{
	/// <summary>
	/// Web API Controller for the Pipeline Designer UI
	/// </summary>
	public class EntityController : SxcApiController
    {
        // todo refactor split

        //private readonly Eav.ManagementUI.FormlyEditUI.EntityController _eavController;
        //public EntityController()
        //{
        //    _eavController = new Eav.ManagementUI.FormlyEditUI.EntityController();
        //}


        //private Serializer _serializer;
        //public Serializer Serializer
        //{
        //    get
        //    {
        //        if (_serializer == null)
        //        {
        //            _serializer = Factory.Container.Resolve<Serializer>();
        //            _serializer.IncludeGuid = true;
        //        }
        //        return _serializer;
        //    }
        //}

        ///// <summary>
        ///// Returns the configuration for a content type
        ///// </summary>
        //[HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        //public object GetEntity(int zoneId, int appId, int entityId)
        //{
        //    return _eavController.GetEntity(this.App.ZoneId, this.App.ZoneId, entityId);
        //}

	}
}