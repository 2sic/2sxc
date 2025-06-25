using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.LookUp.Sys;

public class SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock? BlockForLookupOrNull { get; init; }
}