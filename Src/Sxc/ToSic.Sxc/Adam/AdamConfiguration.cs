using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using static ToSic.Eav.Apps.Adam.AdamConstants;

namespace ToSic.Sxc.Adam
{
    public class AdamConfiguration
    {
        public AdamConfiguration(IAppStates appStates)
        {
            _appStates = appStates;
        }
        private readonly IAppStates _appStates;
        

        public string AdamAppRootFolder
        {
            get
            {
                if (_adamAppRootFolder != null) return _adamAppRootFolder;

                var primaryApp = _appStates.Get(Eav.Constants.PresetIdentity);

                var found = primaryApp.List.FirstOrDefaultOfType(TypeName)?.Value<string>(ConfigFieldRootFolder);

                return _adamAppRootFolder = found ?? AdamFolderMask;
            }
        }

        private static string _adamAppRootFolder;

        internal string PathForApp(AppState app)
        {
            var valuesDic = new Dictionary<string, string>
            {
                { "[AppFolder]", app.Folder },
                { "[ZoneId]", app.ZoneId.ToString() },
                { "[AppId]", app.AppId.ToString() },
                { "[AppGuid]", app.AppGuidName }
            };
            var finalPath = FillMask(valuesDic, AdamAppRootFolder);
            return finalPath;
        }

        private static string FillMask(Dictionary<string, string> valuesDictionary, string mask)
            => valuesDictionary.Aggregate(mask, (current, dicItem)
                => Regex.Replace(current, Regex.Escape(dicItem.Key), dicItem.Value, RegexOptions.CultureInvariant));
    }
}
