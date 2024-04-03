using ToSic.Eav.Data.PiggyBack;
using ToSic.Sxc.Apps;
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

    IAppTyped AppTyped { get; }

    #region AsConverter (internal)

    [PrivateApi("internal use only")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    CodeDataFactory Cdf { get; }

    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="protector"></param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <returns></returns>
    [PrivateApi("new v17.02")]
    TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class;

    #endregion
}