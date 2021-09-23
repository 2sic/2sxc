using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.WebApi.Context
{
    public sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
    {
        public UiContextBuilderUnknown(Dependencies dependencies, WarnUseOfUnknown<UiContextBuilderBase> warn) : base(dependencies)
        {
        }
    }
}
