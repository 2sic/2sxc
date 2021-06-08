namespace ToSic.Sxc.Web.PageService
{
    public struct PagePropertyChange
    {
        public PageChangeModes ChangeMode;
        
        internal PageProperties Property;
        
        public string Value;

        /// <summary>
        /// This is part of the original property, which would be replaced.
        /// </summary>
        public string ReplacementIdentifier;
    }
}
