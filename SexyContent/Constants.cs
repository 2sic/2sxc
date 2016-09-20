namespace ToSic.SexyContent
{
    internal class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        public static string[] ExcludeFolders =
        {
            ".git",
            "node_modules",
            "bower_components",
            ".vs",
            ".data"
        };

    }
}