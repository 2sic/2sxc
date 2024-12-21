using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("Probably always internal, as there is probably no reason to make it public")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModuleService: IHasLog
{
    /// <summary>
    /// Tags added by code, errors, TurnOn etc. which are added to the end of the module.
    /// </summary>
#if NETCOREAPP
    IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(int moduleId);
#else
    IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(); // DNN implementation has missing moduleID on purpose. It is not needed, and it should not be provided by mistake.
#endif

    /// <summary>
    /// Add a tag (like a TurnOn) to the end of the module
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="moduleId"></param>
    /// <param name="nameId"></param>
    /// <param name="noDuplicates"></param>
#if NETCOREAPP
    void AddToMore(IHtmlTag tag, int moduleId, string nameId = null, bool noDuplicates = false);
#else
    void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false);  // DNN implementation has missing moduleID on purpose. It is not needed, and it should not be provided by mistake.
#endif
}