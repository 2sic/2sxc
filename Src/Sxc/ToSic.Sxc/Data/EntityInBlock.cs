using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class EntityInBlock : EntityDecorator, IHasEditingData
    {
        public EntityInBlock(IEntity baseEntity, Guid? parentGuid, string field = null, int index = 0) : base(baseEntity)
        {
            Parent = parentGuid;
            Field = field;
            SortOrder = index;
        }


        public static EntityInBlock PlaceHolder(IEntity parent, string field)
            => new EntityInBlock(Build.FakeEntity(parent.AppId), parent.EntityGuid, field, 0);

        /// <summary>
        /// Sort order in the content-group, because it's often accessed by index
        /// </summary>
        public int SortOrder { get; set; }

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

        /// <inheritdoc />
        public string Field { get; set; }

        /// <inheritdoc />
        public Guid? Parent { get; set; }

    }
}