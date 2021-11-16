using JetBrains.Annotations;
using System;
using System.IO;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        private const string ApiFolder = "api";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="purpose">razor;token;search;api;auto</param>
        /// <returns></returns>
        [Obsolete("This Method is Deprecated", false)]
        private string SanitizePathAndContent(string path, FileContentsDto content, string purpose)
        {
            var name = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var ext = Path.GetExtension(path);
            
            // not sure what this is for, since I believe code should only get here if there was an ext and it's cshtml
            // probably just to prevent some very unexpected create
            if (name == null) name = "missing-name.txt";

            switch (ext?.ToLowerInvariant())
            {
                // .cs files - usually API controllers
                case Extension.Cs:
                    if ((folder?.ToLowerInvariant() ?? "").Contains(ApiFolder.ToLowerInvariant()))
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content =
                            _assetTemplates.GetTemplate(TemplateKey.Api).Replace(AssetTemplates.CsApiTemplateControllerName, nameWithoutExt);
                    }
                    else
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content = _assetTemplates.GetTemplate(purpose == Purpose.Search
                                ? TemplateKey.CustomSearchCsCode
                                : TemplateKey.CsCode)
                            .Replace(AssetTemplates.CsCodeTemplateName, nameWithoutExt);
                    }
                    break;

                // .cshtml files (razor) or .code.cshtml (razor code-behind)
                case Extension.Cshtml:
                    {
                        // ensure all .cshtml start with "_"
                        if (!name.StartsWith(AssetEditor.CshtmlPrefix))
                        {
                            name = AssetEditor.CshtmlPrefix + name;
                            path = (string.IsNullOrWhiteSpace(folder) ? "" : folder + "\\") + name;
                        }

                        // first check the code-extension, because it's longer but also would contain the non-code extension
                        if (name.EndsWith(Extension.CodeCshtml))
                            content.Content = _assetTemplates.GetTemplate(TemplateKey.CsHtmlCode);
                        else if (name.EndsWith(Extension.Cshtml))
                            content.Content = _assetTemplates.GetTemplate(TemplateKey.CsHtml);
                        break;
                    }

                // .html files (Tokens)
                case Extension.Html:
                    content.Content = _assetTemplates.GetTemplate(TemplateKey.Token);
                    break;
            }

            return path;
        }

        private string GetTemplateContent(AssetFromTemplateDto assetFromTemplateDto)
        {
            var name = Path.GetFileName(assetFromTemplateDto.Path);
            var ext = Path.GetExtension(assetFromTemplateDto.Path);
            var nameWithoutExt = name.Substring(0, name.Length - ext.Length);

            return _assetTemplates.GetTemplate(assetFromTemplateDto.TemplateKey)
                .Replace(AssetTemplates.CsApiTemplateControllerName, nameWithoutExt)
                .Replace(AssetTemplates.CsCodeTemplateName, nameWithoutExt);
        }

        private static void EnsureCshtmlStartWithUnderscore(AssetFromTemplateDto assetFromTemplateDto)
        {
            var name = Path.GetFileName(assetFromTemplateDto.Path);
            if ((assetFromTemplateDto.TemplateKey == TemplateKey.CsHtml || assetFromTemplateDto.TemplateKey == TemplateKey.CsHtmlCode)
                && !name.StartsWith(AssetEditor.CshtmlPrefix))
            {
                name = AssetEditor.CshtmlPrefix + name;
                var folder = Path.GetDirectoryName(assetFromTemplateDto.Path) ?? "";
                assetFromTemplateDto.Path = Path.Combine(folder, name);
            }
        }

        [AssertionMethod]
        private string EnsurePathMayBeAccessed(string p, string appPath, bool allowFullAccess)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));
            // security check, to ensure no results leak from outside the app

            if (!allowFullAccess && !p.StartsWith(appPath))
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }
    }
}
