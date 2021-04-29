using Oqtane.Models;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class SxcResource: Resource
    {
        ///// <summary>
        ///// Can be "body" or "head"
        ///// </summary>
        //public string Location = "body";

        /// <summary>
        /// For the contents of a script tag
        /// </summary>
        public string Content;

        public bool IsExternal = true;
    }
}
