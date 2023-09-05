using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.StartUp
{
    /// <summary>
    /// This is a dummy code which just ensures that this project
    /// access all the namespaces of the DLLs we need to build DNN
    /// </summary>
    internal class CompilerDependencies
    {
        private string ToSicSexyContentWebApi => nameof(DynamicApiController);

        private string ToSicSexyContentRazor => nameof(Sxc.Engines.DnnRazorEngine);
    }
}