using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class PropLookupStack: IPropertyLookup
{
    private readonly Func<bool> _getDebug;
    public IPropertyStack Stack { get; }

    public PropLookupStack(IPropertyStack stack, Func<bool> getDebug)
    {
        _getDebug = getDebug;
        Stack = stack;
    }

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynStk", _getDebug());
        path = path.KeepOrNew().Add("DynStack", specs.Field);

        var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), nameof(PropLookupStack));
        if (!specs.Field.HasValue())
            return l.Return(null, "no key");

        var hasPath = specs.Field.Contains(PropertyStack.PathSeparator.ToString());
        var r = hasPath
            ? Stack.InternalGetPath(specs, path)
            : Stack.FindPropertyInternal(specs, path);

        return l.Return(r, $"{(r == null ? "null" : "ok")} using {(hasPath ? "Path" : "Property")}");
    }

    public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
        Stack?._Dump(specs, path) ?? new List<PropertyDumpItem>();

}