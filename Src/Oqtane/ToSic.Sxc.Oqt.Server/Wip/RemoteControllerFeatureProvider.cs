using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Code.Builder;
using Microsoft.Extensions.Hosting;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Oqt.Server.Wip
{
    public class RemoteControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ServiceProvider _serviceProvider;

        public RemoteControllerFeatureProvider(IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();
        }
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var hostingEnvironment = _serviceProvider.Build<IHostEnvironment>();
            var tenantsRoot = Path.Combine(hostingEnvironment.ContentRootPath, @"Content\Tenants");

            if (string.IsNullOrWhiteSpace(tenantsRoot) || !Directory.Exists(tenantsRoot)) return;
            foreach (var tenant in Directory.GetDirectories(tenantsRoot, "*.*", SearchOption.TopDirectoryOnly))
            {
                var sitesPath = Path.Combine(tenant, "Sites");
                if (!Directory.Exists(sitesPath)) continue;
                foreach (var site in Directory.GetDirectories(sitesPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    var sxcAppPath = Path.Combine(site, "2sxc");
                    if (!Directory.Exists(sxcAppPath)) continue;
                    foreach (var app in Directory.GetDirectories(sxcAppPath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        var apiFolderPath = Path.Combine(app, "api");
                        if (!Directory.Exists(apiFolderPath)) continue;
                        foreach (var apiFile in Directory.GetFiles(apiFolderPath, "*.cs", SearchOption.TopDirectoryOnly))
                        {
                            var apiCode = System.IO.File.ReadAllText(apiFile);
                            if (string.IsNullOrWhiteSpace(apiCode)) continue;
                            var className = $"DynCode_{System.IO.Path.GetFileNameWithoutExtension(tenant)}_{System.IO.Path.GetFileNameWithoutExtension(site)}_{System.IO.Path.GetFileNameWithoutExtension(app)}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";

                            var compiledAssembly = new Compiler().Compile(apiFile, className);
                            if (compiledAssembly == null) continue;

                            var assembly = new Runner().Load(compiledAssembly);

                            var candidates = assembly.GetExportedTypes();

                            foreach (var candidate in candidates)
                            {
                                feature.Controllers.Add(candidate.GetTypeInfo());
                            }
                        }
                    }
                }
            }
        }
    }
}
