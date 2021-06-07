using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a wrapper for IEntity objects. It provides nicer access to underlying properties
    /// and automatically handles things like multi-language etc.
    /// The underlying IEntity <see cref="IEntity"/> is in the Entity property. 
    /// <blockquote>
    /// Normally you will use it without caring about these internals. <br/>
    /// Please check @HowTo.DynamicCode.DynamicEntity
    /// </blockquote>
    /// </summary>
    // [PublicApi_Stable_ForUseInYourCode]
    public interface IDynamicEntityGet
    {


        /// <summary>
        /// Get a value of the entity. Usually you will prefer the quick access like
        /// @content.FirstName - which will give you the same things as content.Get("FirstName").
        /// There are two cases to use this:
        /// - when you dynamically assemble the field name in your code, like when using App.Resources or similar use cases.
        /// - to access a field which has a conflicting name with this object, like Get("Parents")
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An object which can be either a string, number, boolean or List&lt;IDynamicEntity&gt;, depending on the field type. Will return null if the field was not found. </returns>
        dynamic Get(string name);


        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <param name="dontRelyOnParameterOrder">
        /// This should enforce the convention that all following parameters (which are optional) must explicitly use the parameter name.
        /// So `Get("FirstName", "en")` won't work, you must use `Get("FirstName", language: "en")` and similar
        /// </param>
        /// <param name="language">Optional language code - like "de-ch" to prioritize that language</param>
        /// <param name="convertLinks">Optionally turn off if links like file:72 are looked up to a real link. Default is true.</param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true);

        

        //#region parents / children

        ///// <summary>
        ///// A dynamic list of entities which point to this item. Important for LINQ style querying or just
        ///// working with various lists. Note that for getting child items of this item you
        ///// can just use the properties, like content.Authors. <br/>
        ///// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        ///// </summary>
        ///// <param name="type">Optional type filter - would only return items of this type. </param>
        ///// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        ///// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        ///// <remarks>New in 9.42 - note also that the parameter-order is reversed to the Children()</remarks>
        //List<IDynamicEntity> Parents(string type = null, string field = null);

        ///// <summary>
        ///// A dynamic list of sub-items. Important for LINQ style querying or just
        ///// working with various lists. Note that for getting child items of this item you
        ///// can just use the properties, like content.Authors. <br/>
        ///// But using Children("Authors", typeName) gives you the ability to restrict to a type.  <br/>
        ///// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        ///// </summary>
        ///// <param name="type">Optional type filter - would only return items of this type. </param>
        ///// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        ///// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        ///// <remarks>New in 10.21.00 - note also that the parameter-order is reversed to the Parents()</remarks>
        //List<IDynamicEntity> Children(string field = null, string type = null);
        //#endregion 
        
    }
}
