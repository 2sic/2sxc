using System.Text.RegularExpressions;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    internal static class StringExtension
    {
        /// <summary>
        /// Get for example en-US from [ref(en-US,ro)].
        /// </summary>
        public static string GetValueReferenceLanguage(this string valueString)
        {
            var match = Regex.Match(valueString, @"\[ref\((?<language>.+),(?<readOnly>.+)\)\]");
            if (match.Success)
            {
                return match.Groups["language"].Value;
            }
            return null;
        }

        /// <summary>
        /// Get for example ro from [ref(en-US,ro)].
        /// </summary>
        public static string GetValueReferenceProtection(this string valueString, string defaultValue = "")
        {
            var match = Regex.Match(valueString, @"\[ref\((?<language>.+),(?<readOnly>.+)\)\]");
            if (match.Success)
            {
                return match.Groups["readOnly"].Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Is the string equals []?
        /// </summary>
        public static bool IsValueNull(this string valueString)
        {
            return valueString == "[]";
        }

        /// <summary>
        /// Is the string equals =(""), an empty string?
        /// </summary>
        public static bool IsValueEmpty(this string valueString)
        {
            return valueString == "[\"\"]";
        }

        /// <summary>
        /// Remove special characters like ?, &, %, - or spaces from a string.
        /// </summary>
        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "");
        }
    }
}