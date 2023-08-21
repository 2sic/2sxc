using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Object in partial Razor files to access parameters handed in.
    /// Example caller:
    ///
    /// ```c#
    /// @Html.Partial(someFile, new { blogPost, file = mainFile, title = "hello" }
    /// ```
    ///
    /// Example partial:
    ///
    /// ```c#
    /// var blogPost = MyModel.Item("BlogPost");
    /// var file = MyModel.File("File");
    /// var title = MyModel.String("Title");
    /// ```
    ///
    /// > [!TIP]
    /// > The common data types such as `string` or <see cref="ITypedItem"/> have methods to quickly get them in the desired type.
    /// > This allows things such as `var message = MyModel.String("Message");`
    /// > For less common types you'll need to use <see cref="Get"/> and cast it as needed, like this:
    /// > `string message = MyModel.Get("Message");`.
    /// </summary>
    /// <remarks>Introduced in v16.02</remarks>
    [PublicApi]
    public interface ITypedModel: IHasKeys
    {
        #region Check if parameters were supplied

        /// <inheritdoc cref="IHasKeys.ContainsKey"/>
        new bool ContainsKey(string name);


        /// <inheritdoc cref="IHasKeys.Keys"/>
        new IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);
        
        #endregion

        #region Get

        /// <summary>
        /// Will get the value and return as object, since the type isn't known. 
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
        /// <returns>Object if found, `null` if not found.</returns>
        object Get(string name, string noParamOrder = Protector, bool? required = default);

        /// <summary>
        /// Will get the value and return as type T as specified.
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The fallback value. If provided, the type is automatically determined.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is provided which is not the `default`.
        /// So eg. `Get&lt;string&gt;(..., fallback: false)` can't be detected, but `..., fallback: "hello"` can.
        /// </param>
        /// <returns>Object of type T if found, `null` if not found.</returns>
        T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool? required = default);

        #endregion

        // 2023-08-17 2dm removed again in 16.03 - don't think this should ever be used
        //dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found.</returns>
        string String(string name, string noParamOrder = Protector, string fallback = default, bool? required = default);

        #region Numbers

        int Int(string name, string noParamOrder = Protector, int fallback = default, bool? required = default);
        float Float(string name, string noParamOrder = Protector, float fallback = default, bool? required = default);
        double Double(string name, string noParamOrder = Protector, double fallback = default, bool? required = default);
        decimal Decimal(string name, string noParamOrder = Protector, decimal fallback = default, bool? required = default);
        #endregion

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not the empty-guid.
        /// </param>
        /// <returns>typed result if found, empty-guid if not found.</returns>
        Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is `true`.
        /// </param>
        /// <returns>typed result if found, false if not found.</returns>
        bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not the default-date.
        /// </param>
        /// <returns>typed result if found, default-date if not found.</returns>
        DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of files, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found.</returns>
        IFile File(string name, string noParamOrder = Protector, IFile fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single file, will return a list containing that file.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<IFile> Files(string name, string noParamOrder = Protector, IEnumerable<IFile> fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of folders, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found.</returns>
        IFolder Folder(string name, string noParamOrder = Protector, IFolder fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single folder, will return a list containing that folder.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<IFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<IFolder> fallback = default, bool? required = default);

        //#region Stacks

        ///// <summary>
        ///// Get a stack which was passed to this
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="noParamOrder"></param>
        ///// <param name="fallback"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //ITypedStack Stack(string name, string noParamOrder = Protector, ITypedStack fallback = default, bool? required = default);

        //#endregion

        #region Item / Entity

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of items, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found.</returns>
        ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single item, will return a list containing that item.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value being a toolbar as specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found</returns>
        IToolbarBuilder Toolbar(string name, string noParamOrder = Protector, IToolbarBuilder fallback = default, bool? required = default);

        #endregion

        #region HtmlTags

        /// <summary>
        /// Will get the value being an `IHtmlTag` as specified (RazorBlade objects)
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found</returns>
        IHtmlTag HtmlTag(string name, string noParamOrder = Protector, IHtmlTag fallback = default,
            bool? required = default);

        /// <summary>
        /// Will get the value being an list (IEnumerable) of `IHtmlTag` as specified (RazorBlade objects)
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">
        /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
        /// It is automatically `false` if a `fallback` is not `null`.
        /// </param>
        /// <returns>typed result if found, `null` if not found</returns>
        IEnumerable<IHtmlTag> HtmlTags(string name, string noParamOrder = Protector,
            IEnumerable<IHtmlTag> fallback = default, bool? required = default);

        #endregion
    }
}