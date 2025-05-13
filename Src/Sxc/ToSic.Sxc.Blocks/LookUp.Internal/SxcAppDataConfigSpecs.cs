using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.LookUp.Internal;

public class SxcAppDataConfigSpecs: AppDataConfigSpecs
{
    public IBlock BlockForLookupOrNull { get; init; }
}