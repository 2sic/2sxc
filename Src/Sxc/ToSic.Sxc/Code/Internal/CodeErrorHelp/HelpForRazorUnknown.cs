using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

/// <summary>
/// Help for compile errors before the Razor base class is known.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class HelpForRazorCompileErrors
{
    /// <summary>
    /// Help when compiling with a namespace which is not known
    /// </summary>
    internal static CodeHelp UnknownNamespace = new(name: "unknown-ns",
        detect: "error CS0234: The type or namespace name",
        uiMessage: @"
Your code seems to have an invalid namespace - like as a '@using xxx' or '@inherits xxx'. Check and fix your code.
",
        detailsHtml: @"
Your code seems to have an invalid namespace - like as a <code>@using xxx</code> or <code>@inherits xxx</code>. Check and fix your code.
");


    /// <summary>
    /// Help when the new Roslyn compiler runs into a conversion problem because of a semicolon at the end of the inherits-statement
    /// </summary>
    internal static CodeHelp ProbablySemicolonAfterInherits = new(name: "inherits-breaks-with-semicolon",
        // full message is like "error CS1003: Syntax error, ',' expected at System.Web.Compilation.AssemblyBuilder.Compile()"
        // but only a part of it is in the initial exception, so we only check for the part that is there
        detect: "Syntax error, ',' expected",
        uiMessage: @"
Your Razor code probably has a semicolon ';' in the wrong place, which breaks the new Roslyn Razor Compiler. Check and fix your code.
",
        detailsHtml: @"
The new Roslyn compiler incorrectly handles <code>@inherits</code> with a trailing semicolon. Remove the semicolon and it should work. 
<br>
<strong>Example</strong>: <br>
<code>@inherits Custom.Hybrid.RazorTyped;</code> <br>
should be <br>
<code>@inherits Custom.Hybrid.RazorTyped</code>
");

    /// <summary>
    /// Help when the new Roslyn compiler runs into a conversion problem because of a comment at the end of the inherits-statement
    /// </summary>
    internal static CodeHelp 
        ProbablyCommentAfterInherits = new(name: "inherits-breaks-with-comment",
        // full message is like "Error: { expected ..., Error: } expected ..., Error: Type or namespace definition, or end-of-file expected"
        // but only a part of it is in the initial exception, so we only check for the part that is there
        detect: @"Error: { expected",
        uiMessage: @"
Your Razor code probably has a comments '//' in the wrong place, which breaks the new Roslyn Razor Compiler. Check and fix your code.
",
        detailsHtml: @"
The new Roslyn compiler incorrectly handles <code>@inherits</code> with a trailing comments. Remove the comments and it should work. 
<br>
<strong>Example</strong>: <br>
<code>@inherits Custom.Hybrid.RazorTyped // comment</code> <br>
should be <br>
<code>@inherits Custom.Hybrid.RazorTyped</code>
");

    internal static CodeHelp RazorBaseClassDoesntInheritCorrectly = new(name: "Razor Base Class doesn't inherit correctly",
        detect: "no suitable method found to override",
        uiMessage: @"
Check for custom AppCode.Razor.SomeRazor that forgets to inherit 'Custom.Hybrid.RazorTyped' or similar.
",
        detailsHtml: @"
Your razor template cshtml seems to inherit <code>AppCode.Razor.SomeRazor</code> from AppCode, while AppCode.Razor.SomeRazor doesn't inherit correctly. Check and fix your code.
<br>
<strong>Example</strong>: <br>
<code>public abstract class SomeRazor</code> <br>
should be <br>
<code>public abstract class SomeRazor : Custom.Hybrid.RazorTyped</code> or similar.<br>
");


    internal static List<CodeHelp> CompileUnknown =
    [
        UnknownNamespace,
        ProbablySemicolonAfterInherits,
        ProbablyCommentAfterInherits,
        RazorBaseClassDoesntInheritCorrectly
    ];

}