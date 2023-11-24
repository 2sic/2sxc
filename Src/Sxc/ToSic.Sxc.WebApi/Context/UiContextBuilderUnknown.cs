using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.WebApi.Context
{
    internal sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
    {
        public UiContextBuilderUnknown(MyServices services, WarnUseOfUnknown<UiContextBuilderBase> _) : base(services)
        {
        }
    }
}
