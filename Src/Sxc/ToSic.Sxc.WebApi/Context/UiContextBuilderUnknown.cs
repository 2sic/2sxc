using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.WebApi.Context
{
    public sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
    {
        public UiContextBuilderUnknown(MyServices services, WarnUseOfUnknown<UiContextBuilderBase> _) : base(services)
        {
        }
    }
}
