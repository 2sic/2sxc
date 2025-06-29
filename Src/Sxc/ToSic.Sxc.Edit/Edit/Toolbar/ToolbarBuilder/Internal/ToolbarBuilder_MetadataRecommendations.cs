﻿using ToSic.Eav.Metadata;

using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial record ToolbarBuilder
{
    private List<string> GetMetadataTypeNames(object? target, string? contentTypes)
    {
        var types = contentTypes.CsvToArrayWithoutEmpty();
        if (!types.Any())
            types = FindMetadataRecommendations(target);

        var finalTypes = new List<string>();
        foreach (var type in types)
            if (type == "*") finalTypes.AddRange(FindMetadataRecommendations(target));
            else finalTypes.Add(type);
        return finalTypes;
    }

    private string[] FindMetadataRecommendations(object? target)
    {
        var l = Log.Fn<string[]>();
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (target == null)
            return l.Return([], "null");

        if (target is IHasMetadata withMetadata)
            target = withMetadata.Metadata;

        if (target is not IMetadata mdOf)
            return l.Return([], "not metadata");

        var recommendations = mdOf.Target?.Recommendations ?? [];

        return l.Return(recommendations, "ok");
    }
}