using ToSic.Sxc.Data.Model;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Data Model as is returned by the <see cref="Pages"/> DataSource.
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
[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, IPageModel, PageModelOfEntity>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IPageModel : IDataModel
{
    /// <inheritdoc cref="IPageModelSync.Id" />
    int Id { get; }

    /// <inheritdoc cref="IPageModelSync.ParentId" />
    int ParentId { get; }

    /// <inheritdoc cref="IPageModelSync.Guid" />
    Guid Guid { get; }

    /// <inheritdoc cref="IPageModelSync.Title" />
    string Title { get; }

    /// <inheritdoc cref="IPageModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="IPageModelSync.IsClickable" />
    bool IsClickable { get; }

    /// <inheritdoc cref="IPageModelSync.Order" />
    int Order { get; }

    /// <inheritdoc cref="IPageModelSync.IsNavigation" />
    bool IsNavigation { get; }

    /// <inheritdoc cref="IPageModelSync.HasChildren" />
    bool HasChildren { get; }

    /// <inheritdoc cref="IPageModelSync.Level" />
    int Level { get; }

    /// <inheritdoc cref="IPageModelSync.LinkTarget" />
    string LinkTarget { get; }

    /// <inheritdoc cref="IPageModelSync.Path" />
    string Path { get; }

    /// <inheritdoc cref="IPageModelSync.Url" />
    string Url { get; }

    /// <inheritdoc cref="IPageModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IPageModelSync.Modified" />
    DateTime Modified { get; }

    /// <inheritdoc cref="IPageModelSync.IsDeleted" />
    bool IsDeleted { get; }

    IEnumerable<IPageModel> Children { get; }
}