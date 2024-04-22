namespace ToSic.Sxc.Dnn.Compile;

internal class DnnRoslynConstants
{
    /// <summary>
    /// The web.config disable warnings which are in the default DNN web.config
    /// </summary>
    public const string DefaultDisableWarnings = "/nowarn:1659;1699;1701;612;618";

    /// <summary>
    /// The default language version to use when compiling
    /// </summary>
    public const string DefaultLangVersion = "8"; // "7.3";

    public const string CompilerOptionLanguageVersion = $"/langversion:{DefaultLangVersion}";
}