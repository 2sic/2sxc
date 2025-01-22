using ToSic.Sxc.Cms.Pages.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Pages;

/// <summary>
/// BETA Data Model as is returned by the <see cref="Pages"/> DataSource.
/// </summary>
/// <remarks>
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn TabInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Tabs.TabInfo.html)
/// * [Oqtane Page](https://docs.oqtane.org/api/Oqtane.Models.Page.html)
///
/// History
/// 
/// * Released v19.01
/// * the previous internal implementation had a property called `Visible` which we finalized to `IsNavigation` to better clarify it purpose.
/// * the previous internal implementation had a property called `Clickable` which we finalized to `IsClickable` to better clarify it purpose.
/// </remarks>
[ModelCreation(Use = typeof(PageModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IPageModel : ICanWrapData
{
    /// <summary>
    /// The page ID.
    ///
    /// * In Dnn it's from `TabInfo.TabID`
    /// * In Oqtane it's `Page.PageId`
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The parent page ID.
    ///
    /// It's usually `0` if it's a top level page.
    ///
    /// * In Dnn it's from `TabInfo.ParentId`
    /// * in Oqtane it's from `Page.ParentId`
    /// </summary>
    int ParentId { get; }

    /// <summary>
    /// The page GUID.
    ///
    /// * In Dnn it's from `TabInfo.UniqueId`
    /// * In Oqtane it's `Guid.Empty` as Oqtane doesn't have page GUIDs
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// The page title.
    ///
    /// * In Dnn it's from `TabInfo.Title`
    /// * in Oqtane it's from `Page.Title`
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The page name.
    ///
    /// * In Dnn it's from `TabInfo.Name`
    /// * in Oqtane it's from `Page.Name`
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines if this item is clickable in the menu.
    ///
    /// * In Dnn it's from `!TabInfo.DisableLink`
    /// * in Oqtane it's from `Page.IsClickable`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    bool IsClickable { get; }

    /// <summary>
    /// Order of this item in a menu.
    /// It is 1 based, so the first item has Order 1.
    ///
    /// * In Dnn it's from `TabInfo.TabOrder`
    /// * in Oqtane it's from `Page.Order`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    int Order { get; }

    /// <summary>
    /// The page visibility - if it should be shown in the menu.
    ///
    /// * In Dnn it's from `TabInfo.IsVisible`
    /// * in Oqtane it's from `Page.IsNavigation`
    /// </summary>
    bool IsNavigation { get; }

    /// <summary>
    /// Info if the page has sub-pages. 
    ///
    /// * In Dnn it's from `TabInfo.HasChildren`
    /// * in Oqtane it's from `Page.HasChildren`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    bool HasChildren { get; }

    /// <summary>
    /// How deep this page is in the breadcrumb.
    /// The number is 1 based, so the top level is 1.
    ///
    /// * In Dnn it's from `TabInfo.Level` (+1)
    /// * in Oqtane it's from `Page.Level` (+1)
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    int Level { get; }

    /// <summary>
    /// WIP
    /// * In Dnn it's from `TabInfo.TabSettings["LinkNewWindow"]`and will be either `_blank` or `` (empty string)
    /// * In Oqtane it's _not implemented_ an will be an empty string
    /// </summary>
    /// <remarks>
    /// Added in v15.02
    /// </remarks>
    string LinkTarget { get; }

    /// <summary>
    /// The path with slashes to this page.
    /// 
    /// * In Dnn it's from `TabInfo.TabPath`
    /// * in Oqtane it's from `Page.Path`
    /// </summary>
    string Path { get; }

    /// <summary>
    /// The public url to this page (without any trailing slashes)
    ///
    /// * In Dnn it's from `TabInf.FullUrl` (last slash removed)
    /// * in Oqtane it's a combination of protocol, site-alias and path
    /// </summary>
    string Url { get; }

    /// <summary>
    /// The page creation date/time.
    ///
    /// * In Dnn it's from `TabInfo.CreatedOnDate`
    /// * in Oqtane it's from `Page.CreatedOn`
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// The page modification date/time.
    ///
    /// * In Dnn it's from `TabInfo.LastModifiedOnDate`
    /// * in Oqtane it's from `Page.ModifiedOn`
    /// </summary>
    DateTime Modified { get; }

    /// <summary>
    /// The page delete-status.
    /// Normally you will only see not-deleted pages, so it should usually be false.
    ///
    /// * In Dnn it's from `TabInfo.IsDeleted`
    /// * in Oqtane it's from `Page.IsDeleted`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    bool IsDeleted { get; }

    IEnumerable<IPageModel> Children { get; }
}