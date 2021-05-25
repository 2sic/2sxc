using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // inspired by https://github.com/pranavkm/OptionalBodyBinding
    public class OptionalBodyFilter : ActionFilterAttribute
    {
        public OptionalBodyFilter()
        {
            // Run before the ModelStateInvalidFilter
            Order = -2001;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var methodParameters = controllerActionDescriptor.MethodInfo.GetParameters();
            for (var i = 0; i < context.ActionDescriptor.Parameters.Count; i++)
            {
                var parameter = context.ActionDescriptor.Parameters[i];
                if (parameter.BindingInfo?.BindingSource != BindingSource.Body)
                {
                    continue;
                }

                if (methodParameters[i].HasDefaultValue)
                {
                    continue;
                }

                context.ActionArguments.TryGetValue(parameter.Name, out var boundValue);
                if (boundValue != null) continue;

                // This should be equivalent of global setting
                // options.AllowEmptyInputInBodyModelBinding = true;
                context.ModelState.MarkFieldSkipped(parameter.Name);
                break;
            }
        }
    }
}
