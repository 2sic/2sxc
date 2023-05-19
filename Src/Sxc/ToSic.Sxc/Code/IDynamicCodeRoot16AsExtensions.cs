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
        #region AsTyped Implementations

        public static ITypedEntity AsTyped(object target, DynamicEntity.MyServices services, ILog log)
        {
            var l = log.Fn<ITypedEntity>();
            switch (target)
            {
                case null:
                    return l.Return(null, "null");
                case ITypedEntity alreadyCmsItem:
                    return l.Return(alreadyCmsItem, "already ok");
                case IDynamicEntity dynEnt:
                    return l.Return(new TypedEntity(dynEnt), nameof(IDynamicEntity));
                case IEntity entity:
                    return l.Return(new TypedEntity(entity, services), nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return l.Return(new TypedEntity(canBeEntity.Entity, services), nameof(ICanBeEntity));
                default:
                    throw l.Ex(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedEntity)}"));
            }

        }

        public static ITypedEntity AsTyped(this IDynamicCodeRoot dynCode, object target) 
            => AsTyped(target, ((DynamicCodeRoot)dynCode).DynamicEntityServices, dynCode.Log);

        public static IEnumerable<ITypedEntity> AsTypedList(object list, DynamicEntity.MyServices services, ILog log)
        {
            var l = log.Fn<IEnumerable<ITypedEntity>>();
            switch (list)
            {
                case null:
                    return l.Return(new List<ITypedEntity>(), "null, empty list");
                case IEnumerable<ITypedEntity> alreadyOk:
                    return l.Return(alreadyOk, "already matches type");
                case IDataSource dsEntities:
                    return l.Return(AsTypedList(dsEntities.List, services, log), "DataSource - convert list");
                case IEnumerable<IEntity> iEntities:
                    return l.Return(iEntities.Select(e => AsTyped(e, services, log)), "IEnum<IEntity>");
                case IEnumerable<IDynamicEntity> dynIDynEnt:
                    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, log)), "IEnum<DynEnt>");
                case IEnumerable<dynamic> dynEntities:
                    return l.Return(dynEntities.Select(e => AsTyped(e as object, services, log)), "IEnum<dynamic>");
                // Check for IEnumerable but make sure it's not a string
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return l.Return(asEnumerable.Cast<object>().Select(e => AsTyped(e, services, log)), "IEnumerable");
                default:
                    throw l.Ex(new ArgumentException($"Type '{list.GetType()}' cannot be converted to {nameof(IEnumerable<ITypedEntity>)}"));
            }
        }

        public static IEnumerable<ITypedEntity> AsTypedList(this IDynamicCodeRoot dynCode, object list) 
            => AsTypedList(list, ((DynamicCodeRoot)dynCode).DynamicEntityServices, dynCode.Log);

        #endregion
    }
}
