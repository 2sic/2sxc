using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class EntityInBlockDecorator: EntityInListDecorator
    {
        public EntityInBlockDecorator(Guid? parentGuid, string field, int index = DefIndex, IEntity presentation = DefPresentation, bool isDemoItem = DefDemo)
            :base(parentGuid, field, index)
        {
            Presentation = presentation;
            IsDemoItem = isDemoItem;
        }

        public static EntityDecorator12<EntityInBlockDecorator> Wrap(IEntity entity, Guid? parentGuid, string field,
            int index = DefIndex, IEntity presentation = DefPresentation, bool isDemoItem = DefDemo)
        {
            return new EntityDecorator12<EntityInBlockDecorator>(entity,
                new EntityInBlockDecorator(parentGuid, field, index, presentation, isDemoItem));
        }

        protected const IEntity DefPresentation = null;
        protected const bool DefDemo = false;


        /// <summary>
        /// Presentation entity of this content-item.
        /// Important to keep content & presentation linked together
        /// </summary>
        public IEntity Presentation { get; set; }

        // 2021-10-12 2dm #dropGroupId - believe this is never used anywhere. Leave comment till EOY 2021
        //#if NETFRAMEWORK
        //        /// <summary>
        //        /// Block ID, because as the group changes, we must be able to find it
        //        /// </summary>
        //        public Guid GroupId { get; set; }
        //#endif 

        /// <summary>
        /// Info if the item is a plain demo/fake item, or if it was added on purpose.
        /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        internal bool IsDemoItem { get; }
    }
}
