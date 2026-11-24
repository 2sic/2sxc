using System.Net;
using System.Text.RegularExpressions;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Search.Sys;

/// <summary>Platform-neutral search preparation shared by the CMS adapters.</summary>
/// <remarks>
/// Shares stream selection, text extraction and controller setup. Each CMS adapter still
/// owns document identity, permissions, modification filtering and provider persistence.
/// </remarks>
[PrivateApi]
public class SearchIndexing(
    Generator<CodeCompiler> codeCompiler,
    Generator<ICodeContextFactory> executionContextFactory)
    : ServiceBase("Sxc.Search", connect: [codeCompiler, executionContextFactory])
{
    public static KeyValuePair<string, IDataStream>[] Streams(IReadOnlyDictionary<string, IDataStream> streams, string selectedStreams)
    {
        // Presentation streams configure rendering. Content streams supply searchable data;
        // an empty selection uses all content streams, while a missing name selects none.
        var selected = selectedStreams.CsvToArrayWithoutEmpty();
        return streams
            .Where(s => s.Key != ViewParts.Presentation && s.Key != ViewParts.ListPresentation)
            .Where(s => selected.Length == 0 || selected.Contains(s.Key, StringComparer.InvariantCultureIgnoreCase))
            .ToArray();
    }

    // Preserve the existing DNN extraction behavior for both adapters: strings/numbers
    // become plain search text. Rendering/output encoding remains the platform's concern.
    public static string Body(IEntity entity, string language)
        => string.Join(", ", entity.Attributes
            .Where(a => a.Value.Type is ValueTypes.String or ValueTypes.Number)
            .Select(a => a.Value[language])
            .Where(value => value != null)
            .Select(value => WebUtility.HtmlDecode(Regex.Replace(value!.ToString()!, "<.*?>", string.Empty)))
            .Where(value => !string.IsNullOrEmpty(value))) + " ";

    public ICustomizeSearch? CreateViewController(IBlock block, string edition, string appPath)
    {
        var l = Log.Fn<ICustomizeSearch>();
        // Same AppCode cache identity as rendering; on Oqtane the app id alone is not tenant-unique.
        var spec = new HotBuildSpec(block.AppId, edition, block.App.AppSpecs.Name, block.App.AppSpecs.CacheKey);
        var instance = codeCompiler.New().InstantiateClass(block.View.ViewController, spec,
            className: null, relativePath: appPath, throwOnError: true);

        if (instance is not ICustomizeSearch customizeSearch)
            return l.ReturnNull("controller does not implement ICustomizeSearch");

        if (instance is ICodeContextRequired needsContext)
            // Typed/dynamic custom code needs the same block, app and log context as the
            // indexed occurrence, even when no page rendering request is running.
            needsContext.ConnectToRoot(executionContextFactory.New().New(new()
            {
                OwnerOrNull = null,
                BlockOrNull = block,
                ParentLog = Log,
                CompatibilityFallback = CompatibilityLevels.CompatibilityLevel10,
            }));

        return l.Return(customizeSearch);
    }
}
