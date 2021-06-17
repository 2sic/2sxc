//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ToSic.Sxc.Data
//{
//    public interface IDynamicEnumerable<TSource>
//    {
//        IEnumerator<TSource> GetEnumerator();

//        string Test();
        
//        TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func,
//            Func<TAccumulate, TResult> resultSelector);

//        TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func);
//        TSource Aggregate(Func<TSource, TSource, TSource> func);
//        bool All(Func<TSource, bool> predicate);
//        bool Any();
//        bool Any(Func<TSource, bool> predicate);
//        IDynamicEnumerable<TSource> Append(TSource element);
//        IDynamicEnumerable<TResult> Cast<TResult>();
//        bool Contains(TSource value);
//        bool Contains(TSource value, IEqualityComparer<TSource> comparer);
//        int Count();
//        int Count(Func<TSource, bool> predicate);
//        IDynamicEnumerable<TSource> Distinct();
//        IDynamicEnumerable<TSource> Distinct(IEqualityComparer<TSource> comparer);
//        TSource First();
//        TSource First(Func<TSource, bool> predicate);
//        TSource FirstOrDefault();
//        TSource FirstOrDefault(Func<TSource, bool> predicate);
//        DynamicOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector);
//        DynamicOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
//        IDynamicEnumerable<TResult> Select<TResult>(Func<TSource, int, TResult> selector);
//        IDynamicEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector);
//        IDynamicEnumerable<TSource> Skip(int count);
//        IDynamicEnumerable<TSource> Take(int count);
//        List<TSource> ToList();
//        IDynamicEnumerable<TSource> Where(Func<TSource, bool> predicate);
//        IDynamicEnumerable<TSource> Where(Func<TSource, int, bool> predicate);
//    }
//}