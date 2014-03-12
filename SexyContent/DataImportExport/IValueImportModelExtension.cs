using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.Import;

namespace ToSic.SexyContent.DataImportExport
{
    public static class IValueImportModelExtension
    {
        public static List<IValueImportModel> ToList(this IValueImportModel valueModel)
        {
            var list = new List<IValueImportModel>();
            list.Add(valueModel);
            return list;
        }

        public static void AppendValueReference(this IValueImportModel valueModel, string language, bool readOnly)
        {
            var valueDimesnions = valueModel.ValueDimensions as List<ValueDimension>;
            if (valueDimesnions == null)
            {
                valueDimesnions = new List<ValueDimension>();
                valueModel.ValueDimensions = valueDimesnions;
            }

            valueDimesnions.Add
            (
                new ValueDimension() { DimensionExternalKey = language, ReadOnly = readOnly }
            );
        }
    }
}