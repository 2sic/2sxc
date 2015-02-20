using System;
using ToSic.Eav;

namespace ToSic.SexyContent.EAVExtensions
{
    public class EntityInContentGroup : EntityDecorator, IHasEditingData
    {
        public EntityInContentGroup(IEntity baseEntity) : base(baseEntity)
        {
        }

        public int SortOrder { get; set; }
        public DateTime ContentGroupItemModified { get; set; }
		public IEntity Presentation { get; set; }
    }
}