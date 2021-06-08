namespace ToSic.Sxc.Oqt.Shared.Models
{
    /// <summary>
    /// Oqtane headers to set. 
    /// Note that this mimics most of another class in Sxc Core.
    /// But since that should not be in the client, it cannot be used to transfer the state of header changes. 
    /// </summary>
    /// <remarks>WIP 12.02 - not done yet</remarks>
    public class OqtHeaders
    {
        public OqtHeaders(string key, string keyProperty, string tag)
        {
            Key = key;
            KeyProperty = keyProperty;
            Tag = tag;
        }
        public string Key { get; }
        public string KeyProperty { get; }
        public string Tag { get; }
    }
}
