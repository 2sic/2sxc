using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Data
{
    // todo: finish change to new IEntity Decorator method
    // 2021-10-11 2dm started it, mostly done, but would have to go through all code and change
    // final accessors
    // Afterwards we would drop this class completely
    [PrivateApi]
    public class EntityInBlock : EntityDecorator12<EntityInBlockDecorator>, IHasEditingData
    {
        public EntityInBlock(IEntity baseEntity, Guid? parentGuid, string field = null, int index = 0, IEntity presentation = null, bool isDemoItem = false) 
            : base(baseEntity, new EntityInBlockDecorator(parentGuid, field, index, presentation, isDemoItem))
        {
        }

        /// <summary>
        /// Sort order in the content-group, because it's often accessed by index
        /// </summary>
        public int SortOrder => Decorator.SortOrder;

        /// <summary>
        /// Presentation entity of this content-item.
        /// Important to keep content & presentation linked together
        /// </summary>
        public IEntity Presentation => Decorator.Presentation;

#if NETFRAMEWORK
        // this is only used in the old element-list, not in use any more

        /// <summary>
        /// Block ID, because as the group changes, we must be able to find it
        /// </summary>
        public Guid GroupId { get; set; }
#endif

        ///// <summary>
        ///// Info if the item is a plain demo/fake item, or if it was added on purpose.
        ///// new 2019-09-18 trying to mark demo-items for better detection in output #1792
        ///// </summary>
        //internal bool IsDemoItem => Decorator.IsDemoItem;

        /// <inheritdoc />
        public string Field => Decorator.Field;

        /// <inheritdoc />
        public Guid? Parent => Decorator.Parent;

    }
}