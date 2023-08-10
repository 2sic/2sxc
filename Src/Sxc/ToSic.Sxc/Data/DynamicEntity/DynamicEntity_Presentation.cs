using ToSic.Eav.Data;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public dynamic Presentation => _p ?? (_p = Helper.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private IDynamicEntity _p;
    }
}
