using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

/// <summary>
/// This should contain help for Razor which is used in all versions.
/// </summary>
partial class HelpDbRazor
{
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> RemovedV20 => field ??=
    [

        // New v20 - detect usage of `PrimaryValue(name, languages, resolveHyperlinks: bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old PrimaryValue / PrimaryValue<T>",
            // Full error is something like: "error CS1061: 'IEntity' does not contain a definition for 'PrimaryValue' and no accessible extension method 'PrimaryValue' accepting a first argument of type 'IEntity' could be found (are you missing a using directive or an assembly reference?) at System.Web.Compilation.AssemblyBuilder.Compile()"
            // But it can also come from a non IEntity object, like from a 'ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Internal.Decorators.EntityInBlockDecorator>'
            Detect = @"does not contain a definition for 'PrimaryValue'",
            UiMessage =
                "Your code seems to use an old 'PrimaryValue(...) method to get values. This has is removed in v20. Please use 'Get(...)' instead.",
        },

        new CodeHelp
        {
            Name = "Detect use of old Value / Value<T>",
            // Full error is something like: Error: Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: 'ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Internal.Decorators.EntityInBlockDecorator>' does not contain a definition for 'Value' at CallSite.Target(Closure , CallSite , Object , String ) at
            // note that "at" has a few spaces in front of it
            Detect = @"does not contain a definition for 'Value'",
            UiMessage =
                "Your code seems to use an old 'Value(...) method to get values. This has is removed in v20. Please use 'Get(...)' instead.",
        },

        new CodeHelp
        {
            Name = "Detect use of old GetBestValue - any which stopped existing",
            // Full error is something like: "ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Sys.Decorators.CmsEditDecorator>' does not contain a definition for 'GetBestValue' at CallSite"
            Detect = @"does not contain a definition for 'GetBestValue'",
            UiMessage =
                "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. Please use Get(...) instead.",
            LinkCode = "brc-20-getbestvalue",
        },

        // New v20 - detect use of using ToSic.Eav.Interfaces
        new CodeHelp
        {
            Name = "Using-ToSic.Eav.Interfaces",
            Detect =
                @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.Eav' (are you missing an assembly reference?)",
            UiMessage =
                "You are probably using the old namespace ToSic.Eav.Interfaces, which is not supported since v20. Replace \"ToSic.Eav.Interfaces\" with \"ToSic.Eav.Data\" in your code.",
            LinkCode = "ErrIEntity",
        },
        new CodeHelp
        {
            Name = "Using-ToSic.SexyContent.Interfaces",
            Detect =
                @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.SexyContent' (are you missing an assembly reference?)",
            UiMessage =
                "You are probably using the old namespace ToSic.SexyContent.Interfaces, which is not supported since v20. Please remove/replace according to upgrade guide TODO!.",
        },

        // New v20 - detect usage of `IEntity` without the namespace 
        new CodeHelp
        {
            Name = "IEntity-Without-Namespace",
            Detect =
                @"error CS0246: The type or namespace name 'IEntity' could not be found (are you missing a using directive or an assembly reference?)",
            UiMessage = "You are probably using IEntity in your code, but missing the @using ToSic.Eav.Data",
        },

        // New v20 - detect usage of `IEntityLight` - but not clear why it could think it exists, since it shouldn't - but in my tests it tried...
        new CodeHelp
        {
            Name = "Detect Convert to IEntityLight",
            // Full error is something like "Unable to cast object of type 'ToSic.Eav.Data.Entity' to type 'ToSic.Eav.Data.IEntityLight'."
            Detect = @"to type 'ToSic.Eav.Data.IEntityLight'",
            UiMessage =
                "Your code seems to use an old interface IEntityLight. Best just use 'ToSic.Eav.Data.IEntity' or see if the conversion is even necessary.",
        },

        // New v20 - detect usage of `IEntityLight` which should not exist in any DLLs any more
        new CodeHelp
        {
            Name = "Detect missing IEntityLight",
            // Full error is something like: "error CS0246: The type or namespace name 'IEntityLight' could not be found (are you missing a using directive or an assembly reference?)"
            Detect = @"error CS0246: The type or namespace name 'IEntityLight' could not be found",
            UiMessage =
                "Your code seems to use an old interface IEntityLight. Best just use 'ToSic.Eav.Data.IEntity' or see if the conversion is even necessary",
        },

        // New v20 - detect usage of `GetBestValue(name, languages, bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old GetBestValue - without the parameter name",
            // Full error is something like: "error CS1501: No overload for method 'GetBestValue' takes 3 arguments at System.Web.Compilation.AssemblyBuilder.Compile()"
            Detect = @"error CS1501: No overload for method 'GetBestValue' takes 3 arguments",
            UiMessage =
                "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. This has been inactive for a long time and is removed in v20. Please see guide TODO!",
        },

        // New v20 - detect usage of `GetBestValue(name, languages, resolveHyperlinks: bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old GetBestValue - with the parameter name",
            // Full error is something like: "error CS1739: The best overload for 'GetBestValue' does not have a parameter named 'resolveHyperlinks' at System.Web.Compilation.AssemblyBuilder.Compile()"
            Detect =
                @"error CS1739: The best overload for 'GetBestValue' does not have a parameter named 'resolveHyperlinks'",
            UiMessage =
                "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. This has been inactive for a long time and is removed in v20. Please see guide TODO!",
        },


        new CodeHelp
        {
            Name = "Detect use of Get<T> without using",
            // Full error is something like: "error CS0308: The non-generic method 'IEntity.Get(string)' cannot be used with type arguments"
            Detect = @"error CS0308: The non-generic method 'IEntity.Get(string)' cannot be used with type arguments",
            UiMessage =
                "Your code seems to use a generic 'Get...(...). This is an extension method as of v20. Make sure you add '@using ToSic.Eav.Data' to your razor file.",
        },

        // Added in v20, but for everything - detect calls to APIs without using parameter names
        new CodeHelp
        {
            Name = "Detect missing parameter names",
            // Full error is something like: "error CS1503: Argument 2: cannot convert from 'string' to 'ToSic.Lib.Coding.NoParamOrder' at System.Web.Compilation.AssemblyBuilder.Compile()"
            // It seems that the ' may be escaped, so we're really trying to just detect the bare minimum
            Detect = @"ToSic.Lib.Coding.NoParamOrder",
            UiMessage =
                "Your code seems to call an API which expects named parameters, and you didn't name them. See help.",
            LinkCode = "https://go.2sxc.org/named-params",
        },

        

        // New v20 - removal of #RemovedV20 #Element
        new()
        {
            Name = "The Blocks.IRenderService was removed",
            Detect = "error CS0234: The type or namespace name 'IRenderService' does not exist in the namespace 'ToSic.Sxc.Blocks'",
            LinkCode = "brc-20-blocks-irenderservice",
            UiMessage = "The old List ToSic.Sxc.Blocks.IRenderService has been replaced/renamed with ToSic.Sxc.Services.IRenderService."
        },

    ];
}
