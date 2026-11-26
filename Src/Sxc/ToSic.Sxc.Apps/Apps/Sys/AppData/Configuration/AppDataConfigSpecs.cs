using ToSic.Eav.Context;

namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AppDataConfigSpecs
{
    /// <summary>
    /// Info if drafts should be shown.
    /// </summary>
    public bool? ShowDrafts { get; init; }

    /// <summary>
    /// Context of Site, App, Module - to provide additional settings and infos
    /// </summary>
    public required IContextOfSite? ContextOfSiteOrMore { get; init; }
}