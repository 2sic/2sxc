#if NETFRAMEWORK

// This is included for compatibility
// It was changed in 2sxc 10.20. 01, but some code in the wild probably uses this for comparison.
// old docs like https://github.com/2sic/2sxc/wiki/razor-sexycontentwebpage.instancepurpose used this namespaces

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Engines
{
    [Obsolete]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public enum InstancePurposes
    {
        WebView = 0,
        IndexingForSearch = 1,
    }
}
#endif