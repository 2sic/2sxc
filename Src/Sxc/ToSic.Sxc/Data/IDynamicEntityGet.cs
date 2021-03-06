﻿using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is minor cross-concerns aspect of Dynamic-Entity-like objects
    /// </summary>
    [PrivateApi]
    public interface IDynamicEntityGet
    {

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
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


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <param name="dontRelyOnParameterOrder">
        /// This should enforce the convention that all following parameters (which are optional) must explicitly use the parameter name.
        /// So <code>Get("FirstName", "en")</code> won't work, you must use <code>Get("FirstName", language: "en")</code> and similar
        /// </param>
        /// <param name="language">Optional language code - like "de-ch" to prioritize that language</param>
        /// <param name="convertLinks">Optionally turn off if links like file:72 are looked up to a real link. Default is true.</param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true);

    }
}
