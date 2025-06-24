using System.Reflection;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyStack;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Sys.Typed;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Sys.Wrappers;

// WIP
// Inspired by https://stackoverflow.com/questions/46948289/how-do-you-convert-any-c-sharp-object-to-an-expandoobject
// That was more complex with ability so set new values and switch between case-insensitive or not but that's not the purpose of this
/// <summary>
/// 
/// </summary>
/// <remarks>
/// Will always return a value even if the property doesn't exist, in which case it resolves to null.
/// </remarks>
[JsonConverter(typeof(DynamicJsonConverter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class PreWrapObject: PreWrapBase, IPropertyLookup, IHasJsonSource, IPreWrap
{
    #region Constructor / Setup

    /// <summary>
    /// Case-insensitive property dictionary
    /// </summary>
    private Dictionary<string, PropertyInfo> PropDic { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="settings">
    /// Determines if properties which are objects should again be wrapped.
    /// When using this for DynamicModel it should be false, otherwise usually true.
    /// </param>
    /// <param name="wrapperSvc"></param>
    [PrivateApi]
    internal PreWrapObject(object? data, WrapperSettings settings, ICodeDataPoCoWrapperService wrapperSvc): base(data)
    {
        WrapperSvc = wrapperSvc;
        _innerObject = data;
        Settings = settings;
        PropDic = CreatePropertyInfoDictionary(data);
    }

    private ICodeDataPoCoWrapperService WrapperSvc { get; }
    private readonly object? _innerObject;
    public override WrapperSettings Settings { get; }

    internal static Dictionary<string, PropertyInfo> CreatePropertyInfoDictionary(object? original)
    {
        var dic = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
        if (original is null)
            return dic;
        var itemType = original.GetType();
        foreach (var propertyInfo in itemType.GetProperties())
            dic[propertyInfo.Name] = propertyInfo;
        return dic;
    }

    #endregion

    #region Keys

    public override bool ContainsKey(string name) => PropDic.ContainsKey(name);

    public override IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string>? only = default)
        => FilterKeysIfPossible(noParamOrder, only, PropDic.Keys);

    #endregion


    public override TryGetResult TryGetWrap(string? name, bool wrapDefault = true)
    {
        if (name.IsEmptyOrWs() || _innerObject == null)
            return new(false, null, null);

        var isPath = name.Contains(PropertyStack.PathSeparator.ToString());

        if (!isPath)
            return TryGetFromNode(name, this, wrapDefault);

        var pathParts = PropertyStack.SplitPathIntoParts(name);
        var node = this;
        for (var i = 0; i < pathParts.Length; i++)
        {
            var part = pathParts[i];
            var result = TryGetFromNode(part, node, true);
            // last one or not found - return a not-found
            if (i == pathParts.Length - 1 || !result.Found) return result;

            node = (result.Result as IHasPropLookup)?.PropertyLookup as PreWrapObject;
            if (node == null) return new(false, null, null);
        }
        return new(false, null, null);

    }

    private TryGetResult TryGetFromNode(string name, PreWrapObject preWrap, bool wrapDefault)
    {
        if (!preWrap.PropDic.TryGetValue(name, out var lookup))
            return new(false, null, null);

        var result = lookup.GetValue(preWrap._innerObject);

        // Probably re-wrap for further dynamic navigation!
        return new(true, result,
            Settings.WrapChildren && wrapDefault
                ? WrapperSvc.ChildNonJsonWrapIfPossible(result, Settings.WrapRealObjects, Settings)
                : result);

    }



    public override object JsonSource() => _innerObject!;
}