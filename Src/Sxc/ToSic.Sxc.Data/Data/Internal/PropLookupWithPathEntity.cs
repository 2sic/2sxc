﻿using ToSic.Eav.Data.PropertyStack.Sys;
using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Data.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PropLookupWithPathEntity(IEntity entity, ICanDebug canDebug) :
    ICanBeEntity, // This is important, to ensure that when used in a stack it can be converted to something else again
    IPropertyLookup
{
    public IEntity Entity { get; } = entity;

    public PropReqResult? FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        specs = specs.SubLog("Sxc.PrpLkp", canDebug.Debug);
        var l = specs.LogOrNull.Fn<PropReqResult?>(specs.Dump(), "DynEntity");
        // check Entity is null (in cases where null-objects are asked for properties)
        if (Entity == null!)
            return l.ReturnNull("no entity");
        if (!specs.Field.HasValue())
            return l.ReturnNull("no path");

        path = path.KeepOrNew().Add("DynEnt", specs.Field);
        var isPath = specs.Field.Contains(PropertyStack.PathSeparator.ToString());
        var propRequest = !isPath
            ? Entity.FindPropertyInternal(specs, path)
            : PropertyStack.TraversePath(specs, path, Entity);
        return l.Return(propRequest, $"{nameof(isPath)}: {isPath}");
    }

    // #DropUseOfDumpProperties
    //public List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path) 
    //    => Entity?._DumpNameWipDroppingMostCases(specs, path) ?? [];

}