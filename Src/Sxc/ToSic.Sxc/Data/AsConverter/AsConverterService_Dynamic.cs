using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {

        #region Dynamic

        public DynamicEntity AsDynamic(IEntity entity) => new DynamicEntity(entity, DynamicEntityServices);

        public DynamicEntity AsDynamic(IEnumerable<IEntity> list) => new DynamicEntity(list: list, parent: null, field: null, appIdOrNull: null, services: DynamicEntityServices);

        public IEnumerable<dynamic> AsDynamicList(object list)
        {
            switch (list)
            {
                case null:
                    return new List<dynamic>();
                case IDataSource dsEntities:
                    return AsDynamicList(dsEntities.List);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(AsDynamic);
                case IEnumerable<IDynamicEntity> dynIDynEnt:
                    return dynIDynEnt;
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                default:
                    return null;
            }
        }


        internal object AsDynamicInternal(object dynObject)
        {
            var l = Log.Fn<object>();
            var typed = AsTypedInternal(dynObject);
            if (typed != null) return l.Return(typed, nameof(ITypedRead));

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

        #endregion


        #region Merge Dynamic

        public dynamic MergeDynamic(params object[] entities)
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


        #endregion
    }
}
