using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code
{
    /// <summary>
    /// The basic DnnDynamicCode without explicitly typed model / kit
    /// </summary>
    [PrivateApi]
    public class DnnDynamicCodeRoot : DnnDynamicCodeRoot<object, ServiceKit>
    {
        public DnnDynamicCodeRoot(Dependencies dependencies): base(dependencies) { }
    }
}