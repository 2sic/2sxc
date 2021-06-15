using System;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityWithList 
    {
        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var propRequest = base.FindPropertyInternal(field, dimensions, parentLogOrNull);
            if (propRequest.IsFinal) return propRequest;

            // Special case on entity lists v12.03
            // If nothing was found so far, try to see if we could find a child-entity with a title matching the field

            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynLst");
            var wrapLog = logOrNull.SafeCall<PropertyRequest>();

            var dynEntityWithTitle = DynEntities.FirstOrDefault(de =>
                field.Equals(de.EntityTitle.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (dynEntityWithTitle == null) return wrapLog("no matching child", propRequest);

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
    }
}
