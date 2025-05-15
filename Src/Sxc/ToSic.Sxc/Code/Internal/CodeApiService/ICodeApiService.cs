using ToSic.Eav.Code;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// This is the same as IDynamicCode, but the root object. 
/// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeApiService : IHasLog, IHasPiggyBack, IExecutionContextDataWip, ICanGetService
{
    #region Context Information

    /// <inheritdoc cref="IDynamicCode.App" />
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCode.Data" />
    IDataSource Data { get; }


    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    ICmsContext CmsContext { get; }

    #endregion

    #region AsConverter (internal)

    [PrivateApi("internal use only")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ICodeDataFactory Cdf { get; }

    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="protector"></param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <returns></returns>
    [PrivateApi]
    TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class;

    #endregion
}