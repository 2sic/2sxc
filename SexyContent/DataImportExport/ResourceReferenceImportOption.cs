namespace ToSic.SexyContent.DataImportExport
{
    public enum ResourceReferenceImportOption
    {
        Keep,
        Resolve
    }

    public static class ResourceReferenceImportOptionExtension
    {
        public static bool IsResolve(this ResourceReferenceImportOption option)
        {
            return option == ResourceReferenceImportOption.Resolve;
        }
    }
}