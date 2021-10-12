using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public dynamic Presentation => _presentation 
                                       ?? (_presentation =  SubDynEntity(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private IDynamicEntity _presentation;
    }
}
