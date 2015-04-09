namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class RecordExportExtension
    {
        public static bool IsBlank(this RecordExport option)
        {
            return option == RecordExport.Blank;
        }
    }
}