using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Adam.Sys.Paths;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamConfiguration(IAppReaderFactory appReaders)
{
    [field: AllowNull, MaybeNull]
    public string AdamAppRootFolder => field ??= GenerateAdamAppRootFolder();

    private string GenerateAdamAppRootFolder()
    {
        var found = appReaders.GetSystemPreset()
            .List
            .First(typeName: AdamConstants.TypeName)?
            .Get<string>(AdamConstants.ConfigFieldRootFolder);

        return found ?? AdamConstants.AdamFolderMask;
    }

    internal string PathForApp(IAppSpecs app)
    {
        var valuesDic = new Dictionary<string, string>
        {
            { AppConstants.AppFolderPlaceholder, app.Folder },
            { "[ZoneId]", app.ZoneId.ToString() },
            { "[AppId]", app.AppId.ToString() },
            { "[AppGuid]", app.NameId }
        };
        var finalPath = FillMask(valuesDic, AdamAppRootFolder);
        return finalPath;
    }

    private static string FillMask(Dictionary<string, string> valuesDictionary, string mask)
        => valuesDictionary.Aggregate(mask, (current, dicItem)
            => Regex.Replace(current, Regex.Escape(dicItem.Key), dicItem.Value, RegexOptions.CultureInvariant));
}