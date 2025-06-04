// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// based on: https://github.dev/dotnet/aspnetcore/tree/v8.0.5
// src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeCompilationFileProvider.cs

using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace ToSic.Sxc.Razor.DotNetOverrides;

internal sealed class RuntimeCompilationFileProvider
{
    private readonly MvcRazorRuntimeCompilationOptions _options;
    private IFileProvider? _compositeFileProvider;

    public RuntimeCompilationFileProvider(IOptions<MvcRazorRuntimeCompilationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    public IFileProvider FileProvider
    {
        get
        {
            if (_compositeFileProvider == null)
            {
                _compositeFileProvider = GetCompositeFileProvider(_options);
            }

            return _compositeFileProvider;
        }
    }

    private static IFileProvider GetCompositeFileProvider(MvcRazorRuntimeCompilationOptions options)
    {
        var fileProviders = options.FileProviders;
        if (fileProviders.Count == 0)
        {
            var message = "Resources.FormatFileProvidersAreRequired(typeof(MvcRazorRuntimeCompilationOptions).FullName,nameof(MvcRazorRuntimeCompilationOptions.FileProviders),typeof(IFileProvider).FullName)";
            throw new InvalidOperationException(message);
        }
        else if (fileProviders.Count == 1)
        {
            return fileProviders[0];
        }

        return new CompositeFileProvider(fileProviders);
    }
}