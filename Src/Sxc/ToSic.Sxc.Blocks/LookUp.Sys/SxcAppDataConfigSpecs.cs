using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.LookUp.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock? BlockForLookupOrNull { get; init; }
}