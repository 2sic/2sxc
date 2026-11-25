namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AppDataConfigSpecs
{
    public bool? ShowDrafts { get; init; }
}