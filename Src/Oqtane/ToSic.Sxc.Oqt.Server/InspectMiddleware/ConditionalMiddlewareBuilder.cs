using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using ToSic.Sxc.Oqt.Server.Controllers.WebApiRouting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
{
    // based on https://github.com/aspnet/AspNetCore/blob/master/src/Middleware/MiddlewareAnalysis/src/AnalysisBuilder.cs
    internal class ConditionalMiddlewareBuilder : IApplicationBuilder
    {
        private readonly string _runBefore;
        public ConditionalMiddlewareBuilder(IApplicationBuilder inner, string runBefore)
        {
            _runBefore = runBefore;
            InnerBuilder = inner;
        }

        private IApplicationBuilder InnerBuilder { get; }

        public IServiceProvider ApplicationServices
        {
            get => InnerBuilder.ApplicationServices;
            set => InnerBuilder.ApplicationServices = value;
        }

        public IDictionary<string, object> Properties => InnerBuilder.Properties;
        public IFeatureCollection ServerFeatures => InnerBuilder.ServerFeatures;
        public RequestDelegate Build() => InnerBuilder.Build();
        public IApplicationBuilder New() => throw new NotImplementedException();

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            return InnerBuilder
                .UseMiddleware<ConditionalMiddleware>(_runBefore)
                .Use(middleware);
        }
    }
}
