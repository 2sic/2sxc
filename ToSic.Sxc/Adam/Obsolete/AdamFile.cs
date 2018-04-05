using System;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Adam
{
    // ReSharper disable once InconsistentNaming
    public interface AdamFile
    {
        string FileName { get; }

        DateTime CreatedOnDate { get; }


        int FileId { get; }

    }
}