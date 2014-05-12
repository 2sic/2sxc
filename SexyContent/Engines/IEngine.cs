using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DotNetNuke.UI.Modules;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.Engines
{
    public interface IEngine
    {
        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns></returns>
        string Render(Template template, string templatePath, App app, ModuleInstanceContext HostingModule, string LocalResourcesPath, IDataSource DataSource);

        /// <summary>
        /// Prepare the templates data
        /// </summary>
        void PrepareViewData();

    }
}