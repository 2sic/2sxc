using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Run.Context
{
    public interface IContextOfBlock: IContextOfSite
    {
        /// <summary>
        /// The page it's running on + parameters for queries, url etc.
        /// </summary>
        IPageInternal Page { get; }

        /// <summary>
        /// The container for our block, basically the module
        /// </summary>
        IModuleInternal Container { get; }

        /// <summary>
        /// Publishing information about the current context
        /// </summary>
        BlockPublishingState Publishing { get; }
    }
}
