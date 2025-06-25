using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.LookUp.Sys;

public class SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock? BlockForLookupOrNull { get; init; }
}