using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    internal class CSharpAssemblyHandling : AppDomainHandling
    {
        internal string GetLanguageVersions()
        {
            CustomAppDomain = CreateNewAppDomain("CSharpAssemblyAppDomain");
            var proxyType = typeof(CSharpAssemblyLoader);
            var loader = (CSharpAssemblyLoader)CustomAppDomain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);
            var result = loader.GetLanguageVersions();

            Unload();

            return result;
        }
    }
}
