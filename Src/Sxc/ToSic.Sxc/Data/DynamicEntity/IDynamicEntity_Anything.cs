using System;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace ToSic.Sxc.Data
{
    /* IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack */
    
    public partial interface IDynamicEntity
    {
        /// <inheritdoc cref="IDynamicAnythingDocs.AnyBooleanProperty"/>
        bool AnyBooleanProperty { get; }

        /// <inheritdoc cref="IDynamicAnythingDocs.AnyDateTimeProperty"/>
        DateTime AnyDateTimeProperty { get; }

        /// <inheritdoc cref="IDynamicAnythingDocs.AnyChildrenProperty"/>
        IEnumerable<IDynamicEntity> AnyChildrenProperty { get; }

        // 2023-08-12 2dm - removed this, as we don't officially have the Json Type any more
        //string AnyJsonProperty { get; }

        /// <inheritdoc cref="IDynamicAnythingDocs.AnyLinkOrFileProperty"/>
        string AnyLinkOrFileProperty { get; }

        /// <inheritdoc cref="IDynamicAnythingDocs.AnyNumberProperty"/>
        decimal AnyNumberProperty { get; }

        /// <inheritdoc cref="IDynamicAnythingDocs.AnyStringProperty"/>
        string AnyStringProperty { get; }

        // 2023-08-11 2dm - disable this, believe the instructions were a bit wrong
        ///// <summary>
        ///// If this DynamicEntity carries a list of items (for example a `BlogPost.Tags` which behaves as the first Tag, but also carries all the tags in it)
        ///// Then you can also use DynamicCode to directly navigate to a sub-item, like `Blogs.Tags.WebDesign`. 
        ///// </summary>
        ///// <remarks>New in 12.03</remarks>
        //IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList { get; }
    }
}
