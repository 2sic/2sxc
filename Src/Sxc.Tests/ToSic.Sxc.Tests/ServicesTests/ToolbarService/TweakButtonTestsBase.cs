using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

public class TweakButtonTestsBase
{
    protected ITweakButton NewTb() => new TweakButton();

    /// <summary>
    /// Compare the UiMerge of the tweak with the expected values
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="tweak"></param>
    protected static void AssertUi(IEnumerable<string> expected, ITweakButton tweak)
        => CollectionAssert.AreEqual(expected.ToList(), ((ITweakButtonInternal)tweak).UiMerge.ToList());
    
    protected static void AssertUi(IEnumerable<object> expected, ITweakButton tweak)
        => CollectionAssert.AreEqual(expected.ToList(), ((ITweakButtonInternal)tweak).UiMerge.ToList());

    protected static void AssertUiJson(IEnumerable<object> expected, ITweakButton tweak)
        => CollectionAssert.AreEqual(ToJson(expected), ToJson(((ITweakButtonInternal)tweak).UiMerge));

    private static List<string> ToJson(IEnumerable<object> values)
        => values
            .Select(v => JsonSerializer.Serialize(v))
            .ToList();

    protected static void AssertParams(IEnumerable<string> expected, ITweakButton tweak)
    {
        var parameters = ((ITweakButtonInternal)tweak).ParamsMerge;
        // Add trace of json objects for debugging
        Trace.WriteLine("expected:" + JsonSerializer.Serialize(expected));
        Trace.WriteLine("actual  :" + JsonSerializer.Serialize(parameters));
        CollectionAssert.AreEqual(expected.ToList(), ((ITweakButtonInternal)tweak).ParamsMerge.ToList());
    }

    protected static void AssertParams(IEnumerable<object> expected, ITweakButton tweak)
    {
        var parameters = ((ITweakButtonInternal)tweak).ParamsMerge;
        // Add trace of json objects for debugging
        Trace.WriteLine("expected:" + JsonSerializer.Serialize(expected));
        Trace.WriteLine("actual  :" + JsonSerializer.Serialize(parameters));
        CollectionAssert.AreEqual(expected.ToList(), parameters.ToList());
    }

    protected static void AssertParamsJson(IEnumerable<object> expected, ITweakButton tweak)
        => CollectionAssert.AreEqual(ToJson(expected), ToJson(((ITweakButtonInternal)tweak).ParamsMerge));

}