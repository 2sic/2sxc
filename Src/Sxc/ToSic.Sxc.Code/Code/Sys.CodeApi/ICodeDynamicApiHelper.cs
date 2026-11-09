using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

/// <summary>
/// WIP
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeDynamicApiHelper: ICreateInstance
{

    #region Moved here from ICodeAnyHelper


    [PrivateApi("internal use only")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ICodeDataFactory Cdf { get; }


    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    ICmsContext CmsContext { get; }

    IDataSource Data { get; }

    ILinkService Link { get; }

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

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

    public IBlock Block { get; }
    IConvertService Convert { get; }


    #region Content, Header, App, Data, Resources, Settings

    dynamic? Content { get; }

    /// <inheritdoc cref="Razor.Html5.Header" />
    dynamic? Header { get; }

    /// <inheritdoc cref="Eav.DataSources.App" />
    IApp App { get; }

    /// <summary>
    /// Almost every use 
    /// </summary>
    IDynamicStack Resources { get; }
    IDynamicStack Settings { get; }


    #endregion


    /// <inheritdoc cref="IDynamicCodeDocs.Edit" />
    IEditService Edit { get; }


    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)" />
    T CreateSource<T>(IDataStream source) where T : IDataSource;


    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)" />
    T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource;

    #endregion

    ServiceKit14 ServiceKit14 { get; }
}