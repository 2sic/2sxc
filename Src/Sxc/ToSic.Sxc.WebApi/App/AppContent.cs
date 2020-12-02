using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Conversion;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Conversion;

namespace ToSic.Sxc.WebApi.App
{
    public class AppContent: BlockWebApiBackendWithContext<AppContent>
    {

        #region Constructor / DI

        public AppContent(IServiceProvider sp, EntityApi entityApi, 
            Lazy<CmsManager> cmsManagerLazy, 
            Lazy<EntitiesToDictionary> entToDicLazy, IContextResolver ctxResolver) : base(sp, cmsManagerLazy, ctxResolver, "Sxc.ApiApC")
        {
            _entityApi = entityApi;
            _entToDicLazy = entToDicLazy;
        }
        private readonly EntityApi _entityApi;
        private readonly Lazy<EntitiesToDictionary> _entToDicLazy;

        #endregion


        #region Get Items

        internal IEnumerable<Dictionary<string, object>> GetItems(string contentType, string appPath = null)
        {
            var wrapLog = Log.Call($"get entities type:{contentType}, path:{appPath}");

            // if app-path specified, use that app, otherwise use from context
            //var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ContextOfAppOrBlock);

            // get the app - if we have the context from the request, use that, otherwise generate full app
            //var app = // _block == null
            //    //? ServiceProvider.Build<Apps.App>().Init(ServiceProvider.Build<AppConfigDelegate>().Init(Log), appIdentity, Log, ContextOfAppOrBlock.UserMayEdit)
            //    /*:*/ GetApp(appIdentity.AppId, ContextOfAppOrBlock.UserMayEdit);

            // verify that read-access to these content-types is permitted
            var permCheck = ThrowIfNotAllowedInType(contentType, GrantSets.ReadSomething, Context.AppState);

            var result = _entityApi.Init(Context.AppState.AppId, permCheck.EnsureAny(GrantSets.ReadDraft), Log)
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
        internal Dictionary<string, object> GetOne(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath)
        {
            Log.Add($"get and serialize after security check type:{contentType}, path:{appPath}");

            //var context = _ctxResolver.AppNameRouteBlock(appPath);

            // if app-path specified, use that app, otherwise use from context
            //var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ContextOfAppOrBlock);

            //var appState = State.Get(appIdentity.AppId);

            // first try to find in all entities incl. drafts
            var itm = getOne(Context.AppState.List);
            var permCheck = ThrowIfNotAllowedInItem(itm, GrantSets.ReadSomething, GetApp(Context.AppState.AppId, Context.UserMayEdit));

            // in case draft wasn't allow, get again with more restricted permissions 
            if (!permCheck.EnsureAny(GrantSets.ReadDraft)) 
                itm = getOne(Context.AppState.ListPublished);

            return InitEavAndSerializer(Context.AppState.AppId, Context.UserMayEdit).Convert(itm);
        }


        #endregion

        #region CreateOrUpdate


        internal Dictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object> newContentItem, int? id = null, string appPath = null)
        {
            Log.Add($"create or update type:{contentType}, id:{id}, path:{appPath}");

            //var context = _ctxResolver.AppNameRouteBlock(appPath);

            // if app-path specified, use that app, otherwise use from context
            //var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ContextOfAppOrBlock);

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null
                ? null
                : Context.AppState.List.GetOrThrow(contentType, id.Value);

            var realApp = GetApp(Context.AppState.AppId, Context.UserMayEdit);
            if (itm == null) ThrowIfNotAllowedInType(contentType, Grants.Create.AsSet(), realApp);
            else ThrowIfNotAllowedInItem(itm, Grants.Update.AsSet(), realApp);

            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = new AppContentEntityBuilder(Log)
                .CreateEntityDictionary(contentType, newContentItem, Context.AppState.AppId);

            var userName = Context.User.IdentityToken;

            if (id == null)
            {
                var entity = realApp.Data.Create(contentType, cleanedNewItem, userName);
                id = entity.EntityId;
            }
            else
                realApp.Data.Update(id.Value, cleanedNewItem, userName);

            return InitEavAndSerializer(Context.AppState.AppId, Context.UserMayEdit)
                .Convert(realApp.Data.Immutable.One(id.Value));
        }

        #endregion
        
        #region helpers / initializers to prep the EAV and Serializer

        private EntitiesToDictionary InitEavAndSerializer(int appId, bool userMayEdit)
        {
            Log.Add($"init eav for a#{appId}");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            var ser = _entToDicLazy.Value.EnableGuids();// Eav.WebApi.Helpers.Serializers.GetSerializerWithGuidEnabled();
            ((DataToDictionary)ser).WithEdit = userMayEdit;
            return ser;
        }
        #endregion


        #region Delete

        internal void Delete(string contentType, int id, string appPath)
        {
            Log.Add($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context

            //var appId = AppFinder.GetAppIdFromPathOrContext(appPath, ContextOfAppOrBlock);

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            //var context = _ctxResolver.AppNameRouteBlock(appPath);

            var entityApi = _entityApi.Init(Context.AppState.AppId, true, Log);
            var itm = entityApi.AppRead.AppState.List.GetOrThrow(contentType, id);
            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), Context.AppState /*GetApp(appId.AppId, ContextOfAppOrBlock.UserMayEdit)*/);
            entityApi.Delete(itm.Type.Name, id);
        }

        internal void Delete(string contentType, Guid guid, string appPath)
        {
            Log.Add($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            //var appIdentity = AppFinder.GetAppIdFromPathOrContext(appPath, ContextOfAppOrBlock);

            //var context = _ctxResolver.AppNameRouteBlock(appPath);
            var entityApi = _entityApi.Init(Context.AppState.AppId, Context.UserMayEdit, Log);
            var itm = Context.AppState.List.GetOrThrow(contentType == "any" ? null : contentType, guid);

            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), GetApp(Context.AppState.AppId, Context.UserMayEdit));

            entityApi.Delete(itm.Type.Name, guid);
        }


        #endregion

        #region Permission Checks

        protected MultiPermissionsTypes ThrowIfNotAllowedInType(string contentType, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsTypes>().Init(Context, alternateApp ?? Context.AppState, contentType, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        protected MultiPermissionsItems ThrowIfNotAllowedInItem(IEntity itm, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsItems>().Init(Context, alternateApp ?? Context.AppState, itm, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        #endregion
    }
}
