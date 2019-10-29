using ToSic.Sxc.Dnn;
using IDynamicCode = ToSic.Sxc.Dnn.IDynamicCode;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    // ReSharper disable once UnusedMember.Global
    public abstract class AppDataDnnHelpers : WithContext, IDynamicCode
    {
        public IDnnContext Dnn { get; private set; }

        internal override void InitShared(Interfaces.IDynamicCode parent)
        {
            if (parent is IDynamicCode withDnn) Dnn = withDnn.Dnn;

            base.InitShared(parent);
        }
    }
}
