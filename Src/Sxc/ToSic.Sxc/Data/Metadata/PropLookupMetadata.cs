using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data;

internal class PropLookupMetadata: IPropertyLookup //, ICanBeEntity
{
    private readonly Func<bool> _getDebug;

    public IHasMetadata Parent { get; }
    //public IEntity Entity { get; }
    //public IPropertyLookup Upstream { get; }

    public PropLookupMetadata(IHasMetadata parent, /*IEntity entity, IPropertyLookup upstream,*/ Func<bool> getDebug)
    {
        _getDebug = getDebug;
        Parent = parent;
        //Entity = entity;
        //Upstream = upstream;
    }

    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.DynEnt", _getDebug());
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
        => Parent?.Metadata?.FirstOrDefault()/* Entity*/?._Dump(specs, path) ?? new List<PropertyDumpItem>();
}