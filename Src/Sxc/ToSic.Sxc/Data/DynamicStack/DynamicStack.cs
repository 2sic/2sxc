using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Keep implementation hidden, only publish interface")]
    public partial class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack
    {
        public DynamicStack(string name, DynamicEntityDependencies dependencies, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources) : base(dependencies)
        {
            var stack = new PropertyStack().Init(name, sources);
            UnwrappedStack = stack;
        }

        protected readonly IPropertyStack UnwrappedStack;

        /// <inheritdoc />
        public IPropertyStack GetContents() => UnwrappedStack;

        /// <inheritdoc />
        public dynamic GetSource(string name)
        {
            var source = UnwrappedStack.GetSource(name)
                         // If not found, create a fake one
                         ?? _Dependencies.DataBuilder.FakeEntity(_Dependencies.BlockOrNull?.AppId ?? 0);

            return SourceToDynamicEntity(source);
        }

        /// <inheritdoc />
        public dynamic GetStack(params string[] names)
        {
            var wrapLog = LogOrNull.Fn<object>();
            var newStack = UnwrappedStack.GetStack(LogOrNull, names);
            var newDynStack = new DynamicStack("New", _Dependencies, newStack.Sources);
            return wrapLog.Return(newDynStack);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return SubDynEntityOrNull(ent);
            return null;
        }

        /// <inheritdoc />
        [PrivateApi("Internal")]
        public override PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynStk", Debug);
            path = path.KeepOrNew().Add("DynStack", specs.Field);

            var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynamicStack");
            if (!specs.Field.HasValue())
                return l.Return(null, "no key");

            var hasPath = specs.Field.Contains(".");
            var r = hasPath
                ? UnwrappedStack.InternalGetPath(specs, path)
                : UnwrappedStack.FindPropertyInternal(specs, path);

            return l.Return(r, $"{(r == null ? "null" : "ok")} using {(hasPath ? "Path" : "Property")}");
        }

        [PrivateApi("Internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
            UnwrappedStack?._Dump(specs, path)
            ?? new List<PropertyDumpItem>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicStack)} is not supported");
    }
}
