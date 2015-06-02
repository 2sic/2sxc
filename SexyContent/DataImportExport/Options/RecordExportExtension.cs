namespace ToSic.Eav.ImportExport.Refactoring.Options
{
    public static class RecordExportExtension
    {
        public static bool IsBlank(this RecordExport option)
        {
            return option == RecordExport.Blank;
        }
    }
}