using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Probably always internal, as there is probably no reason to make it public")]
    public interface IModuleService: IHasLog
    {
        IReadOnlyCollection<IHtmlTag> MoreTags { get; }

        void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false);
    }
}
