using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using DotNetNuke.Common;
using Microsoft.Extensions.DependencyInjection;

// This is a special workaround for DNN 9.4
// Review the readme.md to understand how and why

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn940
{
    public class WebApiHttpControllerActivator : IHttpControllerActivator
    {
        public const string Dnn9DepProviderProperty = "DependencyProvider";
        public IHttpControllerActivator PreviousActivator { get; set; }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            // first try to just get it from the DI - if it's there
            // note that the PreviousActivator doesn't exist
            return PreviousActivator?.Create(request, controllerDescriptor, controllerType) ??
                   // If it's not found (null), then it's probably a dynamically compiled type from a .cs file or similar
                   // Such types are never registered in the DI catalog, as they may change on-the-fly.
                   // In this case we must use ActivatorUtilities, which will create the object and if it expects 
                   // any DI parameters, they will come from the DependencyInjection as should be best practice
                   (IHttpController)ActivatorUtilities.CreateInstance(Dnn9DependencyProvider, controllerType);
        }

        private IServiceProvider Dnn9DependencyProvider
        {
            get
            {
                // Only attempt this once
                if (!FirstTry) return _dnn9DependencyProvider;
                FirstTry = false;

                try
                {
                    _dnn9DependencyProvider =
                        typeof(Globals).GetProperty(Dnn9DepProviderProperty,
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            ?.GetValue(null, null) as IServiceProvider;
                }
                catch
                {
                    /* ignore */
                }

                return _dnn9DependencyProvider;
            }
        }
        private IServiceProvider _dnn9DependencyProvider;
        public bool FirstTry = true;
    }
}