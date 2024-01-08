//using System.Collections.Generic;

//namespace ToSic.Sxc.Adam.Internal;

///// <summary>
///// An ADAM (Automatic Digital Asset Management) folder
///// </summary>
//public interface IFolder<out TFolderId, out TFileId>: Eav.Apps.Assets.IFolder<TFolderId, TFileId>, IAsset
//{
//    /// <summary>
//    /// Get the files in this folder
//    /// </summary>
//    /// <returns>A list of files in this folder as <see cref="IFile"/></returns>
//    IEnumerable<IFile<TFolderId, TFileId>> Files { get; }

//    /// <summary>
//    /// Get the sub-folders in this folder
//    /// </summary>
//    /// <returns>A list of folders inside this folder - as <see cref="IFolder"/>.</returns>
//    IEnumerable<IFolder<TFolderId, TFileId>> Folders { get; }

//}