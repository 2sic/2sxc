using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

public partial class HelpDbRazor
{
    /// <summary>
    /// General problems, such as NoParamOrder - apply to everything
    /// </summary>
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> CompileProblemsGeneral => field ??=
    [
        new()
        {
            Name = "Advanced APIs should use Parameter-Names and not Param-Order",
            // real error is ca. :error CS1503: Argument 2: cannot convert from 'bool' to 'ToSic.Lib.Coding.NoParamOrder' at System.Web.Compilation.AssemblyBuilder.Compile()
            Detect = "to 'ToSic.Sys.Coding.NoParamOrder'",
            LinkCode = "named-params",
            UiMessage = "Many methods have optional parameters - these must be named, otherwise you see this error.",
            DetailsHtml =
                "Many methods have optional parameters - these must be named, otherwise you see this error. <br>" +
                "Example: <br>" +
                "<code>Kit.Page.AssetAttributes(true)</code> " +
                "should be <br><code>Kit.Page.AssetAttributes(optimize: true)</code>.",
        },

    ];


    /// <summary>
    /// Compile problems which should only apply to dynamic code
    /// </summary>
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> CompileProblemsDynamic => field ??=
    [
        new()
        {
            Name = "Can't use Lambda",
            Detect =
                "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
            LinkCode = "ErrLambda",
            UiMessage = "Lambdas have difficulties with dynamic objects."
        },

        new()
        {
            Name = "DynamicEntity not found",
            Detect = "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
            LinkCode = "ErrDynamicEntity",
            UiMessage = "The type DynamicEntity shouldn't be used.",
        },
    ];

    /// <summary>
    /// Errors which are only typical for situations where the Razor Base Class isn't known.
    /// This could be because it's not provided, or because the code is so broken that it can't detect it.
    /// </summary>
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> CompileUnknownOnly => field ??=
    [

        // Help when compiling with a namespace which is not known
        new()
        {
            Name = "unknown-ns",
            Detect = "error CS0234: The type or namespace name",
            UiMessage = """

                        Your code seems to have an invalid namespace - like as a '@using xxx' or '@inherits xxx'. Check and fix your code.

                        """,
            DetailsHtml = """

                          Your code seems to have an invalid namespace - like as a <code>@using xxx</code> or <code>@inherits xxx</code>. Check and fix your code.

                          """
        },
        
        // Help when the new Roslyn compiler runs into a conversion problem because of a semicolon at the end of the inherits-statement
        new()
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
        },

        // Help when the new Roslyn compiler runs into a conversion problem because of a comment at the end of the inherits-statement
        new()
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
        }
    ];
}