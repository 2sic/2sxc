using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

/// <summary>
/// Temporary "bridge" to pass the Convert Service (which ATM cannot be on the IConvertService API, because it's in a later project)
/// </summary>
public static class CodeApiServiceWipMigrating
{
    public static IConvertService GetConvertService(this ICodeApiService parent)
        => ((CodeApiService)parent).Convert;
}