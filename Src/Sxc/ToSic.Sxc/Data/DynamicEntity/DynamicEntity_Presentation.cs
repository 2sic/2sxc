namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public dynamic Presentation => _presentation ?? (_presentation = Entity is EntityInBlock entityInGroup
                ? SubDynEntity(entityInGroup.Presentation)
                : null);
        private IDynamicEntity _presentation;
    }
}
