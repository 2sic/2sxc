using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface IOqtAssetsAndHeader
    {
        /// <summary>
        /// Determines if the context header is needed
        /// </summary>
        bool AddContextMeta { get; }

        string ContextMetaName { get; }
        /// <summary>
        /// Will return the meta-header which the $2sxc client needs for context, page id, request verification token etc.
        /// </summary>
        /// <returns></returns>
        string ContextMetaContents();

        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Scripts();

        /// <summary>
        /// The styles to add
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Styles();
    }
}
