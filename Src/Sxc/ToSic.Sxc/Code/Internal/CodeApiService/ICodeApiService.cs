using ToSic.Eav.Data.PiggyBack;
using ToSic.Sxc.Data;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// This is the same as IDynamicCode, but the root object. 
/// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeApiService : IDynamicCode12, IHasPiggyBack
{
    new IDynamicStack Resources { get; }
    new IDynamicStack Settings { get; }

    #region AsConverter (internal)

    [PrivateApi("internal use only")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    CodeDataFactory _Cdf { get; }

    /// <summary>
    /// Special GetService which will cache the found service so any other kit would use it as well.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    [PrivateApi("new v17.02")]
    TService GetService<TService>(NoParamOrder protector = default, bool reuse = false) where TService : class;

    #endregion
}