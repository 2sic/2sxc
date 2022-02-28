using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.WebApi.Sys
{
    public class InstallControllerReal: HasLog<InstallControllerReal>
    {
        public const string LogSuffix = "Install";

        #region System Installation

        public InstallControllerReal(
            LazyInitLog<IContextOfSite> context,
            Lazy<IEnvironmentInstaller> envInstallerLazy, 
            Lazy<ImportFromRemote> impFromRemoteLazy, 
            Lazy<IUser> userLazy
            ): base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            _context = context.SetLog(Log);
            _envInstallerLazy = envInstallerLazy;
            _impFromRemoteLazy = impFromRemoteLazy;
            _userLazy = userLazy;
        }

        private readonly LazyInitLog<IContextOfSite> _context;
        private readonly Lazy<IEnvironmentInstaller> _envInstallerLazy;
        private readonly Lazy<ImportFromRemote> _impFromRemoteLazy;
        private readonly Lazy<IUser> _userLazy;

        /// <summary>
        /// Finish system installation which had somehow been interrupted
        /// </summary>
        /// <returns></returns>
        public bool Resume() => _envInstallerLazy.Value.ResumeAbortedUpgrade();

        #endregion


        #region App / Content Package Installation

        /// <summary>
        /// Before this was GET Module/RemoteInstallDialogUrl
        /// </summary>
        /// <param name="isContentApp"></param>
        /// <returns></returns>
        public string RemoteWizardUrl(bool isContentApp, IModule module) 
            => _envInstallerLazy.Value.Init(Log)
                .GetAutoInstallPackagesUiUrl(
                    _context.Ready.Site,
                    module,
                    isContentApp);

        /// <summary>
        /// Before this was GET Installer/InstallPackage
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <returns></returns>
        public Tuple<bool, List<Message>> RemotePackage(string packageUrl, IModule container)
        {
            var isApp = !container.IsContent;

            Log.Add("install package:" + packageUrl);

            var block = container.BlockIdentifier;
            var result = _impFromRemoteLazy.Value.Init(_userLazy.Value, Log)
                .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl);

            Log.Add("install completed with success:" + result.Item1);

            return result;
        }

        #endregion
    }
}
