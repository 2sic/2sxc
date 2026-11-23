// ReSharper disable once CheckNamespace
namespace ToSic.Sys.DI;

/// <summary>
/// Special helper to get a ServiceProvider of the page scope, in scenarios where each module has an own scope. 
/// </summary>
/// <remarks>
/// Default constructor will always work, and use the current service provider as the source
/// </remarks>
/// <param name="serviceProvider"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PageScopeAccessor(ISysServiceProvider serviceProvider)
{
    public void InitPageOfModule(ISysServiceProvider pageServiceProvider)
    {
        ServiceProvider = pageServiceProvider;
        ProvidedInModule = true;
    }

    /// <summary>
    /// The page level ServiceProvider
    /// </summary>
    internal ISysServiceProvider ServiceProvider { get; private set; } = serviceProvider;


    /// <summary>
    /// Determines if this page-scope accessor is from the PageDI or from the Module
    /// More for internal use, in case we have trouble debugging
    /// </summary>
    public bool ProvidedInModule { get; private set; } = false;
}