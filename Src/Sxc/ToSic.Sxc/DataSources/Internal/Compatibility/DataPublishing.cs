#if NETFRAMEWORK
namespace ToSic.Sxc.DataSources.Internal.Compatibility;

/// <summary>
/// This is for data sources to determine that they can be published as JSON stream from the module.
/// This was a system we used before queries.
/// </summary>
[PrivateApi("older use case, probably obsolete some day")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DataPublishing
{
    public bool Enabled { get; set; } = false;
    public string Streams { get; set; } = "";
}

#endif