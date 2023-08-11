using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    internal class PreWrapStack
    {
        private readonly ICanDebug _debugState;
        public IPropertyStack Stack { get; }

        public PreWrapStack(IPropertyStack stack, ICanDebug debugState)
        {
            _debugState = debugState;
            Stack = stack;
        }

        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynStk", _debugState.Debug);
            path = path.KeepOrNew().Add("DynStack", specs.Field);

            var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynamicStack");
            if (!specs.Field.HasValue())
                return l.Return(null, "no key");

            var hasPath = specs.Field.Contains(".");
            var r = hasPath
                ? Stack.InternalGetPath(specs, path)
                : Stack.FindPropertyInternal(specs, path);

            return l.Return(r, $"{(r == null ? "null" : "ok")} using {(hasPath ? "Path" : "Property")}");
        }

        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
            Stack?._Dump(specs, path) ?? new List<PropertyDumpItem>();

    }
}
