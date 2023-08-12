using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    internal class PropLookupMetadata: IPropertyLookup
    {
        private readonly Func<bool> _getDebug;

        public Metadata Parent { get; }
        public IEntity Entity { get; }
        public IPropertyLookup Upstream { get; }

        public PropLookupMetadata(Metadata parent, IEntity entity, IPropertyLookup upstream, Func<bool> getDebug)
        {
            _getDebug = getDebug;
            Parent = parent;
            Entity = entity;
            Upstream = upstream;
        }

        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynEnt", _getDebug());
            var safeWrap = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap.ReturnNull("no entity");
            path = path.KeepOrNew().Add("DynEnt", specs.Field);

            // Note: most of the following lines are copied from Metadata
            var list = (Parent as IHasMetadata).Metadata;
            var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(specs.Field));
            if (found == null) return safeWrap.ReturnNull("no entity had attribute");
            var propRequest = found.FindPropertyInternal(specs, path);
            return propRequest?.Result != null 
                ? safeWrap.Return(propRequest, "found")
                : safeWrap.Return(Upstream.FindPropertyInternal(specs, path), "base...");
        }

        // Note: same as the PropLookupWithPathEntity
        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
            => Entity?._Dump(specs, path) ?? new List<PropertyDumpItem>();
    }
}
