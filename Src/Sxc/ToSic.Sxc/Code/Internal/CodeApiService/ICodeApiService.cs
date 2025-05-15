using ToSic.Eav.Code;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// This is the same as IDynamicCode, but the root object. 
/// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeApiService : IExecutionContext, IHasLog, IHasPiggyBack
{
    #region AsConverter (internal)

    [PrivateApi("internal use only")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ICodeDataFactory Cdf { get; }


    #endregion
}