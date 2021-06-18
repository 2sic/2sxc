using System;
using System.Linq;
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
            #region check the two special cases Toolbar / Presentation which the EAV doesn't know
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618
            if (field == "Toolbar") return Toolbar.ToString();
#pragma warning restore 618
#endif
            if (field == ViewParts.Presentation) return Presentation;

            #endregion

            return base.GetInternal(field, language, lookup);
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt");
            var wrapLog = logOrNull.SafeCall<PropertyRequest>();
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return wrapLog("no entity", null);
            var propRequest = Entity.FindPropertyInternal(field, dimensions, logOrNull);
            if (propRequest.IsFinal)
            {
                propRequest.Name = "dynamic";
                return wrapLog(null, propRequest);
            }

            var dynChildField = Entity.Type?.DynamicChildrenField;
            
            if (string.IsNullOrEmpty(dynChildField))
                return propRequest; // TryToNavigateToEntityInList(field, parentLogOrNull) ?? propRequest;
            
            var childField = Get(dynChildField);
            if (childField == null) return propRequest;
            if (!(childField is DynamicEntity dynamicChild)) return propRequest; // TryToNavigateToEntityInList(field, parentLogOrNull) ?? propRequest;
            
            return dynamicChild.TryToNavigateToEntityInList(field, logOrNull) ?? propRequest;
        }


        /// <summary>
        /// Special case on entity lists v12.03
        /// If nothing was found so far, try to see if we could find a child-entity with a title matching the field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parentLogOrNull"></param>
        /// <returns></returns>
        internal PropertyRequest TryToNavigateToEntityInList(string field, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynLst");
            var wrapLog = logOrNull.SafeCall<PropertyRequest>();

            try
            {
                var dynEntityWithTitle = _ListHelper.DynEntities.FirstOrDefault(de =>
                    field.Equals(de.EntityTitle.ToString(), StringComparison.InvariantCultureIgnoreCase));

                if (dynEntityWithTitle == null) return wrapLog("no matching child", null);

                var result = new PropertyRequest
                {
                    FieldType = DataTypes.Entity,
                    Name = field,
                    Result = dynEntityWithTitle,
                    Source = this,
                    SourceIndex = 0
                };

                return wrapLog("named-entity", result);
            }
            catch
            {
                return wrapLog("error", null);
            }
        }
    }
}
