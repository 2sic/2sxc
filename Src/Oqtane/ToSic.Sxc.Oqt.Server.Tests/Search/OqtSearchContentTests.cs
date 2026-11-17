using System;
using Oqtane.Models;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Sxc.Oqt.Server.Search;
using ToSic.Sxc.Search.Sys;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.Search;

/// <summary>Checks document mapping and timestamp boundaries without a database or running host.</summary>
public class OqtSearchContentTests
{
    private static readonly DateTime Modified = new(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Map_UsesOneDocumentPerEntityKeyAndPageModule()
    {
        var first = PageModule();
        var second = PageModule();
        second.PageModuleId = 22;

        // A view can expose the same entity through more than one search stream, and a
        // shared module appears on several pages with different URLs and permissions.
        var result1 = Single(OqtSearchContent.Map([Item(), Item()], first, DateTime.MinValue));
        var result2 = Single(OqtSearchContent.Map([Item()], second, DateTime.MinValue));

        Equal(("21:entity-key", "22:entity-key"), (result1.EntityId, result2.EntityId));
    }

    [Theory]
    [InlineData(-1, true, 0)]
    [InlineData(0, true, 1)]
    [InlineData(1, true, 1)]
    [InlineData(-1, false, 1)]
    public void Map_FiltersByModification_AndIncludesExplicitDeletion(int seconds, bool active, int expectedCount)
    {
        var item = Item();
        item.ModifiedTimeUtc = Modified.AddSeconds(seconds);
        item.IsActive = active;

        var results = OqtSearchContent.Map([item], PageModule(), Modified);

        Equal(expectedCount, results.Count);
        if (!active)
            True(Single(results).IsDeleted);
    }

    [Fact]
    public void Map_DeletesUnchangedItems_WhenPageWasSoftDeletedSinceCheckpoint()
    {
        var binding = PageModule();
        var lastIndexedOn = Modified.AddMinutes(1);
        binding.ModifiedOn = Modified;
        binding.Page.IsDeleted = true;
        binding.Page.ModifiedOn = lastIndexedOn.AddMinutes(1);
        var item = Item();

        // With Page in Ignore Entities, neither the unchanged placement nor the page edit
        // makes the native job pass DateTime.MinValue. The adapter receives the normal cutoff.
        var deletion = Single(OqtSearchContent.Map([item], binding, lastIndexedOn));

        True(deletion.IsDeleted);
        Empty(OqtSearchContent.Map([item], binding, binding.Page.ModifiedOn.AddMinutes(1)));
    }

    // An expiry inside the checkpoint window requires cleanup even when the item is unchanged.
    [Theory]
    [InlineData(1, false, 1)]   // expired since the last run: deletion sent
    [InlineData(10, false, 0)]  // expired before the last run: not sent again
    [InlineData(10, true, 1)]   // edited after expiry: sent, but deleted
    public void Map_DeletesExpiredItems_WhenExpiryFallsInWindowOrItemWasEdited(int expiredMinutesAgo, bool edited, int expectedCount)
    {
        var now = DateTime.UtcNow;
        var binding = PageModule();
        binding.Page.ExpiryDate = now.AddMinutes(-expiredMinutesAgo);
        var item = Item();
        item.ModifiedTimeUtc = edited ? now.AddMinutes(-1) : now.AddDays(-1);

        var results = OqtSearchContent.Map([item], binding, now.AddMinutes(-5));

        Equal(expectedCount, results.Count);
        All(results, document => True(document.IsDeleted));
    }

    [Fact]
    public void FromEntity_IndexesFreshEdits_WhenDatabaseTimestampsLoseTheirKind()
    {
        var modifiedUtc = Modified.AddSeconds(1);
        var modified = DateTime.SpecifyKind(modifiedUtc, DateTimeKind.Unspecified);
        var entity = new EntityAssembler().Create(appId: 1, contentType: null!, titleField: "Title", modified: modified);
        var binding = PageModule();
        var item = OqtSearchContent.FromEntity(entity, "Content", "en-us", binding);

        var document = Single(OqtSearchContent.Map([item], binding, DateTime.SpecifyKind(Modified, DateTimeKind.Unspecified)));

        Equal(modifiedUtc, document.ContentModifiedOn);
        Equal(DateTimeKind.Utc, document.ContentModifiedOn.Kind);
    }

    [Theory]
    [InlineData("", "?id=42", "/articles?id=42")]
    [InlineData("/details?sort=asc", "&id=42", "/details?sort=asc&id=42")]
    [InlineData("/details#content", "id=42", "/details?id=42#content")]
    public void Map_CombinesCustomUrlAndQueryString(string url, string query, string expected)
    {
        var item = Item();
        item.Url = url;
        item.QueryString = query;

        var result = Single(OqtSearchContent.Map([item], PageModule(), DateTime.MinValue));

        Equal(expected, result.Url);
    }

    [Fact]
    public void Map_RejectsMissingUniqueKeys()
    {
        var item = Item();
        item.UniqueKey = "";

        Throws<InvalidOperationException>(() => OqtSearchContent.Map([item], PageModule(), DateTime.MinValue));
    }

    private static SearchItemData Item() => new() { UniqueKey = "entity-key", ModifiedTimeUtc = Modified };

    private static PageModule PageModule() => new()
    {
        PageModuleId = 21,
        PageId = 11,
        ModuleId = 7,
        Module = new() { ModuleId = 7, SiteId = 3, ModifiedOn = Modified, ModifiedBy = "editor" },
        Page = new() { PageId = 11, SiteId = 3, Path = "articles" },
    };
}
