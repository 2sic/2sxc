namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Object which contains the info to generate the class code file.
/// </summary>
internal class CodeFileRaw(string typeName, string body, string introComment): ICodeFile
{
    /// <inheritdoc />
    public string FileName { get; } = typeName + ".cs";

    /// <inheritdoc />
    public string Path { get; set; } = "Data";

    /// <summary>
    /// The main code body of the class.
    /// It's separate from the intro, to allow optional check if the file changed.
    /// </summary>
    public string BodyWithoutIntro { get; } = body;

    /// <summary>
    /// Intro comment to add to the top of the file.
    /// It always changes a bit, as it has a time stamp and version number.
    /// </summary>
    public string IntroComment { get; } = introComment;

    /// <inheritdoc />
    public string Body => IntroComment + BodyWithoutIntro;
}