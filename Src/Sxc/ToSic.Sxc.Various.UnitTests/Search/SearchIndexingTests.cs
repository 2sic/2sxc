using System.Collections.Generic;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Search.Sys;

namespace ToSic.Sxc.Tests.Search;

public class SearchIndexingTests
{
    [Theory]
    [InlineData("", 2)]
    [InlineData(" content ", 1)]
    [InlineData("CONTENT, header", 2)]
    [InlineData("Presentation,ListPresentation", 0)]
    [InlineData("missing", 0)]
    public void Streams_RespectsSelection_AndExcludesPresentation(string selected, int expectedCount)
    {
        var streams = new Dictionary<string, IDataStream>
        {
            ["Content"] = null!,
            ["Header"] = null!,
            ["Presentation"] = null!,
            ["ListPresentation"] = null!,
        };

        var result = SearchIndexing.Streams(streams, selected);

        Equal(expectedCount, result.Length);
    }
}
