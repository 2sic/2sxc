using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ToSic.SexyContent.Statics
{
    public class TemplateManager
    {
        public const string RazorC = "C# Razor";
        public const string RazorVb = "VB Razor";
        public const string TokenReplace = "Token";

        public SexyContent SxContext;
        public App App;
        public TemplateManager(SexyContent sxc)
        {
            SxContext = sxc;
            App = sxc.App;
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
        /// <param name="server"></param>
        /// <param name="templateLocation"></param>
        internal void EnsureTemplateFolderExists(HttpServerUtility server, string templateLocation)
        {
            var portalPath = templateLocation == SexyContent.TemplateLocations.HostFileSystem ? server.MapPath(SexyContent.PortalHostDirectory) : SxContext.OwnerPS.HomeDirectoryMapPath;
            var sexyFolderPath = Path.Combine(portalPath, SexyContent.TemplateFolder);

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            if (!sexyFolder.Exists)
                sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles("web.config").Any())
                File.Copy(server.MapPath(SexyContent.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, SexyContent.WebConfigFileName));

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

            EnsureTemplateFolderExists(server, templateLocation);

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
            return (from d in files where d.Name != SexyContent.WebConfigFileName select (d.FullName).Replace(templatePathRootMapPath + "\\", "").Replace('\\', '/'));
        }

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationId, App app)
        {
            var rootFolder = (locationId == SexyContent.TemplateLocations.PortalFileSystem ? app.OwnerPS.HomeDirectory : SexyContent.PortalHostDirectory);
            rootFolder += SexyContent.TemplateFolder + "/" + app.Folder;
            return rootFolder;
        }
    }
}