using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Api.Api01;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Api.Api01;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.App;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.Helper;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using IApp = ToSic.Eav.Apps.IApp;

namespace ToSic.Sxc.WebApi.App
{
    public class AppContent : ServiceBase
    {
        private readonly Generator<Apps.App> _app;
        private readonly Generator<MultiPermissionsTypes> _typesPermissions;
        private readonly Generator<MultiPermissionsItems> _itemsPermissions;

        #region Constructor / DI

        public AppContent(Generator<Apps.App> app,
            EntityApi entityApi,
            LazySvc<IConvertToEavLight> entToDicLazy,
            IContextResolver ctxResolver,
            Generator<MultiPermissionsTypes> typesPermissions,
            Generator<MultiPermissionsItems> itemsPermissions,
            LazySvc<AppManager> appManagerLazy,
            LazySvc<SimpleDataController> dataControllerLazy) : base("Sxc.ApiApC")
        {
            ConnectServices(
                _app = app,
                _entityApi = entityApi,
                _entToDicLazy = entToDicLazy,
                _ctxResolver = ctxResolver,
                _appManagerLazy = appManagerLazy,
                _typesPermissions = typesPermissions,
                _itemsPermissions = itemsPermissions,
                _dataControllerLazy = dataControllerLazy
            );
        }

        private readonly EntityApi _entityApi;
        private readonly LazySvc<IConvertToEavLight> _entToDicLazy;
        private readonly IContextResolver _ctxResolver;
        private readonly LazySvc<AppManager> _appManagerLazy;
        private readonly LazySvc<SimpleDataController> _dataControllerLazy;
        private AppManager AppManager => _appManager.Get(() => _appManagerLazy.Value.InitQ(AppState, showDrafts: false));
        private readonly GetOnce<AppManager> _appManager = new GetOnce<AppManager>();

        public AppContent Init(string appName)
        {
            // if app-path specified, use that app, otherwise use from context
            Context = _ctxResolver.AppNameRouteBlock(appName);
            return this;
        }
        protected IContextOfApp Context;

        protected AppState AppState => Context?.AppState ??
                                       throw new Exception(
                                           "Can't access AppState before Context is ready. Did you forget to call Init(...)?");

        #endregion


        #region Get Items

        public IEnumerable<IDictionary<string, object>> GetItems(string contentType, string appPath = null)
        {
            var wrapLog = Log.Fn<IEnumerable<IDictionary<string, object>>>($"get entities type:{contentType}, path:{appPath}");

            // verify that read-access to these content-types is permitted
            var permCheck = ThrowIfNotAllowedInType(contentType, GrantSets.ReadSomething, AppState);

            var result = _entityApi.Init(AppState.AppId, permCheck.EnsureAny(GrantSets.ReadDraft))
                .GetEntities(contentType)
                ?.ToList();
            return wrapLog.Return(result, "found: " + result?.Count);
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
            Log.A($"get and serialize after security check type:{contentType}, path:{appPath}");

            // first try to find in all entities incl. drafts
            var itm = getOne(AppState.List);
            var permCheck = ThrowIfNotAllowedInItem(itm, GrantSets.ReadSomething, AppState);

            // in case draft wasn't allow, get again with more restricted permissions 
            if (!permCheck.EnsureAny(GrantSets.ReadDraft))
                itm = getOne(AppState.ListPublished);

            return InitEavAndSerializer(AppState.AppId, Context.UserMayEdit).Convert(itm);
        }


        #endregion

        #region CreateOrUpdate


