using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

public interface ICodeAnyApiHelper
{
    #region TODO: WHEN READY and moved deeper in the structure
    // 1. Block
    // 2. Convert
    // 3. Re-Type the Resources and Settings

    IBlock Block { get; }


    #endregion

    #region Context & Data

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    ICmsContext CmsContext { get; }

    /// <inheritdoc cref="Razor.Html5.Data" />
    IDataSource Data { get; }


    #endregion

    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="protector"></param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <returns></returns>
    [PrivateApi("new v17.02")]
    TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type? type = default) where TService : class;

    #region Helpers / Tools

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

    [PrivateApi("internal use only")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ICodeDataFactory Cdf { get; }

    #endregion

    #region Common Services (Link)

    /// <inheritdoc cref="Razor.Html5.Link" />
    ILinkService Link { get; }

    IConvertService Convert { get; }

    #endregion

}