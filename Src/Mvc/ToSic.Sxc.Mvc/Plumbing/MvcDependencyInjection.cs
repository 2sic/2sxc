using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ToSic.Sxc.Mvc.WebApi.Context;

namespace ToSic.Sxc.Mvc.Plumbing
{
    public static class MvcDependencyInjection
    {
        public static IServiceCollection AddSxcMvc(this IServiceCollection services)
        {
            services.AddTransient<MvcContextBuilder>();
            // services.AddNewtonsoftJson();
            return services;
        }
    }
}
