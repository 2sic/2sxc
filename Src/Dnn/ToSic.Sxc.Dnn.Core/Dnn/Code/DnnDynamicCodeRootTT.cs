using ToSic.Lib.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code
{
    [PrivateApi]
    public class DnnDynamicCodeRoot<TModel, TServiceKit> : DynamicCodeRoot<TModel, TServiceKit>, Sxc.Code.IDynamicCode, IDnnDynamicCode, IHasDynamicCodeRoot
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public DnnDynamicCodeRoot(Dependencies dependencies): base(dependencies, DnnConstants.LogName) { }

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn => _dnn.Get(GetService<IDnnContext>);
        private readonly GetOnce<IDnnContext> _dnn = new GetOnce<IDnnContext>();
    }
}