using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.Adam
{
    public partial class AdamTransUpload<TFolderId, TFileId>
    {
        #region More Checks

        internal bool CustomFileFilterOk(string additionalFilter, string fileName)
        {
            var wrapLog = Log.Fn<bool>();
            var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "";
            var hasNonAzChars = new Regex("[^a-z]", RegexOptions.IgnoreCase);

            Log.A($"found additional file filter: {additionalFilter}");
            var filters = additionalFilter.Split(',').Select(f => f.Trim()).ToList();
            Log.A($"found {filters.Count} filters in {additionalFilter}, will test on {fileName} with ext {extension}");

            foreach (var f in filters)
            {
                // just a-z characters
                if (!hasNonAzChars.IsMatch(f))
                    if (string.Equals(extension, f, StringComparison.InvariantCultureIgnoreCase))
                        return wrapLog.Return(true, $"filter {f} matched filename {fileName}");
                    else
                        continue;

                // could be regex or simple *.ext
                if (f.StartsWith("*."))
                    if (string.Equals(extension, f.Substring(2), StringComparison.InvariantCultureIgnoreCase))
                        return wrapLog.Return(true, $"filter {f} matched filename {fileName}");
                    else
                        continue;

                // it's a regex
                try
                {
                    if (new Regex(f, RegexOptions.IgnoreCase).IsMatch(fileName))
                        return wrapLog.Return(true, $"filter {f} matched filename {fileName}");
                }
                catch
                {
                    Log.A($"filter {f} was detected as reg-ex but threw error");
                }

            }

            return wrapLog.Return(false);
        }

        #endregion

    }
}
