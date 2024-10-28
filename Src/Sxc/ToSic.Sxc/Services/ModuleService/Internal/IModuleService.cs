using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("Probably always internal, as there is probably no reason to make it public")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModuleService: IHasLog
{
    /// <summary>
    /// Tags added by code, errors, TurnOn etc. which are added to the end of the module.
    /// </summary>
    IReadOnlyCollection<IHtmlTag> MoreTags { get; }

    /// <summary>
    /// Add a tag (like a TurnOn) to the end of the module
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="nameId"></param>
    /// <param name="noDuplicates"></param>
    void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false);
}