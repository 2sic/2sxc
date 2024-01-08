using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Services;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ModuleService: ServiceBase, IModuleService
{
    public ModuleService() : base(SxcLogging.SxcLogName + ".ModSvc") { }

    public void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false)
    {
        if (tag is null) return;
        nameId ??= tag.ToString();
        if (noDuplicates && ExistingKeys.Contains(nameId)) return;
        ExistingKeys.Add(nameId);
        _moreTags.Add(tag);
    }

    public IReadOnlyCollection<IHtmlTag> MoreTags => _moreTags;
    private readonly List<IHtmlTag> _moreTags = new();

    private readonly HashSet<string> ExistingKeys = new();
}