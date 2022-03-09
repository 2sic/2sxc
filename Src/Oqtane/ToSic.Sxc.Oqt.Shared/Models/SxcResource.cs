using System.Collections.Generic;
using Oqtane.Models;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class SxcResource: Resource
    {
        public string UniqueId { get; set; }

        /// <summary>
        /// For the contents of a script tag
        /// </summary>
        public string Content { get; set; }

        public bool IsExternal { get; set; }= true;

        /// <summary>
        /// Used to store all other html attributes from html tag.
        /// </summary>
        public IDictionary<string, string> HtmlAttributes { get; set; }
    }
}
