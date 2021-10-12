using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Keep implementation hidden, only publish interface")]
    public partial class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack
    {
        public DynamicStack(string name, DynamicEntityDependencies dependencies, params KeyValuePair<string, IPropertyLookup>[] entities) : base(dependencies)
        {
            var stack = new PropertyStack();
            stack.Init(name, entities);
            UnwrappedContents = stack;
        }
        
        public IPropertyStack UnwrappedContents { get; }
        public IPropertyStack GetContents() => UnwrappedContents;

        public dynamic GetSource(string name)
        {
            var source = UnwrappedContents.GetSource(name)
                         // If not found, create a fake one
                         ?? _Dependencies.DataBuilder.FakeEntity(_Dependencies.BlockOrNull?.AppId ?? 0);

            return SourceToDynamicEntity(source);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return SubDynEntityOrNull(ent);
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

        [PrivateApi]

        [PrivateApi("Internal")]
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            return UnwrappedContents?._Dump(languages, path, parentLogOrNull)
                   ?? new List<PropertyDumpItem>();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicStack)} is not supported");
    }
}
