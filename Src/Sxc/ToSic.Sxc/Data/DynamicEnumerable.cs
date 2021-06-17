//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using ToSic.Eav.Documentation;

//namespace ToSic.Sxc.Data
//{
//    [PrivateApi("WIP")]
//    public class DynamicEnumerable<TSource>: IEnumerable<TSource>, IDynamicEnumerable<TSource>
//    {
//        public string Test() => "This is a test";
        
//        #region Constructor and Plumbing
//        protected readonly IEnumerable<TSource> Original;

//        internal DynamicEnumerable(IEnumerable<TSource> original) => Original = original;
//        public IEnumerator<TSource> GetEnumerator() => Original.GetEnumerator();

//        IEnumerator IEnumerable.GetEnumerator() => Original.GetEnumerator();

//        private DynamicEnumerable<TResult> Wrap<TResult>(IEnumerable<TResult> contents) => new DynamicEnumerable<TResult>(contents);
//        private DynamicOrderedEnumerable<TResult> Wrap<TResult>(IOrderedEnumerable<TResult> contents) => new DynamicOrderedEnumerable<TResult>(contents);

//        private Exception Warning(string name) => new NotImplementedException(
//            $"Method {name} is not implemented. This is a {nameof(DynamicEnumerable<TSource>)} object. " +
//            "It only has the most common LINQ commands for quick use in Razor / DynamicCode. " +
//            "To use this method, first cast to an IEnumerable<dynamic>.");

//        #endregion

//        #region Aggregate - just warn

//        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func,
//            Func<TAccumulate, TResult> resultSelector)
//            => Original.Aggregate(seed, func, resultSelector);

//        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
//            => Original.Aggregate(seed, func);

//        public TSource Aggregate(Func<TSource, TSource, TSource> func) => Original.Aggregate(func);

//        #endregion

//        #region All / Any

//        public bool All(Func<TSource, bool> predicate) => Original.All(predicate);

//        public bool Any() => Original.Any();

//        public bool Any(Func<TSource, bool> predicate) => Original.Any(predicate);
//        #endregion

//        #region Append / AsEnumerable - Warn
        
//        public IDynamicEnumerable<TSource> Append(TSource element) => throw Warning(nameof(Append));

//        #endregion

        
//        // Maybe do
//        // - Append
//        // - AsEnumerable
//        // - Average

//        public IDynamicEnumerable<TResult> Cast<TResult>() => Wrap(Original.Cast<TResult>());

//        // Maybe
//        // - Concat

//        public bool Contains(TSource value) => Original.Contains(value);

//        public bool Contains(TSource value, IEqualityComparer<TSource> comparer) => Original.Contains(value, comparer);

//        public int Count() => Original.Count();

//        public int Count(Func<TSource, bool> predicate) => Original.Count(predicate);


//        // Maybe
//        // - DefaultIfEmpty

//        public IDynamicEnumerable<TSource> Distinct() => Wrap(Original.Distinct());

//        public IDynamicEnumerable<TSource> Distinct(IEqualityComparer<TSource> comparer) => Wrap(Original.Distinct(comparer));

//        // Maybe
//        // - ElementAt
//        // - ElementAtOrDefault
//        // - Empty
//        // - Except

//        public TSource First() => Original.First();

//        public TSource First(Func<TSource, bool> predicate) => Original.First(predicate);

//        public TSource FirstOrDefault() => Original.FirstOrDefault();

//        public TSource FirstOrDefault(Func<TSource, bool> predicate) => Original.FirstOrDefault(predicate);

//        // Todo
//        // - GroupBy - ca. 10 overloads

//        // Maybe
//        // - GroupJoin
//        // - Intersect
//        // - Join

//        // Todo
//        // - Last
//        // - LastOrDefault
//        // - LongCount
//        // - Max
//        // - Min
//        // - OfType


//        // Order By
//        // TODO: WOULDN't REALLY WORK YET, AS WE CAN'T RE-WRAP
//        public DynamicOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector) => Wrap(Original.OrderBy(keySelector));

//        public DynamicOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer) => Wrap( Original.OrderBy(keySelector, comparer));

//        // Todo
//        // - OrderByDescnding

//        // ToWarn
//        // - Prepend
//        // - Range
//        // - Repeat

//        // Todo
//        // - Reverse
//        // - Select

//        #region Select & Select Many

//        public IDynamicEnumerable<TResult> Select<TResult>(Func<TSource, int, TResult> selector) => Wrap(Original.Select(selector));

//        public IDynamicEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) => Wrap(Original.Select(selector));

//        // todo:
//        // - Select Many



//        #endregion

//        // ToWarn
//        // - SelectEqual

//        // Todo
//        // - Single
//        // - SingleOrDefault


//        #region Skip

//        public IDynamicEnumerable<TSource> Skip(int count) => Wrap(Original.Skip(count));

//        // todo:
//        // - SkipLast
//        // - SkipWhile

//        #endregion

//        // todo
//        // - Sum
//        // 

//        #region Takes

//        public IDynamicEnumerable<TSource> Take(int count) => Wrap(Original.Take(count));

//        // todo
//        // - TakeLast
//        // - TakeWhile


//        #endregion

//        #region ThenBy & ThenByDescending - Not here, only on the DynamicOrderedEnumerable
//        #endregion

//        // Todo
//        // - ToArray
//        // - ToDictionary
//        // - ToHashSet

//        public List<TSource> ToList() => Original.ToList();

//        // ToWarn
//        // - ToLookup
//        // - Union
//        // - Zip

//        #region Where

//        public IDynamicEnumerable<TSource> Where(Func<TSource, bool> predicate) => Wrap(Original.Where(predicate));

//        public IDynamicEnumerable<TSource> Where(Func<TSource, int, bool> predicate) => Wrap(Original.Where(predicate));



//        #endregion

//        // ToWarn
//        // - Zip


//    }
//}
