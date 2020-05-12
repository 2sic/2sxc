using System;
using ToSic.Eav.Data;

// Since DynamicEntity... is a wrapper,
// These things ensure that various standalone wrappers are still regarded as equals
// If the underlying entity is the same
namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IEquatable<IDynamicEntity>
    {
        #region Changing comparison operation to internally compare the entities, not this wrapper

        public static bool operator ==(DynamicEntity d1, IDynamicEntity d2) => OverrideIsEqual(d1, d2);

        public static bool operator !=(DynamicEntity d1, IDynamicEntity d2) => !OverrideIsEqual(d1, d2);

        /// <summary>
        /// Check if they are equal, based on the underlying entity. 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <remarks>
        /// It's important to do null-checks first, because if anything in here is null, it will otherwise throw an error. 
        /// But we can't use != null, because that would call the != operator and be recursive.
        /// </remarks>
        /// <returns></returns>
        protected static bool OverrideIsEqual(DynamicEntity d1, IDynamicEntity d2)
        {
            // check most basic case - they are really the same object or both null
            if (ReferenceEquals(d1, d2))
                return true;

            return Equals(d1?.Entity, d2?.Entity);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
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
        // ReSharper disable once NonReadonlyMemberInGetHashCode - required so we can set the entity in inherited object
        public override int GetHashCode() => Entity?.GetHashCode() ?? 0;

        /// <inheritdoc />
        public bool Equals(IDynamicEntity dynObj) => Entity == dynObj?.Entity;

        #endregion
    }
}
