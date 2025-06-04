using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Data;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PropLookupStack(IPropertyStack stack, Func<bool> getDebug) : IPropertyLookup
{
    public IPropertyStack Stack { get; } = stack;

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynStk", getDebug());
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

    // #DropUseOfDumpProperties
    //public List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path) =>
    //    Stack?._DumpNameWipDroppingMostCases(specs, path) ?? [];

}