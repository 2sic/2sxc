using ToSic.Sxc.Dnn;
using IDynamicCode = ToSic.Sxc.Web.IDynamicCode;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    // ReSharper disable once UnusedMember.Global
    public abstract class AppDataDnnHelpers : WithContext, Dnn.IDynamicCode
    {
        public IDnnContext Dnn { get; private set; }

        internal override void InitShared(IDynamicCode parent)
        {
            if (parent is Dnn.IDynamicCode withDnn) Dnn = withDnn.Dnn;

            base.InitShared(parent);
        }
    }
}
