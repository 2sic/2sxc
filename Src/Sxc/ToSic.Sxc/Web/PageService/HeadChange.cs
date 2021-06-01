using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.PageService
{
    public struct HeadChange
    {
        public PageChangeModes ChangeMode;

        public TagBase Tag;

        /// <summary>
        /// This is part of the original property, which would be replaced.
        /// </summary>
        public string ReplacementIdentifier;
    }
}
