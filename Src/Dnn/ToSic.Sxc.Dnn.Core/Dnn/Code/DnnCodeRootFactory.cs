using System;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code
{
    /// <summary>
    /// Special helper which will create the code-root based on the parent class requesting it.
    /// If the parent is generic supporting IDynamicModel[Model, Kit] it will create the generic root
    /// </summary>
    public class DnnCodeRootFactory: HasLog
    {
        public DnnCodeRootFactory(IServiceProvider serviceProvider): base(DnnConstants.LogName + ".CDRFac")
        {
            _serviceProvider = serviceProvider;
        }
        private readonly IServiceProvider _serviceProvider;

        public DynamicCodeRoot BuildDynamicCodeRoot(object customCode)
        {
            // New v14 case - the Razor component implements IDynamicData<model, Kit>
            // Will return null if error or not such interface
            var codeRoot = BuildGenericCodeRoot(customCode.GetType());

            // Default case / old case - just a non-generic DnnDynamicCodeRoot
            return codeRoot ?? _serviceProvider.Build<DnnDynamicCodeRoot>();
        }

        /// <summary>
        /// Special helper for new Kit-based Razor templates in v14
        /// </summary>
        /// <returns>`null` if not applicable, otherwise the typed DynamicRoot</returns>
        private DynamicCodeRoot BuildGenericCodeRoot(Type customCode)
        {
            var wrapLog = Log.Fn<DynamicCodeRoot>();
            try
            {
                var requiredDynCode = typeof(IDynamicCode<,>);

                // 1. Detect if it's an IDynamicCode<TModel, TServiceKit>
                var interfaceOnCode = customCode
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requiredDynCode);

                if (interfaceOnCode == null) return wrapLog.ReturnNull();

                var typesArgs = interfaceOnCode.GetGenericArguments();
                if (typesArgs.Length != requiredDynCode.GetGenericArguments().Length) return wrapLog.ReturnNull();

                var kitType = typesArgs[1];
                if (!kitType.IsSubclassOf(typeof(ServiceKit))) return wrapLog.ReturnNull();

                // 2. If yes, generate a DnnDynamicCodeRoot<TModel, TServiceKit> using the same types
                var genType = typeof(DnnDynamicCodeRoot<,>);
                var finalType = genType.MakeGenericType(typesArgs);

                // 3. return that
                var codeRoot = _serviceProvider.GetService(finalType) as DynamicCodeRoot;
                return wrapLog.Return(codeRoot);
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                return wrapLog.ReturnNull();
            }
        }
    }
}
