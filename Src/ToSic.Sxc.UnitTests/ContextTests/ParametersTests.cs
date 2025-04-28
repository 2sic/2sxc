using System;
using ToSic.Sxc.Context;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.ContextTests;


public class ParametersTests
{
    #region Initial Data and Helper Methods

    /// <summary>
    /// Take the default params, modify them in some way, and verify that the count / result matches expectations
    /// </summary>
    /// <param name="count"></param>
    /// <param name="exp"></param>
    /// <param name="pFunc"></param>
    private void ModifyDefAndVerify(int count, string exp, Func<IParameters, IParameters> pFunc)
    {
        var p = pFunc(ParametersId27SortDescending());
        Equal(count, p.Count);
        Equal(exp, p.ToString());
    }

    #endregion

    #region Get Tests

    [Theory]
    [InlineData("27", "id")]
    [InlineData("descending", "sort")]
    [InlineData(null, "unknown")]
    public void Get(string expected, string key)
        => Equal(expected, ParametersId27SortDescending().Get(key));

    [Theory]
    [InlineData("27", "id")]
    [InlineData("descending", "sort")]
    [InlineData(null, "unknown")]
    public void GetString(string expected, string key)
        => Equal(expected, ParametersId27SortDescending().String(key));

    [Theory]
    [InlineData("27", "id")]
    [InlineData("descending", "sort")]
    [InlineData(null, "unknown")]
    public void GetStringT(string expected, string key)
        => Equal(expected, ParametersId27SortDescending().Get<string>(key));

    [Theory]
    [InlineData(27, "id")]
    [InlineData(0, "sort")]
    [InlineData(0, "unknown")]
    public void GetInt(int expected, string key)
        => Equal(expected, ParametersId27SortDescending().Int(key));

    [Theory]
    [InlineData(27, "id")]
    [InlineData(0, "sort")]
    [InlineData(0, "unknown")]
    public void GetBool(int expected, string key)
        => Equal(expected, ParametersId27SortDescending().Int(key));

    #endregion


    [Fact]
    public void BasicParameters()
    {
        var p = ParametersId27SortDescending();
        Equal(2, p.Count);
        True(p.ContainsKey("id"));
        True(p.ContainsKey("ID"));
    }

    [Fact]
    public void BasicCaseSensitivity()
    {
        var p = ParametersId27SortDescending();
        True(p.ContainsKey("id"));
        True(p.ContainsKey("ID"));
        False(p.ContainsKey("fake"));
    }




    #region Add String / Null

        

    [Theory]
    [InlineData(3, Id27SortDescending + "&test=wonderful", "test", "wonderful")]
    [InlineData(3, Id27SortDescending + "&test", "test", null, "null")]
    [InlineData(3, Id27SortDescending + "&test", "test", "", "empty")]
    public void AddStringNew(int count, string expected, string key = "id", string value = default, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

    [Fact]
    public void AddStringNewNoValue()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test", p => p.TestAdd("test"));

    [Fact]
    public void AddStringNewMultipleSameKey()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test=awesome&test=wonderful",
            p => p.TestAdd("test", "wonderful").TestAdd("Test", "awesome"));



    #endregion

    #region Add / Set boolean

