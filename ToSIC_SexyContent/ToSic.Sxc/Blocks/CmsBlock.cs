using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    [PrivateApi("not sure yet what to call this, CmsBlock isn't right, because it's more of a BlockHost or something")]
    public partial class CmsBlock : HasLog, ICmsBlock
    {
        #region App-level information

        public IApp App => Block.App;

        #endregion

        /// <summary>
        /// The url-parameters (or alternative thereof) to use when picking views or anything
        /// Note that it's not the same type as the request.querystring to ease migration to future coding conventions
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; }

        #region Info for current runtime instance

        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public IAppEnvironment Environment { get; }

        public IEnvironmentFactory EnvFac { get; }

        public IContainer Container { get; }

        public IBlock Block { get; }

        #endregion

        #region Constructor
        internal CmsBlock(IBlock cb, 
            IContainer container, 
            IEnumerable<KeyValuePair<string, string>> urlParams = null, 
            ILog parentLog = null)
            : base("Sxc.CmsIns", parentLog, $"get CmsInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}")
        {
            Log.Add("constructor");
            EnvFac = Factory.Resolve<IEnvironmentFactory>();
            Environment = EnvFac.Environment(parentLog);
            Block = cb;
            Container = container;

            // keep url parameters, because we may need them later for view-switching and more
            Parameters = urlParams;
        }

        #endregion

    }
}