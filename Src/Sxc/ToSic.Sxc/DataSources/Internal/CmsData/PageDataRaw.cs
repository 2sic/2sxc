using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the page,
/// until it's converted to an IEntity in the <see cref="Pages"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn TabInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Tabs.TabInfo.html)
/// * [Oqtane Page](https://docs.oqtane.org/api/Oqtane.Models.Page.html)
/// 
/// Important: this is an internal object.
/// We're just including in in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageDataRaw: IRawEntity
{
    public static DataFactoryOptions Option = new(typeName: "Page", titleField: nameof(Name));

    /// <summary>
    /// The page ID.
    ///
    /// * In Dnn it's from `TabInfo.TabID`
    /// * In Oqtane it's `Page.PageId`
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The parent page ID.
    ///
    /// It's usually `0` if it's a top level page.
    ///
    /// * In Dnn it's from `TabInfo.ParentId`
    /// * in Oqtane it's from `Page.ParentId`
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// The page GUID.
    ///
    /// * In Dnn it's from `TabInfo.UniqueId`
    /// * In Oqtane it's `Guid.Empty` as Oqtane doesn't have page GUIDs
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The page title.
    ///
    /// * In Dnn it's from `TabInfo.Title`
    /// * in Oqtane it's from `Page.Title`
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The page name.
    ///
    /// * In Dnn it's from `TabInfo.Name`
    /// * in Oqtane it's from `Page.Name`
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Determines if this item is clickable in the menu.
    ///
    /// * In Dnn it's from `!TabInfo.DisableLink`
    /// * in Oqtane it's from `Page.IsClickable`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    public bool Clickable { get; set; }


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
    public int Order { get; set; }

    /// <summary>
    /// The page visibility - if it should be shown in the menu.
    ///
    /// * In Dnn it's from `TabInfo.IsVisible`
    /// * in Oqtane it's from `Page.IsNavigation`
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Info if the page has sub-pages. 
    ///
    /// * In Dnn it's from `TabInfo.HasChildren`
    /// * in Oqtane it's from `Page.HasChildren`
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    public bool HasChildren { get; set; }

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
    public int Level { get; set; }

    /// <summary>
    /// WIP
    /// * In Dnn it's from `TabInfo.TabSettings["LinkNewWindow"]`and will be either `_blank` or `` (empty string)
    /// * In Oqtane it's _not implemented_ an will be an empty string
    /// </summary>
    /// <remarks>
    /// Added in v15.02
    /// </remarks>
    public string LinkTarget { get; set; }


    /// <summary>
    /// The path with slashes to this page.
    /// 
    /// * In Dnn it's from `TabInfo.TabPath`
    /// * in Oqtane it's from `Page.Path`
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// The public url to this page (without any trailing slashes)
    ///
    /// * In Dnn it's from `TabInf.FullUrl` (last slash removed)
    /// * in Oqtane it's a combination of protocol, site-alias and path
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The page creation date/time.
    ///
    /// * In Dnn it's from `TabInfo.CreatedOnDate`
    /// * in Oqtane it's from `Page.CreatedOn`
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// The page modification date/time.
    ///
    /// * In Dnn it's from `TabInfo.LastModifiedOnDate`
    /// * in Oqtane it's from `Page.ModifiedOn`
    /// </summary>
    public DateTime Modified { get; set; }

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
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        // v14+
        { Eav.Data.Attributes.TitleNiceName, Title },
        { nameof(Name), Name },
        { nameof(ParentId), ParentId },
        { nameof(Visible), Visible },
        { nameof(Path), Path },
        { nameof(Url), Url },
        // New in v15.01
        { nameof(Clickable), Clickable },
        { nameof(Order), Order },
        { nameof(IsDeleted), IsDeleted },
        { nameof(Level), Level },
        { nameof(HasChildren), HasChildren },
        // New in v15.02
        { nameof(LinkTarget), LinkTarget }
    };
}