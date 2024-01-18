
using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Razor.Internal;

// 2024-01-09 2dm seems unused
internal class RazorServiceUnknown(WarnUseOfUnknown<RazorServiceUnknown> _) : IRazorService
{
    public string Render(string partialName, object model) => "";

    public void ConnectToRoot(ICodeApiService codeRoot) { /* Do nothing */ }
}