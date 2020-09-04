using System;

namespace ToSic.Sxc.Blocks
{
    public class ViewParts
    {
        public const string ViewFieldInContentBlock = "Template";
        public const string TemplateContentType = "Template";

        public const string Content = "Content";
        public const string ContentLower = "content";

        public const string Presentation = "Presentation";
        public const string PresentationLower = "presentation";

        public static string[] ContentPair = { Content, Presentation };
        public static string[] HeaderPair = {ListContent, ListPresentation};

        public static string[] PickPair(string primaryField)
        {
            string lowered = primaryField.ToLower();
            if (lowered == ContentLower || lowered == PresentationLower) return ContentPair;
            if (lowered == ListContentLower || lowered == ListPresentationLower) return HeaderPair;
            throw new Exception($"tried to find field pair, but input was '{primaryField}' - can't figure it out.");
        }


        public const string ListContent = "ListContent";
        public const string ListContentLower = "listcontent";

        // todo: not implemented in tokens just yet
        public const string Header = "Header";
        public const string HeaderLower = "header";

        public const string ListPresentation = "ListPresentation";
        public const string ListPresentationLower = "listpresentation";
    }
}
