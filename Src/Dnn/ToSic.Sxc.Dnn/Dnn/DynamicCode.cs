using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;

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
        public IDnnContext Dnn => DynCode?.Dnn;

        [PrivateApi] public DnnDynamicCode DynCode => (UnwrappedContents as IHasDynCodeContext)?.DynCode;

        public new dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrapLog = Log.Call<dynamic>($"{virtualPath}, ..., {name}, {relativePath}");
            // usually we don't have a relative path, so we use the preset path from when this class was instantiated
            relativePath = relativePath ?? CreateInstancePath;
            var result = DynCode?.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, relativePath, throwOnError);
            if (result != null) return wrapLog("ok", result);

            Log.Add("DynCode doesn't exist or returned null, will use standard CreateInstance");
            result = base.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, relativePath, throwOnError);

            return wrapLog((result != null).ToString(), result);
        }
    }
}
