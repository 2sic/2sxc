using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.App
{
    public class AppContent : WebApiBackendBase<AppContent>
    {
        private CmsManager CmsManager => _cmsManager ?? (_cmsManager = _cmsManagerLazy.Value.Init(Context.AppState, _withDrafts, Log));
        private CmsManager _cmsManager;
        private bool _withDrafts = false;


        #region Constructor / DI
        protected IContextOfApp Context;

        public AppContent(IServiceProvider sp, EntityApi entityApi, Lazy<IConvertToEavLight> entToDicLazy, IContextResolver ctxResolver, Lazy<CmsManager> cmsManagerLazy) : base(sp, "Sxc.ApiApC")
        {
            _entityApi = entityApi;
            _entToDicLazy = entToDicLazy;
            _ctxResolver = ctxResolver;
            _cmsManagerLazy = cmsManagerLazy;   
        }
        private readonly EntityApi _entityApi;
        private readonly Lazy<IConvertToEavLight> _entToDicLazy;
        private readonly IContextResolver _ctxResolver;
        private readonly Lazy<CmsManager> _cmsManagerLazy;

        public AppContent Init(string appName, ILog parentLog)
        {
            Log.LinkTo(parentLog);

            // if app-path specified, use that app, otherwise use from context
            Context = _ctxResolver.AppNameRouteBlock(appName);

            return this;
        }
        #endregion


        #region Get Items

        public IEnumerable<IDictionary<string, object>> GetItems(string contentType, string appPath = null)
        {
            var wrapLog = Log.Call($"get entities type:{contentType}, path:{appPath}");

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
        public IDictionary<string, object> GetOne(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath)
        {
            Log.Add($"get and serialize after security check type:{contentType}, path:{appPath}");

            // first try to find in all entities incl. drafts
            var itm = getOne(Context.AppState.List);
            var permCheck = ThrowIfNotAllowedInItem(itm, GrantSets.ReadSomething, Context.AppState);

            // in case draft wasn't allow, get again with more restricted permissions 
            if (!permCheck.EnsureAny(GrantSets.ReadDraft))
                itm = getOne(Context.AppState.ListPublished);

            return InitEavAndSerializer(Context.AppState.AppId, Context.UserMayEdit).Convert(itm);
        }


        #endregion

        #region CreateOrUpdate


        public IDictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object> newContentItem, int? id = null, string appPath = null)
        {
            Log.Add($"create or update type:{contentType}, id:{id}, path:{appPath}");

            // if app-path specified, use that app, otherwise use from context

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null
                ? null
                : Context.AppState.List.GetOrThrow(contentType, id.Value);

            if (itm == null) ThrowIfNotAllowedInType(contentType, Grants.Create.AsSet(), Context.AppState);
            else ThrowIfNotAllowedInItem(itm, Grants.Update.AsSet(), Context.AppState);

            // Convert to case-insensitive dictionary just to be safe!
            var newContentItemCaseInsensitive = new Dictionary<string, object>(newContentItem, StringComparer.InvariantCultureIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = new AppContentEntityBuilder(Log)
                .CreateEntityDictionary(contentType, newContentItemCaseInsensitive, Context.AppState);

            var userName = Context.User.IdentityToken;

            var realApp = GetApp(Context.AppState.AppId, Context.UserMayEdit);
            if (id == null)
            {
                Log.Add($"create new entity because id is null");
                var metadata = GetMetadata(newContentItemCaseInsensitive);
                Log.Add($"metadata: {metadata}");
                var entity = realApp.Data.Create(contentType, cleanedNewItem, userName, metadata);
                Log.Add($"new entity created: {entity}");
                id = entity.EntityId;
                Log.Add($"new entity id: {id}");
                ParentRelationship(newContentItemCaseInsensitive);
            }
            else
                realApp.Data.Update(id.Value, cleanedNewItem, userName);

            return InitEavAndSerializer(Context.AppState.AppId, Context.UserMayEdit)
                .Convert(realApp.Data.List.One(id.Value));
        }

        private void ParentRelationship(IDictionary<string, object> newContentItemCaseInsensitive)
        {
            if (!newContentItemCaseInsensitive.Keys.Contains(Attributes.JsonKeyParentRelationship))
            {
                Log.Add("'ParentRelationship' key is missing");
                return;
            }

            var parentRelationship = newContentItemCaseInsensitive[Attributes.JsonKeyParentRelationship] as JObject;
            if (parentRelationship == null)
            {
                Log.Add("'ParentRelationship' value is null");
                return;
            }

            var parentGuid = (Guid?) parentRelationship["Parent"];
            if (!parentGuid.HasValue)
            {
                Log.Add("'Parent' guid is missing");
                return;
            };

            var entity = Context.AppState.List.One(parentGuid.Value);
            if (entity == null)
            {
                Log.Add("Parent entity is missing");
                return;
            }

            var entityId = (int?)parentRelationship["EntityId"];
            var ids = new[] { entityId as int? };
            var index = (int)parentRelationship["Index"];
            var willAdd = (bool?)parentRelationship["Add"] ?? true;
            var field = (string) parentRelationship["Field"];
            var fieldPair = new[] { field };

            if (willAdd) // this cannot be auto-detected, it must be specified
                CmsManager.Entities.FieldListAdd(entity, fieldPair, index, ids, _withDrafts);
            else
                CmsManager.Entities.FieldListReplaceIfModified(entity, fieldPair, index, ids, _withDrafts);

            Log.Add($"new ParentRelationship a:{willAdd},e:{entityId},p:{parentGuid},f:{field},i:{index}");
        }

        private Target GetMetadata(Dictionary<string, object> newContentItemCaseInsensitive)
        {
            var wrapLog = Log.Call<Target>($"item dictionary key count: {newContentItemCaseInsensitive.Count}");

            if (!newContentItemCaseInsensitive.Keys.Contains(Attributes.JsonKeyMetadataFor)) return wrapLog("'For' key is missing", null);

            var metadataFor = newContentItemCaseInsensitive[Attributes.JsonKeyMetadataFor] as JObject;
            if (metadataFor == null) return wrapLog("'For' value is null", null);

            var metaData = new Target(GetTargetType(metadataFor[Attributes.TargetNiceName]), null)
            {
                KeyGuid = (Guid?) metadataFor[Attributes.GuidNiceName],
                KeyNumber = (int?) metadataFor[Attributes.NumberNiceName],
                KeyString = (string) metadataFor[Attributes.StringNiceName]
            };
            return wrapLog($"new metadata g:{metaData.KeyGuid},n:{metaData.KeyNumber},s:{metaData.KeyString}", metaData);
        }

        private static int GetTargetType(JToken target)
        {
            if (target.Type == JTokenType.Integer) return (int) target;
            if (target.Type == JTokenType.String && Enum.TryParse<TargetTypes>((string) target, out var targetTypes)) return (int) targetTypes;
            throw new ArgumentOutOfRangeException(Attributes.TargetNiceName, "Value is not 'int' or TargetTypes 'string'.");
        }

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, bool showDrafts) => GetService<Apps.App>().Init(ServiceProvider, appId, Log, null, showDrafts);


        #endregion

        #region helpers / initializers to prep the EAV and Serializer

        private IConvertToEavLight InitEavAndSerializer(int appId, bool userMayEdit)
        {
            Log.Add($"init eav for a#{appId}");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            var ser = _entToDicLazy.Value;
            ser.WithGuid = true;
            ((ConvertToEavLightWithCmsInfo)ser).WithEdit = userMayEdit;
            return ser;
        }
        #endregion


        #region Delete

        public void Delete(string contentType, int id, string appPath)
        {
            Log.Add($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            var entityApi = _entityApi.Init(Context.AppState.AppId, true, Log);
            var itm = entityApi.AppRead.AppState.List.GetOrThrow(contentType, id);
            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), Context.AppState);
            entityApi.Delete(itm.Type.Name, id);
        }

        public void Delete(string contentType, Guid guid, string appPath)
        {
            Log.Add($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context

            var entityApi = _entityApi.Init(Context.AppState.AppId, Context.UserMayEdit, Log);
            var itm = Context.AppState.List.GetOrThrow(contentType == "any" ? null : contentType, guid);

            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), Context.AppState);

            entityApi.Delete(itm.Type.Name, guid);
        }


        #endregion

        #region Permission Checks

        protected MultiPermissionsTypes ThrowIfNotAllowedInType(string contentType, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = GetService<MultiPermissionsTypes>().Init(Context, alternateApp ?? Context.AppState, contentType, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        protected MultiPermissionsItems ThrowIfNotAllowedInItem(IEntity itm, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = GetService<MultiPermissionsItems>().Init(Context, alternateApp ?? Context.AppState, itm, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        #endregion
    }
}
