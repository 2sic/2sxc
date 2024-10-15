using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

// Code is required to razor runtime compilation.
// Required for functioning of ToSic.Sxc.Razor.RazorEngine.RenderTemplate() 
[assembly: ProvideApplicationPartFactory(typeof(ToSic.Sxc.Oqt.Server.Plumbing.MyApplicationPartFactory))]
namespace ToSic.Sxc.Oqt.Server.Plumbing;

/// <summary>
/// We're not 100% sure of this purpose again, but we believe the Razor compiler tries to go through application
/// parts and fails if this isn't included
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class MyApplicationPartFactory : ApplicationPartFactory
{
    public override IEnumerable<ApplicationPart> GetApplicationParts(Assembly assembly)
    {
        yield return new CompilationReferencesProvider(assembly);
    }
}