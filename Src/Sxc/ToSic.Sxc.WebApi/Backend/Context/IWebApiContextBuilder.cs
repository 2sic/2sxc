namespace ToSic.Sxc.Backend.Context;

/// <summary>
/// Helper object which will determine the current context based on headers, url-parameters etc.
/// Will be slightly different depending on the platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IWebApiContextBuilder
{
    ISxcContextResolver PrepareContextResolverForApiRequest();
}