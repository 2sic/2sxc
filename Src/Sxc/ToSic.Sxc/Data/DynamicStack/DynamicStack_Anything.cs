using System;
using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack
    {
        /// <inheritdoc />
        public bool AnyBooleanProperty { get; }

        /// <inheritdoc />
        public DateTime AnyDateTimeProperty { get; }

        /// <inheritdoc />
        public IEnumerable<DynamicEntity> AnyChildrenProperty { get; }

        /// <inheritdoc />
        public string AnyJsonProperty { get; }

        /// <inheritdoc />
        public string AnyLinkOrFileProperty { get; }

        /// <inheritdoc />
        public double AnyNumberProperty { get; }

        /// <inheritdoc />
        public string AnyStringProperty { get; }

    }
}
