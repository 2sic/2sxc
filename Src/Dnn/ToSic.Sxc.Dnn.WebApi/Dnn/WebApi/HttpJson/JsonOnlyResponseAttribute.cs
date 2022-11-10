using System;
using System.Linq;
using System.Web.Http.Controllers;

// Special case: this should enforce json formatting
// It's only needed in .net4x where the default is xml
namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    public class JsonOnlyResponseAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Remove(controllerSettings.Formatters.XmlFormatter);

            // for older apis we need to leave NewtonsoftJson
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<UseOldNewtonsoftForHttpJsonAttribute>().Any()) 
                return;

            // remove new apis remove NewtonsoftJson
            controllerSettings.Formatters.Remove(controllerSettings.Formatters.JsonFormatter);
        }
    }
}

