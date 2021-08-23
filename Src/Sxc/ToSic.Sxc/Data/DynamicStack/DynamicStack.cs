using System;
using System.Collections.Generic;
using System.Dynamic;
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
            var source = UnwrappedContents.GetSource(name)
                         // If not found, create a fake one
                         ?? _Dependencies.DataBuilder.FakeEntity(_Dependencies.Block?.AppId ?? 0);

            return SourceToDynamicEntity(source);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return SubDynEntity(ent);
            return null;
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynStk");

            var wrapLog = logOrNull.SafeCall<PropertyRequest>($"{nameof(field)}: {field}", "DynamicStack");
            var result = UnwrappedContents.FindPropertyInternal(field, dimensions, logOrNull);
            return wrapLog(result == null ? "null" : "ok", result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");
    }
}
