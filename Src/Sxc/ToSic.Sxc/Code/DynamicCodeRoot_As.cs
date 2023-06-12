using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {

        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = default) => DynamicJacket.AsDynamicJacket(json, fallback, Log);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, DynamicEntityServices);

        internal DynamicEntity.MyServices DynamicEntityServices => _dynEntDependencies.Get(() => 
            Services.DynamicEntityDependencies.Value.Init(Block, 
                CmsContext.SafeLanguagePriorityCodes(),
                Log, 
                CompatibilityLevel, 
                this.GetKit<ServiceKit14>,
                // Important: Get AdamManager must be a function, as it also depends on this DynEntServices
                // This is not the ideal solution, should probably be improved...
                () => AdamManager));
        private readonly GetOnce<DynamicEntity.MyServices> _dynEntDependencies = new GetOnce<DynamicEntity.MyServices>();

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => AsDynamicInternal(dynamicEntity);

        private object AsDynamicInternal(object dynObject)
        {
            var l = Log.Fn<object>();
            var typed = AsTypedInternal(dynObject);
            if (typed != null) return l.Return(typed, nameof(ITypedObject));

            switch (dynObject)
            {
                //case null:
                //    return wrapLog.Return( this.AsDynamicFromJson(null), "null");
                //case string strObject:
                //    return wrapLog.Return(this.AsDynamicFromJson(strObject), "string");
                //case IDynamicEntity dynEnt:
                //    return wrapLog.Return(dynEnt, "DynamicEntity");
                //// New case - should avoid re-converting dynamic json, DynamicStack etc.
                //case ISxcDynamicObject sxcDyn:
                //    return wrapLog.Return(sxcDyn, "Dynamic Something");
                //case IEntity entity:
                //    return wrapLog.Return(new DynamicEntity(entity, DynamicEntityServices), "IEntity");
                case DynamicObject typedDynObject:
                    return l.Return(typedDynObject, "DynamicObject");
                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    if (dynObject.GetType().IsValueType) return l.Return(dynObject, "bad call - value type");

                    // 2021-09-14 new - just convert to a DynamicReadObject
                    var result = DynamicHelpers.WrapIfPossible(dynObject, true, true, false);
                    if (!(result is null)) return l.Return(result, "converted to dyn-read");

                    // Note 2dm 2021-09-14 returning the original object was actually the default till now.
                    // Unknown conversion, just return the original and see what happens/breaks
                    // probably not a good solution
                    return l.Return(dynObject, "unknown, return original");
            }
        }

        private ITypedObject AsTypedInternal(object dynObject)
        {
            var l = Log.Fn<ITypedObject>();
            switch (dynObject)
            {
                case null:
                    return l.Return(this.AsDynamicFromJson(null), "null");
                case string strObject:
                    return l.Return(this.AsDynamicFromJson(strObject), "string");
                case IDynamicEntity dynEnt:
                    return l.Return(dynEnt, "DynamicEntity");
                // New case - should avoid re-converting dynamic json, DynamicStack etc.
                case ISxcDynamicObject sxcDyn:
                    return l.Return(sxcDyn, "Dynamic Something");
                case IEntity entity:
                    return l.Return(new DynamicEntity(entity, DynamicEntityServices), "IEntity");
                //case DynamicObject typedDynObject:
                //    return wrapLog.Return(typedDynObject, "DynamicObject");
                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    //if (dynObject.GetType().IsValueType) return wrapLog.Return(dynObject, "bad call - value type");

                    // 2021-09-14 new - just convert to a DynamicReadObject
                    var result = DynamicHelpers.WrapIfPossible(dynObject, true, true, false);
                    if (result is ITypedObject resTyped) return l.Return(resTyped, "converted to dyn-read");

                    //// Note 2dm 2021-09-14 returning the original object was actually the default till now.
                    //// Unknown conversion, just return the original and see what happens/breaks
                    //// probably not a good solution
                    //return wrapLog.Return(dynObject, "unknown, return original");
                    return l.Return(null, "unknown, return original");
            }
        }

        /// <inheritdoc />
        [PublicApi]
        public dynamic AsDynamic(params object[] entities)
        {
            if (entities == null || !entities.Any()) return null;
            if (entities.Length == 1) return AsDynamicInternal(entities[0]);

            return MergeTyped(entities);
            //// New case: many items found, must create a stack
            //var sources = entities
            //    .Select(e => e as IPropertyLookup)
            //    .Where(e => e != null)
            //    .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
            //    .ToList();
            //return new DynamicStack(Eav.Constants.NullNameId, DynamicEntityServices, sources);
        }

        [PrivateApi]
        public ITypedObject MergeTyped(params object[] entities)
        {
            if (entities == null || !entities.Any()) return null;
            if (entities.Length == 1) return AsTypedInternal(entities[0]);
            // New case: many items found, must create a stack
            var sources = entities
                .Select(e => e as IPropertyLookup)
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
                .ToList();
            return new DynamicStack(Eav.Constants.NullNameId, DynamicEntityServices, sources);
        }


        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => ((ICanBeEntity)dynamicEntity).Entity;

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
        {
            switch (list)
            {
                case null:
                    return new List<dynamic>();
                case IDataSource dsEntities:
                    return AsList(dsEntities.List);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(e => AsDynamic(e));
                case IEnumerable<IDynamicEntity> dynIDynEnt:
                    return dynIDynEnt;
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                default:
                    return null;
            }
        }


        #endregion

        #region Convert

        /// <inheritdoc />
        public IConvertService Convert => _convert ?? (_convert = Services.ConvertService.Value);
        private IConvertService _convert;

        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => DynamicEntityServices.AdamManager.Folder(item.Entity, fieldName);

        /// <summary>
        /// Special helper - if the DynamicCode is generated by the service or used in a WebApi there is no block, but we can figure out the context.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private AdamManager GetAdamManager()
        {
            IContextOfApp contextOfApp = Block?.Context;
            // TODO: @2dm - find out / document why this could even be null
            if (contextOfApp == null)
            {
                if (App == null)
                    throw new Exception("Can't create App Context for ADAM - no block, no App");
                contextOfApp = Services.ContextOfApp.Value;
                contextOfApp.ResetApp(App);
            }

            return Services.AdamManager.Value.Init(contextOfApp, DynamicEntityServices, CompatibilityLevel);
        }
        private AdamManager AdamManager => _admMng.Get(GetAdamManager);
        private readonly GetOnce<AdamManager> _admMng = new GetOnce<AdamManager>();


        #endregion
    }
}
