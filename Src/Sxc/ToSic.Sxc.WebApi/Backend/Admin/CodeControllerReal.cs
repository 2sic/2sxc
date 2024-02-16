using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code.Internal.Documentation;
using ToSic.Sxc.Code.Internal.Generate;

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeControllerReal(DataModelGenerator modelGenerator) : ServiceBase("Api.CodeRl")
{
    public const string LogSuffix = "Code";

    public class HelpItem
    {
        // the name of the class
        public string Term { get; set; }
        // message from the attribute
        public string[] Help { get; set; }


        // not supported in System.Text.Json
        //// ignore some of serialization exceptions
        //// https://www.newtonsoft.com/json/help/html/serializationerrorhandling.htm
        //[OnError]
        //internal void OnError(StreamingContext context, ErrorContext errorContext)
        //{
        //    errorContext.Handled = true;
        //}
    }

    public IEnumerable<HelpItem> InlineHelp(string language)
    {
        var wrapLog = Log.Fn<IEnumerable<HelpItem>>($"InlineHelp:l:{language}", timer: true);

        if (_inlineHelp != null) return wrapLog.ReturnAsOk(_inlineHelp);

        // TODO: stv# how to use languages?

        try
        {
            _inlineHelp = AssemblyHandling.GetTypes(Log)
                .Where(t => t?.IsDefined(typeof(DocsAttribute)) ?? false)
                .Select(t => new HelpItem
                {
                    Term = t?.Name,
                    Help = t?.GetCustomAttribute<DocsAttribute>()?.GetMessages(t?.FullName)
                }).ToArray();
        }
        catch (Exception e)
        {
            Log.A("Exception in inline help.");
            Log.Ex(e);
        }

        return wrapLog.ReturnAsOk(_inlineHelp);
    }
    private static IEnumerable<HelpItem> _inlineHelp;

    public void GenerateDataModels(int appId, string edition)
    {
        var wrapLog = Log.Fn($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        var dataModelGenerator = modelGenerator.Setup(appId, edition);
        dataModelGenerator.GenerateAndSaveFiles();

        wrapLog.Done();
    }

}