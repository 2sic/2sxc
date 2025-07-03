using System.Diagnostics;
using System.Text.Json;
using ToSic.Sxc.Services.TurnOn.Sys;

namespace ToSic.Sxc.Tests.ServicesTests.TurnOnTests;


public class PickOrBuildSpecsTests
{
    [Fact]
    public void RunAnonObject() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run" },
            TurnOnTestAccessors.TacPickOrBuildSpecs(new { run = "window.run"})
        );

    [Fact]
    public void RunOnly() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run")
        );

    [Fact]
    public void RunArgsOnly() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Args = ["test"] },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", args: new string[] { "test" })
        );
    [Fact]
    public void RunArgsAndData() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Args = ["test"], Data = 42 },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", args: new string[] { "test" }, data: 42)
        );


    [Fact]
    public void RunAndDataNumber() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Data = 42 },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", data: 42)
        );

    [Fact]
    public void RunAndDataString() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Data = "my-data" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", data: "my-data")
        );

    [Fact]
    public void RunAndRequireString() =>
        CompareJsonsAndTrace(
            new() { Run = "window.run", Require = "window.xyz" },
            TurnOnTestAccessors.TacPickOrBuildSpecs("window.run", require: "window.xyz")
        );

    [Fact]
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
        Equal(expectedJson, actualJson);
    }
}