using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
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
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, DynamicEntityDependencies);

        internal DynamicEntityDependencies DynamicEntityDependencies =>
            _dynamicEntityDependencies
            ?? (_dynamicEntityDependencies = _serviceProvider.Build<DynamicEntityDependencies>().Init(Block, 
                CmsContext.SafeLanguagePriorityCodes(), Log, CompatibilityLevel));
        private DynamicEntityDependencies _dynamicEntityDependencies;

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => AsDynamicInternal(dynamicEntity);

        private dynamic AsDynamicInternal(object dynObject)
        {
            var wrapLog = Log.Fn<dynamic>();
            switch (dynObject)
            {
                case null:
                    return wrapLog.Return(AsDynamic(null as string), "null");
                case string strObject:
                    return wrapLog.Return(AsDynamic(strObject), "string");
                case IDynamicEntity dynEnt:
                    return wrapLog.Return(dynEnt, "DynamicEntity");
                // New case - should avoid re-converting dynamic json, DynamicStack etc.
                case ISxcDynamicObject sxcDyn:
                    return wrapLog.Return(sxcDyn, "Dynamic Something");
                case IEntity entity:
                    return wrapLog.Return(new DynamicEntity(entity, DynamicEntityDependencies), "IEntity");
                case DynamicObject typedDynObject:
                    return wrapLog.Return(typedDynObject, "DynamicObject");
                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    if (dynObject.GetType().IsValueType) return wrapLog.Return(dynObject, "bad call - value type");

                    // 2021-09-14 new - just convert to a DynamicReadObject
                    var result = DynamicHelpers.WrapIfPossible(dynObject, true, true, false);
                    if (!(result is null)) return wrapLog.Return(result, "converted to dyn-read");

                    // Note 2dm 2021-09-14 returning the original object was actually the default till now.
                    // Unknown conversion, just return the original and see what happens/breaks
                    // probably not a good solution
                    return wrapLog.Return(dynObject, "unknown, return original");
            }
        }

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities)
        {
            if (entities == null || !entities.Any()) return null;
            if (entities.Length == 1)
                return AsDynamicInternal(entities[0]);
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
                    .Init(ContextForAdamManager(), CompatibilityLevel, Log);
            return _adamManager.Folder(entity, fieldName);
        }
        private AdamManager _adamManager;

        /// <summary>
        /// Special helper - if the DynamicCode is generated by the service or used in a WebApi there is no block, but we can figure out the context.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IContextOfApp ContextForAdamManager()
        {
            if(_contextOfApp != null) return _contextOfApp;
            if (Block?.Context != null) return _contextOfApp = Block.Context;
            if (App == null) throw new Exception("Can't create App Context for ADAM - no block, no App");
            _contextOfApp = GetService<IContextOfApp>();
            _contextOfApp.Init(Log);
            _contextOfApp.ResetApp(App);
            return _contextOfApp;
        }
        private IContextOfApp _contextOfApp;

        #endregion
    }
}
