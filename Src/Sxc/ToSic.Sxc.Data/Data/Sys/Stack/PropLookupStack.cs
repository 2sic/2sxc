using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Data.Sys.PropertyStack;

namespace ToSic.Sxc.Data.Sys.Stack;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PropLookupStack(IPropertyStack stack, Func<bool> getDebug) : IPropertyLookup
{
    public IPropertyStack Stack { get; } = stack;

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynStk", getDebug());
        path = path.Add("DynStack", specs.Field);

        var l = specs.LogOrNull.Fn<PropReqResult?>(specs.Dump(), nameof(PropLookupStack));
        if (!specs.Field.HasValue())
            return l.Return(PropReqResult.Null(path), "no key");

        var hasPath = specs.Field.Contains(PropertyStack.PathSeparator.ToString());
        var r = hasPath
            ? Stack.InternalGetPath(specs, path)
            : Stack.FindPropertyInternal(specs, path);

        return l.Return(r, $"{(r.Result == null ? "null" : "ok")} using {(hasPath ? "Path" : "Property")}");
    }
}