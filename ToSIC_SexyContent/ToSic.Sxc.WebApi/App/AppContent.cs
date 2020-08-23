using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.App
{
    public class AppContent: WebApiBackendBase
    {
        #region Constructor / DI

        public AppContent() : base("Sxc.ApiApC")
        {

        }

        public AppContent Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            return this;
        }

        #endregion


        #region Get Items

        internal IEnumerable<Dictionary<string, object>> GetItems(IInstanceContext context, string contentType, IBlockBuilder ctxBlockBuilder, string appPath = null)
        {
            var wrapLog = Log.Call($"get entities type:{contentType}, path:{appPath}");

            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ctxBlockBuilder);

            // get the app - if we have the context from the request, use that, otherwise generate full app
            var app = ctxBlockBuilder == null
                ? Factory.Resolve<Apps.App>().Init(appIdentity, Log)
                : GetApp(appIdentity.AppId, ctxBlockBuilder);

            // verify that read-access to these content-types is permitted
            var permCheck = new MultiPermissionsTypes(context, app, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var result = new EntityApi(appIdentity.AppId, permCheck.EnsureAny(GrantSets.ReadDraft), Log)
                .GetEntities(contentType)
                ?.ToList();
            wrapLog("found: " + result?.Count);
            return result;
        }


        #endregion

        #region Get One 

        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, object> GetOne(IInstanceContext context, IBlockBuilder ctxBlockBuilder, string contentType, Func<EntityApi, IEntity> getOne, string appPath)
        {
            Log.Add($"get and serialize after security check type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ctxBlockBuilder);

            var entityApi = new EntityApi(appIdentity.AppId, true, Log);

            var itm = getOne(entityApi);
            var permCheck = new MultiPermissionsItems(context, GetApp(appIdentity.AppId, ctxBlockBuilder), itm, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // in case draft wasn't allow, get again with more restricted permissions 
            if (!permCheck.EnsureAny(GrantSets.ReadDraft))
            {
                entityApi = new EntityApi(appIdentity.AppId, false, Log);
                itm = getOne(entityApi);
            }

            return InitEavAndSerializer(appIdentity.AppId, ctxBlockBuilder?.UserMayEdit ?? false).Convert(itm);
        }


        #endregion

        #region CreateOrUpdate


        internal Dictionary<string, object> CreateOrUpdate(IInstanceContext context, IBlockBuilder ctxBlockBuilder, string contentType, Dictionary<string, object> newContentItem, int? id = null, string appPath = null)
        {
            Log.Add($"create or update type:{contentType}, id:{id}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ctxBlockBuilder);

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null
                ? null
                : new EntityApi(appIdentity.AppId, true, Log).GetOrThrow(contentType, id.Value);

            var ok = itm == null
                ? new MultiPermissionsTypes(context, GetApp(appIdentity.AppId, ctxBlockBuilder), contentType, Log)
                    .EnsureAll(Grants.Create.AsSet(), out var error)
                : new MultiPermissionsItems(context, GetApp(appIdentity.AppId, ctxBlockBuilder), itm, Log)
                    .EnsureAll(Grants.Update.AsSet(), out error);
            if (!ok)
                throw HttpException.PermissionDenied(error);

            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = new AppContentEntityBuilder(Log)
                .CreateEntityDictionary(contentType, newContentItem, appIdentity.AppId);

            var userName = context.User.IdentityToken;

            // try to create
            // 2020-08-21 disabled publish check, don't think it's relevant in API mode
            // var publish = Factory.Resolve<IPagePublishing>().Init(Log);
            // var enablePublish = publish.IsEnabled(context.Container.Id);
            var currentApp = GetApp(appIdentity.AppId, ctxBlockBuilder);
            //Factory.Resolve<Apps.App>().Init(appIdentity,
            //    ConfigurationProvider.Build(false, false,
            //        ctxBlockBuilder?.Block.Data.Configuration.LookUps), true, Log);

            if (id == null)
            {
                var entity = currentApp.Data.Create(contentType, cleanedNewItem, userName);
                id = entity.EntityId;
            }
            else
                currentApp.Data.Update(id.Value, cleanedNewItem, userName);

            return InitEavAndSerializer(appIdentity.AppId, ctxBlockBuilder?.UserMayEdit ?? false)
                .Convert(currentApp.Data.List.One(id.Value));
        }

        #endregion
        
        #region helpers / initializers to prep the EAV and Serializer

        private Eav.Conversion.EntitiesToDictionary InitEavAndSerializer(int appId, bool userMayEdit)
        {
            Log.Add($"init eav for a#{appId}");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            var ser = Eav.WebApi.Helpers.Serializers.GetSerializerWithGuidEnabled();
            ((DataToDictionary)ser).WithEdit = userMayEdit;
            return ser;
        }
        #endregion


        #region Delete

        internal void Delete(IInstanceContext context, IBlockBuilder ctxBlockBuilder, string contentType, int id, string appPath)
        {
            Log.Add($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ctxBlockBuilder);

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            var entityApi = new EntityApi(appIdentity.AppId, true, Log);
            var itm = entityApi.GetOrThrow(contentType, id);
            var permCheck = new MultiPermissionsItems(context, GetApp(appIdentity.AppId, ctxBlockBuilder), itm, Log);
            if (!permCheck.EnsureAll(Grants.Delete.AsSet(), out var error))
                throw HttpException.PermissionDenied(error);
            entityApi.Delete(itm.Type.Name, id);
        }

        internal void Delete(IInstanceContext context, IBlockBuilder ctxBlockBuilder, string contentType, Guid guid, string appPath)
        {
            Log.Add($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ctxBlockBuilder);

            var entityApi = new EntityApi(appIdentity.AppId, true, Log);
            var itm = entityApi.GetOrThrow(contentType == "any" ? null : contentType, guid);

            var permCheck = new MultiPermissionsItems(context, GetApp(appIdentity.AppId, ctxBlockBuilder), itm, Log);
            if (!permCheck.EnsureAll(Grants.Delete.AsSet(), out var error))
                throw HttpException.PermissionDenied(error);

            entityApi.Delete(itm.Type.Name, guid);
        }


        #endregion
    }
}
