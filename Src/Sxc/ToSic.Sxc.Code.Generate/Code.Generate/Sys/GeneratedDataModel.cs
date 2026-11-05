namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Object which contains the info to generate the class code file.
/// </summary>
/// <param name="BodyPlain">
/// The main code body of the class.
/// It's separate from the intro, to allow optional check if the file changed.
/// </param>
/// <param name="IntroComment">
/// Intro comment to add to the top of the file.
/// It always changes a bit, as it has a time stamp and version number.
/// </param>
internal record GeneratedDataModel(string TypeName, string BodyPlain, string IntroComment) : IGeneratedFile
{
    /// <inheritdoc />
    public string FileName => TypeName + ".cs";

    /// <inheritdoc />
    public string Path => "Data";

    /// <inheritdoc />
    public string Body => IntroComment + BodyPlain;

    /// <inheritdoc />
    public IReadOnlyCollection<IGeneratedFileInfo> Dependencies => [];
}