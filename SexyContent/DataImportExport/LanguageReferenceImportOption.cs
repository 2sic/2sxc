namespace ToSic.SexyContent.DataImportExport
{
    public enum LanguageReferenceImportOption
    {
        Keep,
        Resolve
    }

    public static class LanguageReferenceImportOptionExtension
    {
        public static bool IsResolve(this LanguageReferenceImportOption option)
        {
            return option == LanguageReferenceImportOption.Resolve;
        }
    }
}