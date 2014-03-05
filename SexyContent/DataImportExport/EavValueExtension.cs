using DotNetNuke.Services.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public static class EavValueExtension
    {
        /// <summary>
        /// Resolve resources referenced, for example hyperlinks File:4711 to /file.txt.
        /// </summary>
        public static string ResolveValueReference(this EavValue value)
        {
            if (value.IsHyperlink())
            {
                return ReolveHyperlink(value);
            }
            return value.Value;
        }

        private static string ReolveHyperlink(EavValue value)
        {
            var match = Regex.Match(value.Value, @".+\:(?<id>\d+)");
            if (!match.Success)
            {
                return value.Value;
            }

            var hyperlinkId = int.Parse(match.Groups["id"].Value);
            var fileInfo = FileManager.Instance.GetFile(hyperlinkId);
            if (fileInfo == null)
            {
                return value.Value;
            }

            return fileInfo.RelativePath;
        }

        private static bool IsHyperlink(this EavValue value)
        {
            return value.Attribute.Type == "Hyperlink";
        }

        /// <summary>
        /// Of this EAV value, get reference to the dimension specified. If no reference 
        /// exists, the result will be null.
        /// </summary>
        public static ValueDimension GetValueDimension(this EavValue value, Dimension dimension)
        {
            return value.ValuesDimensions.FirstOrDefault(dim => dim.DimensionID == dimension.DimensionID);
        }

        /// <summary>
        /// Get all dimensions this EAV value references.
        /// </summary>
        public static IEnumerable<ValueDimension> GetValueDimensions(this EavValue value)
        {
            return value.ValuesDimensions;
        }

        /// <summary>
        /// Get all dimensions this EAV references, but not the dimesnion specified by 
        /// valueDimension (the valueDimension may also be called the current dimension).
        /// </summary>
        public static IEnumerable<ValueDimension> GetValueReferenceDimensions(this EavValue value, Dimension valueDimension)
        {
            return value.GetValueDimensions().Where(dim => !dim.ReadOnly)
                                             .Where(dim => dim.DimensionID != valueDimension.DimensionID)
                                             .ToList();
        }
    }
}