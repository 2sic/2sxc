﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToSic.Sxc.WebApi.Adam
{
    public partial class AdamTransUpload<TFolderId, TFileId>
    {
        #region More Checks

        internal bool CustomFileFilterOk(string additionalFilter, string fileName)
        {
            var wrapLog = Log.Call<bool>();
            var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "";
            var hasNonAzChars = new Regex("[^a-z]", RegexOptions.IgnoreCase);

            Log.Add($"found additional file filter: {additionalFilter}");
            var filters = additionalFilter.Split(',').Select(f => f.Trim()).ToList();
            Log.Add($"found {filters.Count} filters in {additionalFilter}, will test on {fileName} with ext {extension}");

            foreach (var f in filters)
            {
                // just a-z characters
                if (!hasNonAzChars.IsMatch(f))
                    if (string.Equals(extension, f, StringComparison.InvariantCultureIgnoreCase))
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                    else
                        continue;

                // could be regex or simple *.ext
                if (f.StartsWith("*."))
                    if (string.Equals(extension, f.Substring(2), StringComparison.InvariantCultureIgnoreCase))
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                    else
                        continue;

                // it's a regex
                try
                {
                    if (new Regex(f, RegexOptions.IgnoreCase).IsMatch(fileName))
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                }
                catch
                {
                    Log.Add($"filter {f} was detected as reg-ex but threw error");
                }

            }

            return false;
        }

        #endregion

    }
}
