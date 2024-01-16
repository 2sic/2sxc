namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ViewParts
{
    public const string ViewFieldInContentBlock = "Template";
    public const string TemplateContentType = "Template";

    public const string Content = "Content";
    public const string ContentLower = "content";

    public const string Presentation = "Presentation";
    public const string PresentationLower = "presentation";



    private const string ListContent = "ListContent";
    public const string ListContentLower = "listcontent";
    public static readonly string FieldHeader = ListContent;

    // todo: not implemented in tokens just yet
    public const string Header = "Header";
    public const string HeaderLower = "header";

    public const string ListPresentation = "ListPresentation";
    public const string ListPresentationLower = "listpresentation";
    public static readonly string FieldHeaderPresentation = "ListPresentation";

    // Stream Names
    public static string StreamHeader = Header;
    public static string StreamHeaderOld = ListContent;
        

    #region Field Pairs for saving / loading

    public static string[] ContentPair = [Content, Presentation];
    public static string[] HeaderPair = [FieldHeader, FieldHeaderPresentation];

    public static string[] PickFieldPair(string primaryField)
    {
        var lowered = primaryField.ToLowerInvariant();
        switch (lowered)
        {
            case ContentLower:
            case PresentationLower:
                return ContentPair;
            case ListContentLower:
            case ListPresentationLower:
            case HeaderLower:   // new in 11.13, trying to move away from ListContent naming
                return HeaderPair;
            default:
                throw new($"tried to find field pair, but input was '{primaryField}' - can't figure it out.");
        }
    }


    #endregion

}