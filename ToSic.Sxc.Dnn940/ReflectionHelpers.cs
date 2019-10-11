//using System;
//using System.Reflection;

//namespace ToSic.Sxc.Dnn940
//{
//    /// <summary>
//    /// Needed to access DNN internal properties
//    /// </summary>
//    /// <remarks>
//    /// Code inspired by http://dotnetfollower.com/wordpress/2012/12/c-how-to-set-or-get-value-of-a-private-or-internal-property-through-the-reflection/
//    /// </remarks>
//    public static class ReflectionHelper
//    {
//        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
//        {
//            PropertyInfo propInfo;
//            do
//            {
//                propInfo = type.GetProperty(propertyName,
//                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//                type = type.BaseType;
//            }
//            while (propInfo == null && type != null);
//            return propInfo;
//        }

//        public static object GetPropertyValue(this object obj, string propertyName)
//        {
//            if (obj == null)
//                throw new ArgumentNullException(nameof(obj));
//            var objType = obj.GetType();
//            var propInfo = GetPropertyInfo(objType, propertyName);
//            if (propInfo == null)
//                throw new ArgumentOutOfRangeException(nameof(propertyName),
//                    $"Couldn't find property {propertyName} in type {objType.FullName}");
//            return propInfo.GetValue(obj, null);
//        }

//        public static void SetPropertyValue(this object obj, string propertyName, object val)
//        {
//            if (obj == null)
//                throw new ArgumentNullException(nameof(obj));
//            var objType = obj.GetType();
//            var propInfo = GetPropertyInfo(objType, propertyName);
//            if (propInfo == null)
//                throw new ArgumentOutOfRangeException(nameof(propertyName),
//                    $"Couldn't find property {propertyName} in type {objType.FullName}");
//            propInfo.SetValue(obj, val, null);
//        }
//    }
//}