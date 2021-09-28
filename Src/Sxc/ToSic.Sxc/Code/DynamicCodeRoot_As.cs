using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {

        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, DynamicEntityDependencies);

        private DynamicEntityDependencies DynamicEntityDependencies =>
            _dynamicEntityDependencies
            ?? (_dynamicEntityDependencies = _serviceProvider.Build<DynamicEntityDependencies>().Init(Block, 
                CmsContext.SafeLanguagePriorityCodes(), Log, CompatibilityLevel));
        private DynamicEntityDependencies _dynamicEntityDependencies;

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => AsDynamicInternal(dynamicEntity);

        private dynamic AsDynamicInternal(object dynObject)
        {
            var wrapLog = Log.Call<dynamic>();
            switch (dynObject)
            {
                case null:
                    return wrapLog("null", AsDynamic(null as string));
                case string strObject:
                    return wrapLog("string", AsDynamic(strObject));
                case IDynamicEntity dynEnt:
                    return wrapLog("DynamicEntity", dynEnt);
                // New case - should avoid re-converting dynamic json, DynamicStack etc.
                case ISxcDynamicObject sxcDyn:
                    return wrapLog("Dynamic Something", sxcDyn);
                case IEntity entity:
                    return wrapLog("IEntity", new DynamicEntity(entity, DynamicEntityDependencies));
                case DynamicObject typedDynObject:
                    return wrapLog("DynamicObject", typedDynObject);
                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    if (dynObject.GetType().IsValueType) return wrapLog("bad call - value type", dynObject);

                    // 2021-09-14 new - just convert to a DynamicReadObject
                    var result = DynamicHelpers.WrapIfPossible(dynObject, true, true, false);
                    if (!(result is null)) return wrapLog("converted to dyn-read", result);

                    // Note 2dm 2021-09-14 returning the original object was actually the default till now.
                    // Unknown conversion, just return the original and see what happens/breaks
                    // probably not a good solution
                    return wrapLog("unknown, return original", dynObject);
            }
        }

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities)
        {
            if (entities == null || !entities.Any()) return null;
            if (entities.Length == 1)
            {
                return AsDynamicInternal(entities[0]);
                //var first = entities[0];
                //if (first is IDynamicEntity dynEntity) return dynEntity;
                //if (first is IEntity entity) return new DynamicEntity(entity, DynamicEntityDependencies);
                //// Unknown, just return it and hope for the best
                //// probably not a good solution
                //return first;
            }
            // New case: many items found, must create a stack
            var sources = entities
                .Select(e => e as IPropertyLookup)
                .Where(e => e !=null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e));
            return new DynamicStack("unknown", DynamicEntityDependencies, sources.ToArray());
        }


        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => ((IDynamicEntity)dynamicEntity).Entity;

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
        public IConvertService Convert => _convert ?? (_convert = _serviceProvider.Build<IConvertService>());
        private IConvertService _convert;

        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
        {
            if (_adamManager == null)
                _adamManager = GetService<AdamManager>()
                    .Init(Block.Context, CompatibilityLevel, Log);
            return _adamManager.Folder(entity, fieldName);
        }
        private AdamManager _adamManager;

        #endregion
    }
}
