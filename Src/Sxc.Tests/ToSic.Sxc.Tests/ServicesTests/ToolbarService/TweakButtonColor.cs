using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Sxc.Edit.Toolbar.Internal;
using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

[TestClass]
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

    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorPrimary(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color));

    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorPrimaryBoth(string expected, string color)
        => AssertUi([$"{RuleColor}={expected},{expected}"], NewTb().TacColor(color + "," + color));

    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorBackgroundNamed(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(background: color));

    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorForeground(string expected, string color)
        => AssertUi([$"{RuleColor}=,{expected}"], NewTb().TacColor(foreground: color));

    /// <summary>
    /// If the primary color is set, foreground will be ignored
    /// </summary>
    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorPrimaryAndForeground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color, foreground: color));

    /// <summary>
    /// If the primary color is set, foreground will be ignored
    /// </summary>
    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorPrimaryAndBackground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected}"], NewTb().TacColor(color, background: color));

    /// <summary>
    /// If the primary color is set, background will be ignored
    /// </summary>
    [DynamicData(nameof(ColorTestData))]
    [TestMethod]
    public void ColorBackgroundAndForeground(string expected, string color)
        => AssertUi([$"{RuleColor}={expected},{expected}"], NewTb().TacColor(background: color, foreground: color));

    /// <summary>
    /// This test ensures that fg/bg are placed in the correct locations,
    /// as all other tests use the same value for both fg and bg
    /// </summary>
    [TestMethod]
    public void ColorBackgroundAndForegroundPositions()
        => AssertUi([$"{RuleColor}=red,blue"], NewTb().TacColor(background: "red", foreground: "blue"));


    [TestMethod]
    public void Tooltip()
        => AssertUi([$"{RuleTooltip}=Hello"], NewTb().TacTooltip("Hello"));


}