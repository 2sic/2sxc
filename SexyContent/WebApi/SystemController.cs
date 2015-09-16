using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	public class SystemController : SxcApiController
	{

	    [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public dynamic GetLanguages()
	    {
	        var portalId = Dnn.Portal.PortalId;
            var zoneId = SexyContent.GetZoneID(portalId);
            var cultures = SexyContent.GetCulturesWithActiveState(portalId, zoneId.Value).Select(c => new
            {
                c.Code,
                Culture = c.Text,
                IsEnabled = c.Active
            });
	        return cultures;
	    }

	    /// <summary>
	    /// Helper to prepare a quick-info about 1 content type
	    /// </summary>
	    /// <param name="allCTs"></param>
	    /// <param name="staticName"></param>
	    /// <param name="maybeEntity"></param>
	    /// <returns></returns>
	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SwitchLanguage(string cultureCode, bool enable)
	    {
            //var Item = e.Item as GridEditableItem;
            // var CultureCode = Item.GetDataKeyValue("Code").ToString();

            // Activate or Deactivate the Culture
            Sexy.SetCultureState(cultureCode, enable, Dnn.Portal.PortalId);
	    }

        

    }
}