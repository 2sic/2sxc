using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;


public class TweakButtonColor: TweakButtonTestsBase
{
    public static IEnumerable<object[]> ColorTestData =>
    [
        ["red", "red"],
        ["pink", "pink"],
        ["000000", "000000"],
        ["000000", "#000000"],
        ["aabbcc", "aabbcc"],
        ["aabbcc", "#aabbcc"],
    ];

    [Theory]
    [MemberData(nameof(ColorTestData))]
    public void ColorPrimary(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color));

    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorPrimaryBoth(string expected, string color)
        => AssertUi([$"{RuleColor}={expected},{expected}"], NewTb().TacColor(color + "," + color));

    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorBackgroundNamed(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(background: color));

    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorForeground(string expected, string color)
        => AssertUi([$"{RuleColor}=,{expected}"], NewTb().TacColor(foreground: color));

    /// <summary>
    /// If the primary color is set, foreground will be ignored
    /// </summary>
    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorPrimaryAndForeground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color, foreground: color));

    /// <summary>
    /// If the primary color is set, foreground will be ignored
    /// </summary>
    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorPrimaryAndBackground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color, background: color));

    /// <summary>
    /// If the primary color is set, background will be ignored
    /// </summary>
    [MemberData(nameof(ColorTestData))]
    [Theory]
    public void ColorBackgroundAndForeground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected},{expected}"], NewTb().TacColor(background: color, foreground: color));

    /// <summary>
    /// This test ensures that fg/bg are placed in the correct locations,
    /// as all other tests use the same value for both fg and bg
    /// </summary>
    [Fact]
    public void ColorBackgroundAndForegroundPositions()
        => AssertUi([$"{RuleColor}=red,blue"], NewTb().TacColor(background: "red", foreground: "blue"));


    [Fact]
    public void Tooltip()
        => AssertUi([$"{RuleTooltip}=Hello"], NewTb().TacTooltip("Hello"));


}