using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        [PrivateApi]
        protected override object GetInternal(string field, string language = null, bool lookup = true)
        {
            #region check the two special cases #1 Toolbar 
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618
            if (field == "Toolbar") return Toolbar.ToString();
#pragma warning restore 618
#endif
            #endregion

            // Check #2 Presentation which the EAV doesn't know
            if (field == ViewParts.Presentation) return Presentation;

            return base.GetInternal(field, language, lookup);
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt");
            var safeWrap = logOrNull.SafeCall<PropertyRequest>($"{nameof(field)}", "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap("no entity", null);
            var propRequest = Entity.FindPropertyInternal(field, dimensions, logOrNull);

            if (!propRequest.IsFinal)
            {
                var result = Entity.TryToNavigateToEntityInList(field, this, logOrNull);
                logOrNull?.SafeAdd(result == null ? "not-found" : "entity-in-list");
                propRequest = result ?? propRequest;
                // return safeWrap(, result ?? propRequest);
            }
            else
                propRequest.Name = "dynamic";

            if (propRequest.Result is IEntity entityResult) propRequest.Result = SubDynEntity(entityResult);
            return safeWrap(null, propRequest);
        }


        ///// <summary>
        ///// Special case on entity lists v12.03
        ///// If nothing was found so far, try to see if we could find a child-entity with a title matching the field
        ///// </summary>
        ///// <returns></returns>
        //internal static PropertyRequest TryToNavigateToEntityInList(IEntity entity, object parentDynEntity, string field, ILog parentLogOrNull)
        //{
        //    var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.SubLst");
        //    var safeWrap = logOrNull.SafeCall<PropertyRequest>();
            
        //    var dynChildField = entity.Type?.DynamicChildrenField;
        //    if (string.IsNullOrEmpty(dynChildField)) return safeWrap("no dyn-child", null);


        //    var children = entity.Children(dynChildField);
        //    if (children == null) return safeWrap("no child", null);
        //    // if (!(childField is DynamicEntity dynamicChild)) return safeWrap("child not DynamicEntity", null);
        //    if (children.First().EntityId == 0) return safeWrap("Child is placeholder, no real entries", null);
            

        //    try
        //    {
        //        var dynEntityWithTitle = children
        //            .FirstOrDefault(de => field.Equals(de.GetBestTitle(), StringComparison.InvariantCultureIgnoreCase));

        //        if (dynEntityWithTitle == null) return safeWrap("no matching child", null);

        //        // Forward debug state if it's active
        //        // if(parentLogOrNull != null) dynEntityWithTitle.SetDebug(true);
        //        //if (parentDynEntity is IPropertyStackLookup parentStack)
        //        //    dynEntityWithTitle = new EntityWithStackNavigation(dynEntityWithTitle, parentStack, field, 0);
                
        //        var result = new PropertyRequest
        //        {
        //            FieldType = DataTypes.Entity,
        //            Name = field,
        //            Result = dynEntityWithTitle,
        //            Source = parentDynEntity,
        //            SourceIndex = 0
        //        };

        //        return safeWrap("named-entity", result);
        //    }
        //    catch
        //    {
        //        return safeWrap("error", null);
        //    }
        //}
    }
}
