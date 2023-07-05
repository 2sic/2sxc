using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Oqt.Server.Custom;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase<TRealController> : OqtControllerBase<TRealController> where TRealController : class, IHasLog
    {
        #region Setup

        protected OqtStatefulControllerBase(string logSuffix) : base(logSuffix)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            SysHlp = new(this, Helper, HistoryLogGroup);
        }

        /// <summary>
        /// Special helper to move all Razor logic into a separate class.
        /// For architecture of Composition over Inheritance.
        /// </summary>
        [PrivateApi]
        internal OqtWebApiHelper SysHlp { get; }

        #endregion


        public override void OnActionExecuting(ActionExecutingContext context) => Log.Do(() =>
        {
            base.OnActionExecuting(context);
            SysHlp.InitializeBlockContext(context);
        });
    }
}
