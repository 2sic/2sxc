﻿using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Sys.Metadata;

internal class PropLookupMetadata(IHasMetadata parent, Func<bool> getDebug) : IPropertyLookup
{
    public IHasMetadata Parent { get; } = parent;

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynEnt", getDebug());
        var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
        // check Entity is null (in cases where null-objects are asked for properties)
        if (Parent?.Metadata == null)
            return l.Return(PropReqResult.Null(path),"no parent with metadata");
        path = path.KeepOrNew().Add("DynEnt", specs.Field);

        // Note: most of the following lines are copied from Metadata
        var list = (Parent /*as IHasMetadata*/).Metadata;
        var found = list.FirstOrDefault(md => md.Attributes.ContainsKey(specs.Field));
        if (found == null)
            return l.Return(PropReqResult.Null(path), "no entity had attribute");
        var propRequest = found.FindPropertyInternal(specs, path);
        return l.Return(propRequest, propRequest.Result == null ? "found null" : "found value");
    }

    // #DropUseOfDumpProperties
    //// Note: This kind of doesn't really work
    //// but the Metadata isn't expected to dump properly
    //// May need some tweaking to just iterate through all, if really needed
    //public List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path)
    //    => Parent?.Metadata?.FirstOrDefault()?._DumpNameWipDroppingMostCases(specs, path) ?? [];
}