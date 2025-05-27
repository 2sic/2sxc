using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.LookUp.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface ICodeDynamicApiHelper: ICodeAnyApiHelper, ICreateInstance
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
    IDynamicStack Resources { get; }
    IDynamicStack Settings { get; }


    #endregion


    /// <inheritdoc cref="IDynamicCode.Edit" />
    IEditService Edit { get; }


    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    T CreateSource<T>(IDataStream source) where T : IDataSource;


    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource;

    #endregion

    ServiceKit14 ServiceKit14 { get; }
}