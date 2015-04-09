namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class LanguageMissingExportExtension
    {
        public static bool IsCreate(this LanguageMissingExport option)
        {
            return option == LanguageMissingExport.Create;
        }
    }
}