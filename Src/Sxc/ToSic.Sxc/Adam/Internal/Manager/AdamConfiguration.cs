using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.Specs;
using static ToSic.Eav.Apps.Internal.AdamConstants;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamConfiguration(IAppReaderFactory appReaders)
{
    public string AdamAppRootFolder
    {
        get
        {
            if (_adamAppRootFolder != null)
                return _adamAppRootFolder;

            var found = appReaders.GetSystemPreset().List.FirstOrDefaultOfType(TypeName)?.Get<string>(ConfigFieldRootFolder);

            return _adamAppRootFolder = found ?? AdamFolderMask;
        }
    }

    private static string _adamAppRootFolder;

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