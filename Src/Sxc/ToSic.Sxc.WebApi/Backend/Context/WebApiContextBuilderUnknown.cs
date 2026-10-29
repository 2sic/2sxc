#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Backend.Context;

internal class WebApiContextBuilderUnknown(WarnUseOfUnknown<WebApiContextBuilderUnknown> _) : IWebApiContextBuilder
{
    public IContextCompleteService PrepareContextResolverForApiRequest()
        => throw new NotImplementedException();
}