using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;

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
    public partial class SxcInstance :ISxcInstance
    {
        #region App-level information

        internal int? ZoneId => ContentBlock.ZoneId;
        internal int? AppId => ContentBlock.AppId;
        public App App => ContentBlock.App;
        public bool IsContentApp => ContentBlock.IsContentApp;

        #endregion

        public Log Log { get; }

        /// <summary>
        /// The url-parameters (or alternative thereof) to use when picking views or anything
        /// Note that it's not the same type as the request.querystring to ease migration to future coding conventions
        /// </summary>
        internal IEnumerable<KeyValuePair<string, string>> Parameters;

        #region Info for current runtime instance
        public ContentGroup ContentGroup => ContentBlock.ContentGroup;


        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public IAppEnvironment Environment { get; }

        public IEnvironmentFactory EnvFac { get; }

        public IInstanceInfo EnvInstance { get; }

        internal IContentBlock ContentBlock { get; }


        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal ITenant Tenant => ContentBlock.Tenant;

        public ViewDataSource Data => ContentBlock.Data;


        #endregion

        #region Constructor
        internal SxcInstance(IContentBlock  cb, 
            IInstanceInfo envInstance, 
            IEnumerable<KeyValuePair<string, string>> urlparams = null, 
            Log parentLog = null)
        {
            Log = new Log("Sxc.Instnc", parentLog, $"get SxcInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}");
            EnvFac = Factory.Resolve<IEnvironmentFactory>();
            Environment = EnvFac.Environment(parentLog);
            ContentBlock = cb;
            EnvInstance = envInstance;

            // keep url parameters, because we may need them later for view-switching and more
            Parameters = urlparams;
        }

        #endregion

    }
}