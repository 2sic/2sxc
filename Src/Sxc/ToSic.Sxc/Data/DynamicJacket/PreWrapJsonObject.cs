using System.Text.Json.Nodes;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class PreWrapJsonObject(CodeJsonWrapper wrapper, JsonObject item)
    : PreWrapJsonBase(wrapper, item), IWrapper<JsonObject>
{
    public JsonObject GetContents() => item;

    public override object JsonSource() => item;

    #region Keys

    public override IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) 
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

    public override TryGetResult TryGetWrap(string name, bool wrapDefault = true)
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
            if (i == pathParts.Length -1 || !result.Found) return result;
            node = result.Raw as JsonObject;
            if (node == null) return new(false, null, null);
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

    private const string DumpSourceName = "Dynamic";

    public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
    {
        if (item == null || !item.Any()) return [];

        if (string.IsNullOrEmpty(path)) path = DumpSourceName;

        var allProperties = item.ToList();

        var simpleProps = allProperties.Where(p => p.Value is not JsonObject);
        var resultDynChildren = simpleProps.Select(p => new PropertyDumpItem
            {
                Path = path + PropertyDumpItem.Separator + p.Key,
                Property = FindPropertyInternal(specs.ForOtherField(p.Key),
                    new PropertyLookupPath().Add("DynJacket", p.Key)),
                SourceName = DumpSourceName
            })
            .ToList();

        var objectProps = allProperties
            .Where(p => p.Value is JsonObject)
            .SelectMany(p =>
            {
                var jacket = Wrapper.CreateDynJacketObject(p.Value.AsObject());
                return ((IHasPropLookup)jacket).PropertyLookup._Dump(specs, path + PropertyDumpItem.Separator + p.Key);
            })
            .Where(p => p is not null);

        resultDynChildren.AddRange(objectProps);

        // TODO: JArrays

        return resultDynChildren.OrderBy(p => p.Path).ToList();
    }


    #endregion
}