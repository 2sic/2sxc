using System.Collections.Specialized;

namespace ToSic.Sxc.Web.Sys.Url;

public class NameValueCollectionSort
{
    [Theory]
    [InlineData(nameof(Bac), "a,c", "a,c,b")]
    [InlineData(nameof(Bac), "c", "c,a,b")]
    [InlineData(nameof(BacdeWithDNull), "a,c", "a,c,b,d,e")]
    [InlineData(nameof(BacdeWithDNull), "c,d", "c,d,a,b,e")]
    [InlineData(nameof(BacdeWithDEmpty), "a,c", "a,c,b,d,e")]
    [InlineData(nameof(BacdeWithDEmpty), "c,d", "c,d,a,b,e")]
    public void SortWithPrioritization(string name, string sort, string expected)
    {
        var sorted = NameCollections[name].Sort(sort);
        Equal(expected.Split(','), sorted.AllKeys);
    }

    [Theory]
    [InlineData(nameof(Bac), "a,b,c", "standard a-z")]
    [InlineData(nameof(BacdeWithDNull), "a,b,c,d,e", "the null entry remains where it is")]
    [InlineData(nameof(BacdeWithDEmpty), "a,b,c,d,e", "the empty entry is in normal spot")]
    public void SortWithoutPrioritization(string name, string expected, string notes)
    {
        var sorted = NameCollections[name].Sort();
        Equal(expected.Split(','), sorted.AllKeys);
    }

    #region Test Data

    private static NameValueCollection Bac => new()
    {
        { "b", "2" },
        { "a", "1" },
        { "c", "3" }
    };

    private static NameValueCollection BacdeWithDNull => new()
    {
        { "b", "2" },
        { "a", "1" },
        { "c", "3" },
        { "d", null },
        { "e", "9" }
    };
    private static NameValueCollection BacdeWithDEmpty => new()
    {
        { "b", "2" },
        { "a", "1" },
        { "c", "3" },
        { "d", "" },
        { "e", "9" }
    };

    private static Dictionary<string, NameValueCollection> NameCollections => new()
    {
        { nameof(Bac), Bac },
        { nameof(BacdeWithDNull), BacdeWithDNull },
        { nameof(BacdeWithDEmpty), BacdeWithDEmpty }
    };



    #endregion
}