        public IDictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object> newContentItem, int? id = null, string appPath = null)
        {
            Log.A($"create or update type:{contentType}, id:{id}, path:{appPath}");

            // if app-path specified, use that app, otherwise use from context

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null
                ? null
                : AppState.List.GetOrThrow(contentType, id.Value);

            if (itm == null) ThrowIfNotAllowedInType(contentType, GrantSets.CreateSomething, AppState);
            else ThrowIfNotAllowedInItem(itm, GrantSets.WriteSomething, AppState);

            // Convert to case-insensitive dictionary just to be safe!
            var newContentItemCaseInsensitive = new Dictionary<string, object>(newContentItem, StringComparer.InvariantCultureIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = new AppContentEntityBuilder(Log)
                .CreateEntityDictionary(contentType, newContentItemCaseInsensitive, AppState);

            // add owner
            if (cleanedNewItem.Any(v => v.Key.ToLowerInvariant() == Attributes.EntityFieldOwner))
                cleanedNewItem.Add(Attributes.EntityFieldOwner, Context.User.IdentityToken);

            var dataController = DataController(AppState);
            if (id == null)
            {
                Log.A($"create new entity because id is null");
                var metadata = GetMetadata(newContentItemCaseInsensitive);
                Log.A($"metadata: {metadata}");

                var ids = dataController.Create(contentType, new List<Dictionary<string, object>> { cleanedNewItem }, metadata);
                id = ids.FirstOrDefault();

                Log.A($"new entity id: {id}");
                var added = AddParentRelationship(newContentItemCaseInsensitive, id.Value);
            }
            else
                dataController.Update(id.Value, cleanedNewItem);

            return InitEavAndSerializer(AppState.AppId, Context.UserMayEdit)
                .Convert(AppState.List.One(id.Value));
        }

        private bool AddParentRelationship(IDictionary<string, object> newContentItemCaseInsensitive, int addedEntityId)
        {
            var wrapLog = Log.Fn<bool>($"item dictionary key count: {newContentItemCaseInsensitive.Count}");

            if (!newContentItemCaseInsensitive.Keys.Contains(SaveApiAttributes.ParentRelationship))
                return wrapLog.ReturnFalse($"'{SaveApiAttributes.ParentRelationship}' key is missing");

            var objectOrNull = newContentItemCaseInsensitive[SaveApiAttributes.ParentRelationship];
            if (objectOrNull == null) return wrapLog.ReturnFalse($"'{SaveApiAttributes.ParentRelationship}' value is null");

            if (!(objectOrNull is JsonObject parentRelationship))
                return wrapLog.ReturnNull($"'{SaveApiAttributes.ParentRelationship}' value is not JsonObject");

            var parentGuid = (Guid?)parentRelationship[SaveApiAttributes.ParentRelParent];
            if (!parentGuid.HasValue) return wrapLog.ReturnFalse($"'{SaveApiAttributes.ParentRelParent}' guid is missing");

            var parentEntity = AppState.List.One(parentGuid.Value);
            if (parentEntity == null) return wrapLog.ReturnFalse("Parent entity is missing");

            //var entityId = (int?)parentRelationship["EntityId"];
            var ids = new[] { addedEntityId as int? };
            var index = (int)parentRelationship[SaveApiAttributes.ParentRelIndex];
            //var willAdd = (bool?)parentRelationship["Add"];
            var field = (string)parentRelationship[SaveApiAttributes.ParentRelField];
            var fields = new[] { field };

            AppManager.Entities.FieldListAdd(parentEntity, fields, index, ids, asDraft: false, forceAddToEnd: false);

            //return wrapLog.ReturnTrue($"new ParentRelationship a:{willAdd},e:{entityId},p:{parentGuid},f:{field},i:{index}");
            return wrapLog.ReturnTrue($"new ParentRelationship p:{parentGuid},f:{field},i:{index}");
        }

        private Target GetMetadata(Dictionary<string, object> newContentItemCaseInsensitive)
        {
            var wrapLog = Log.Fn<Target>($"item dictionary key count: {newContentItemCaseInsensitive.Count}");

            if (!newContentItemCaseInsensitive.Keys.Contains(Attributes.JsonKeyMetadataFor)) return wrapLog.ReturnNull($"'{Attributes.JsonKeyMetadataFor}' key is missing");

            var objectOrNull = newContentItemCaseInsensitive[Attributes.JsonKeyMetadataFor];
            if (objectOrNull == null) return wrapLog.ReturnNull($"'{Attributes.JsonKeyMetadataFor}' value is null");

            if (!(objectOrNull is JsonObject metadataFor))
                return wrapLog.ReturnNull($"'{Attributes.JsonKeyMetadataFor}' value is not JsonObject");

            var metaData = new Target(GetTargetType(metadataFor[Attributes.TargetNiceName]?.AsValue()), null)
            {
                KeyGuid = (Guid?)metadataFor[Attributes.GuidNiceName],
                KeyNumber = (int?)metadataFor[Attributes.NumberNiceName],
                KeyString = (string)metadataFor[Attributes.StringNiceName]
            };
            return wrapLog.Return(metaData,
                $"new metadata g:{metaData.KeyGuid},n:{metaData.KeyNumber},s:{metaData.KeyString}");

        }

        private static int GetTargetType(JsonValue target)
        {
            switch (target.GetValue<JsonElement>().ValueKind)
            {
                case JsonValueKind.Number:
                    return (int)target;
                case JsonValueKind.String when Enum.TryParse<TargetTypes>((string)target, out var targetTypes):
                    return (int)targetTypes;
                default:
                    throw new ArgumentOutOfRangeException(Attributes.TargetNiceName, "Value is not 'int' or TargetTypes 'string'.");
            }
        }

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, bool showDrafts) => _app.New().Init(appId, null, showDrafts);

        // TODO: THIS SHOULD probably replace The GetApp above, as it's just an indirect way of getting the data-controller?
        private SimpleDataController DataController(IAppIdentity app) => _dataController ?? (_dataController = _dataControllerLazy.Value.Init(app.ZoneId, app.AppId));
        private SimpleDataController _dataController;

        #endregion

        #region helpers / initializers to prep the EAV and Serializer

        private IConvertToEavLight InitEavAndSerializer(int appId, bool userMayEdit)
        {
            Log.A($"init eav for a#{appId}");
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
            Log.A($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            var entityApi = _entityApi.Init(AppState.AppId, true);
            var itm = entityApi.AppRead.AppState.List.GetOrThrow(contentType, id);
            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), AppState);
            entityApi.Delete(itm.Type.Name, id);
        }

        public void Delete(string contentType, Guid guid, string appPath)
        {
            Log.A($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context

            var entityApi = _entityApi.Init(AppState.AppId, Context.UserMayEdit);
            var itm = AppState.List.GetOrThrow(contentType == "any" ? null : contentType, guid);

            ThrowIfNotAllowedInItem(itm, Grants.Delete.AsSet(), AppState);

            entityApi.Delete(itm.Type.Name, guid);
        }


        #endregion

        #region Permission Checks

        protected MultiPermissionsTypes ThrowIfNotAllowedInType(string contentType, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = _typesPermissions.New().Init(Context, alternateApp ?? AppState, contentType);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        protected MultiPermissionsItems ThrowIfNotAllowedInItem(IEntity itm, List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = _itemsPermissions.New().Init(Context, alternateApp ?? AppState, itm);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return permCheck;
        }

        #endregion
    }
}
