using System.Reflection;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

internal static class TypeReflectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TFunc">Signature of the method</typeparam>
    /// <param name="type"></param>
    /// <param name="methodName">name of the method</param>
    /// <param name="bindingAttr"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static TFunc GetDelegateToMethod<TFunc>(this Type type, string methodName, BindingFlags bindingAttr) where TFunc : Delegate
    {
        // Find the internal static method using reflection - it's internal and static, so we need to specify those binding flags
        var method = type.GetMethod(methodName, bindingAttr);

        // Make sure we found it - this should always be the case
        if (method == null)
            throw new InvalidOperationException($"Unable to locate {type.Name}.{methodName}");

        // Create a delegate for the method - this will allow us to call it efficiently without reflection after the initial lookup
        var delegateToInternalMethod = method.CreateDelegate(typeof(TFunc));

        return (TFunc)delegateToInternalMethod;
    }
}