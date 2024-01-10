using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("Probably always internal, as there is probably no reason to make it public")]
public interface IModuleService: IHasLog
{
    /// <summary>
    /// Tags added by code, errors, TurnOn etc. which are added to the end of the module.
    /// </summary>
    IReadOnlyCollection<IHtmlTag> MoreTags { get; }

    /// <summary>
    /// Add a tag (eg. a TurnOn) to the end of the module
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="nameId"></param>
    /// <param name="noDuplicates"></param>
    void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false);
}