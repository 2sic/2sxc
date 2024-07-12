using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.TurnOnTests;

[TestClass]
public class PickOrBuildSpecsTests
{
    [TestMethod]
    public void RunAnonObject() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run" },
            TurnOnTestAccessors.TacPickOrBuildSpecs(new { run = "window.run"})
        );

    [TestMethod]
    public void RunOnly() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run")
        );

    [TestMethod]
    public void RunArgsOnly() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Args = new[] { "test" } },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", args: new string[] { "test" })
        );
    [TestMethod]
    public void RunArgsAndData() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Args = new[] { "test" }, Data = 42 },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", args: new string[] { "test" }, data: 42)
        );


    [TestMethod]
    public void RunAndDataNumber() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Data = 42 },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", data: 42)
        );

    [TestMethod]
    public void RunAndDataString() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Data = "my-data" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", data: "my-data")
        );

    [TestMethod]
    public void RunAndRequireString() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Require = "window.xyz" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", require: "window.xyz")
        );

    [TestMethod]
    public void RunAndRequireArray() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Require = new[] { "window.xyz", "window.abc" } },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", require: new[] { "window.xyz", "window.abc" })
        );

    private static void CompareJsonsAndTrace(TurnOnSpecs expected, object actual)
    {
        var expectedJson = JsonSerializer.Serialize(expected);
        var actualJson = JsonSerializer.Serialize(actual);
        Trace.WriteLine("expected: " + expectedJson);
        Trace.WriteLine("actual: " + actualJson);
        Assert.AreEqual(expectedJson, actualJson);
    }
}