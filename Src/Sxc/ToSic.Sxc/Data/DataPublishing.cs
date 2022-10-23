using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is for data sources to determine that they can be published as JSON stream from the module.
    /// This was a system we used before queries.
    /// </summary>
    [PrivateApi("older use case, probably obsolete some day")]
    public class DataPublishing
    {
        public bool Enabled { get; set; }
        public string Streams { get; set; }

        public DataPublishing()
        {
            Enabled = false;
            Streams = "";
        }

    }

    

}