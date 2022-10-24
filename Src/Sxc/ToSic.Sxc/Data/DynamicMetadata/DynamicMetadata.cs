using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    public class DynamicMetadata: DynamicEntity, IDynamicMetadata
    {
        internal DynamicMetadata(IMetadataOf metadata, IEntity parentOrNull, DynamicEntityDependencies dependencies)
            : base(metadata, parentOrNull, "Metadata", Eav.Constants.TransientAppId, dependencies)
        {
            _metadata = metadata;
        }

        [PrivateApi("Hide this")]
        private readonly IMetadataOf _metadata;

        IMetadataOf IHasMetadata.Metadata => _metadata;

        public bool HasType(string typeName) => _metadata.HasType(typeName);

        public IEnumerable<IEntity> OfType(string typeName) => _metadata.OfType(typeName);

        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull, PropertyLookupPath path)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt", Debug);
            var safeWrap = logOrNull.Fn<PropertyRequest>($"{nameof(field)}: {field}", "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap.ReturnNull("no entity");
            path = path.KeepOrNew().Add("DynEnt", field);

            // Note: most of the following lines are copied from Metadata
            var list = (this as IHasMetadata).Metadata; // ? MetadataWithoutPermissions : OfType(typeName);
            var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(field));
            if (found == null) return safeWrap.ReturnNull("no entity had attribute");
            var propRequest = found.FindPropertyInternal(field, dimensions, logOrNull, path);
            if (propRequest != null && propRequest.Result != null)
                return safeWrap.Return(propRequest, "found");

            return safeWrap.Return(base.FindPropertyInternal(field, dimensions, parentLogOrNull, path), "base...");
        }
    }
}
