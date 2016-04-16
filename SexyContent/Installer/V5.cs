using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;

namespace ToSic.SexyContent.Installer
{
    internal class V5: VersionBase
    {

        public V5(string version, Logger sharedLogger) : base(version, sharedLogger)  { }


        /// <summary>
        /// While upgrading to 05.04.02, make sure the template folders get renamed to "2sxc"
        /// </summary>
        internal void Version050500()
        {
            logger.LogStep("05.05.00", "Starting", false);

            var portalController = new PortalController();
            var portals = portalController.GetPortals();
            var pathsToCopy = portals.Cast<PortalInfo>().Select(p => p.HomeDirectoryMapPath).ToList();
            pathsToCopy.Add(HttpContext.Current.Server.MapPath("~/Portals/_default/"));
            logger.LogStep("05.05.00", "Starting paths for " + pathsToCopy.Count + " paths", false);
            foreach (var path in pathsToCopy)
            {
                logger.LogStep("05.05.00", "Path: " + path, false);
                var portalFolder = new DirectoryInfo(path);
                if (portalFolder.Exists)
                {
                    var oldSexyFolder = new DirectoryInfo(Path.Combine(path, "2sexy"));
                    var newSexyFolder = new DirectoryInfo(Path.Combine(path, "2sxc"));
                    var newSexyContentFolder = new DirectoryInfo(Path.Combine(newSexyFolder.FullName, "Content"));
                    if (oldSexyFolder.Exists && !newSexyFolder.Exists)
                    {
                        logger.LogStep("05.05.00", "Path: " + path + " will copy from 2Sexy to 2sxc", false);

                        // Move 2sexy directory to 2sxc/Content
                        Helpers.DirectoryCopy(oldSexyFolder.FullName, newSexyContentFolder.FullName, true);

                        // Leave info message in the content folder
                        File.WriteAllText(Path.Combine(oldSexyFolder.FullName, "__WARNING - old copy of files - READ ME.txt"), "This is a short information\r\n\r\n2sxc renamed the main folder from \"[Portal]/2Sexy\" to \"[Portal]/2sxc\" in version 5.5.\r\n\r\nTo make sure that links to images/css/js still work, the old folder was copied and this was left. Please clean up and delete the entire \"[Portal]/2Sexy/\" folder once you're done. \r\n\r\nMany thanks!\r\n2sxc\r\n\r\nPS: Remember that you might have ClientDependency activated, so maybe you still have bundled & minified  JS/CSS-Files in your cache pointing to the old location. When done cleaning up, we recommend increasing the version just to be sure you're not seeing an old files that don't exist any more. ");

                        // Move web.config (should be directly in 2sxc)
                        if (File.Exists(Path.Combine(newSexyContentFolder.FullName, "web.config")))
                            File.Move(Path.Combine(newSexyContentFolder.FullName, "web.config"), Path.Combine(newSexyFolder.FullName, "web.config"));

                    }
                }
            }
            logger.LogStep("05.05.00", "Paths done", false);
        }

    }
}