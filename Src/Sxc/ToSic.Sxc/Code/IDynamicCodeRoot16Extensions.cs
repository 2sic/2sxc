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
    public static class IDynamicCodeRoot16Extensions
    {
        #region AsCms Implementations

        public static ICmsEntity AsCms(this IDynamicCodeRoot dynCode, object target)
        {
            var l = dynCode.Log.Fn<ICmsEntity>();
            switch (target)
            {
                case null:
                    return l.Return(null, "null");
                case ICmsEntity alreadyCmsItem:
                    return l.Return(alreadyCmsItem, "already ok");
                case IDynamicEntity dynEnt:
                    return l.Return(new CmsEntity(dynEnt), nameof(IDynamicEntity));
                case IEntity entity:
                    return l.Return(new CmsEntity(entity, ((DynamicCodeRoot)dynCode).DynamicEntityServices), nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return l.Return(new CmsEntity(canBeEntity.Entity, ((DynamicCodeRoot)dynCode).DynamicEntityServices), nameof(ICanBeEntity));
                default:
                    throw new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(CmsEntity)}");
            }
        }

        public static IEnumerable<ICmsEntity> AsCmsList(this IDynamicCodeRoot dynCode, object list)
        {
            switch (list)
            {
                case null:
                    return new List<ICmsEntity>();
                case IEnumerable<ICmsEntity> alreadyOk:
                    return alreadyOk;
                case IDataSource dsEntities:
                    return dynCode.AsCmsList(dsEntities.List);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(dynCode.AsCms);
                case IEnumerable<IDynamicEntity> dynIDynEnt:
                    return dynIDynEnt.Select(dynCode.AsCms);
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities.Select(dynCode.AsCms);
                // Check for IEnumerable but make sure it's not a string
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return asEnumerable.Cast<object>().Select(dynCode.AsCms);
                default:
                    return null;
            }
        }


        #endregion

        #region AsCms Implementations

        public static ITypedEntity AsTyped(this IDynamicCodeRoot dynCode, object target)
        {
            var l = dynCode.Log.Fn<ITypedEntity>();
            switch (target)
            {
                case null:
                    return l.Return(null, "null");
                case ITypedEntity alreadyCmsItem:
                    return l.Return(alreadyCmsItem, "already ok");
                case IDynamicEntity dynEnt:
                    return l.Return(new TypedEntity(dynEnt), nameof(IDynamicEntity));
                case IEntity entity:
                    return l.Return(new TypedEntity(entity, ((DynamicCodeRoot)dynCode).DynamicEntityServices), nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return l.Return(new TypedEntity(canBeEntity.Entity, ((DynamicCodeRoot)dynCode).DynamicEntityServices), nameof(ICanBeEntity));
                default:
                    throw new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(CmsEntity)}");
            }
        }

        public static IEnumerable<ITypedEntity> AsTypedList(this IDynamicCodeRoot dynCode, object list)
        {
            switch (list)
            {
                case null:
                    return new List<ITypedEntity>();
                case IEnumerable<ITypedEntity> alreadyOk:
                    return alreadyOk;
                case IDataSource dsEntities:
                    return dynCode.AsTypedList(dsEntities.List);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(dynCode.AsTyped);
                case IEnumerable<IDynamicEntity> dynIDynEnt:
                    return dynIDynEnt.Select(dynCode.AsTyped);
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities.Select(dynCode.AsTyped);
                // Check for IEnumerable but make sure it's not a string
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return asEnumerable.Cast<object>().Select(dynCode.AsTyped);
                default:
                    return null;
            }
        }

        #endregion
    }
}
