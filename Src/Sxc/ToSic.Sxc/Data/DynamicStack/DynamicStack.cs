using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions)
        {
            var result = UnwrappedContents.FindPropertyInternal(field, dimensions);
            if (result == null) return null;
            
            if (!(result.Result is IEnumerable<IEntity> entityChildren)) return result;
            
            var navigationWrapped = entityChildren.Select(e =>
                new EntityWithStackNavigation(e, UnwrappedContents, field, result.SourceIndex)).ToList();
            result.Result = navigationWrapped;
            return result;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
