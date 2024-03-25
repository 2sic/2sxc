using System.Collections.Immutable;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

/// <summary>
/// Must be public because of side effect with old/dynamic razor.
/// Example which would fail if it's internal:
/// - `tweak: b => b.Tooltip(Resources.LabelRegistrations).Filter("EventDate", d.EntityId))`
/// In this case `.Filter` would fail because the tooltip comes from a dynamic object,
/// so then the compiler will eval the resulting object, and it can't be internal.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TweakButton: ITweakButton, ITweakButtonInternal
{
    IImmutableList<object> ITweakButtonInternal.UiMerge => _uiMerge;
    IImmutableList<object> ITweakButtonInternal.ParamsMerge => _paramsMerge;
    IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> ITweakButtonInternal.Named => _named;
    private readonly IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> _named;
    private readonly IImmutableList<object> _paramsMerge;
    private readonly IImmutableList<object> _uiMerge;

    internal TweakButton()
    {
        _uiMerge = ImmutableList.Create<object>();
        _paramsMerge = ImmutableList.Create<object>();
        _named = new Dictionary<string, Func<ITweakButton, ITweakButton>>().ToImmutableDictionary();
    }

    private TweakButton(ITweakButton original, IImmutableList<object> ui = default, IImmutableList<object> @params = default, IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> named = default)
    {
        var origInternal = original as ITweakButtonInternal;
        _uiMerge = ui ?? origInternal?.UiMerge ?? ImmutableList.Create<object>();
        _paramsMerge = @params ?? origInternal?.ParamsMerge ?? ImmutableList.Create<object>();
        _named = named ?? origInternal?.Named ?? new Dictionary<string, Func<ITweakButton, ITweakButton>>().ToImmutableDictionary();
    }

    /// <summary>
    /// Helper to create an empty TweakButton function as fallback when not provided.
    /// </summary>
    /// <param name="btn"></param>
    /// <returns></returns>
    public static ITweakButton NoOp(ITweakButton btn) => btn;

    #region UI

    public ITweakButton Note(
        string note,
        NoParamOrder noParamOrder = default,
        string type = default,
        string background = default,
        int delay = default,
        int linger = default,
        string format = default
    )
    {
        var noteProps = new Dictionary<string, object> { [nameof(note)] = note };
        if (type != default) noteProps[nameof(type)] = type;
        if (background != default) noteProps[nameof(background)] = background;
        if (delay != default) noteProps[nameof(delay)] = delay;
        if (linger != default) noteProps[nameof(linger)] = linger;
        if (format != default) noteProps["asHtml"] = format.EqualsInsensitive("html");
        return Ui(new { note = noteProps });

        //void AddIfNotDefault<T>(string name, T value)
        //{
        //    if (value != default) noteProps[name] = value;
        //}
    }

    public ITweakButton Show(bool show = true) => Ui("show", show.ToString().ToLowerInvariant());

    public ITweakButton Color(string color = default, NoParamOrder noParamOrder = default, string background = default,
        string foreground = default)
    {
        if (color == default)
        {
            color = background;
            if (foreground != default) color += $",{foreground}";
        }

        // Remove hashes, because they break the URL api, and spaces, as they shouldn't be there
        color = color?.Replace("#", "").Replace(" ", "");

        return Ui("color", color);
    }

    public ITweakButton Tooltip(string value) => value.IsEmptyOrWs() ? this : Ui("title", value);

    public ITweakButton Group(string value) => value.IsEmptyOrWs() ? this : Ui("group", value);

    public ITweakButton Icon(string value) => value.IsEmptyOrWs() ? this : Ui(new { icon = value });

    public ITweakButton Classes(string value) => value.IsEmptyOrWs() ? this : Ui("class", value);

    public ITweakButton Position(int value) => Ui("pos", value);

    public ITweakButton Ui(object value) => value == null ? this : new(this, _uiMerge.Add(value));

    public ITweakButton Ui(string name, object value) => (value ?? name) == null ? this : Ui($"{name}={value}");

    #endregion

    #region Params

    public ITweakButton FormParameters(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixForm));

    public ITweakButton FormParameters(string name, object value) => (value ?? name) == null ? this : FormParameters($"{name}={value}");

    public ITweakButton Parameters(object value) => value == null ? this : new(this, @params: _paramsMerge.Add(value));
    public ITweakButton Parameters(string name, object value) => (value ?? name) == null ? this : Parameters($"{name}={value}");

    public ITweakButton Prefill(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixPrefill));

    public ITweakButton Prefill(string name, object value) => (value ?? name) == null ? this : Prefill($"{name}={value}");

    public ITweakButton Filter(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixFilters));
    public ITweakButton Filter(string name, object value) => (value ?? name) == null ? this : Filter($"{name}={value}");

    public ITweakButton AddNamed(string name, Func<ITweakButton, ITweakButton> value) => value == null ? this : new(this, named: _named.Add(name, value));

    #endregion
}