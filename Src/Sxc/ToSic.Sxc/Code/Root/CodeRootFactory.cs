using System;
using System.Linq;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Special helper which will create the code-root based on the parent class requesting it.
    /// If the parent is generic supporting IDynamicModel[Model, Kit] it will create the generic root
    /// </summary>
    [Serializable]
    public class CodeRootFactory: ServiceBase
    {
        public CodeRootFactory(IServiceProvider serviceProvider): base($"{Constants.SxcLogName}.CDRFac")
        {
            _serviceProvider = serviceProvider;
        }
        private readonly IServiceProvider _serviceProvider;

        public DynamicCodeRoot BuildCodeRoot(object customCodeOrNull, IBlock blockOrNull, ILog parentLog, int compatibilityFallback)
        {
            var compatibility = (customCodeOrNull as ICompatibilityLevel)?.CompatibilityLevel ?? compatibilityFallback;
            var l = Log.Fn<DynamicCodeRoot>($"{nameof(compatibility)}: {compatibility}");
            // New v14 case - the Razor component implements IDynamicData<model, Kit>
            // Will return null if error or not such interface
            var codeRoot = customCodeOrNull != null ? BuildGenericCodeRoot(customCodeOrNull.GetType()) : null;

            // Default case / old case - just a non-generic DnnDynamicCodeRoot
            codeRoot = codeRoot ?? _serviceProvider.Build<DynamicCodeRoot>(Log);

            codeRoot.InitDynCodeRoot(blockOrNull, parentLog).SetCompatibility(compatibility);

            return l.ReturnAsOk(codeRoot);
        }

        /// <summary>
        /// Special helper for new Kit-based Razor templates in v14
        /// </summary>
        /// <returns>`null` if not applicable, otherwise the typed DynamicRoot</returns>
        private DynamicCodeRoot BuildGenericCodeRoot(Type customType)
        {
            var l = Log.Fn<DynamicCodeRoot>();
            try
            {
                var requiredDynCode = typeof(IDynamicCodeKit<>);

                // 1. Detect if it's an IDynamicCode<TModel, TServiceKit>
                var interfaceOnCode = customType
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requiredDynCode);

                if (interfaceOnCode == null) return l.ReturnNull();

                var typesArgs = interfaceOnCode.GetGenericArguments();
                if (typesArgs.Length != requiredDynCode.GetGenericArguments().Length) return null;

                var kitType = typesArgs[typesArgs.Length - 1];
                if (!kitType.IsSubclassOf(typeof(ServiceKit))) return null;

                // 2. If yes, generate a DnnDynamicCodeRoot<TModel, TServiceKit> using the same types
                var finalType = typeof(DynamicCodeRoot<,>).MakeGenericType(typeof(object), kitType);

                // 3. return that
                var codeRoot = _serviceProvider.Build<DynamicCodeRoot>(finalType);
                return l.ReturnAsOk(codeRoot);
            }
            catch (Exception ex)
            {
                l.Done(ex);
                return null;
            }
        }
    }
}
