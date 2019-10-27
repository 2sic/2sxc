using System;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.EAVExtensions
{
    public class EntityInContentGroup : EntityDecorator, IHasEditingData
    {
        public EntityInContentGroup(Eav.Interfaces.IEntity baseEntity) : base(baseEntity)
        {
        }

        /// <summary>
        /// Sort order in the content-group, because it's often accessed by index
        /// </summary>
        public int SortOrder { get; set; }

        public DateTime ContentGroupItemModified { get; set; }

        /// <summary>
        /// Presentation entity of this content-item.
        /// Important to keep content & presentation linked together
        /// </summary>
		public Eav.Interfaces.IEntity Presentation { get; set; }

        /// <summary>
        /// ContentGroup ID, because as the group changes, we must be able to find it
        /// </summary>
		public Guid GroupId { get; set; }

        /// <summary>
        /// Info if the item is a plain demo/fake item, or if it was added on purpose.
        /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        internal bool IsDemoItem { get; set; }
    }
}