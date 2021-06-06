namespace ToSic.Sxc.Oqt.Shared.Models
{
    /// <summary>
    /// Used to transfer what / how page properties should change based on the Razor file
    /// </summary>
    public class OqtPagePropertyChanges
    {
        public OqtPageProperties Property { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }

        public OqtPagePropertyOperation Change { get; set; }
    }

    public enum OqtPageProperties
    {
        Title,
        Keywords,
        Description,
        Base
    }

    public enum OqtPagePropertyOperation
    {
        Replace,
        ReplaceOrSkip,
        Prefix,
        Suffix
    }
}
