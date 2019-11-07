using System;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Apps.Assets;
using IFile = ToSic.Eav.Apps.Adam.IFile;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Adam
{
    // ReSharper disable once InconsistentNaming
    public interface AdamFile: IFile
    {
        //string FileName { get; }

        DateTime CreatedOnDate { get; }

        //int FileId { get; }


    }
}