using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Sys.Metadata;

internal class PropLookupMetadata(IHasMetadata parent, Func<bool> getDebug) : IPropertyLookup
{
    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynEnt", getDebug());
        var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
        // check Entity is null (in cases where null-objects are asked for properties)
        if (parent.Metadata == null! /* paranoid */)
            return l.Return(PropReqResult.Null(path),"no parent with metadata");
        path = path.KeepOrNew().Add("DynEnt", specs.Field);

        // Note: most of the following lines are copied from Metadata
        var list = parent.Metadata;
        var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(specs.Field));
        if (found == null)
            return l.Return(PropReqResult.Null(path), "no entity had attribute");
        var propRequest = found.FindPropertyInternal(specs, path);
        return l.Return(propRequest, propRequest.Result == null ? "found null" : "found value");
    }
}