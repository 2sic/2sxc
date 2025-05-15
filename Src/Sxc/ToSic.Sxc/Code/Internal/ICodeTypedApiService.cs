using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface ICodeTypedApiService: ICodeAnyApiHelper
{
    #region Content, Header, App, Data, Resources, Settings

    ///// <inheritdoc cref="System.Web.UI.WebControls.Content" />
    //dynamic Content { get; }

    ///// <inheritdoc cref="Razor.Html5.Header" />
    //dynamic Header { get; }

    IAppTyped AppTyped { get; }


    #endregion



    ///// <inheritdoc cref="IDynamicCode.CmsContext" />
    //ICmsContext CmsContext { get; }

    ///// <inheritdoc cref="IDynamicCode.Edit" />
    //IEditService Edit { get; }


    ///// <inheritdoc cref="Razor.Html5.Link" />
    //ILinkService Link { get; }

}