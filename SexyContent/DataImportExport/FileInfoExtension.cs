using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public static class FileInfoExtension
    {
        public static void WriteStream(this FileInfo fileInfo, Stream fileContent)
        {
            using (var fileWriter = fileInfo.OpenWrite())
            {
                fileContent.CopyTo(fileWriter);
                fileContent.Position = 0;
            }
        }

        public static Stream ReadStream(this FileInfo fileInfo)
        {
            var fileContent = new MemoryStream();
            using (var fileReader = fileInfo.OpenRead())
            {
                fileReader.CopyTo(fileContent);
                fileContent.Position = 0;
            }
            return fileContent;
        }
    }
}