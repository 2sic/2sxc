using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Code
{
    [PrivateApi]
    public class DnnDynamicCodeRoot : DynamicCodeRoot, Sxc.Code.IDynamicCode, IDnnDynamicCode, IHasDynamicCodeRoot
    {
        public DnnDynamicCodeRoot(Dependencies dependencies): base(dependencies, DnnConstants.LogName) { }

        // 2022-05-10 2dm - disabled this, must be a leftover as it doesn't do anything
        ///// <summary>
        ///// Standard constructor
        ///// </summary>
        ///// <param name="block">CMS Block which provides context and maybe some edit-allowed info.</param>
        ///// <param name="parentLog">parent logger for logging what's happening</param>
        ///// <param name="compatibility">compatibility level - changes behaviour if level 9 or 10</param>
        //public override IDynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility)
        //{
        //    base.Init(block, parentLog, compatibility);
        //    return this;
        //}

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn => _dnn ?? (_dnn = GetService<IDnnContext>());
        private IDnnContext _dnn;
    }
}