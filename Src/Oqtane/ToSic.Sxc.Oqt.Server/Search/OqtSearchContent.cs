using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Data;
using ToSic.Sxc.Search;
using ToSic.Sxc.Search.Sys;

namespace ToSic.Sxc.Oqt.Server.Search;

internal static class OqtSearchContent
{
    private const string EntityName = "2sxc";

    internal static SearchItemData FromEntity(IEntity entity, string stream, string language, PageModule pageModule)
        => new()
        {
            Entity = entity,
            UniqueKey = entity.EntityGuid != Guid.Empty ? entity.EntityGuid.ToString() : $"{stream}-{entity.EntityId}",
            Title = entity.Title?[language]?.ToString() ?? "(no title)",
            Body = SearchIndexing.Body(entity, language),
            ModifiedTimeUtc = Utc(entity.Modified == DateTime.MinValue ? pageModule.Module.ModifiedOn : entity.Modified),
            CultureCode = language,
        };

    /// <summary>Maps customized items and selects changes for Oqtane's incremental indexer.</summary>
    public static List<SearchContent> Map(IEnumerable<ISearchItem> items, PageModule pageModule, DateTime lastIndexedOn)
    {
        var since = Utc(lastIndexedOn);
        var now = DateTime.UtcNow;
        var page = pageModule.Page;
        var pageInactive = page.IsDeleted || !Utilities.IsEffectiveAndNotExpired(page.EffectiveDate, page.ExpiryDate);
        var inactive = pageInactive || pageModule.IsDeleted
            || !Utilities.IsEffectiveAndNotExpired(pageModule.EffectiveDate, pageModule.ExpiryDate);

        // With Page in Ignore Entities, the job keeps the cutoff even when the page was edited.
        // Expiry also needs its own check because crossing the date leaves ModifiedOn unchanged.
        var deleteUnchanged = (pageInactive && Utc(page.ModifiedOn) >= since)
            || ExpiredSince(pageModule.ExpiryDate, since, now)
            || ExpiredSince(page.ExpiryDate, since, now);
        var documents = new List<SearchContent>();
        // Same key from several streams: keep the first, as one document per occurrence.
        foreach (var item in items.DistinctBy(item => item.UniqueKey, StringComparer.Ordinal))
        {
            // DNN rejects empty keys too; here they would collapse into one document.
            if (string.IsNullOrWhiteSpace(item.UniqueKey))
                throw new InvalidOperationException("Search items require a stable unique key.");

            var modified = Utc(item.ModifiedTimeUtc);
            if (item.IsActive && modified < since && !deleteUnchanged)
                continue;

            documents.Add(new()
            {
                EntityName = EntityName,
                // Shared modules have a separate document for each page occurrence.
                EntityId = $"{pageModule.PageModuleId}:{item.UniqueKey}",
                Title = item.Title,
                Description = item.Description,
                Body = item.Body,
                Url = GetUrl(item, page.Path),
                ContentModifiedOn = modified,
                // Edits to content on an inactive occurrence must stay out of the index.
                IsDeleted = inactive || !item.IsActive,
            });
        }
        return documents;
    }

    private static string GetUrl(ISearchItem item, string pagePath)
    {
        // SearchIndexJob supplies the page URL unless customization needs to extend it.
        if (string.IsNullOrWhiteSpace(item.QueryString))
            return item.Url;

        // ISearchItem allows a custom Url with a #fragment; the query belongs before it.
        var url = string.IsNullOrEmpty(item.Url) ? "/" + pagePath.TrimStart('/') : item.Url;
        var fragmentAt = url.IndexOf('#');
        var fragment = fragmentAt >= 0 ? url[fragmentAt..] : "";
        var path = fragmentAt >= 0 ? url[..fragmentAt] : url;
        return path + (path.Contains('?') ? "&" : "?") + item.QueryString.TrimStart('?', '&') + fragment;
    }

    // Oqtane saves the scan's start time, so an expiry seen during one scan can fall
    // in the next scan's window too. Oqtane treats now > ExpiryDate as expired.
    private static bool ExpiredSince(DateTime? expiry, DateTime since, DateTime now)
        => expiry is { } value && Utc(value) >= since && Utc(value) < now;

    // EAV transaction timestamps and Oqtane's Search_LastIndexedOn are stored as UTC but
    // load as Unspecified. Converting them as local time would shift the boundary.
    internal static DateTime Utc(DateTime value)
        => value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
}
