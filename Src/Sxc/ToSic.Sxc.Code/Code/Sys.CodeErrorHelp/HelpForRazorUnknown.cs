using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

/// <summary>
/// Help for compile errors before the Razor base class is known.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class HelpForRazorCompileErrors
{
    /// <summary>
    /// Help when compiling with a namespace which is not known
    /// </summary>
    internal static CodeHelp UnknownNamespace = new()
    {
        Name = "unknown-ns",
        Detect = "error CS0234: The type or namespace name",
        UiMessage = """
                    
                    Your code seems to have an invalid namespace - like as a '@using xxx' or '@inherits xxx'. Check and fix your code.

                    """,
        DetailsHtml = """

                      Your code seems to have an invalid namespace - like as a <code>@using xxx</code> or <code>@inherits xxx</code>. Check and fix your code.

                      """
    };


    /// <summary>
    /// Help when the new Roslyn compiler runs into a conversion problem because of a semicolon at the end of the inherits-statement
    /// </summary>
    internal static CodeHelp ProbablySemicolonAfterInherits = new()
    {
        Name = "inherits-breaks-with-semicolon",
        // full message is like "error CS1003: Syntax error, ',' expected at System.Web.Compilation.AssemblyBuilder.Compile()"
        // but only a part of it is in the initial exception, so we only check for the part that is there
        Detect = "Syntax error, ',' expected",
        UiMessage = """

                    Your Razor code probably has a semicolon ';' in the wrong place, which breaks the new Roslyn Razor Compiler. Check and fix your code.

                    """,
        DetailsHtml = """

                      The new Roslyn compiler incorrectly handles <code>@inherits</code> with a trailing semicolon. Remove the semicolon and it should work. 
                      <br>
                      <strong>Example</strong>: <br>
                      <code>@inherits Custom.Hybrid.RazorTyped;</code> <br>
                      should be <br>
                      <code>@inherits Custom.Hybrid.RazorTyped</code>

                      """
    };

    /// <summary>
    /// Help when the new Roslyn compiler runs into a conversion problem because of a comment at the end of the inherits-statement
    /// </summary>
    internal static CodeHelp 
        ProbablyCommentAfterInherits = new()
        {
            Name = "inherits-breaks-with-comment",
            // full message is like "Error: { expected ..., Error: } expected ..., Error: Type or namespace definition, or end-of-file expected"
            // but only a part of it is in the initial exception, so we only check for the part that is there
            Detect = @"Error: { expected",
            UiMessage = """

                        Your Razor code probably has a comments '//' in the wrong place, which breaks the new Roslyn Razor Compiler. Check and fix your code.

                        """,
            DetailsHtml = """

                          The new Roslyn compiler incorrectly handles <code>@inherits</code> with a trailing comments. Remove the comments and it should work. 
                          <br>
                          <strong>Example</strong>: <br>
                          <code>@inherits Custom.Hybrid.RazorTyped // comment</code> <br>
                          should be <br>
                          <code>@inherits Custom.Hybrid.RazorTyped</code>

                          """
        };

    // v20 CustomizeData() removed, so this is no longer relevant
    internal static CodeHelp CustomizeDataRemoved = new()
    {
        Name = nameof(CustomizeDataRemoved),
        Detect = "CustomizeData()': no suitable method found to override",
        UiMessage = """

                    CustomizeData(...) has been removed in v20.

                    """,
        DetailsHtml = """

                      <code>CustomizeData(...)</code> is an old API which has been removed in v20. It was used to customize the data passed to Razor templates, but now this is done differently.

                      """,
        LinkCode = "brc-20-customizedata",
    };

    // v20 CustomizeSearch() removed, so this is no longer relevant
    internal static CodeHelp CustomizeSearchRemoved = new()
    {
        Name = nameof(CustomizeSearchRemoved),
        Detect = "CustomizeSearch(Dictionary", // just the extract, since there are multiple overloads
        UiMessage = """

                    CustomizeSearch(...) has been removed in v20.

                    """,
        DetailsHtml = """

                      <code>CustomizeSearch(...)</code> is an old API which has been removed in v20. It was used to customize the data passed to Razor templates, but now this is done differently.

                      """,
        LinkCode = "brc-20-customizedata",
    };

    // v20 CustomizeSearch() removed, so this is no longer relevant
    internal static CodeHelp CustomizeSearchRemovedISearchItemDetection = new()
    {
        Name = nameof(CustomizeSearchRemovedISearchItemDetection),
        Detect = "The type or namespace name 'ISearchItem' could not be found",
        UiMessage = """

                    ISearchItem should not be used in v20.

                    """,
        DetailsHtml = """

                      <code>ISearchItem(...)</code> should not be used in Razor any more. It was used to customize data for the search indexer, now this is done differently.

                      """,
        LinkCode = "brc-20-customizedata",
    };

    /// <summary>
    /// Factory to generate CodeHelp for missing methods after v20 auto-inherits removal
    /// </summary>
    internal static CodeHelp MissingAfterAutoInherits(string methodName) => new()
    {
        Name = $"{methodName.ToLowerInvariant()}-missing-after-auto-inherits-removed",
        Detect = $"error CS0103: The name '{methodName}' does not exist in the current context",
        UiMessage = $"""

                    Your Razor code is using '{methodName}', but the base class is missing. In v20, automatic @inherits was removed. Add the correct @inherits statement to your .cshtml file.

                    """,
        DetailsHtml = $"""

                      Your Razor template uses <code>{methodName}</code>, but the required base class is missing. In v20, automatic <code>@inherits</code> was removed for security and clarity. You must now add the correct <code>@inherits</code> statement at the top of your .cshtml file.
                      <br>
                      <strong>Example</strong>: <br>
                      <code>@inherits ToSic.SexyContent.Razor.SexyContentWebPage</code>
                      <br>
                      See the docs for more info.

                      """,
        LinkCode = "brc-20-stop-auto-inherits",
    };

    /// <summary>
    /// Help for missing AsDynamic after v20 auto-inherits removal
    /// </summary>
    internal static CodeHelp AsDynamicMissingAfterAutoInheritsRemoved = MissingAfterAutoInherits("AsDynamic");

    /// <summary>
    /// Help for missing CreateInstance after v20 auto-inherits removal
    /// </summary>
    internal static CodeHelp CreateInstanceMissingAfterAutoInheritsRemoved = MissingAfterAutoInherits("CreateInstance");

    /// <summary>
    /// Help for missing Link after v20 auto-inherits removal
    /// </summary>
    internal static CodeHelp LinkMissingAfterAutoInheritsRemoved = MissingAfterAutoInherits("Link");

    /// <summary>
    /// Help for missing Edit after v20 auto-inherits removal
    /// </summary>
    internal static CodeHelp EditMissingAfterAutoInheritsRemoved = MissingAfterAutoInherits("Edit");

    /// <summary>
    /// Help for missing 'Module' property on DnnHelper after v20 changes
    /// </summary>
    internal static CodeHelp DnnHelperModuleMissing = new()
    {
        Name = "dnnhelper-module-missing-after-v20",
        Detect = "error CS1061: 'DnnHelper<dynamic>' does not contain a definition for 'Module'",
        UiMessage = """

                    Your Razor code is trying to use 'Module', but this property is not available. In v20, APIs and base classes changed. Make sure your template inherits from the correct base class and check the docs for updated usage.

                    """,
        DetailsHtml = """

                      The <code>Module</code> property is not available on <code>DnnHelper&lt;dynamic&gt;</code> in v20. This is likely because your template is missing the correct <code>@inherits</code> statement or is using an outdated API. 
                      <br>
                      <strong>Solution:</strong> <br>
                      Ensure your Razor file has the correct <code>@inherits</code> statement for v20. See the docs for more info.
                      <br>
                      <code>@inherits ToSic.SexyContent.Razor.SexyContentWebPage</code>
                      <br>
                      See the docs for more info.

                      """,
        LinkCode = "brc-20-stop-auto-inherits",
    };

    /// <summary>
    /// Help for Razor file not deriving from RazorComponentBase after v20 changes
    /// </summary>
    //internal static CodeHelp MustDeriveFromRazorComponentBase = new()
    //{
    //    Name = "must-derive-from-razorcomponentbase",
    //    Detect = "must derive from RazorComponentBase",
    //    UiMessage = """

    //                Your Razor file must inherit from the correct base class. In v20, Razor files must explicitly use @inherits with the right base class. Add the correct @inherits statement to your .cshtml file.

    //                """,
    //    DetailsHtml = """

    //                  The error means your Razor file does not inherit from <code>RazorComponentBase</code> or another required base class. In v20, automatic <code>@inherits</code> was removed for security and clarity. You must now add the correct <code>@inherits</code> statement at the top of your .cshtml file.
    //                  <br>
    //                  <strong>Example</strong>: <br>
    //                  <code>@inherits ToSic.SexyContent.Razor.SexyContentWebPage</code>
    //                  <br>
    //                  See the docs for more info.

    //                  """,
    //    LinkCode = "brc-20-stop-auto-inherits",
    //};

    //    internal static CodeHelp RazorBaseClassDoesNotInheritCorrectly = new()
//    {
//        Name = "Razor Base Class doesn't inherit correctly",
//        Detect = "no suitable method found to override",
//        UiMessage = @"
//Check for custom AppCode.Razor.SomeRazor that forgets to inherit 'Custom.Hybrid.RazorTyped' or similar.
//",
//        DetailsHtml = @"
//Your razor template cshtml seems to inherit <code>AppCode.Razor.SomeRazor</code> from AppCode, while AppCode.Razor.SomeRazor doesn't inherit correctly. Check and fix your code.
//<br>
//<strong>Example</strong>: <br>
//<code>public abstract class SomeRazor</code> <br>
//should be <br>
//<code>public abstract class SomeRazor : Custom.Hybrid.RazorTyped</code> or similar.<br>
//"
//    };

    // New v20 - removal of #RemovedV20 #Element
    private static readonly CodeHelp ListObjectNotFound = new()
    {
        Name = "List not available on SexyContentWebPage, resulting in c# thinking it could want to access List<T>",
        Detect = "error CS0305: Using the generic type 'List<T>' requires 1 type arguments",
        LinkCode = "brc-20-list-element",
        UiMessage = "The old List (of Element) object had to be removed."
    };

    // New v20 - removal of #RemovedV20 #Element
    internal static readonly CodeHelp BlocksIRenderServiceRemoved = new()
    {
        Name = "The Blocks.IRenderService was removed",
        Detect = "error CS0234: The type or namespace name 'IRenderService' does not exist in the namespace 'ToSic.Sxc.Blocks'",
        LinkCode = "brc-20-blocks-irenderservice",
        UiMessage = "The old List ToSic.Sxc.Blocks.IRenderService has been replaced/renamed with ToSic.Sxc.Services.IRenderService."
    };

    internal static List<CodeHelp> RemovedApisInV20ForAllRazorClasses =
    [
        // New v20
        CustomizeDataRemoved,
        CustomizeSearchRemoved,
        CustomizeSearchRemovedISearchItemDetection,
        // old, disabled
        //RazorBaseClassDoesNotInheritCorrectly,
    ];

    internal static List<CodeHelp> CompileUnknown =
    [
        UnknownNamespace,
        ProbablySemicolonAfterInherits,
        ProbablyCommentAfterInherits,
        ..RemovedApisInV20ForAllRazorClasses,
        // also v20
        ListObjectNotFound, // v20 - removal of #RemovedV20 #Element
        AsDynamicMissingAfterAutoInheritsRemoved, // new help for missing AsDynamic after auto-inherits removed
        CreateInstanceMissingAfterAutoInheritsRemoved, // new help for missing CreateInstance after auto-inherits removed
        LinkMissingAfterAutoInheritsRemoved, // new help for missing Link after auto-inherits removed
        EditMissingAfterAutoInheritsRemoved, // new help for missing Edit after auto-inherits removed
        DnnHelperModuleMissing, // new help for missing Dnn.Module after v20
        BlocksIRenderServiceRemoved,
        //MustDeriveFromRazorComponentBase, // new help for must derive from RazorComponentBase
        HelpForRazor12.GetBestValueGone,
    ];

}