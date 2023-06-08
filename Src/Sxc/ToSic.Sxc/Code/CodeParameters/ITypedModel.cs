using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
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

        object Get(string name, string noParamOrder = Protector, bool? required = default);

        T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool? required = default);

        #endregion

        dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool? required = default);
        string String(string name, string noParamOrder = Protector, string fallback = default, bool? required = default);

        #region Numbers

        int Int(string name, string noParamOrder = Protector, int fallback = default, bool? required = default);
        float Float(string name, string noParamOrder = Protector, float fallback = default, bool? required = default);
        double Double(string name, string noParamOrder = Protector, double fallback = default, bool? required = default);
        decimal Decimal(string name, string noParamOrder = Protector, decimal fallback = default, bool? required = default);
        #endregion

        Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool? required = default);
        bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool? required = default);
        DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool? required = default);
        IFile File(string name, string noParamOrder = Protector, IFile fallback = default, bool? required = default);
        IEnumerable<IFile> Files(string name, string noParamOrder = Protector, IEnumerable<IFile> fallback = default, bool? required = default);
        IFolder Folder(string name, string noParamOrder = Protector, IFolder fallback = default, bool? required = default);
        IEnumerable<IFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<IFolder> fallback = default, bool? required = default);

        #region Item / Entity

        IEntity Entity(string name, string noParamOrder = Protector, IEntity fallback = default, bool? required = default);

        ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool? required = default);
        IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool? required = default);

        #endregion

    }
}