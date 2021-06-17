using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

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
            return SourceToDynamicEntity(source);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return SubDynEntity(ent);
            return null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetInternal(binder.Name);
            return true;
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynStk");

            var wrapLog = logOrNull.SafeCall<PropertyRequest>();
            var result = UnwrappedContents.FindPropertyInternal(field, dimensions, logOrNull);
            if (result == null) return wrapLog("null", null);
            
            if (!(result.Result is IEnumerable<IEntity> entityChildren)) 
                return wrapLog("not entity-list", result);
            
            var navigationWrapped = entityChildren.Select(e =>
                new EntityWithStackNavigation(e, UnwrappedContents, field, result.SourceIndex)).ToList();
            result.Result = navigationWrapped;
            return wrapLog(null, result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
