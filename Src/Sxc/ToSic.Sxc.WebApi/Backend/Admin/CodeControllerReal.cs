using System.Reflection;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Code.Sys.Documentation;
using ToSic.Sys.Utils.Assemblies;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeControllerReal(
    CopilotContentTypeAutoGenerateService codeGenerate) 
    : ServiceBase("Api.CodeRl", connect: [codeGenerate])
{
    public const string LogSuffix = "Code";

    public class HelpItem
    {
        // the name of the class
        public required string Term { get; set; }
        // message from the attribute
        public required string[] Help { get; set; }
    }

    public IEnumerable<HelpItem> InlineHelp(string language)
    {
        var l = Log.Fn<IEnumerable<HelpItem>>($"InlineHelp:l:{language}", timer: true);

        if (_inlineHelp != null)
            return l.ReturnAsOk(_inlineHelp);

        // TODO: stv# how to use languages?

        try
        {
            _inlineHelp = AssemblyHandling.GetTypes(Log)
                .Where(t => t != null!)
                .Where(t => t.IsDefined(typeof(DocsAttribute)))
                .Select(t => new HelpItem
                {
                    Term = t.Name,
                    Help = t.GetCustomAttribute<DocsAttribute>()?.GetMessages(t.FullName) ?? []
                })
                .ToArray();
            return l.ReturnAsOk(_inlineHelp);
        }
        catch (Exception e)
        {
            l.A("Exception in inline help.");
            l.Ex(e);
            return l.ReturnAsError([]);
        }
    }
    private static IEnumerable<HelpItem>? _inlineHelp;

    public RichResult GenerateDataModels(int appId, string? edition, string generator, int configurationId = 0)
    {
        var l = Log.Fn<RichResult>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        var result = codeGenerate.GenerateDataModels(appId, edition, generator, configurationId);
        return l.Return(new RichResult
            {
                Ok = result.Ok,
                Message = result.Message,
            }
            .WithTime(l));
    }

}
