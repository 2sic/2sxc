using System;
using ToSic.Eav.Apps.Assets;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Adam
{
    // ReSharper disable once InconsistentNaming
    public interface AdamFile: IAdamFile
    {
        //string FileName { get; }

        DateTime CreatedOnDate { get; }

        //int FileId { get; }


    }
}