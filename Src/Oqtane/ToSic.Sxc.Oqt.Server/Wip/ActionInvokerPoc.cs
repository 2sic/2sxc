//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Abstractions;
//using Microsoft.AspNetCore.Mvc.Controllers;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Mvc.Formatters;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;
//using Microsoft.Extensions.Options;

//namespace ToSic.Sxc.Oqt.Server.Wip
//{
//    class ActionInvokerPoc
//    {
//        private readonly IActionSelector _actionSelector;

//        private readonly IActionInvokerFactory _actionInvokerFactory;

//        public ActionInvokerPoc(IActionSelector actionSelector,  IActionInvokerFactory actionInvokerFactory)
//        {
//            _actionSelector = actionSelector;
//            _actionInvokerFactory = actionInvokerFactory;
//        }
//        public void Test1(HttpContext httpContext, string controller, string action)
//        {
//            var context = new RouteContext(httpContext);
//            context.RouteData = new RouteData();
//            context.RouteData.Values.Add("controller", controller);
//            context.RouteData.Values.Add("action", action);

//            var actionDescriptor = _actionSelector.Select(context);
//            if (actionDescriptor == null)
//                throw new NullReferenceException("Action cannot be located, please check whether module has been installed properly");

//            var actionContext = new ActionContext(httpContext, context.RouteData, actionDescriptor);

//            var invoker = _actionInvokerFactory.CreateInvoker(actionContext)
                
                
//                , actionDescriptor as ControllerActionDescriptor);
//            var result = await invoker.InvokeAction() as ViewResult; null)
//        }

//        private ControllerActionInvoker CreateInvoker(
//            IFilterMetadata[] filters,
//            ControllerActionDescriptor actionDescriptor,
//            object controller,
//            IDictionary<string, object> arguments = null,
//            IList<IValueProviderFactory> valueProviderFactories = null,
//            RouteData routeData = null,
//            ILogger logger = null,
//            object diagnosticListener = null)
//        {
//            Assert.NotNull(actionDescriptor.MethodInfo);

//            if (arguments == null)
//            {
//                arguments = new Dictionary<string, object>();
//            }

//            if (valueProviderFactories == null)
//            {
//                valueProviderFactories = new List<IValueProviderFactory>();
//            }

//            if (routeData == null)
//            {
//                routeData = new RouteData();
//            }

//            if (logger == null)
//            {
//                logger = new NullLoggerFactory().CreateLogger<ControllerActionInvoker>();
//            }

//            var httpContext = new DefaultHttpContext();

//            var options = Options.Create(new MvcOptions());

//            var services = new ServiceCollection();
//            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
//            services.AddSingleton<IOptions<MvcOptions>>(options);
//            services.AddSingleton<IActionResultExecutor<ObjectResult>>(new ObjectResultExecutor(
//                new DefaultOutputFormatterSelector(options, NullLoggerFactory.Instance),
//                new TestHttpResponseStreamWriterFactory(),
//                NullLoggerFactory.Instance,
//                options));

//            httpContext.Response.Body = new MemoryStream();
//            httpContext.RequestServices = services.BuildServiceProvider();

//            var formatter = new Mock<IOutputFormatter>();
//            formatter
//                .Setup(f => f.CanWriteResult(It.IsAny<OutputFormatterCanWriteContext>()))
//                .Returns(true);

//            formatter
//                .Setup(f => f.WriteAsync(It.IsAny<OutputFormatterWriteContext>()))
//                .Returns<OutputFormatterWriteContext>(async c =>
//                {
//                    await c.HttpContext.Response.WriteAsync(c.Object.ToString());
//                });

//            options.Value.OutputFormatters.Add(formatter.Object);

//            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
//            if (diagnosticListener != null)
//            {
//                diagnosticSource.SubscribeWithAdapter(diagnosticListener);
//            }

//            var objectMethodExecutor = ObjectMethodExecutor.Create(
//                actionDescriptor.MethodInfo,
//                actionDescriptor.ControllerTypeInfo,
//                ParameterDefaultValues.GetParameterDefaultValues(actionDescriptor.MethodInfo));

//            var actionMethodExecutor = ActionMethodExecutor.GetExecutor(objectMethodExecutor);

//            var cacheEntry = new ControllerActionInvokerCacheEntry(
//                new FilterItem[0],
//                (c) => controller,
//                null,
//                (_, __, args) =>
//                {
//                    foreach (var item in arguments)
//                    {
//                        args[item.Key] = item.Value;
//                    }

//                    return Task.CompletedTask;
//                },
//                objectMethodExecutor,
//                actionMethodExecutor);

//            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
//            var controllerContext = new ControllerContext(actionContext)
//            {
//                ValueProviderFactories = valueProviderFactories,
//            };

//            var invoker = new ControllerActionInvoker(
//                logger,
//                diagnosticSource,
//                ActionContextAccessor.Null,
//                new ActionResultTypeMapper(),
//                controllerContext,
//                cacheEntry,
//                filters);
//            return invoker;
//        }
//    }
//}
