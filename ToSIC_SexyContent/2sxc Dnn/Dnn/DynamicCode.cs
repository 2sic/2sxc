using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Code;
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
    public abstract class DynamicCode : Sxc.Code.DynamicCode, IDnnDynamicCode, IHasDynCodeContext
    {
        /// <inheritdoc />
        public IDnnContext Dnn => DynCode.Dnn;

        [PrivateApi] public DnnDynamicCode DynCode => (UnwrappedContents as IHasDynCodeContext)?.DynCode; // { get; set; }

        //internal override void InitShared(IDynamicCode parent, string path)
        //{
        //    if (parent is IHasDynCodeContext withDnn) DynCode = withDnn.DynCode;
        //    base.InitShared(parent, path);
        //}

        public new dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            // try to create a DNN instance, but if that's not possible, use base
            DynCode?.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, relativePath, throwOnError)
            ?? base.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, relativePath, throwOnError);
    }
}
