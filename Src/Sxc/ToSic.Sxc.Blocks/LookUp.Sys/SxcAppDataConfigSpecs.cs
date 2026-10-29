using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.LookUp.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock? BlockForLookupOrNull { get; init; }
}