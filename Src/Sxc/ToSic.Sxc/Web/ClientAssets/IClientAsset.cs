namespace ToSic.Sxc.Web
{
    public interface IClientAsset
    {
        /// <summary>
        /// Asset ID for use in HTML - ideally should ensure that this asset is only loaded once
        /// </summary>
        string Id { get; set; }

        bool IsJs { get; set; }
        string Url { get; set; }
        int Priority { get; set; }
        string PosInPage { get; set; }
        bool AutoOpt { get; set; }
        bool IsExternal { get; set; }
        string Content { get; set; }
    }
}