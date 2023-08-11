using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data.Decorators;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    internal class PreWrapEntity: ICanBeEntity, IPropertyLookup
    {
        private readonly ICanDebug _debugState;
        public IEntity Entity { get; }
        public CodeEntityHelper Helper { get; }

        public PreWrapEntity(IEntity entity, CodeEntityHelper helper, ICanDebug debugState)
        {
            _debugState = debugState;
            Entity = entity;
            Helper = helper;
        }

        public dynamic Presentation => _p ?? (_p = Helper.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private IDynamicEntity _p;

        public IMetadata Metadata => _metadata ?? (_metadata = new Metadata(Entity?.Metadata, Entity, Helper.Cdf));
        private Metadata _metadata;

        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynEnt", _debugState.Debug);
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

        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
            Entity == null || !Entity.Attributes.Any()
                ? new List<PropertyDumpItem>()
                : Entity._Dump(specs, path);

    }
}
