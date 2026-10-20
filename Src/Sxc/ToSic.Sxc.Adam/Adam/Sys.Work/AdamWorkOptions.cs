namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AdamWorkOptions
{
    public bool UsePortalRoot { get; init; }
    public string Field { get; init; } = "";
    public Guid ItemGuid { get; init; }
    public string ContentType { get; init; } = "";
    public int AppId { get; init; }
}