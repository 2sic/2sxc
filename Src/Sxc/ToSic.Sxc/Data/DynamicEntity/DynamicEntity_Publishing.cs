namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => Helper.SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetDraft(Entity));

        /// <inheritdoc />
        public dynamic GetPublished() => Helper.SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetPublished(Entity));
    }
}
