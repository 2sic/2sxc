using ToSic.Eav.Documentation;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14: Razor14<dynamic, ServiceKit14>
    {

    }
}
