namespace ToSic.Sxc.Code.Help
{
    public static class CodeFileInfoExtensions
    {
        /// <summary>
        /// ThisAppCode is supported in RazorTyped and newer, and
        /// enabled when "using ThisApp.Code" is used
        /// </summary>
        /// <param name="razorType"></param>
        /// <returns></returns>
        public static bool ThisAppRequirements(this CodeFileInfo razorType)
            => razorType.ThisApp
            && razorType.Type == CodeFileTypes.V16;
    }
}
