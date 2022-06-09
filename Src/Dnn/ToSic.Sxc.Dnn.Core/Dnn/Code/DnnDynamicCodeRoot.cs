using ToSic.Eav.Documentation;
using ToSic.Sxc.Services.Kits;

namespace ToSic.Sxc.Dnn.Code
{
    /// <summary>
    /// The basic DnnDynamicCode without explicitly typed model / kit
    /// </summary>
    [PrivateApi]
    public class DnnDynamicCodeRoot : DnnDynamicCodeRoot<object, KitNone>
    {
        public DnnDynamicCodeRoot(Dependencies dependencies): base(dependencies) { }
    }
}