using System;
using System.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.WebApi.ApiExplorer
{
    [PrivateApi]
    public class ApiExplorerJs
    {
        /// <summary>
        /// Give common type names a simple naming and only return the original for more complex types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string JsTypeName(Type type)
        {
            // Case 1: Generic type - get the main type and then recursively get the nice names for the parts
            if (type.IsGenericType)
            {
                var mainName = type.Name;
                if (mainName.Contains("`")) mainName = mainName.Substring(0, mainName.IndexOf('`'));
                var parts = type.GenericTypeArguments.Select(JsTypeName);
                return $"{mainName}<{string.Join(", ", parts)}>";
            }

            // Case 2: Array - get inner type and add []
            if (type.IsArray) return JsTypeName(type.GetElementType()) + "[]";

            // Case 3: Most common basic types
            if (type == typeof(string)) return "string";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long int";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime)) return "datetime as string";
            if (type == typeof(Guid)) return "guid as string";

            // Case 4: Unknown - in case we don't know let's just return the difficult name
            return type.FullName;
        }
    }
}
