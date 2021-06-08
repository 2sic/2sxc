using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public partial class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack
    {
        public DynamicStack(DynamicEntityDependencies dependencies, params KeyValuePair<string, IPropertyLookup>[] entities) : base(dependencies)
        {
            var stack = new PropertyStack();
            stack.Init(entities);
            UnwrappedContents = stack;
        }
        
        public IPropertyStack UnwrappedContents { get; }

        public dynamic GetSource(string name)
        {
            var source = UnwrappedContents.GetSource(name);
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return SubDynEntity(ent);
            return null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _getValue(binder.Name);
            return true;
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions) => UnwrappedContents.FindPropertyInternal(field, dimensions);

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
