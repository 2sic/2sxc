namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => SubDynEntityOrNull(Entity?.GetDraft());

        /// <inheritdoc />
        public dynamic GetPublished() => SubDynEntityOrNull(Entity?.GetPublished());

    }
}
