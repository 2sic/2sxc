#if NETFRAMEWORK
using System;
using ToSic.Eav.Apps.Services;
using ToSic.Eav.Data;

namespace ToSic.Sxc.DataSources.Internal;

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