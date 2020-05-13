using System;
using ToSic.Eav.Data;

// Since DynamicEntity... is a wrapper,
// These things ensure that various standalone wrappers are still regarded as equals
// If the underlying entity is the same
namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityWithList : IEquatable<DynamicEntityWithList>
    {
        #region Operators

        public static bool operator ==(DynamicEntityWithList d1, DynamicEntityWithList d2) => OverrideIsEqual(d1, d2);

        public static bool operator !=(DynamicEntityWithList d1, DynamicEntityWithList d2) => !OverrideIsEqual(d1, d2);

        public static bool operator ==(DynamicEntityWithList d1, IDynamicEntity d2) => OverrideIsEqual(d1, d2);

        public static bool operator !=(DynamicEntityWithList d1, IDynamicEntity d2) => !OverrideIsEqual(d1, d2);

        #endregion

        // temp - should be inherited from the parent...?
        // ReSharper disable once RedundantOverriddenMember
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is DynamicEntityWithList listObj)
                return Equals(Entity, listObj.Entity);
            if (obj is IDynamicEntity deObj)
                return Entity == deObj.Entity;
            if (obj is IEntity entObj)
                return Entity == entObj;

            return false;
        }

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two DynamicEntities to be equal when they host the same entity, this uses the Entity.HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc />
        public bool Equals(DynamicEntityWithList dynObj) => base.Equals(dynObj);
    }
}
