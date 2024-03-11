using ToSic.Lib.Services;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ModuleService() : ServiceBase(SxcLogName + ".ModSvc"), IModuleService
{
    public void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false)
    {
        if (tag is null) return;
        nameId ??= tag.ToString();
        if (noDuplicates && ExistingKeys.Contains(nameId)) return;
        ExistingKeys.Add(nameId);
        _moreTags.Add(tag);
    }

    public IReadOnlyCollection<IHtmlTag> MoreTags => _moreTags;
    private readonly List<IHtmlTag> _moreTags = [];

    private readonly HashSet<string> ExistingKeys = [];
}