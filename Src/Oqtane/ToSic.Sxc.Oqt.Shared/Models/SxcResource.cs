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
    }
}
