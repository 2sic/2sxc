using System;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compiler;
using ToSic.Sxc.Dnn.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnAppAndDataHelpers : AppAndDataHelpersBase, ISharedCodeBuilder, IHasDnnContext
    {
        public DnnAppAndDataHelpers(SxcInstance sxcInstance) : this(sxcInstance, null) {}

        public DnnAppAndDataHelpers(SxcInstance sxcInstance, Log parentLog): base(sxcInstance, new DnnTenant(null), parentLog)
        {
            // Init things than require module-info or similar, but not 2sxc
            var instance = sxcInstance?.EnvInstance;
            Dnn = new DnnHelper(instance);
            Link = new DnnLinkHelper(Dnn);
        }

        #region IHasDnnContext
        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public DnnHelper Dnn { get; }
        #endregion


    }
}