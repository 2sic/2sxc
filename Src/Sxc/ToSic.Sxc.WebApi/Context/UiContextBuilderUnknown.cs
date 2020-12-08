using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.Context
{
    public sealed class UiContextBuilderUnknown: UiContextBuilderBase, IIsUnknown
    {
        public UiContextBuilderUnknown(Dependencies dependencies) : base(dependencies)
        {
        }
    }
}
