using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        public const int MaxRecursions = 3;

        #region AsTyped Implementations


        public ITypedItem AsItem(object target)
            => AsTyped(target, MaxRecursions);

        private ITypedItem AsTyped(object target, int recursions)
        {
            var l = Log.Fn<ITypedItem>();
            if (recursions <= 0)
                throw l.Done(new Exception($"Conversion with {nameof(AsItem)} failed, max recursions reached"));

            ITypedItem ConvertOrNullAndLog(IEntity e, string typeName) => e == null
                ? l.ReturnNull($"empty {typeName}")
                : l.Return(new DynamicEntity(e, DynamicEntityServices), typeName);

            switch (target)
            {
                case null:
                    return l.ReturnNull("null");
                case string _:
                    throw l.Done(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedItem)}"));
                case ITypedItem alreadyCmsItem:
                    return l.Return(alreadyCmsItem, "already ok");
                //case IDynamicEntity dynEnt:
                //    return l.Return(new TypedItem(dynEnt), nameof(IDynamicEntity));
                case IEntity entity:
                    return ConvertOrNullAndLog(entity, nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return ConvertOrNullAndLog(canBeEntity.Entity, nameof(ICanBeEntity));
                case IDataSource ds:
                    return ConvertOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataSource));
                case IDataStream ds:
                    return ConvertOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataStream));
                case IEnumerable<IEntity> entList:
                    return ConvertOrNullAndLog(entList.FirstOrDefault(), nameof(IEnumerable<IEntity>));
                case IEnumerable enumerable:
                    var enumFirst = enumerable.Cast<object>().FirstOrDefault();
                    if (enumFirst is null) return l.ReturnNull($"{nameof(IEnumerable)} with null object");
                    // retry conversion
                    return l.Return(AsTyped(enumFirst, recursions - 1));
                default:
                    throw l.Done(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedItem)}"));
            }

        }

        public IEnumerable<ITypedItem> AsItems(object list)
            => AsItemList(list, MaxRecursions);

        internal IEnumerable<ITypedItem> AsItemList(object list, int recursions)
        {
            var l = Log.Fn<IEnumerable<ITypedItem>>();
            if (recursions <= 0)
                throw l.Done(new Exception($"Conversion with {nameof(AsItem)} failed, max recursions reached"));

            switch (list)
            {
                case null:
                    return l.Return(new List<ITypedItem>(), "null, empty list");
                case IEnumerable<ITypedItem> alreadyOk:
                    return l.Return(alreadyOk, "already matches type");
                case IDataSource dsEntities:
                    return l.Return(AsItemList(dsEntities.List, recursions - 1), "DataSource - convert list");
                case IDataStream dataStream:
                    return l.Return(AsItemList(dataStream.List, recursions - 1), "DataSource - convert list");
                case IEnumerable<IEntity> iEntities:
                    return l.Return(iEntities.Select(e => AsTyped(e, MaxRecursions)), "IEnum<IEntity>");
                //case IEnumerable<IDynamicEntity> dynIDynEnt:
                //    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<DynEnt>");
                case IEnumerable<dynamic> dynEntities:
                    return l.Return(dynEntities.Select(e => AsTyped(e as object, MaxRecursions)), "IEnum<dynamic>");
                // Variations of single items - should be converted to list
                case IEntity _:
                case IDynamicEntity _:
                case ITypedItem _:
                case ICanBeEntity _:
                    var converted = AsTyped(list, MaxRecursions);
                    return converted != null
                        ? l.Return(new List<ITypedItem> { converted }, "single item to list")
                        : l.Return(new List<ITypedItem>(), "typed but converted to null; empty list");
                // Check for IEnumerable but make sure it's not a string
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return l.Return(asEnumerable.Cast<object>().Select(e => AsTyped(e, MaxRecursions)), "IEnumerable");
                default:
                    throw l.Done(new ArgumentException($"Type '{list.GetType()}' cannot be converted to {nameof(IEnumerable<ITypedItem>)}"));
            }
        }

        #endregion
    }
}
