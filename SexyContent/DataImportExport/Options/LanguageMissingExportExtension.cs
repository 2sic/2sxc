namespace ToSic.Eav.ImportExport.Refactoring.Options
{
    public static class LanguageMissingExportExtension
    {
        public static bool IsCreate(this LanguageMissingExport option)
        {
            return option == LanguageMissingExport.Create;
        }
    }
}