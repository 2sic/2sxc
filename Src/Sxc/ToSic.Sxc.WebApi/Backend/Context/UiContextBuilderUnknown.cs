using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Backend.Context;

internal sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
{
    public UiContextBuilderUnknown(MyServices services, WarnUseOfUnknown<UiContextBuilderBase> _) : base(services)
    {
    }
}