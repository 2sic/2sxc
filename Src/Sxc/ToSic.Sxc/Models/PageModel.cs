using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Data Model as is returned by the <see cref="Pages"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn TabInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Tabs.TabInfo.html)
/// * [Oqtane Page](https://docs.oqtane.org/api/Oqtane.Models.Page.html)
/// 
/// </summary>
/// <remarks>
/// * Released v19.01
/// * the previous internal implementation had a property called `Visible` which we finalized to `IsNavigation` to better clarify it purpose.
/// * the previous internal implementation had a property called `Clickable` which we finalized to `IsClickable` to better clarify it purpose.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class PageModel: DataModel, IPageModel
{
    /// <inheritdoc />
    public int Id => _entity.EntityId;
    /// <inheritdoc />
    public int ParentId => _entity.Get<int>(nameof(ParentId));
    /// <inheritdoc />
    public Guid Guid => _entity.EntityGuid;
    /// <inheritdoc />
    public string Title => _entity.Get<string>(nameof(Title));
    /// <inheritdoc />
    public string Name => _entity.Get<string>(nameof(Name));
    /// <inheritdoc />
    public bool IsClickable => _entity.Get<bool>(nameof(IsClickable));
    /// <inheritdoc />
    public int Order => _entity.Get<int>(nameof(Order));
    /// <inheritdoc />
    public bool IsNavigation => _entity.Get<bool>(nameof(IsNavigation));
    /// <inheritdoc />
    public bool HasChildren => _entity.Get<bool>(nameof(HasChildren));
    /// <inheritdoc />
    public int Level => _entity.Get<int>(nameof(Level));
    /// <inheritdoc />
    public string LinkTarget => _entity.Get<string>(nameof(LinkTarget));
    /// <inheritdoc />
    public string Path => _entity.Get<string>(nameof(Path));
    /// <inheritdoc />
    public string Url => _entity.Get<string>(nameof(Url));
    /// <inheritdoc />
    public DateTime Created => _entity.Created;
    /// <inheritdoc />
    public DateTime Modified => _entity.Modified;
    /// <inheritdoc />
    public bool IsDeleted => _entity.Get<bool>(nameof(IsDeleted));

    public IEnumerable<PageModel> Children => AsList<PageModel>(_entity.Children(field: nameof(Children)));
}