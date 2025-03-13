using ToSic.Lib.Coding;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

internal static class TweakButtonTestAccessors
{
    public static ITweakButton TacShow(this ITweakButton button, bool show = true)
        => button.Show(show);

    public static ITweakButton TacColor(this ITweakButton button, string color = default,
        NoParamOrder noParamOrder = default, string background = default,
        string foreground = default)
        => button.Color(color, noParamOrder, background, foreground);

    public static ITweakButton TacTooltip(this ITweakButton button, string value)
        => button.Tooltip(value);

    public static ITweakButton TacGroup(this ITweakButton button, string value)
    => button.Group(value);

    public static ITweakButton TacIcon(this ITweakButton button, string value)
        => button.Icon(value);

    public static ITweakButton TacClasses(this ITweakButton button, string value)
        => button.Classes(value);

    public static ITweakButton TacPosition(this ITweakButton button, int value)
        => button.Position(value);

    public static ITweakButton TacUi(this ITweakButton button, object value)
        => button.Ui(value);

    public static ITweakButton TacUi(this ITweakButton button, string name, object value)
        => button.Ui(name, value);

    public static ITweakButton TacFormParameters(this ITweakButton button, object value)
        => button.FormParameters(value);

    public static ITweakButton TacFormParameters(this ITweakButton button, string name, object value)
        => button.FormParameters(name, value);

    public static ITweakButton TacParameters(this ITweakButton button, object value)
        => button.Parameters(value);

    public static ITweakButton TacParameters(this ITweakButton button, string name, object value)
        => button.Parameters(name, value);

    public static ITweakButton TacPrefill(this ITweakButton button, object value)
        => button.Prefill(value);

    public static ITweakButton TacPrefill(this ITweakButton button, string name, object value)
        => button.Prefill(name, value);

    public static ITweakButton TacFilter(this ITweakButton button, string name, object value)
        => button.Filter(name, value);

    public static ITweakButton TacFilter(this ITweakButton button, object value)
        => button.Filter(value);

    public static ITweakButton TacCondition(this ITweakButton button, bool value)
        => button.Condition(value);



}