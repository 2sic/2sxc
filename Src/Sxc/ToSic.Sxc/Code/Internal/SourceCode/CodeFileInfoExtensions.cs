namespace ToSic.Sxc.Code.Internal.SourceCode;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class CodeFileInfoExtensions
{
    /// <summary>
    /// AppCode is supported in RazorTyped and newer, and
    /// enabled when "using AppCode" is used
    /// </summary>
    /// <param name="razorType"></param>
    /// <returns></returns>
    public static bool IsHotBuildSupported(this CodeFileInfo razorType)
        => razorType.AppCode
           && razorType.Type == CodeFileTypes.V16;
}