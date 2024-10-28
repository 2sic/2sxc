using System.IO;
using JetBrains.Annotations;
using ToSic.Sxc.Apps.Internal.Assets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private string GetTemplateContent(AppFileDto assetFromTemplateDto)
    {
        var name = Path.GetFileName(assetFromTemplateDto.Path);
        var ext = Path.GetExtension(assetFromTemplateDto.Path);
        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);

        return _assetTemplates.GetTemplates()
            .FirstOrDefault(t => t.Key.Equals(assetFromTemplateDto.TemplateKey, StringComparison.InvariantCultureIgnoreCase))
            ?.Body
            .Replace(AssetTemplates.CsApiTemplateControllerName, nameWithoutExt)
            .Replace(AssetTemplates.CsCodeTemplateName, nameWithoutExt)
            .Replace(AssetTemplates.CsDataSourceName, nameWithoutExt);
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