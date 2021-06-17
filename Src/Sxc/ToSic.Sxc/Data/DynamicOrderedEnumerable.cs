//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ToSic.Sxc.Data
//{
//    public class DynamicOrderedEnumerable<TSource>: DynamicEnumerable<TSource>, IOrderedEnumerable<TSource>
//    {
//        internal DynamicOrderedEnumerable(IOrderedEnumerable<TSource> original) : base(original)
//        {
//        }

//        public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool @descending) 
//            => ((IOrderedEnumerable<TSource>) Original).CreateOrderedEnumerable(keySelector, comparer, @descending);

//        #region ThenBy & ThenByDescending



//        #endregion

//    }
//}
