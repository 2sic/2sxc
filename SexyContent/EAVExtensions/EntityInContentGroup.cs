using ToSic.Eav;

namespace ToSic.SexyContent.EAVExtensions
{
    public class EntityInContentGroup : EntityDecorator, IHasEditingData
    {
        public EntityInContentGroup(IEntity baseEntity) : base(baseEntity)
        {
        }

        public int SortOrder { get; set; }
    }
}