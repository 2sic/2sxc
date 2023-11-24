using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Infrastructure
{

    /// <summary>
    /// Helper object which will determine the current context based on headers, url-parameters etc.
    /// Will be slightly different depending on the platform.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IWebApiContextBuilder
    {
        IContextResolver PrepareContextResolverForApiRequest();
    }
}