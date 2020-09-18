namespace ToSic.Sxc.WebApi.Context
{
    public class InstanceDto
    {
        public int Id;

        // todo: find out if we can get rid of this - to DNN specific
        public int ModuleId;
        public bool ShowOnAllPages;
        public string Title;
        public bool IsDeleted;
        public PageDto Page;
    }
}
