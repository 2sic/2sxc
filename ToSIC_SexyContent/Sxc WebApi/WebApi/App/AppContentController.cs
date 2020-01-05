using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Security;
using ToSic.Sxc.Serializers;
using ToSic.Sxc.WebApi.Cms.Refactor;
using Factory = ToSic.Eav.Factory;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [AllowAnonymous]
    public class AppContentController : SxcApiControllerBase
	{
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("Api.ApCont");
	    }


        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null, string cultureCode = null)
        {
            var wraplog = Log.Call($"get entities type:{contentType}, path:{appPath}, culture:{cultureCode}");

            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, CmsBlock);

            // verify that read-access to these content-types is permitted
            var permCheck = new MultiPermissionsTypes(CmsBlock, appIdentity.AppId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exp))
                throw exp;

            //2018-09-15 2dm replaced
            //var context = GetContext(SxcBlock, Log);
            //PerformSecurityCheck(appIdentity, contentType, Grants.Read, appPath == null ? context.Dnn.Module : null);
            var result = new EntityApi(appIdentity.AppId, Log).GetEntities(contentType, cultureCode);
            wraplog("found: " + result?.Count());
            return result;
        }

        #endregion


        #region GetOne by ID / GUID

	    [HttpGet]
	    [AllowAnonymous] // will check security internally, so assume no requirements
	    public Dictionary<string, object> GetOne(string contentType, int id, string appPath = null)
	        => GetAndSerializeOneAfterSecurityChecks(contentType,
	            appId => new EntityApi(appId, Log).GetOrThrow(contentType, id), appPath);


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, object> GetOne(string contentType, Guid guid, string appPath = null)
            => GetAndSerializeOneAfterSecurityChecks(contentType,
                appId => new EntityApi(appId, Log).GetOrThrow(contentType, guid), appPath);
        


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<int, IEntity> getOne, string appPath)
        {
            Log.Add($"get and serialie after security check type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, CmsBlock);

            var itm = getOne(appIdentity.AppId);
            var permCheck = new MultiPermissionsItems(CmsBlock, appIdentity.AppId, itm, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
                throw exception;
            return InitEavAndSerializer(appIdentity.AppId).Convert(itm);
        }

        #endregion

        #region ContentBlock - retrieving data of the current instance as is
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
	    public HttpResponseMessage GetContentBlockData()
        {
            Log.Add("get content block data");
            // Important note: we are NOT supporting url-view switch at the moment for this
            // reason is, that this kind of data-access is fairly special
            // and not recommended for future use cases, where we have the query etc.
            // IF you want to support View-switching in this, do a deep review w/2dm first!
            // - note that it's really not needed, as you can always use a query or something similar instead
            // - not also that if ever you do support view switching, you will need to ensure security checks

            var dataHandler = new GetContentBlockDataLight(CmsBlock);

            // must access engine to ensure pre-processing of data has happened, 
            // especially if the cshtml contains a override void CustomizeData()
            ((Sxc.Blocks.CmsBlock)CmsBlock).GetRenderingEngine(Purpose.PublishData);  

            var dataSource = CmsBlock.Block.Data;
            string json;
            if (dataSource.Publish.Enabled)
            {
                var publishedStreams = dataSource.Publish.Streams;
                var streamList = publishedStreams.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                json = dataHandler.GetJsonFromStreams(dataSource, streamList);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {ReasonPhrase = dataHandler.GeneratePleaseEnableDataError(CmsBlock.Container.Id)});
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
        #endregion



        #region Create
        [HttpPost]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, object> CreateOrUpdate([FromUri] string contentType, [FromBody] Dictionary<string, object> newContentItem, [FromUri] int? id = null, [FromUri] string appPath = null)
        {
            Log.Add($"create or update type:{contentType}, id:{id}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, CmsBlock);

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null
                ? null
                : new EntityApi(appIdentity.AppId, Log).GetOrThrow(contentType, id.Value);

            var ok = itm == null
                ? new MultiPermissionsTypes(CmsBlock, appIdentity.AppId, contentType, Log)
                    .EnsureAll(Grants.Create.AsSet(), out var exp)
                : new MultiPermissionsItems(CmsBlock, appIdentity.AppId, itm, Log)
                    .EnsureAll(Grants.Update.AsSet(), out exp);
            if (!ok)
                throw exp;

            //2018-09-15 2dm moved/disabled
            //var context = GetContext(SxcBlock, Log);
            //PerformSecurityCheck(appIdentity, contentType, perm, appPath == null ? context.Dnn.Module : null, itm);

            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = new AppContentEntityBuilder(Log)
                .CreateEntityDictionary(contentType, newContentItem, appIdentity.AppId);

            var userName = new DnnUser().IdentityToken;

            // try to create
            var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
            // 2018-09-22 new
            // todo: something looks wrong here, I think create/update would fail if it doesn't have a moduleid
            var currentApp = new Apps.App(new DnnTenant(PortalSettings), appIdentity.ZoneId, appIdentity.AppId, 
                ConfigurationProvider.Build(false, publish.IsEnabled(ActiveModule.ModuleID),
                    CmsBlock.Block.Data.Configuration.LookUps), true, Log);
            // 2018-09-22 old
            //currentApp.InitData(false, 
            //    publish.IsEnabled(ActiveModule.ModuleID), 
            //    SxcBlock.Data.ConfigurationProvider);
            if (id == null)
            {
                currentApp.Data.Create(contentType, cleanedNewItem, userName);
                // Todo: try to return the newly created object 
                return null;
            }

            currentApp.Data.Update(id.Value, cleanedNewItem, userName);
            return InitEavAndSerializer(appIdentity.AppId).Convert(currentApp.Data.List.One(id.Value));
        }

        #endregion



        #region Delete

        [HttpDelete]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, int id, [FromUri] string appPath = null)
        {
            Log.Add($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, CmsBlock);

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            var itm = new EntityApi(appIdentity.AppId, Log).GetOrThrow(contentType, id);
            var permCheck = new MultiPermissionsItems(CmsBlock, appIdentity.AppId, itm, Log);
            if (!permCheck.EnsureAll(Grants.Delete.AsSet(), out var exception))
                throw exception;
            //2018-09-15 2dm moved/disabled
            //var context = GetContext(SxcBlock, Log);
            //PerformSecurityCheck(appIdentity, itm.Type.Name, Grants.Delete, appPath == null ? context.Dnn.Module : null, itm);
            new EntityApi(appIdentity.AppId, Log).Delete(itm.Type.Name, id);
        }


	    [HttpDelete]
	    [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, Guid guid, [FromUri] string appPath = null)
	    {
            Log.Add($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, CmsBlock);

            var entityApi = new EntityApi(appIdentity.AppId, Log);
	        var itm = entityApi.GetOrThrow(contentType == "any" ? null : contentType, guid);

	        var permCheck = new MultiPermissionsItems(CmsBlock, appIdentity.AppId, itm, Log);
	        if (!permCheck.EnsureAll(Grants.Delete.AsSet(), out var exception))
	            throw exception;

            entityApi.Delete(itm.Type.Name, guid);
        }

        #endregion


        #region helpers / initializers to prep the EAV and Serializer

        // 2018-04-18 2dm disabled init-serializer, don't think it's actually ever used!
        private Eav.Serialization.EntitiesToDictionary InitEavAndSerializer(int appId)
        {
            Log.Add($"init eav for a#{appId}");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            var ser = Eav.WebApi.Helpers.Serializers.GetSerializerWithGuidEnabled();
            ((Serializer)ser).Cms = CmsBlock;
            return ser;
        }
        #endregion

    }
}
