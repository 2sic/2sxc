using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Special helper which will create the code-root based on the parent class requesting it.
/// If the parent is generic supporting IDynamicModel[Model, Kit] it will create the generic root
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeApiServiceFactory(IServiceProvider serviceProvider) : ServiceBase($"{SxcLogName}.CDRFac", connect: [/* never! serviceProvider */ ])
{
    public CodeApiService BuildCodeRoot(object customCodeOrNull, IBlock blockOrNull, ILog parentLog, int compatibilityFallback)
    {
        var compatibility = (customCodeOrNull as ICompatibilityLevel)?.CompatibilityLevel ?? compatibilityFallback;
        var l = Log.Fn<CodeApiService>($"{nameof(compatibility)}: {compatibility}");
        // New v14 case - the Razor component implements IDynamicData<model, Kit>
        // Will return null if error or not such interface
        var codeRoot = customCodeOrNull != null ? BuildGenericCodeRoot(customCodeOrNull.GetType()) : null;

        // Default case / old case - just a non-generic DnnDynamicCodeRoot
        codeRoot ??= serviceProvider.Build<CodeApiService>(Log);

        codeRoot.InitDynCodeRoot(blockOrNull, parentLog).SetCompatibility(compatibility);

        return l.ReturnAsOk(codeRoot);
    }

    /// <summary>
    /// Special helper for new Kit-based Razor templates in v14
    /// </summary>
    /// <returns>`null` if not applicable, otherwise the typed DynamicRoot</returns>
    private CodeApiService BuildGenericCodeRoot(Type customType)
    {
        var l = Log.Fn<CodeApiService>();
        try
        {
            var requiredDynCode = typeof(IHasKit<>);

            // 1. Detect if it's an IDynamicCode<TModel, TServiceKit>
            var interfaceOnCode = customType
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requiredDynCode);

            if (interfaceOnCode == null) return l.ReturnNull();

            var typesArgs = interfaceOnCode.GetGenericArguments();
            if (typesArgs.Length != requiredDynCode.GetGenericArguments().Length) return null;

            var kitType = typesArgs[typesArgs.Length - 1];
            if (!kitType.IsSubclassOf(typeof(ServiceKit))) return null;

            // 2. If yes, generate a CodeApiService<TModel, TServiceKit> using the same types
            var finalType = typeof(CodeApiService<,>).MakeGenericType(typeof(object), kitType);

            // 3. return that
            var codeRoot = serviceProvider.Build<CodeApiService>(finalType);
            return l.ReturnAsOk(codeRoot);
        }
        catch (Exception ex)
        {
            l.Done(ex);
            return null;
        }
    }
}