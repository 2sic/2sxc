using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class PropLookupWithPathEntity:
        ICanBeEntity,       // This is important, to ensure that when used in a stack it can be converted to something else again
        IPropertyLookup
    {
        private readonly ICanDebug _canDebug;

        public IEntity Entity { get; }

        public PropLookupWithPathEntity(IEntity entity, ICanDebug canDebug)
        {
            _canDebug = canDebug;
            Entity = entity;
        }

        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynEnt", _canDebug.Debug);
            var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return l.ReturnNull("no entity");
            if (!specs.Field.HasValue()) return l.ReturnNull("no path");

            path = path.KeepOrNew().Add("DynEnt", specs.Field);
            var isPath = specs.Field.Contains(PropertyStack.PathSeparator.ToString());
            var propRequest = !isPath
                ? Entity.FindPropertyInternal(specs, path)
                : PropertyStack.TraversePath(specs, path, Entity);
            return l.Return(propRequest, $"{nameof(isPath)}: {isPath}");
        }

        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) 
            => Entity?._Dump(specs, path) ?? new List<PropertyDumpItem>();

    }
}
