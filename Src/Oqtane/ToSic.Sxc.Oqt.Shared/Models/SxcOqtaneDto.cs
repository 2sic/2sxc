using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class SxcOqtaneDto
    {
        public string GeneratedHtml { get; set; }

        //public IOqtAssetsAndHeader AssetsAndHeaders { get; set; }

        public List<SxcResource> Resources { get; set; }

        /// <summary>
        /// Determines if the context header is needed
        /// </summary>
        public bool AddContextMeta { get; set; }

        public string ContextMetaName { get; set; }
        /// <summary>
        /// Will return the meta-header which the $2sxc client needs for context, page id, request verification token etc.
        /// </summary>
        /// <returns></returns>
        public string ContextMetaContents { get; set; }

        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public List<string> Scripts { get; set; }

        /// <summary>
        /// The styles to add
        /// </summary>
        /// <returns></returns>
        public List<string> Styles { get; set; }
    }
}
