using JetBrains.Annotations;
using System;
using System.IO;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="purpose">razor;token;search;api;auto</param>
        /// <returns></returns>
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
                case AssetEditor.CsExtension:
                    if ((folder?.ToLowerInvariant() ?? "").Contains(AssetEditor.CsApiFolder.ToLowerInvariant()))
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content =
                            _assetTemplates.GetTemplate(AssetTemplateType.WebApi).Replace(AssetTemplates.CsApiTemplateControllerName, nameWithoutExt);
                    }
                    else
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content = _assetTemplates.GetTemplate(purpose == AssetEditor.PurposeType.Search
                                ? AssetTemplateType.CustomSearchCsCode
                                : AssetTemplateType.CsCode)
                            .Replace(AssetTemplates.CsCodeTemplateName, nameWithoutExt);
                    }
                    break;

                // .cshtml files (razor) or .code.cshtml (razor code-behind)
                case AssetEditor.CshtmlExtension:
                    {
                        // ensure all .cshtml start with "_"
                        if (!name.StartsWith(AssetEditor.CshtmlPrefix))
                        {
                            name = AssetEditor.CshtmlPrefix + name;
                            path = (string.IsNullOrWhiteSpace(folder) ? "" : folder + "\\") + name;
                        }

                        // first check the code-extension, because it's longer but also would contain the non-code extension
                        if (name.EndsWith(AssetEditor.CodeCshtmlExtension))
                            content.Content = _assetTemplates.GetTemplate(AssetTemplateType.CsHtmlCode);
                        else if (name.EndsWith(AssetEditor.CshtmlExtension))
                            content.Content = _assetTemplates.GetTemplate(AssetTemplateType.CsHtml);
                        break;
                    }

                // .html files (Tokens)
                case AssetEditor.TokenHtmlExtension:
                    content.Content = _assetTemplates.GetTemplate(AssetTemplateType.Token);
                    break;
            }

            return path;
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
