using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

/// <summary>
/// WIP
/// </summary>
[PrivateApi("internal use only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeTypedApiHelper
{
    #region Moved here from ICodeAnyHelper

    ICodeDataFactory Cdf { get; }

    #region Context & Data

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    ICmsContext CmsContext { get; }

    IDataSource Data { get; }

    ILinkService Link { get; }

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

    #endregion

    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <param name="type">rare option: type name of the service to create; probably not relevant any more today</param>
    /// <returns></returns>
    [PrivateApi("new v17.02")]
    TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default) where TService : class;

    #endregion


    #region Content, Header, App, Data, Resources, Settings

    IAppTyped AppTyped { get; }

    public ITypedStack AllSettings { get; }

    public ITypedStack AllResources { get; }

    #endregion

    ServiceKit16 ServiceKit16 { get; }
}