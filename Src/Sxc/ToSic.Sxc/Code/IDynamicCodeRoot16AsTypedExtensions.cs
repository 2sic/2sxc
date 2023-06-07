using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    // ReSharper disable once InconsistentNaming
    public static class IDynamicCodeRoot16AsExtensions
    {
        public const int MaxRecursions = 3;

        #region AsTyped Implementations

        
        public static ITypedItem AsTyped(this IDynamicCodeRoot dynCode, object target)
            => AsTyped(target, ((DynamicCodeRoot)dynCode).DynamicEntityServices, MaxRecursions, dynCode.Log);

        private static ITypedItem AsTyped(object target, DynamicEntity.MyServices services, int recursions, ILog log)
        {
            var l = log.Fn<ITypedItem>();
            if (recursions <= 0)
                throw l.Done(new Exception($"Conversion with {nameof(AsTyped)} failed, max recursions reached"));

            ITypedItem ConvertOrNullAndLog(IEntity e, string typeName) => e == null
                ? l.ReturnNull($"empty {typeName}")
                : l.Return(new TypedItem(e, services), typeName);

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
                    return l.Return(new TypedItem(entity, services), nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return l.Return(new TypedItem(canBeEntity.Entity, services), nameof(ICanBeEntity));
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
                    return l.Return(AsTyped(enumFirst, services, recursions - 1, log));
                default:
                    throw l.Done(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedItem)}"));
            }

        }

        public static IEnumerable<ITypedItem> AsTypedList(this IDynamicCodeRoot dynCode, object list) 
            => AsTypedList(list, ((DynamicCodeRoot)dynCode).DynamicEntityServices, MaxRecursions, dynCode.Log);

        internal static IEnumerable<ITypedItem> AsTypedList(object list, DynamicEntity.MyServices services, int recursions, ILog log)
        {
            var l = log.Fn<IEnumerable<ITypedItem>>();
            if (recursions <= 0)
                throw l.Done(new Exception($"Conversion with {nameof(AsTyped)} failed, max recursions reached"));

            switch (list)
            {
                case null:
                    return l.Return(new List<ITypedItem>(), "null, empty list");
                case IEnumerable<ITypedItem> alreadyOk:
                    return l.Return(alreadyOk, "already matches type");
                case IDataSource dsEntities:
                    return l.Return(AsTypedList(dsEntities.List, services, recursions -1, log), "DataSource - convert list");
                case IDataStream dataStream:
                    return l.Return(AsTypedList(dataStream.List, services, recursions - 1, log), "DataSource - convert list");
                case IEnumerable<IEntity> iEntities:
                    return l.Return(iEntities.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<IEntity>");
                //case IEnumerable<IDynamicEntity> dynIDynEnt:
                //    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<DynEnt>");
                case IEnumerable<dynamic> dynEntities:
                    return l.Return(dynEntities.Select(e => AsTyped(e as object, services, MaxRecursions, log)), "IEnum<dynamic>");
                // Variations of single items - should be converted to list
                case IEntity _:
                case IDynamicEntity _:
                case ITypedItem _:
                case ICanBeEntity _:
                    var converted = AsTyped(list, services, MaxRecursions, log);
                    return converted != null
                        ? l.Return(new List<ITypedItem> { converted }, "single item to list")
                        : l.Return(new List<ITypedItem>(), "typed but converted to null; empty list");
                // Check for IEnumerable but make sure it's not a string
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return l.Return(asEnumerable.Cast<object>().Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnumerable");
                default:
                    throw l.Done(new ArgumentException($"Type '{list.GetType()}' cannot be converted to {nameof(IEnumerable<ITypedItem>)}"));
            }
        }

        #endregion
    }
}
