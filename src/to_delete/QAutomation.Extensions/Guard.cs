using System;
using System.Collections.Generic;
using System.Linq;

namespace QAutomation.Extensions
{
    public static class Guard
    {
        private const string MustFollowDefaultMessage =
            "The object does not follows the given rule.\nSee call stack for details";

        public static void GreaterThan(IComparable greater, IComparable than)
        {
            GreaterThan(greater, than, string.Format("{0} is not greater than {1}", greater, than));
        }

        public static void GreaterThan(IComparable greater, IComparable than, string message)
        {
            GreaterThan(greater, than, () => { throw new ArgumentOutOfRangeException(message); });
        }

        public static void GreaterThan(IComparable greater, IComparable than, Action ifNotGreaterAction)
        {
            MustFollow(() => greater.CompareTo(than) > 0, ifNotGreaterAction);
        }


        public static void GreaterOrEqualTo(IComparable greater, IComparable than)
        {
            GreaterOrEqualTo(greater, than, string.Format("{0} is not greater or equal than {1}", greater, than));
        }

        public static void GreaterOrEqualTo(IComparable greater, IComparable than, string message)
        {
            GreaterOrEqualTo(greater, than, () => { throw new ArgumentOutOfRangeException(message); });
        }
        public static void GreaterOrEqualTo(IComparable greater, IComparable than, Action ifNotGreaterOrEqualAction)
        {
            MustFollow(() => greater.CompareTo(than) >= 0, ifNotGreaterOrEqualAction);
        }

        public static void SmallerThan(IComparable smaller, IComparable than)
        {
            SmallerThan(smaller, than, string.Format("{0} is not smaller than {1}", smaller, than));
        }

        public static void SmallerThan(IComparable smaller, IComparable than, string message)
        {
            SmallerThan(smaller, than, () => { throw new ArgumentOutOfRangeException(message); });
        }

        public static void SmallerThan(IComparable smaller, IComparable than, Action ifNotSmallerThanAction)
        {
            MustFollow(() => smaller.CompareTo(than) < 0, ifNotSmallerThanAction);
        }


        public static void SmallerOrEqualTo(IComparable smaller, IComparable than)
        {
            SmallerOrEqualTo(smaller, than, string.Format("{0} is not smaller or equal than {1}", smaller, than));
        }

        public static void SmallerOrEqualTo(IComparable smaller, IComparable than, string message)
        {
            SmallerOrEqualTo(smaller, than, () => { throw new ArgumentOutOfRangeException(message); });
        }
        public static void SmallerOrEqualTo(IComparable smaller, IComparable than, Action ifNotSmallerOrEqualAction)
        {
            MustFollow(() => smaller.CompareTo(than) <= 0, ifNotSmallerOrEqualAction);
        }


        public static void NotNull(object[] objects)
        {
            foreach (var obj in objects)
                NotNull(obj, "Object is null: obj");
        }

        public static void NotNull<T>(T obj) where T : class
        {
            NotNull(obj, "Object is null: obj");
        }

        public static void NotNull<T>(T tObj, string message) where T : class
        {
            NotNull(tObj, () => { throw new NullReferenceException(message); });
        }

        public static void NotNull<T>(T tObj, Action ifNotFollowsAction) where T : class
        {
            MustFollow(!tObj.IsNull(), ifNotFollowsAction);
        }

        public static void MustFollow(Func<bool> perdicate)
        {
            MustFollow(perdicate, MustFollowDefaultMessage);
        }

        //TODO: replace generic exception with dedicated one which get call stack data
        public static void MustFollow(Func<bool> perdicate, string message)
        {
            MustFollow(perdicate, () => { throw new Exception(message); });
        }

        public static void MustFollow(Func<bool> perdicate, Action ifNotFollowsAction)
        {
            MustFollow(perdicate(), ifNotFollowsAction);
        }


        public static void MustFollow(bool condition)
        {
            MustFollow(condition, MustFollowDefaultMessage);
        }

        public static void MustFollow(bool condition, Action ifNotFollowsAction)
        {
            if (!condition)
                ifNotFollowsAction();
        }

        public static void MustFollow(bool condition, string message)
        {
            MustFollow(condition, () => { throw new InvalidOperationException(message); });
        }

        public static void HasValue(string source)
        {
            HasValue(source, "String value required");
        }

        public static void HasValue(string source, string message)
        {
            HasValue(source, () => { throw new ArgumentException(message, "source"); });
        }

        public static void HasValue(string source, Action action)
        {
            MustFollow(!string.IsNullOrEmpty(source) && !string.IsNullOrWhiteSpace(source), action);
        }

        public static void NotEmpty<T>(IEnumerable<T>[] source)
        {
            foreach (var s in source)
                NotEmpty(s);
        }

        public static void NotEmpty<T>(IEnumerable<T> source)
        {
            NotEmpty(source, "The source sequence is empty.");
        }

        public static void NotEmpty<T>(IEnumerable<T> source, Action notEmptyAction)
        {
            MustFollow(!source.IsNull() && source.Any(), notEmptyAction);
        }

        public static void NotEmpty<T>(IEnumerable<T> source, string message)
        {
            NotEmpty(source, () => { throw new ArgumentException(message); });
        }

        public static void ContainsKey<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            MustFollow(() => source.Select(x => x.Key).ToArray().Contains(key),
                () =>
                {
                    throw new KeyNotFoundException(string.Format("The source does not contain key ({0})", key));
                });
        }
    }
}