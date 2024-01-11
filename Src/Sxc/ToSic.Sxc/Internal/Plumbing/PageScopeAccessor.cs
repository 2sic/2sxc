namespace ToSic.Sxc.Internal.Plumbing;

/// <summary>
/// Special helper to get a ServiceProvider of the page scope, in scenarios where each module has an own scope. 
/// </summary>
/// <remarks>
/// Default constructor will always work, and use the current service provider as the source
/// </remarks>
/// <param name="serviceProvider"></param>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageScopeAccessor(IServiceProvider serviceProvider)
{
    public void InitPageOfModule(IServiceProvider pageServiceProvider)
    {
        ServiceProvider = pageServiceProvider;
        ProvidedInModule = true;
    }

    /// <summary>
    /// The page level ServiceProvider
    /// </summary>
    internal IServiceProvider ServiceProvider { get; private set; } = serviceProvider;


    /// <summary>
    /// Determines if this page-scope accessor is from the PageDI or from the Module
    /// More for internal use, in case we have trouble debugging
    /// </summary>
    public bool ProvidedInModule { get; private set; } = false;
}