    [Theory]
    [InlineData(3, Id27SortDescending + "&test=true", "test", true)]
    [InlineData(3, Id27SortDescending + "&test=false", "test", false)]
    public void AddBool(int count, string expected, string key, bool value, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));


    [Theory]
    [InlineData(3, Id27SortDescending + "&test=true", "test", true)]
    [InlineData(3, Id27SortDescending + "&test=false", "test", false)]
    [InlineData(2, "id=true&sort=descending", "id", true, "replace int-id with bool id")]
    [InlineData(2, "id=false&sort=descending", "id", false, "replace int-id with bool id")]
    public void SetBool(int count, string expected, string key, bool value, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

    #endregion

    #region Add Numbers

    [Theory]
    [InlineData(3, Id27SortDescending + "&test=7", "test", 7)]
    [InlineData(3, Id27SortDescending + "&test=-7", "test", -7)]
    [InlineData(3, Id27SortDescending + "&test=7", "test", 7L)]
    [InlineData(3, Id27SortDescending + "&test=7.7", "test", 7.7)]
    [InlineData(3, Id27SortDescending + "&test=-7.7", "test", -7.7)]
    [InlineData(3, Id27SortDescending + "&test=7.7", "test", 7.7F)]
    [InlineData(3, Id27SortDescending + "&test=-7.7", "test", -7.7F)]
    public void AddNumberObjects(int count, string expected, string key, object value, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

    #endregion

    #region Add Dates

    private static readonly DateTime TestDate = new(2042, 4, 2);
    private static readonly DateTime TestDateTime = new(2042, 4, 2, 3, 4, 56);

    [Fact]
    public void AddNewDate()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test=2042-04-02", p => p.TestAdd("test", TestDate));
        
    [Fact]
    public void AddNewDateTime()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test=2042-04-02T03:04:56", p => p.TestAdd("test", TestDateTime));

    #endregion

    [Theory]
    [InlineData(3, Id27SortDescending + "&test=wonderful", "test", "wonderful")]
    [InlineData(3, Id27SortDescending + "&test", "test", null, "null")]
    [InlineData(3, Id27SortDescending + "&test", "test", "", "empty")]
    public void SetNew(int count, string expected, string key = "id", string value = default, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

    [Fact]
    public void SetNewNoValue()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test", p=>p.TestSet("test"));


    [Fact]
    public void SetNewMultipleSameKey()
        => ModifyDefAndVerify(3, Id27SortDescending + "&test=awesome", p => p.TestSet("test", "wonderful").TestSet("test", "awesome"));

    [Theory]
    [InlineData(2, "id=27&id=42&sort=descending", "id", "42")]
    [InlineData(2, Id27SortDescending, "id", "", "empty string")]
    [InlineData(2, Id27SortDescending, "id", null, "null string")]
    public void AddExisting(int count, string expected, string key = "id", string value = default, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));


    [Theory]
    [InlineData(2, "id=42&sort=descending", "id", "42")]
    [InlineData(2, "id&sort=descending", "id", "", "reset to empty string")]
    [InlineData(2, "id&sort=descending", "id", null, "reset to null")]
    public void SetExisting(int count, string expected, string key = "id", string value = default, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

    [Theory]
    [InlineData(1, "sort=descending", "id", "remove existing")]
    [InlineData(2, Id27SortDescending, "something", "remove non-existing")]
    public void Remove(int count, string expected, string key, string testName = default)
        => ModifyDefAndVerify(count, expected, p => p.TestRemove(key));

    [Fact]
    public void RemoveOfMultiple()
    {
        var p = ParametersId27SortDescending().TestAdd("id", 42).TestAdd("id", "hello")
            .TestRemove("id", "42");
        Equal("id=27&id=hello&sort=descending", p.ToString());
    }

    [Theory]
    [InlineData("id=27&id=hello", "id=27&id=42&id=hello", "id", "42")]
    [InlineData("id=27&id=42&id=hello", "id=27&id=42&id=hello", "id", "999")]
    [InlineData("id=27&id=42&id=hello", "id=27&id=42&id=hello", "notdefined", "999")]
    [InlineData("id=27&id=42", "id=27&id=42&id=hello", "id", "HELLO")]
    [InlineData("ID=27&ID=42", "id=27&id=42&id=hello", "ID", "HELLO", "removing a specific key will reset key casing")]
    public void RemoveOfMultipleFromBlank(string expected, string initial, string key, string rmvValue, string note = default)
    {
        var p = initial.AsParameters().TestRemove(key, rmvValue);
        Equal(expected, p.ToString());
    }

    [Theory]
    [InlineData(true, "id")]
    [InlineData(true, "sort")]
    [InlineData(false, "dummy")]
    [InlineData(false, "id.xyz")]
    [InlineData(false, "id.xyz.abc")]
    public void ContainsKey(bool exists, string key, string testName = default)
        => Equal(exists, ParametersId27SortDescending().ContainsKey(key));

    #region Count Tests

    public class ParameterCountTest
    {
        public int Count;
        public Func<IParameters, IParameters> Prepare = p => p;
    }

    public static TheoryData<ParameterCountTest> CountTests =>
    [
        new() { Count = 2 },
        new() { Count = 2, Prepare = p => p.TestSet("id") },
        new() { Count = 3, Prepare = p => p.TestSet("new") },
        new() { Count = 1, Prepare = p => p.TestRemove("id") },
    ];

    [Theory]
    [MemberData(nameof(CountTests))]
    public void CountKeys(ParameterCountTest pct)
        => Equal(pct.Count, pct.Prepare(ParametersId27SortDescending()).Keys().Count());

    #endregion


    [Theory]
    [InlineData(true, "id")]
    [InlineData(true, "sort")]
    [InlineData(false, "dummy")]
    public void IsNotEmpty(bool expected, string key, string testName = default)
        => Equal(expected, ParametersId27SortDescending().IsNotEmpty(key));//, testName);

    [Theory]
    [InlineData("id=42&sort=descending", "id", "42", "should replace")]
    [InlineData("id=27&new=hello&sort=descending", "new", "hello", "should append")]
    [InlineData("sort=descending", "id", "27", "should remove")]
    [InlineData("id&sort=descending", "id", "", "empty value should remove")]
    [InlineData("id&sort=descending", "id", null, "null should ???")]
    public void ToggleFrom27Descending(string expected, string key, string value, string note)
        => Equal(expected, ParametersId27SortDescending().TestToggle(key, value).ToString());//, note);

    [Theory]
    [InlineData("id=42", "id", "42", "should add")]
    [InlineData("new=hello", "new", "hello", "should add")]
    public void ToggleFromEmpty(string expected, string key, string value, string note)
    {
        var p = "".AsParameters().TestToggle(key, value);
        Equal(expected, p.ToString());//, note);
    }

    [Theory]
    [InlineData("id=42", "id", "42", "should replace")]
    [InlineData("id=hello", "id", "hello", "should replace")]
    public void ToggleFromExisting(string expected, string key, string value, string note)
    {
        var pOriginal = NewParameters(new() { { "id", "999" } });
        var p = pOriginal.TestToggle(key, value);
        Equal(expected, p.ToString());
    }

    [Theory]
    [InlineData("id=42", "id=42", "id", "should preserve")]
    [InlineData("id=42", "id=42&sort=descending", "id", "should preserve id only")]
    [InlineData("id=27&id=42", "id=42&id=27&sort=descending", "id", "should preserve id only")]
    [InlineData("sort=descending", "id=42&sort=descending", "sort", "should preserve id only")]
    public void Filter(string expected, string initial, string names, string testNotes = default)
    {
        var p = initial.AsParameters().Filter(names);
        Equal(expected, p.ToString());
    }
}