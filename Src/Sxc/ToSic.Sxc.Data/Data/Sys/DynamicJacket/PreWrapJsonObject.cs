using System.Text.Json.Nodes;
using ToSic.Eav.Data.Sys.PropertyDump;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Data.Sys.PropertyStack;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Sys.DynamicJacket;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PreWrapJsonObject(CodeJsonWrapper wrapper, JsonObject item)
    : PreWrapJsonBase(wrapper, item), IWrapper<JsonObject>,
        IPropertyDumpCustom
{
    public JsonObject GetContents() => item;

    public override object JsonSource() => item;

    #region Keys

    public override IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string>? only = default) 
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, item.Select(p => p.Key));

    public override bool ContainsKey(string name)
    {
        if (name.IsEmptyOrWs() || item == null) 
            return false;

        var isPath = name.Contains(PropertyStack.PathSeparator.ToString());
        if (!isPath)
            return JsonObjectContainsKey(item, name);

        var pathParts = PropertyStack.SplitPathIntoParts(name);
        var node = item;
        for (var i = 0; i < pathParts.Length; i++)
        {
            var part = pathParts[i];
            var result = TryGetFromNode(part, node);
            // last one or not found - return a not-found
            if (i == pathParts.Length - 1 || !result.Found) return result.Found;
            node = result.Raw as JsonObject;
            if (node == null) return false;
        }

        return false;
    }

    private bool JsonObjectContainsKey(JsonObject jsonObject, string name)
        => jsonObject.Any(p => name.EqualsInsensitive(p.Key));

    #endregion

    #region TryGet

    public override TryGetResult TryGetWrap(string? name, bool wrapDefault = true)
    {
        if (name.IsEmptyOrWs() || item == null || !item.Any())
            return new(false, null, null);

        var isPath = name.Contains(PropertyStack.PathSeparator.ToString());
        if (!isPath)
            return TryGetFromNode(name, item);

        var pathParts = PropertyStack.SplitPathIntoParts(name);
        var node = item;
        for (var i = 0; i < pathParts.Length; i++)
        {
            var part = pathParts[i];
            var result = TryGetFromNode(part, node);
            // last one or not found - return a not-found
            if (i == pathParts.Length -1 || !result.Found)
                return result;
            node = result.Raw as JsonObject;
            if (node == null)
                return new(false, null, null);
        }
        return new(false, null, null);
    }

    private TryGetResult TryGetFromNode(string name, JsonObject node)
    {
        var result = node
            .FirstOrDefault(p => p.Key.EqualsInsensitive(name));

        var found = !result.Equals(default(KeyValuePair<string, JsonNode>));
        return new(found, result.Value, found ? Wrapper.IfJsonGetValueOrJacket(result.Value) : null);
    }

    #endregion

    #region Debug / Dump

    public List<PropertyDumpItem> _DumpProperties(PropReqSpecs specs, string path, IPropertyDumpService dumpService)
        => item == null || !item.Any()
            ? []
            : new PreWrapJsonDumperHelper().Dump(this, Wrapper, item, specs, path, dumpService);

    #endregion
}