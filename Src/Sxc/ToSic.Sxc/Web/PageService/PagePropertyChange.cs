namespace ToSic.Sxc.Web.PageService
{
    public struct PagePropertyChange
    {
        public PageChangeModes ChangeMode { get; set; }
        
        internal PageProperties Property { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// This is part of the original property, which would be replaced.
        /// </summary>
        public string ReplacementIdentifier { get; set; }
    }
}
