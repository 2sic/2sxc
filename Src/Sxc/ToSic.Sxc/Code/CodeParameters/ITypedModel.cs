using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
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
    /// var blogPost = TypedModel.Item("BlogPost");
    /// var file = TypedModel.File("File");
    /// var title = TypedModel.String("Title");
    /// ```
    /// </summary>
    [WorkInProgressApi("WIP v16.02")]
    public interface ITypedModel
    {
        #region Check if parameters were supplied

        /// <summary>
        /// Ensure that all the specified parameter names were provided.
        /// Note that it only checks if it is provided, so null still counts. The type is also not checked.
        /// Use:
        /// 
        /// * `if (!TypedModel.All(name1)) { ... }`
        /// * `if (!TypedModel.All(name1, name2, name3)) { ... }`
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool HasAll(params string[] names);

        /// <summary>
        /// Ensure that any the specified parameter names were provided.
        /// Note that it only checks if it is provided, so null still counts. The type is also not checked.
        /// Use:
        /// 
        /// * `if (!TypedModel.All(name1)) { ... }`
        /// * `if (!TypedModel.Any(name1, name2, name3)) { ... }`
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool HasAny(params string[] names);

        /// <summary>
        /// Throw an exception if _none_ of the supplied names were provided as parameters.
        /// </summary>
        /// <param name="names"></param>
        /// <returns>a `null` string, just so it could be used in razor directly without a code block</returns>
        string RequireAny(params string[] names);

        /// <summary>
        /// Throw an exception if _not all_ of the supplied names were provided as parameters.
        /// </summary>
        /// <param name="names"></param>
        /// <returns>a `null` string, just so it could be used in razor directly without a code block</returns>
        string RequireAll(params string[] names);
        #endregion

        #region Get

        /// <summary>
        /// Will get the value and return as object, since the type isn't known. 
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="required">If required, throw error if not found. Default is required.</param>
        /// <returns>Object if found, `null` if not found.</returns>
        object Get(string name, string noParamOrder = Protector, bool? required = default);

        /// <summary>
        /// Will get the value and return as type T as specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The fallback value. If provided, the type is automatically determined.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is provided which is non-`default`. So a bool fallback `false` can't be detected, but `true` can.</param>
        /// <returns>Object of type T if found, `null` if not found.</returns>
        T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool? required = default);

        #endregion

        dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
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
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not the empty-guid.</param>
        /// <returns>typed result if found, empty-guid if not found.</returns>
        Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is `true`.</param>
        /// <returns>typed result if found, false if not found.</returns>
        bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not the default-date.</param>
        /// <returns>typed result if found, default-date if not found.</returns>
        DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of files, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, `null` if not found.</returns>
        IFile File(string name, string noParamOrder = Protector, IFile fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single file, will return a list containing that file.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<IFile> Files(string name, string noParamOrder = Protector, IEnumerable<IFile> fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of folders, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, `null` if not found.</returns>
        IFolder Folder(string name, string noParamOrder = Protector, IFolder fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single folder, will return a list containing that folder.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<IFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<IFolder> fallback = default, bool? required = default);

        #region Item / Entity

        IEntity Entity(string name, string noParamOrder = Protector, IEntity fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a list of items, then this will only return the first one.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, `null` if not found.</returns>
        ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool? required = default);

        /// <summary>
        /// Will get the value if specified.
        /// If the value is a single item, will return a list containing that item.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, empty-list if not found.</returns>
        IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool? required = default);


        /// <summary>
        /// Will get the value being a toolbar as specified.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">The optional fallback value.</param>
        /// <param name="required">If required (default), throw error if not found. If automatically `false` if a `fallback` is not `null`.</param>
        /// <returns>typed result if found, `null` if not found</returns>
        IToolbarBuilder Toolbar(string name, string noParamOrder = Protector, IToolbarBuilder fallback = default, bool? required = default);

        #endregion

    }
}