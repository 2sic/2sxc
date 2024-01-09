using System;
using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.WebApi.Infrastructure;

internal class WebApiContextBuilderUnknown: IWebApiContextBuilder
{
    public WebApiContextBuilderUnknown(WarnUseOfUnknown<WebApiContextBuilderUnknown> _) { }

    public ISxcContextResolver PrepareContextResolverForApiRequest()
    {
        throw new NotImplementedException();
    }
}