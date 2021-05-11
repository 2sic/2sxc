using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region SharedCode Compiler

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrap = Log.Call<dynamic>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "CreateInstance",
                $"{nameof(name)},{nameof(throwOnError)}");

            var cache = MemoryCache.Default;
            var instance = cache[virtualPath.ToLowerInvariant()];
            if (instance == null)
            {
                // Compile
                var compiler = Deps.CodeCompilerLazy.IsValueCreated
                    ? Deps.CodeCompilerLazy.Value
                    : Deps.CodeCompilerLazy.Value.Init(Log);
                instance = compiler
                    //new CodeCompiler(_serviceProvider)
                    //.Init(Log)
                    .InstantiateClass(virtualPath, name, relativePath, throwOnError);

                // if it supports all our known context properties, attach them
                if (instance is ICoupledDynamicCode isShared) isShared.DynamicCodeCoupling(this);

                if (instance != null)
                    cache.Set(virtualPath.ToLowerInvariant(), instance, GetCacheItemPolicy(virtualPath));
            }

            return wrap((instance != null).ToString(), instance);
        }

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        private CacheItemPolicy GetCacheItemPolicy(string virtualPath)
        {
            var serverPaths = GetService<IServerPaths>();
            var fullFilePath = serverPaths.FullContentPath(virtualPath);
            var filePaths = new List<string> { fullFilePath };

            var cacheItemPolicy = new CacheItemPolicy();
            // expire cache item if not used in 30 mins
            cacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(30);
            // expire cache item on CS file change
            cacheItemPolicy.ChangeMonitors.Add(new
                HostFileChangeMonitor(filePaths));
            return cacheItemPolicy;
        }

        #endregion
    }
}
