
using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Razor.Internal;

// 2024-01-09 2dm seems unused
internal class RazorServiceUnknown(WarnUseOfUnknown<RazorServiceUnknown> _) : IRazorService
{
    public string Render(string partialName, object model) => "";

    public void ConnectToRoot(IDynamicCodeRoot codeRoot) { /* Do nothing */ }
}