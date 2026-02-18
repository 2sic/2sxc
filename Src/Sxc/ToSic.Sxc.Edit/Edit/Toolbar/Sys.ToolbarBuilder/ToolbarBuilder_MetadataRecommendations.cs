using ToSic.Eav.Metadata;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private ICollection<string> GetMetadataTypeNames(object? target, string? contentTypes)
    {
        var types = contentTypes.CsvToArrayWithoutEmpty();
        if (!types.Any())
            return FindMetadataRecommendations(target);

        var typesToReturn = types
            .SelectMany(t => t == "*" ? FindMetadataRecommendations(target) : [t])
            .ToList();

        return typesToReturn;
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