﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    public class Metadata: DynamicEntity, IMetadata
    {
        internal Metadata(IMetadataOf metadata, IEntity parentOrNull, MyServices services)
            : base(metadata, parentOrNull, "Metadata", Eav.Constants.TransientAppId, services)
        {
            _metadata = metadata;
        }

        [PrivateApi("Hide this")]
        private readonly IMetadataOf _metadata;

        IMetadataOf IHasMetadata.Metadata => _metadata;

        public bool HasType(string type) => _metadata.HasType(type);

        public IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

        [PrivateApi("Internal")]
        public override PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynEnt", Debug);
            var safeWrap = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap.ReturnNull("no entity");
            path = path.KeepOrNew().Add("DynEnt", specs.Field);

            // Note: most of the following lines are copied from Metadata
            var list = (this as IHasMetadata).Metadata;
            var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(specs.Field));
            if (found == null) return safeWrap.ReturnNull("no entity had attribute");
            var propRequest = found.FindPropertyInternal(specs, path);
            if (propRequest != null && propRequest.Result != null)
                return safeWrap.Return(propRequest, "found");

            return safeWrap.Return(base.FindPropertyInternal(specs, path), "base...");
        }
    }
}