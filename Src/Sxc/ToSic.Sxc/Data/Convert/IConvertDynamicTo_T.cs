//using System.Collections.Generic;
//using ToSic.Eav.Documentation;

//namespace ToSic.Sxc.Data
//{
//    /// <summary>
//    /// Convert dynamic objects - usually dynamic entities - to something else.
//    /// Usually used to describe objects that can convert dynamic entities to dictionaries
//    /// </summary>
//    /// <remarks>
//    /// This is only shown in the docs to show the API. To get such an object, use the <see cref="IConvertToDictionary"/>.
//    /// </remarks>
//    /// <typeparam name="T"></typeparam>
//    [PublicApi]
//    public interface IConvertDynamic<out T>
//    {
//        /// <summary>
//        /// Return a converted list of dynamic objects
//        /// </summary>
//        IEnumerable<T> Convert(IEnumerable<object> dynamicList);

//        /// <summary>
//        /// Return a converted dynamic Entity
//        /// </summary>
//        T Convert(IDynamicEntity dynamicEntity);
//    }
//}
