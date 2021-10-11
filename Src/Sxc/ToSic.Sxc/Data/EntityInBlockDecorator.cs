using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class EntityInBlockDecorator: IHasEditingData, IDecorator<IEntity>
    {
        public EntityInBlockDecorator(Guid? parentGuid, string field, int index = 0, IEntity presentation = null, bool isDemoItem = false)
        {
            Parent = parentGuid;
            Field = field;
            SortOrder = index;
            Presentation = presentation;
            IsDemoItem = isDemoItem;
        }

        /// <summary>
        /// Sort order in the content-group, because it's often accessed by index
        /// </summary>
        public int SortOrder { get; }

        /// <summary>
        /// Presentation entity of this content-item.
        /// Important to keep content & presentation linked together
        /// </summary>
        public IEntity Presentation { get; set; }

#if NETFRAMEWORK
        /// <summary>
        /// Block ID, because as the group changes, we must be able to find it
        /// </summary>
        public Guid GroupId { get; set; }
#endif 

        /// <summary>
        /// Info if the item is a plain demo/fake item, or if it was added on purpose.
        /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        internal bool IsDemoItem { get; }

        /// <inheritdoc />
        public string Field { get; set; }

        /// <inheritdoc />
        public Guid? Parent { get; set; }
    }
}
