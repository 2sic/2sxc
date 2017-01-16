using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace ToSic.SexyContent.Internal
{
    public class TemplateManager
    {
        public const string RazorC = "C# Razor";
        public const string RazorVb = "VB Razor";
        public const string TokenReplace = "Token";

        public App App;
        public TemplateManager(App app)
        {
            App = app;
        }

        /// <summary>
        /// Creates a template file if it does not already exists, and uses a default text to insert. Returns the new path
        /// </summary>
        public string CreateTemplateFileIfNotExists(string name, string type, string location, HttpServerUtility server, string contents = "")
        {
            if (type == RazorC)
            {
                if (!name.StartsWith("_"))
                    name = "_" + name;
                if (Path.GetExtension(name) != ".cshtml")
                    name += ".cshtml";
            }
            else if (type == RazorVb)
            {
                if (!name.StartsWith("_"))
                    name = "_" + name;
                if (Path.GetExtension(name) != ".vbhtml")
                    name += ".vbhtml";
            }
            else if (type == TokenReplace)
            {
                if (Path.GetExtension(name) != ".html")
                    name += ".html";
            }

            var templatePath = Regex.Replace(name, @"[?:\/*""<>|]", "");
            var absolutePath = server.MapPath(Path.Combine(GetTemplatePathRoot(location, App), templatePath));

            if (!File.Exists(absolutePath))
            {
                var stream = new StreamWriter(File.Create(absolutePath));
                stream.Write(contents);
                stream.Flush();
                stream.Close();
            }

            return templatePath;
        }


        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="templateLocation"></param>
        public void EnsureTemplateFolderExists(string templateLocation)
        {
            var portalPath = templateLocation == Settings.TemplateLocations.HostFileSystem 
                ? HostingEnvironment.MapPath(Settings.PortalHostDirectory) 
                : App.OwnerPortalSettings.HomeDirectoryMapPath;
            var sexyFolderPath = Path.Combine(portalPath, Settings.TemplateFolder);

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            if (!sexyFolder.Exists)
                sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles("web.config").Any())
                File.Copy(HostingEnvironment.MapPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (!String.IsNullOrEmpty(App.Folder))
            {
                var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
                if (!contentFolder.Exists)
                    contentFolder.Create();
            }


        }

        /// <summary>
        /// Returns all template files in the template folder.
        /// </summary>
        public IEnumerable<string> GetTemplateFiles(HttpServerUtility server, string templateType, string templateLocation)
        {
            var templatePathRootMapPath = server.MapPath(GetTemplatePathRoot(templateLocation, App));
            var directory = new DirectoryInfo(templatePathRootMapPath);

            EnsureTemplateFolderExists(templateLocation);

            // Filter the files according to type
            var fileFilter = "*.html";
            switch (templateType)
            {
                case RazorC:
                    fileFilter = "*.cshtml";
                    break;
                case RazorVb:
                    fileFilter = "*.vbhtml";
                    break;
                case TokenReplace:
                    fileFilter = "*.html";
                    break;
            }

            var files = directory.GetFiles(fileFilter, SearchOption.AllDirectories);
            return (from d in files where d.Name != Settings.WebConfigFileName select (d.FullName).Replace(templatePathRootMapPath + "\\", "").Replace('\\', '/'));
        }

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationId, App app)
        {
            var rootFolder = locationId == Settings.TemplateLocations.HostFileSystem
                ? Settings.PortalHostDirectory
                : app.OwnerPortalSettings.HomeDirectory;
            rootFolder += Settings.TemplateFolder + "/" + app.Folder;
            return rootFolder;
        }
    }
}