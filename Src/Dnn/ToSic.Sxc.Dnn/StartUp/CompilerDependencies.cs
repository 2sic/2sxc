using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.StartUp;

/// <summary>
/// This is a dummy code which just ensures that this project
/// access all the namespaces of the DLLs we need to build DNN
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CompilerDependencies
{
    private string ToSicSexyContentWebApi => nameof(DynamicApiController);

    private string ToSicSexyContentRazor => nameof(DnnRazorEngine);
}