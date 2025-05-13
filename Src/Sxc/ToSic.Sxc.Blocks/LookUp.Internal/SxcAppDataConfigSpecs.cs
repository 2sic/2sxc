using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.LookUp.Internal;

internal class SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock BlockForLookupOrNull { get; init; }
}