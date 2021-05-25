using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Adam
{
    public class AdamPathsBase : HasLog, IAdamPaths
    {
        #region DI Constructor & Init

        public AdamPathsBase(IServerPaths serverPaths) : this(serverPaths, LogNames.Basic)
        {

        }

        protected AdamPathsBase(IServerPaths serverPaths, string logPrefix) : base($"{logPrefix}.AdmPth")
        {
            _serverPaths = serverPaths;
        }
        private readonly IServerPaths _serverPaths;

        public IAdamPaths Init(AdamManager adamManager, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AdamManager = adamManager;
            return this;
        }
        protected AdamManager AdamManager { get; private set; }

        #endregion



        public string PhysicalPath(string path)
        {
            ThrowIfPathContainsDotDot(path);

            // check if it's already a physical path
            if (Path.IsPathRooted(path)) return path;

            // check if it already has the root path attached, otherwise add
            path = path.StartsWith(AdamManager.Site.ContentPath) ? path : Path.Combine(AdamManager.Site.ContentPath, path);
            return _serverPaths.FullContentPath(path.Backslash());
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        public static void ThrowIfPathContainsDotDot(string path)
        {
            if (path.Contains("..")) throw new ArgumentException("path may not contain ..", nameof(path));
        }

        public string RelativeFromAdam(string path)
        {
            var adamPosition = path.ForwardSlash().IndexOf("adam/", StringComparison.InvariantCultureIgnoreCase);
            return adamPosition <= 0
                ? path
                : path.Substring(adamPosition);
        }

        public virtual string Url(string path) => Path.Combine(AdamManager.Site.ContentPath, path).ForwardSlash();
    }
}
