using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public partial class DynamicEntity : DynamicEntityBase, IDynamicEntity, ICompatibilityLevel
    {
        [PrivateApi]
        public IEntity Entity { get; private set; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, string[] dimensions, int compatibility, IBlock block, IServiceProvider serviceProvider): base(block, serviceProvider, dimensions, compatibility)
        {
            SetEntity(entity);
        }

        [PrivateApi]
        protected void SetEntity(IEntity entity)
        {
            Entity = entity;
            EntityForEqualityCheck = (Entity as IEntityWrapper)?.EntityForEqualityCheck ?? Entity;
        }


        /// <inheritdoc />
        public object EntityTitle => Entity?.Title[Dimensions];


        /// <inheritdoc />
        public bool IsDemoItem => Entity is EntityInBlock entInCg && entInCg.IsDemoItem;


    }
}