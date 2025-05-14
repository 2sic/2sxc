using System.IO;
using System.Text.RegularExpressions;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Backend.Adam;

partial class AdamTransUpload<TFolderId, TFileId>
{
    #region More Checks

    internal bool CustomFileFilterOk(string additionalFilter, string fileName)
    {
        var l = Log.Fn<bool>();
        var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "";
        var hasNonAzChars = new Regex("[^a-z]", RegexOptions.IgnoreCase);

        l.A($"found additional file filter: {additionalFilter}");
        var filters = additionalFilter.CsvToArrayWithoutEmpty();
        l.A($"found {filters.Length} filters in {additionalFilter}, will test on {fileName} with ext {extension}");

        foreach (var f in filters)
        {
            // just a-z characters
            if (!hasNonAzChars.IsMatch(f))
                if (string.Equals(extension, f, StringComparison.InvariantCultureIgnoreCase))
                    return l.ReturnTrue($"filter {f} matched filename {fileName}");
                else
                    continue;

            // could be regex or simple *.ext
            if (f.StartsWith("*."))
                if (string.Equals(extension, f.Substring(2), StringComparison.InvariantCultureIgnoreCase))
                    return l.ReturnTrue($"filter {f} matched filename {fileName}");
                else
                    continue;

            // it's a regex
            try
            {
                if (new Regex(f, RegexOptions.IgnoreCase).IsMatch(fileName))
                    return l.Return(true, $"filter {f} matched filename {fileName}");
            }
            catch
            {
                l.A($"filter {f} was detected as reg-ex but threw error");
            }

        }

        return l.ReturnFalse();
    }

    #endregion

}