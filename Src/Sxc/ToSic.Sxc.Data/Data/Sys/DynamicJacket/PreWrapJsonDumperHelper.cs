using System.Text.Json.Nodes;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyDump;

namespace ToSic.Sxc.Data.Sys.DynamicJacket;
internal class PreWrapJsonDumperHelper
{
    private const string DumpSourceName = "Dynamic";

    public List<PropertyDumpItem> Dump(PreWrapJsonObject parent, CodeJsonWrapper wrapper, JsonObject item,
        PropReqSpecs specs, string path, IPropertyDumpService dumpService)
    {
        if (!item.Any())
            return [];

        if (string.IsNullOrEmpty(path))
            path = DumpSourceName;

        var allProperties = item.ToList();

        var simpleProps = allProperties
            .Where(p => p.Value is not JsonObject);
        var resultDynChildren = simpleProps
            .Select(p => new PropertyDumpItem
            {
                Path = path + PropertyDumpItem.Separator + p.Key,
                Property = parent.FindPropertyInternal(specs.ForOtherField(p.Key),
                    new PropertyLookupPath().Add("DynJacket", p.Key)),
                SourceName = DumpSourceName
            })
            .ToList();

        var objectProps = allProperties
            .Where(p => p.Value is JsonObject)
            .SelectMany(p =>
            {
                var jacket = wrapper.CreateDynJacketObject(p.Value!.AsObject());
                return ((IHasPropLookup)jacket).PropertyLookup is IPropertyDumpCustom dumper
                    ? dumper._DumpProperties(specs, path + PropertyDumpItem.Separator + p.Key, dumpService)
                    : [];
            })
            .Where(p => p is not null);

        resultDynChildren.AddRange(objectProps);

        // TODO: JArrays

        return resultDynChildren
            .OrderBy(p => p.Path)
            .ToList();
    }
}
