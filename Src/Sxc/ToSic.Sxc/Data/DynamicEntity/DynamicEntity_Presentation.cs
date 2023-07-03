using ToSic.Eav.Data;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public dynamic Presentation => _presentation 
                                       ?? (_presentation =  SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private IDynamicEntity _presentation;
    }
}
