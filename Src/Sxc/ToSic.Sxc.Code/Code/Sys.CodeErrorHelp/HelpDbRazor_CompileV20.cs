using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

/// <summary>
/// Help for compile errors before the Razor base class is known.
/// </summary>
partial class HelpDbRazor
{



    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> CompileRemovedApisInV20ForAllRazorClasses => field ??=
    [
        // New v20
        
        // v20 CustomizeData() removed, so this is no longer relevant
        new()
        {
            Name = "CustomizeDataRemoved",
            Detect = "CustomizeData()': no suitable method found to override",
            UiMessage = """

                        CustomizeData(...) has been removed in v20.

                        """,
            DetailsHtml = """

                          <code>CustomizeData(...)</code> is an old API which has been removed in v20. It was used to customize the data passed to Razor templates, but now this is done differently.

                          """,
            LinkCode = "brc-20-customizedata",
        },

        // v20 CustomizeSearch() removed, so this is no longer relevant
        new()
        {
            Name = "CustomizeSearchRemoved",
            Detect = "CustomizeSearch(Dictionary", // just the extract, since there are multiple overloads
            UiMessage = """

                        CustomizeSearch(...) has been removed in v20.

                        """,
            DetailsHtml = """

                          <code>CustomizeSearch(...)</code> is an old API which has been removed in v20. It was used to customize the data passed to Razor templates, but now this is done differently.

                          """,
            LinkCode = "brc-20-customizedata",
        },
        
        // v20 CustomizeSearch() removed, so this is no longer relevant
        new()
        {
            Name = "CustomizeSearchRemovedISearchItemDetection",
            Detect = "The type or namespace name 'ISearchItem' could not be found",
            UiMessage = """

                        ISearchItem should not be used in v20.

                        """,
            DetailsHtml = """

                          <code>ISearchItem(...)</code> should not be used in Razor any more. It was used to customize data for the search indexer, now this is done differently.

                          """,
            LinkCode = "brc-20-customizedata",
        },
    ];



    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> CompilerErrorsWhenOldAutomaticWebConfigIsMissing => field ??=
    [

        // Help for missing AsDynamic after v20 auto-inherits removal
        GenHelpForMissingWebConfigAutoInherits("AsDynamic"),

        // Help for missing CreateInstance after v20 auto-inherits removal
        GenHelpForMissingWebConfigAutoInherits("CreateInstance"),

        // Help for missing Link after v20 auto-inherits removal
        GenHelpForMissingWebConfigAutoInherits("Link"),

        // Help for missing Edit after v20 auto-inherits removal
        GenHelpForMissingWebConfigAutoInherits("Edit"),

        // Help for missing 'Module' property on DnnHelper after v20 changes
        new()
        {
            Name = "dnnhelper-missing-after-v20",
            Detect = "CS1061: 'DnnHelper<dynamic>' does not contain a definition for",
            UiMessage = """

                        Your Razor code is trying to use 'Module', 'Portal', etc, but this property is not available. In v20, APIs and base classes changed. Make sure your template inherits from the correct base class and check the docs for updated usage.

                        """,
            DetailsHtml = """

                          The <code>Module</code> or <code>Portal</code> or etc property is not available on <code>DnnHelper&lt;dynamic&gt;</code> in v20. This is likely because your template is missing the correct <code>@inherits</code> statement or is using an outdated API. 
                          <br>
                          <strong>Solution:</strong> <br>
                          Ensure your Razor file has the correct <code>@inherits</code> statement for v20. See the docs for more info.
                          <br>
                          <code>@inherits ToSic.SexyContent.Razor.SexyContentWebPage</code>
                          <br>
                          See the docs for more info.

                          """,
            LinkCode = "brc-20-stop-auto-inherits",
        },

        
        // New v20 - removal of #RemovedV20 #Element resulting in `List` being mistaken for LINQ Lists
        new()
        {
            Name = "List not available on SexyContentWebPage, resulting in c# thinking it could want to access List<T>",
            Detect = "error CS0305: Using the generic type 'List<T>' requires 1 type arguments",
            LinkCode = "brc-20-list-element",
            UiMessage = "The old List (of Element) object had to be removed."
        },
    ];


    /// <summary>
    /// Factory to generate CodeHelp for missing methods after v20 auto-inherits removal
    /// </summary>
    private static CodeHelp GenHelpForMissingWebConfigAutoInherits(string methodName) => new()
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

}