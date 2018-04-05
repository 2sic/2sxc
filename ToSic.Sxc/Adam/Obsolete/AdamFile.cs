using System;
using ToSic.Sxc.Adam;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Adam
{
    // ReSharper disable once InconsistentNaming
    public interface AdamFile: IFile
    {
        string FileName { get; }

        DateTime CreatedOnDate { get; }

        int FileId { get; }


    }
}