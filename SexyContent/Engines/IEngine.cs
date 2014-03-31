using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="TemplatePath"></param>
        /// <param name="TemplateText"></param>
        /// <returns></returns>
        string Render(Template template, string templatePath, App app, List<Element> Elements, Element ListElement, ModuleInstanceContext HostingModule, string LocalResourcesPath, IDataSource DataSource);
    }
}