using ToSic.SexyContent.Razor.Helpers;
using ToSic.Sxc.Dnn.Interfaces;
using ToSic.Sxc.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    // ReSharper disable once UnusedMember.Global
    public abstract class WithDnnContext: WithContext, IHasDnnContext
    {
        public DnnHelper Dnn { get; private set; }

        internal override void InitShared(IAppAndDataHelpers parent)
        {
            if (parent is IHasDnnContext withDnn) Dnn = withDnn.Dnn;

            base.InitShared(parent);
        }
    }
}
