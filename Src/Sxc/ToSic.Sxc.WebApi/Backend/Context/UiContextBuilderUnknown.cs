namespace ToSic.Sxc.Backend.Context;

internal sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
{
    public UiContextBuilderUnknown(Dependencies services, WarnUseOfUnknown<UiContextBuilderBase> _) : base(services)
    {
    }
}