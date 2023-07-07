using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public abstract class DynamicCodeBase : ServiceForDynamicCode, ICompatibilityLevel
    {
        #region Constructor / Setup

        /// <summary>
        /// Main constructor, NOT for DI may never have parameters, otherwise inheriting code will run into problems. 
        /// </summary>
        protected DynamicCodeBase(string logName) : base(logName) { }

        /// <summary>
        /// Special helper to move all Razor logic into a separate class.
        /// For architecture of Composition over Inheritance.
        /// </summary>
        [PrivateApi]
        protected internal CodeHelper SysHlp => _sysHlp ?? (_sysHlp = new CodeHelper().Init(this));
        private CodeHelper _sysHlp;


        [PrivateApi]
        public override void ConnectToRoot(IDynamicCodeRoot codeRoot) => base.Log.Do(() =>
        {
            base.ConnectToRoot(codeRoot);
            SysHlp.ConnectToRoot(codeRoot);
        });

        [PrivateApi] public abstract int CompatibilityLevel { get; } // Constants.CompatibilityLevel9Old;

        #endregion
    }
}
