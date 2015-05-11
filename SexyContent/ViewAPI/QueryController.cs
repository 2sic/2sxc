using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class QueryController : SxcApiController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        [ValidateAntiForgeryToken]                                          // currently only available for modules, so always require the security token
        public dynamic Query([FromUri] string name)
        {
            // return "success - your query is '" + queryName + "'";


            // Try to find the query, abort if not found
            if (!App.Query.ContainsKey(name))
                throw new Exception("Can't find Query with name '" + name + "'");

            var query = App.Query[name] as DeferredPipelineQuery;

            var queryConf = query.QueryDefinition;

            var permissionChecker = new Permissions(App.ZoneId, App.AppId, queryConf.EntityGuid, Dnn.Module);

            var readAllowed = permissionChecker.UserMay('r');

            if (readAllowed)
                return Sxc.Serializer.Prepare(query);
            else
                throw new Exception("Not allowed for query '" + name + "'");


        }



    }
}