using System.Text.Json.Nodes;
using ToSic.Lib.Data;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;


namespace ToSic.Sxc.Data;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PreWrapJsonArray(CodeJsonWrapper wrapper, JsonArray jsonArray)
    : PreWrapJsonBase(wrapper, jsonArray), IWrapper<JsonArray>
{
    protected readonly JsonArray UnwrappedContents = jsonArray;

    public JsonArray GetContents() => UnwrappedContents;

    public override object JsonSource() => UnwrappedContents;

    #region Keys

    public override IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, UnwrappedContents?.Select((p, i) => i.ToString()));

    public override bool ContainsKey(string name)
    {
        if (name.IsEmpty() || UnwrappedContents == null) return false;
        var index = name.ConvertOrFallback<int>(fallback: -1, numeric: true);
        if (index == -1) return false;
        return index < UnwrappedContents.Count;
    }

    #endregion

    public override TryGetResult TryGetWrap(string name, bool wrapDefault = true)
    {
        if (UnwrappedContents == null || !UnwrappedContents.Any())
            return new(false, null, null);

        var found = UnwrappedContents.FirstOrDefault(p =>
        {
            if (p is not JsonObject pJObject) return false;
            return HasPropertyWithValue(pJObject, "Name", name)
                   || HasPropertyWithValue(pJObject, "Title", name);
        });

        return new(false, found,
            Wrapper.IfJsonGetValueOrJacket(found));
    }


    private static bool HasPropertyWithValue(JsonObject obj, string propertyName, string value)
    {
        if (obj.TryGetPropertyValue(propertyName, out var propVal) && propVal is JsonValue jVal && jVal.TryGetValue<string>(out var strVal))
            return strVal.EqualsInsensitive(value);

        return false;

    }

    // #DropUseOfDumpProperties
    //public override List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path)
    //    => [new() { Path = $"Not supported on {nameof(DynamicJacketList)}" }];


}