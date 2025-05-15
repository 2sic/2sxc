using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface ICodeDynamicApiService: ICodeAnyApiHelper
{

    #region Content, Header, App, Data, Resources, Settings

    /// <inheritdoc cref="System.Web.UI.WebControls.Content" />
    dynamic Content { get; }

    /// <inheritdoc cref="Razor.Html5.Header" />
    dynamic Header { get; }

    /// <inheritdoc cref="Eav.DataSources.App" />
    IApp App { get; }

    /// <summary>
    /// Almost every use 
    /// </summary>
    /*IDynamicStack*/
    object Resources { get; }
    /*IDynamicStack*/
    object Settings { get; }


    #endregion


    ///// <inheritdoc cref="IDynamicCode.CmsContext" />
    //ICmsContext CmsContext { get; }

    /// <inheritdoc cref="IDynamicCode.Edit" />
    IEditService Edit { get; }


    ///// <inheritdoc cref="Razor.Html5.Link" />
    //ILinkService Link { get; }

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    T CreateSource<T>(IDataStream source) where T : IDataSource;


    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource;

    #endregion

}