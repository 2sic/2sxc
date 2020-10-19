namespace ToSic.Sxc.WebApi.Context
{
    public class InstanceDto
    {
        /// <summary>
        /// The Module Id / Instance Id.
        /// Note that an instance can be used again, in which case the UsageId will differ
        /// </summary>
        public int Id;

        /// <summary>
        /// The usage id - in case an instance is reused.
        /// </summary>
        public int UsageId;

        public bool ShowOnAllPages;
        public string Title;
        public bool IsDeleted;
        public PageDto Page;
    }
}
