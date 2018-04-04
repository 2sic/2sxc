using System;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.EAVExtensions
{
    public class EntityInContentGroup : EntityDecorator, IHasEditingData
    {
        public EntityInContentGroup(Eav.Interfaces.IEntity baseEntity) : base(baseEntity)
        {
        }

        public int SortOrder { get; set; }
        public DateTime ContentGroupItemModified { get; set; }
		public Eav.Interfaces.IEntity Presentation { get; set; }
		public Guid GroupId { get; set; }
    }
}