#if NETFRAMEWORK
using ToSic.Eav.Apps.Services;

namespace ToSic.Sxc.DataSources.Internal.Compatibility;

[Obsolete("this is just a workaround so that old code still works - especially Mobius forms which used this in V3")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CacheWithGetContentType
{
    private readonly IAppContentTypeService _app;

    internal CacheWithGetContentType(IAppContentTypeService app)
    {
        _app = app;
    }

    public IContentType GetContentType(string typeName) => _app.GetContentType(typeName);
}

#endif