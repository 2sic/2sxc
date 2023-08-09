namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetDraft(Entity));

        /// <inheritdoc />
        public dynamic GetPublished() => SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetPublished(Entity));
    }
}
