using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Files;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Adam
{
    internal class SecurityCheckHelpers: HasLog<SecurityCheckHelpers>
    {
        #region Constructors / DI

        public SecurityCheckHelpers() : base("Sxc.AdmSec")
        {
        }


        #endregion

        #region Simple Static Checks

        


        internal static bool DestinationIsInItem(Guid guid, string field, string path, out HttpExceptionAbstraction preparedException)
        {
            var inAdam = Sxc.Adam.Security.PathIsInItemAdam(guid, field, path);
            preparedException = inAdam
                ? null
                : HttpException.PermissionDenied("Can't access a resource which is not part of this item.");
            return inAdam;
        }

        //[AssertionMethod]
        internal static bool IsKnownRiskyExtension(string fileName)
            => FileNames.IsKnownRiskyExtension(fileName);

        //[AssertionMethod]
        internal static void ThrowIfAccessingRootButNotAllowed(bool usePortalRoot, bool userIsRestricted)
        {
            if (usePortalRoot && userIsRestricted)
                throw HttpException.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
        }


        #endregion

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
                    if (string.Equals(extension,f, StringComparison.InvariantCultureIgnoreCase))
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
