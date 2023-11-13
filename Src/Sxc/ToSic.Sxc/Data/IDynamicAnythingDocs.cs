using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace ToSic.Sxc.Data
{
    /* IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack */

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IDynamicAnythingDocs
    {

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.IsFemale` or whatever other property your item has.
        /// If it's a true/false, it will just magically work and return a `bool`. If it doesn't exist, it will return null. 
        /// </summary>
        bool AnyBooleanProperty { get; }

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.Birthday` or whatever other property your item has.
        /// If it's a date/time, it will just magically work and return a `DateTime`. If it doesn't exist, it will return null. 
        /// </summary>
        DateTime AnyDateTimeProperty { get; }

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.Tags` or whatever other property your item has.
        /// If it's contains relationships, it will just magically work and return a list of further `DynamicEntity` objects.
        /// If it doesn't exist, it will return null. 
        /// </summary>
        /// <remarks>
        /// Very often you'll want to use LINQ to further sort or query these items.
        /// But the Razor compiler cannot know that it got a list, so using `.Any()` or similar fails.
        /// To fix this, put an `AsList` around it - a bit like `AsList(myThing.Tags)`.
        /// Sometimes you'll also need to help a bit more with `AsList(myThings.Tags as object)`.
        /// Now you can do things like `var tags = AsList(myThings.Tags as object); if (myTags.Any()) {...}`
        /// Read more about this in the [Dnn LINQ Tutorials](https://2sxc.org/dnn-tutorials/en/razor/linq/home)
        /// </remarks>
        IEnumerable<IDynamicEntity> AnyChildrenProperty { get; }

        // 2023-08-12 2dm - removed this, as we don't officially have the Json Type any more
        ///// <summary>
        ///// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        ///// Since the object is dynamic, you can just use `.Gps` or whatever other property your item has.
        ///// If the field contains JSON, it will just magically work and return a `string`.
        ///// If it doesn't exist, it will return null. 
        ///// </summary>
        ///// <remarks>
        ///// Very often you'll want to use the Json as a dynamic object again.
        ///// Just pass the result through `AsDynamic` and it will work.
        ///// Example: `var gps = AsDynamic(myThing.Gps); var lat = gps.Lat;`
        ///// Read more about this in the [Dnn JSON Tutorials](https://2sxc.org/dnn-tutorials/en/razor/json/home)
        ///// </remarks>
        //string AnyJsonProperty { get; }

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.Image` or whatever other property your item has.
        /// If it's a link, it will just magically work and return a `string`. If it doesn't exist, it will return null. 
        /// </summary>
        /// <remarks>
        /// Note that many internal references in the CMS use `file:2742` or similar. This will automatically be resolved to the real
        /// link which your output needs. 
        /// </remarks>
        string AnyLinkOrFileProperty { get; }

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.Length` or whatever other property your item has.
        /// If it's a number, it will just magically work and return a `double`. If it doesn't exist, it will return null. 
        /// </summary>
        decimal AnyNumberProperty { get; }

        /// <summary>
        /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
        /// Since the object is dynamic, you can just use `.FirstName` or whatever other property your item has.
        /// If it's a string, it will just magically work. If it doesn't exist, it will return null. 
        /// </summary>
        /// <remarks>
        /// Remember to use `@Html.Raw(...)` if you want the html to be preserved and not cleaned when placed in the page. 
        /// </remarks>
        string AnyStringProperty { get; }

    }
}
