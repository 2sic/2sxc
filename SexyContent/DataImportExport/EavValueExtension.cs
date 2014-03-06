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
        
        public static string GetLanguage(this EavValue value, string valueLanguage)
        {
            return value.ValuesDimensions.Select(reference => reference.Dimension.ExternalKey)
                                         .FirstOrDefault(language => language == valueLanguage);
        }

        public static IEnumerable<string> GetLanguages(this EavValue value)
        {
            return value.ValuesDimensions.Select(reference => reference.Dimension.ExternalKey);
        }

        public static IEnumerable<string> GetLanguagesReferenced(this EavValue value, string valueLanguage, bool referenceReadWrite)
        {
            return value.ValuesDimensions.Where(reference => referenceReadWrite ? !reference.ReadOnly : true)
                                         .Where(reference => reference.Dimension.ExternalKey != valueLanguage)
                                         .Select(reference => reference.Dimension.ExternalKey)
                                         .ToList();
        }

        public static bool IsLanguageReadOnly(this EavValue value, string language)
        {
            var languageReference = value.ValuesDimensions.FirstOrDefault(reference => reference.Dimension.ExternalKey == language);
            if (languageReference == null)
            {
                return false;
            }
            return languageReference.ReadOnly;
        }
    }
}