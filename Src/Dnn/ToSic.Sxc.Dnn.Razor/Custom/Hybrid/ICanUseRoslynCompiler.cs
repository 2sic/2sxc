using ToSic.Sxc.Dnn.Razor;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

internal interface ICanUseRoslynCompiler
{
    void AttachRazorEngine(DnnRazorEngine razorEngine);

    HelperResult RoslynRenderPage(string virtualPath, object data);
}