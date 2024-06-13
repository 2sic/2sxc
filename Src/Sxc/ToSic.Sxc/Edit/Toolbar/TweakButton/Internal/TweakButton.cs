using System.Collections;
using System.Collections.Immutable;
using System.Text.Json;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

/// <summary>
/// Must be public because of side effect with old/dynamic razor.
/// Example which would fail if it's internal:
/// - `tweak: b => b.Tooltip(Resources.LabelRegistrations).Filter("EventDate", d.EntityId))`
/// In this case `.Filter` would fail because the tooltip comes from a dynamic object,
/// so then the compiler will eval the resulting object, and it can't be internal.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TweakButton: ITweakButton, ITweakButtonInternal
{
    IImmutableList<object> ITweakButtonInternal.UiMerge => _uiMerge;
    IImmutableList<object> ITweakButtonInternal.ParamsMerge => _paramsMerge;
    IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> ITweakButtonInternal.Named => _named;

    private readonly IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> _named;
    private readonly IImmutableList<object> _paramsMerge;
    private readonly IImmutableList<object> _uiMerge;

    public bool? _condition { get; }

    internal TweakButton()
    {
        _uiMerge = ImmutableList.Create<object>();
        _paramsMerge = ImmutableList.Create<object>();
        _named = new Dictionary<string, Func<ITweakButton, ITweakButton>>().ToImmutableDictionary();
    }

    private TweakButton(ITweakButton original, IImmutableList<object> ui = default, IImmutableList<object> parameters = default, IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> named = default, bool? condition = null)
    {
        var origInternal = original as ITweakButtonInternal;
        _uiMerge = ui ?? origInternal?.UiMerge ?? ImmutableList.Create<object>();
        _paramsMerge = parameters ?? origInternal?.ParamsMerge ?? ImmutableList.Create<object>();
        _named = named ?? origInternal?.Named ?? new Dictionary<string, Func<ITweakButton, ITweakButton>>().ToImmutableDictionary();
        _condition = condition ?? origInternal?._condition;
    }

    /// <summary>
    /// Helper to create an empty TweakButton function as fallback when not provided.
    /// </summary>
    /// <param name="btn"></param>
    /// <returns></returns>
    internal static ITweakButton NoOp(ITweakButton btn) => btn;

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
        if (format != default) noteProps[ToolbarConstants.NoteSettingHtml] = format.EqualsInsensitive(ToolbarConstants.NoteFormatHtml);
        return Ui(new { note = noteProps });
    }

    public ITweakButton Show(bool show = true)
        => Ui(ToolbarConstants.RuleShow, show ? "true" : "false");

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

        return Ui(ToolbarConstants.RuleColor, color);
    }

    public ITweakButton Tooltip(string value)
        => value.IsEmptyOrWs() ? this : Ui(ToolbarConstants.RuleTooltip, value);

    public ITweakButton Group(string value)
        => value.IsEmptyOrWs() ? this : Ui(ToolbarConstants.RuleGroup, value);

    public ITweakButton Icon(string value)
        => value.IsEmptyOrWs() ? this : Ui(new { icon = value });

    public ITweakButton Classes(string value)
        => value.IsEmptyOrWs() ? this : Ui(ToolbarConstants.RuleClass, value);

    public ITweakButton Position(int value)
        => Ui(ToolbarConstants.RulePosition, value);

    public ITweakButton Ui(object value)
        => value == null ? this : new(this, _uiMerge.Add(value));

    public ITweakButton Ui(string name, object value)
        => (value ?? name) == null ? this : Ui($"{name}={value}");

    #endregion

    #region Params

    public ITweakButton FormParameters(object value)
        => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, ToolbarConstants.RuleParamPrefixForm));

    public ITweakButton FormParameters(string name, object value)
        => (value ?? name) == null ? this : FormParameters($"{name}={value}");

    public ITweakButton Parameters(object value)
        => value == null ? this : new(this, parameters: _paramsMerge.Add(value));

    public ITweakButton Parameters(string name, object value)
        => (value ?? name) == null ? this : Parameters($"{name}={value}");

    public ITweakButton Prefill(object value)
        => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, ToolbarConstants.RuleParamPrefixPrefill));

    public ITweakButton Prefill(string name, object value)
        => (value ?? name) == null ? this : Prefill($"{name}={ValueToString(value)}");

    public ITweakButton Filter(object value)
        => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, ToolbarConstants.RuleParamPrefixFilter));

    public ITweakButton Filter(string name, object value)
        => (value ?? name) == null ? this : Filter($"{name}={ValueToString(value)}");

    /// <summary>
    /// WIP trying to get Filter with an array of IDs to return [1,2,3] instead of Int32[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string ValueToString(object value) => value switch
    {
        null => null,
        string str => str,
        IEnumerable => JsonSerializer.Serialize(value),
        _ => value.ToString()
    };

    /// <summary>
    /// Special internal add-rule, which is typically on the main toolbar,
    /// but will then only be applied to buttons with exactly this name...?
    ///
    /// Used in image responsive to add notes to the buttons,
    /// but different notes for e.g. Copyright etc.
    ///
    /// Not fully documented or standardized.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal ITweakButton AddNamed(string name, Func<ITweakButton, ITweakButton> value)
        => value == null ? this : new(this, named: _named.Add(name, value));

    #endregion

    public ITweakButton Condition(bool value)
        => new TweakButton(this, condition: value);

}