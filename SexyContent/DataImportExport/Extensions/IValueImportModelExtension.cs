using System.Collections.Generic;
using ToSic.Eav.Import;

namespace ToSic.SexyContent.DataImportExport.Extensions
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
            var valueDimesnions = valueModel.ValueDimensions as List<ValueDimension>;
            if (valueDimesnions == null)
            {
                valueDimesnions = new List<ValueDimension>();
                valueModel.ValueDimensions = valueDimesnions;
            }

            if (!string.IsNullOrEmpty(language))
            {
                valueDimesnions.Add
                (
                    new ValueDimension { DimensionExternalKey = language, ReadOnly = readOnly }
                );
            }
        }
    }
}