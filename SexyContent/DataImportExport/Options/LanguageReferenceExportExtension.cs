namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class LanguageReferenceExportExtension
    {
        public static bool IsLink(this LanguageReferenceExport option)
        {
            return option == LanguageReferenceExport.Link;
        }

        public static bool IsResolve(this LanguageReferenceExport option)
        {
            return option == LanguageReferenceExport.Resolve;
        }
    }
}