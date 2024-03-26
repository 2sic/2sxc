namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    internal class AppApiCacheItem
    {
        public bool FlagForRemove { get; set; } = default;
        public bool IsAppCode { get; set; } = default;
        public string AppCodePath { get; set; } = default;
        public string DllName { get; set; } = default;
    }
}
