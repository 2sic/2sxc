using System;
using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using ToSic.Lib.Documentation;
using static ToSic.Sxc.Code.Help.ObsoleteHelp;

namespace ToSic.Sxc.Code.Help
{
    [PrivateApi]
    public static class Obsolete10
    {

        internal static CodeHelp SystemConvertIncorrectUse = new CodeHelp(name: "System.Convert-Incorrect-Use",
            detect: "error CS0117: 'System.Convert' does not contain a definition for",
            linkCode: null,
            uiMessage: @"
You are probably calling Convert.ToXXX(...), so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. 
Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions, which could be why you see this error. ",
            detailsHtml: @"
You seem to be calling <code>Convert.To...(...)</code>, so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. <br>
Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions. <br>
<ol>
    <li>If you want <code>System.Convert</code> make sure your code is correct (see MS docs) </li>
    <li>If you want the <code>IConvertService</code> use <code>Kit.Convert.To...(...)</code> </li>
</ol>");

        internal static CodeHelp CreateSourceStringObsolete = new CodeHelp(name: "CreateSource-String-Obsolete",
            detect: @"error CS0411: The type arguments for method .*\.CreateSource.*cannot be inferred from the usage",
            detectRegex: true,
            linkCode: "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.Services.IDataService.html#ToSic_Sxc_Services_IDataService_GetSource_",
            uiMessage: $@"
You are probably calling CreateSource(stringNameOfSource, ...) which {IsNotSupportedIn12Plus}. 
",
            detailsHtml: $@"
You are probably calling <code>CreateSource(stringNameOfSource, ...)</code> which {IsNotSupportedIn12Plus}. Use: 
<ol>
    <li>Kit.Data.GetSource&lt;TypeName&gt;(...)</li>
    <li>Kit.Data.GetSource(appDataSourceName, ...)</li>
</ol>
");

        internal static CodeHelp DnnObjectNotInHybrid = new CodeHelp(name: "Object-Dnn-Not-In-Hybrid",
            detect: @"error CS0118: 'Dnn' is a 'namespace' but is used like a 'variable'",
            uiMessage: $@"
You are probably trying to use the 'Dnn' object which is not supported in 'Custom.Hybrid.Razor' templates. 
",
            detailsHtml: $@"
You are probably trying to use the <code>Dnn</code> object which is not supported in <code>Custom.Hybrid.Razor</code> templates. Use: 
<ol>
    <li>Other APIs such as <code>CmsContext</code> to get page/module etc. information</li>
    <li>If really necessary (not recommended) use the standard Dnn APIs to get the necessary objects.</li>
</ol>
");


        internal static CodeHelp ListNotExist12 = CreateNotExistCodeHelp("List", "AsDynamic(Data)");

        internal static CodeHelp ListObsolete12 = new CodeHelp(ListNotExist12, detect: "does not contain a definition for 'List'");


        internal static CodeHelp ListObsolete12MisCompiledAsGenericList = new CodeHelp(ListNotExist12,
            detect: @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments");

        internal static CodeHelp ListContentNotExist12 = CreateNotExistCodeHelp("ListContent", "Header");

        internal static CodeHelp ListPresentationNotExist12 = CreateNotExistCodeHelp("ListPresentation", "Header.Presentation");

        internal static CodeHelp PresentationNotExist12 = CreateNotExistCodeHelp("Presentation", "Content.Presentation");



        internal static dynamic AsDynamicForList()
            => throw new Exception("AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

        internal static dynamic CreateSourceString()
            => throw new Exception($"CreateSource(string, ...) {IsNotSupportedIn12Plus}. Please use CreateSource<DataSourceTypeName>(...) instead.");

        private static dynamic NotSupported(string original, string recommended)
            => throw new Exception($"{original} {IsNotSupportedIn12Plus}. Use {recommended}.");

        public static object AsDynamicKvp() => NotSupported("AsDynamic(KeyValuePair<int, IEntity>", "AsDynamic(IEnumerable<IEntity>...)");
        public static object Presentation() => NotSupported("Presentation", "Content.Presentation");
        public static object ListPresentation() => NotSupported("ListPresentation", "Header.Presentation");
        public static object ListContent() => NotSupported("ListContent", "Header");
        public static IEnumerable<object> List() => NotSupported("List", "Data[\"Default\"].List");

        public static object AsDynamicInterfacesIEntity()
            => NotSupported($"AsDynamic(Eav.Interfaces.IEntity)", "Please cast your data to ToSic.Eav.Data.IEntity.");

        public static object AsDynamicKvpInterfacesIEntity()
            => NotSupported("AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>)", "Please cast your data to ToSic.Eav.Data.IEntity.");

        public static IEnumerable<object> AsDynamicIEnumInterfacesIEntity()
            => NotSupported("AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities)", "Please cast your data to ToSic.Eav.Data.IEntity.");
    }
}
