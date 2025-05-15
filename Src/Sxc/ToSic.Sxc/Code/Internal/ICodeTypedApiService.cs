using ToSic.Eav.DataSource;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface ICodeTypedApiService: ICodeAllApiService
{
    #region Content, Header, App, Data, Resources, Settings

    /// <inheritdoc cref="System.Web.UI.WebControls.Content" />
    dynamic Content { get; }

    /// <inheritdoc cref="Razor.Html5.Header" />
    dynamic Header { get; }

    ///// <inheritdoc cref="IDynamicCode.App" />
    //IApp App { get; }

    IAppTyped AppTyped { get; }

    /// <inheritdoc cref="IDynamicCode.Data" />
    IDataSource Data { get; }

    ///// <summary>
    ///// Almost every use 
    ///// </summary>
    ///*IDynamicStack*/
    //object Resources { get; }
    ///*IDynamicStack*/
    //object Settings { get; }


    #endregion



    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    ICmsContext CmsContext { get; }

    /// <inheritdoc cref="IDynamicCode.Edit" />
    IEditService Edit { get; }


    /// <inheritdoc cref="Razor.Html5.Link" />
    ILinkService Link { get; }

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);


}