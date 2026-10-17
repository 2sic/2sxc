using ToSic.Sxc.Services.Sys;
using ToSic.Sys.Utils.Types;

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Special helper which will create the code-root based on the parent class requesting it.
/// </summary>
/// <remarks>
/// If the parent is generic supporting IHasKit[Kit] it will create the generic root
/// Note that there is currently not IHasKit[Model, Kit] - so the model is always `object` for now.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExecutionContextFactory(IServiceProvider serviceProvider)
    : ServiceBase($"{SxcLogName}.ExCtxF", connect: [/* never! serviceProvider */]), IExecutionContextFactory
{

    /// <inheritdoc/>
    public IExecutionContext New(ExecutionContextOptions options)
    {
        var l = Log.Fn<ExecutionContext>($"{nameof(options.Compatibility)}: {options.Compatibility}");

        // New v14 case - the Razor component implements IDynamicData<model, Kit>
        // which specifies what kit version to use.
        // Try to respect that or null if error or not such interface
        var executionContext = options.OwnerOrNull != null
            ? TryBuildCodeApiServiceForDynamic(options.OwnerOrNull.GetType())
            : null;

        // Default or old case - just a non-generic DnnDynamicCodeRoot
        // Also applies if previous call didn't succeed
        executionContext ??= serviceProvider.Build<ExecutionContext>();

        executionContext.Setup(options);

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

            // 1. Detect if it's an IHasKit<TServiceKit>
            var interfaceOnCode = customType
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericTypeOf(requiredDynCode));

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
            var exCtx = serviceProvider.Build<ExecutionContext>(finalType);
            return l.ReturnAsOk(exCtx);
        }
        catch (Exception ex)
        {
            l.Done(ex);
            return null;
        }
    }

}