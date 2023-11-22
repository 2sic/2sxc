using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    internal class RazorEngineAssemblyHandling : AppDomainHandling
    {
        internal string GetLanguageVersions()
        {
            CustomAppDomain = CreateNewAppDomain("RazorEngineAssemblyAppDomain");
            var proxyType = typeof(RazorEngineAssemblyLoader);
            var loader = (RazorEngineAssemblyLoader)CustomAppDomain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);
            var result = loader.GetLanguageVersions();

            Unload();

            return result;
        }
    }
}
