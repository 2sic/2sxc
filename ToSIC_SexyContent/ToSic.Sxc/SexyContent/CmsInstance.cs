using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    public partial class CmsInstance : HasLog, ICmsBlock
    {
        #region App-level information

        public int/*?*/ ZoneId => Block.ZoneId;
        public int/*?*/ AppId => Block.AppId;
        public IApp App => Block.App;
        //public bool IsContentApp => Block.IsContentApp;

        #endregion

        /// <summary>
        /// The url-parameters (or alternative thereof) to use when picking views or anything
        /// Note that it's not the same type as the request.querystring to ease migration to future coding conventions
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; private set; }

        #region Info for current runtime instance
        //public BlockConfiguration BlockConfiguration => Block.BlockConfiguration;


        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public IAppEnvironment Environment { get; }

        public IEnvironmentFactory EnvFac { get; }

        public IInstanceInfo EnvInstance { get; }

        public IBlock Block { get; }


        // 2019-11-09 2dm shrinking api
        ///// <summary>
        ///// This returns the Tenant/Portal of the original module. When a module is mirrored across portals,
        ///// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        ///// </summary>
        //internal ITenant Tenant => Block.Tenant;

        //public IBlockDataSource Data => Block.Data;


        #endregion

        #region Constructor
        internal CmsInstance(IBlock  cb, 
            IInstanceInfo envInstance, 
            IEnumerable<KeyValuePair<string, string>> urlparams = null, 
            ILog parentLog = null)
            : base("Sxc.Instnc", parentLog, $"get SxcInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}")
        {
            // Log = new Log();
            EnvFac = Factory.Resolve<IEnvironmentFactory>();
            Environment = EnvFac.Environment(parentLog);
            Block = cb;
            EnvInstance = envInstance;

            // keep url parameters, because we may need them later for view-switching and more
            Parameters = urlparams;
        }

        #endregion

    }
}