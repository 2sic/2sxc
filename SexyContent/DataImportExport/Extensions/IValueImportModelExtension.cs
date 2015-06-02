using System.Collections.Generic;
using ToSic.Eav.Import;

namespace ToSic.Eav.ImportExport.Refactoring.Extensions
{
    internal static class IValueImportModelExtension
    {
        public static List<IValueImportModel> ToList(this IValueImportModel valueModel)
        {
            var list = new List<IValueImportModel>();
            list.Add(valueModel);
            return list;
        }

        /// <summary>
        /// Append a language reference (ValueDimension) to this value (ValueImportModel).
        /// </summary>
        public static void AppendLanguageReference(this IValueImportModel valueModel, string language, bool readOnly)
        {
            var valueDimesnions = valueModel.ValueDimensions as List<Import.ValueDimension>;
            if (valueDimesnions == null)
            {
                valueDimesnions = new List<Import.ValueDimension>();
                valueModel.ValueDimensions = valueDimesnions;
            }

            if (!string.IsNullOrEmpty(language))
            {
                valueDimesnions.Add
                (
                    new Import.ValueDimension { DimensionExternalKey = language, ReadOnly = readOnly }
                );
            }
        }
    }
}