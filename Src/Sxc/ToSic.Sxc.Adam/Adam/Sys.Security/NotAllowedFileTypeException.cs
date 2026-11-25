namespace ToSic.Sxc.Adam.Sys.Security;

public class NotAllowedFileTypeException(string filename, string? message = null)
    : Exception($"file {filename} has an unsupported file type. {message}");
