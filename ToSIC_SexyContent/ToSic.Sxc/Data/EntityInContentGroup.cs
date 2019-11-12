using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.SexyContent.EAVExtensions;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Data
{
    [PrivateApi("final name not good yet, in future probably not called content-group")]
    public class EntityInContentGroup : EntityDecorator, IHasEditingData
    {
        public EntityInContentGroup(IEntity baseEntity) : base(baseEntity)
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
		public IEntity Presentation { get; set; }

        /// <summary>
        /// Block ID, because as the group changes, we must be able to find it
        /// </summary>
		public Guid GroupId { get; set; }

        /// <summary>
        /// Info if the item is a plain demo/fake item, or if it was added on purpose.
        /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        internal bool IsDemoItem { get; set; }
    }
}