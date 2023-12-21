namespace ToSic.Sxc.Code.Help
{
    public static class CodeFileInfoExtensions
    {
        /// <summary>
        /// MyAppCode is supported in RazorTyped and newer, and
        /// enabled when "using MyApp.Code" is used
        /// </summary>
        /// <param name="razorType"></param>
        /// <returns></returns>
        public static bool MyAppRequirements(this CodeFileInfo razorType)
            => razorType.MyApp
            && razorType.Type == CodeFileTypes.V16;
    }
}
