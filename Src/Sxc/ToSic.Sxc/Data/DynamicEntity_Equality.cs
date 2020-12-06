using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

// Since DynamicEntity... is a wrapper,
// These things ensure that various standalone wrappers are still regarded as equals
// If the underlying entity is the same
namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IEquatable<IEntityWrapper>
    {
        [PrivateApi]
        public IEntity EntityForEqualityCheck { get; private set; }

        #region Changing comparison operation to internally compare the entities, not this wrapper

        [PrivateApi]
        public static bool operator ==(DynamicEntity d1, IEntityWrapper d2) => EntityWrapperEquality.IsEqual(d1, d2);

        [PrivateApi]
        public static bool operator !=(DynamicEntity d1, IEntityWrapper d2) => !EntityWrapperEquality.IsEqual(d1, d2);

        [PrivateApi]
        public bool Equals(IEntityWrapper other) => EntityWrapperEquality.EqualsWrapper(this, other);

        /// <inheritdoc />
        [PrivateApi]
        public override bool Equals(object obj) => EntityWrapperEquality.EqualsObj(this, obj);

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two DynamicEntities to be equal when they host the same entity, this uses the Entity.HashCode
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        public override int GetHashCode() => EntityWrapperEquality.GetHashCode(this);

        /// <inheritdoc />
        [PrivateApi]
        public bool Equals(IDynamicEntity dynObj) => EntityWrapperEquality.EqualsWrapper(this, dynObj);

        #endregion
    }
}
