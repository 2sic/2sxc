using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using IDynamicCode = ToSic.Sxc.Code.IDynamicCode;

namespace ToSic.Sxc.Dnn
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// This is a base class for custom code files with context. <br/>
    /// If you create a class file for dynamic use and inherit from this, then the compiler will automatically add objects like Link, Dnn, etc.
    /// The class then also has AsDynamic(...) and AsList(...) commands like a normal razor page.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class DynamicCode : Sxc.Code.DynamicCode, Code.IDnnDynamicCode
    {
        public IDnnContext Dnn { get; private set; }

        internal override void InitShared(IDynamicCode parent)
        {
            if (parent is Code.IDnnDynamicCode withDnn) Dnn = withDnn.Dnn;

            base.InitShared(parent);
        }
    }
}
