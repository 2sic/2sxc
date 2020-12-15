namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => SubDynEntity(Entity?.GetDraft());

        /// <inheritdoc />
        public dynamic GetPublished() => SubDynEntity(Entity?.GetPublished());

    }
}
