﻿namespace ToSic.Sxc.Adam
{
    internal class Classification
    {
        public const string Folder = "folder";
        public const string File = "file";
        public const string Image = "image";
        public const string Document = "document";

        public static string TypeName(string ext)
        {
            switch (ext.ToLowerInvariant())
            {
                case "png":
                case "jpg":
                case "jpgx":
                case "jpeg":
                case "gif":
                case "svg":
                    return Image;
                case "doc":
                case "docx":
                case "txt":
                case "pdf":
                case "xls":
                case "xlsx":
                    return Document;
                default:
                    return File;
            }
        }
    }
}
