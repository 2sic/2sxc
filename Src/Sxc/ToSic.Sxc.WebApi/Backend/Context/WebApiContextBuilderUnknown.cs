using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Backend.Context;

internal class WebApiContextBuilderUnknown(WarnUseOfUnknown<WebApiContextBuilderUnknown> _) : IWebApiContextBuilder
{
    public ISxcContextResolver PrepareContextResolverForApiRequest()
    {
        throw new NotImplementedException();
    }
}