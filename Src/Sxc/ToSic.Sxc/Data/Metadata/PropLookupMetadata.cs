using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data;

internal class PropLookupMetadata(IHasMetadata parent, Func<bool> getDebug) : IPropertyLookup
{
    public IHasMetadata Parent { get; } = parent;

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynEnt", getDebug());
        var safeWrap = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
        // check Entity is null (in cases where null-objects are asked for properties)
        if (Parent?.Metadata == null) return safeWrap.ReturnNull("no parent with metadata");
        path = path.KeepOrNew().Add("DynEnt", specs.Field);

        // Note: most of the following lines are copied from Metadata
        var list = (Parent /*as IHasMetadata*/).Metadata;
        var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(specs.Field));
        if (found == null) return safeWrap.ReturnNull("no entity had attribute");
        var propRequest = found.FindPropertyInternal(specs, path);
        return // propRequest?.Result != null
            /*?*/ safeWrap.Return(propRequest, propRequest?.Result == null ? "found null" : "found value");
        //: safeWrap.Return(propRequest, "found null");
        // safeWrap.Return(Upstream.FindPropertyInternal(specs, path), "base...");
    }

    // Note: This kind of doesn't really work
    // but the Metadata isn't expected to dump properly
    // May need some tweaking to just iterate through all, if really needed
    public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
        => Parent?.Metadata?.FirstOrDefault()/* Entity*/?._Dump(specs, path) ?? [];
}