namespace ToSic.Eav.ImportExport.Refactoring.Options
{
    public static class ResourceReferenceImportExtension
    {
        public static bool IsResolve(this ResourceReferenceImport option)
        {
            return option == ResourceReferenceImport.Resolve;
        }
    }
}