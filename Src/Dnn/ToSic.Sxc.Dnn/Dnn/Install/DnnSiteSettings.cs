using System.IO;
using System.Web;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Install
{
    public class DnnSiteSettings
    {
        // todo: #performance try to cache the result of settings-stored in a static variable, this full check
        // todo: shouldn't have to happen every time

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        public void EnsureSiteIsConfigured(IBlock block, HttpServerUtility server)
        {
            var sexyFolder = new DirectoryInfo(block.Context.Site.AppsRootPhysicalFull); //server.MapPath(block.Context.Tenant.AppsRootPhysical));
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, Eav.Constants.ContentAppName));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
            {
                // configure it
                var tm = block.Context.ServiceProvider.Build<TemplateHelpers>().Init(block.App, block.Log);
                tm.EnsureTemplateFolderExists(false);
            }
        }
    }
}