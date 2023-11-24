namespace ToSic.Sxc.Context;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPage
{
    /// <summary>
    /// The Id of the page.
    /// 
    /// 🪒 Use in Razor: `CmsContext.Page.Type`
    /// </summary>
    /// <remarks>
    /// Corresponds to the Dnn `TabId` or the Oqtane `Page.PageId`
    /// </remarks>
    int Id { get; }

    /// <summary>
    /// The page parameters, cross-platform.
    /// Use this for easy access to url parameters like ?id=xyz
    /// with `CmsContext.Page.Parameters["id"]` as a replacement for `Request.QueryString["id"]`
    /// 
    /// 🪒 Use in Razor: `CmsContext.Page.Parameters["id"]`
    /// </summary>
    IParameters Parameters { get; }


    IPage Init(int id);


    // unsure if used
    /// <summary>
    /// The resource specific url, like the one to this page or portal
    /// </summary>
    string Url { get; }

}