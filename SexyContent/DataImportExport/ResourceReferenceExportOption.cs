namespace ToSic.SexyContent.DataImportExport
{
    public enum ResourceReferenceExportOption
    {
        Link = 0,
        Resolve = 1
    }

    public static class ResourceReferenceExportOptionExtension
    {
        public static bool IsResolve(this ResourceReferenceExportOption option)
        {
            return option == ResourceReferenceExportOption.Resolve;
        }
    }
}