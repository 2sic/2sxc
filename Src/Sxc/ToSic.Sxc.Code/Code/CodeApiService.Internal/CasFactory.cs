using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Special helper which will create the code-root based on the parent class requesting it.
/// If the parent is generic supporting IDynamicModel[Model, Kit] it will create the generic root
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExecutionContextFactory(IServiceProvider serviceProvider)
    : ServiceBase($"{SxcLogName}.CDRFac", connect: [/* never! serviceProvider */ ]), IExecutionContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <param name="parentClassOrNull"></param>
    /// <param name="blockOrNull"></param>
    /// <param name="parentLog"></param>
    /// <param name="compatibilityFallback"></param>
    /// <returns></returns>
    public IExecutionContext New(object? parentClassOrNull, IBlock blockOrNull, ILog parentLog, int compatibilityFallback)
    {
        var compatibility = (parentClassOrNull as ICompatibilityLevel)?.CompatibilityLevel ?? compatibilityFallback;
        var l = Log.Fn<ExecutionContext>($"{nameof(compatibility)}: {compatibility}");

        // New v14 case - the Razor component implements IDynamicData<model, Kit>
        // which specifies what kit version to use.
        // Try to respect that or null if error or not such interface
        var executionContext = parentClassOrNull == null
            ? null
            : TryBuildCodeApiServiceForDynamic(parentClassOrNull.GetType());

        // Default case / old case - just a non-generic DnnDynamicCodeRoot
        executionContext ??= serviceProvider.Build<ExecutionContext>(Log);

        executionContext
            .InitDynCodeRoot(blockOrNull, parentLog)
            .SetCompatibility(compatibility);

        return l.ReturnAsOk(executionContext);
    }

    /// <summary>
    /// Special helper for new Kit-based Razor templates in v14
    /// </summary>
    /// <returns>`null` if not applicable, otherwise the typed DynamicRoot</returns>
    private ExecutionContext? TryBuildCodeApiServiceForDynamic(Type customType)
    {
        var l = Log.Fn<ExecutionContext>();
        try
        {
            var requiredDynCode = typeof(IHasKit<>);

            // 1. Detect if it's an IDynamicCode<TModel, TServiceKit>
            var interfaceOnCode = customType
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requiredDynCode);

            if (interfaceOnCode == null)
                return l.ReturnNull();

            var typesArgs = interfaceOnCode.GetGenericArguments();
            if (typesArgs.Length != requiredDynCode.GetGenericArguments().Length)
                return null;

            var kitType = typesArgs[typesArgs.Length - 1];
            if (!kitType.IsSubclassOf(typeof(ServiceKit)))
                return null;

            // 2. If yes, generate a CodeApiService<TModel, TServiceKit> using the same types
            var finalType = typeof(ExecutionContext<,>).MakeGenericType(typeof(object), kitType);

            // 3. return that
            var codeRoot = serviceProvider.Build<ExecutionContext>(finalType);
            return l.ReturnAsOk(codeRoot);
        }
        catch (Exception ex)
        {
            l.Done(ex);
            return null;
        }
    }
}