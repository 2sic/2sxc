using ToSic.Eav.Context;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AdamWorkOptions()
{
    public bool UsePortalRoot { get; init; }
    public string Field { get; init; } = "";
    public Guid ItemGuid { get; init; }
    public string ContentType { get; init; } = "";
    public int AppId { get; init; }

    /// <summary>
    /// Prepared server-side context for this app, including module permissions when available.
    /// </summary>
    public required IContextOfApp? Context { get; init; }
}
