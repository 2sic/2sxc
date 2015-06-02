namespace ToSic.Eav.ImportExport.Refactoring.Options
{
    public static class ResourceReferenceExportExtension
    {
        public static bool IsResolve(this ResourceReferenceExport option)
        {
            return option == ResourceReferenceExport.Resolve;
        }
    }
}