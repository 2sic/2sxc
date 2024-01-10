using System;
using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Backend.Context;

internal class WebApiContextBuilderUnknown(WarnUseOfUnknown<WebApiContextBuilderUnknown> _) : IWebApiContextBuilder
{
    public ISxcContextResolver PrepareContextResolverForApiRequest()
    {
        throw new NotImplementedException();
    }
}