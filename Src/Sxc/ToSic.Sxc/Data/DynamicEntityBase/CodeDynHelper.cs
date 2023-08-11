using ToSic.Eav.Data;
using ToSic.Sxc.Data.Decorators;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    internal class CodeDynHelper
    {
        public IEntity Entity { get; }
        public SubDataFactory SubDataFactory { get; }

        public CodeDynHelper(IEntity entity, SubDataFactory subDataFactory)
        {
            Entity = entity;
            SubDataFactory = subDataFactory;
        }

        public dynamic Presentation => _p ?? (_p = SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private IDynamicEntity _p;

        public IMetadata Metadata => _metadata ?? (_metadata = new Metadata(Entity?.Metadata, Entity, SubDataFactory.Cdf));
        private Metadata _metadata;

    }
}
