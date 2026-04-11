using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ERandom = UnityEngine.Random;

public static class ListExt
{

    /// <summary>
    /// Helpful for iterator all element in list without index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
    {
        foreach (T item in sequence) action(item);
    }

    /// <summary>
    /// Helpful for iterator all element in list with index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
    {
        int i = 0;
        foreach (T item in sequence) { action(item, i); i++; }
    }

    /// <summary>
    /// Fill list with value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name=""></param>
    public static void Fill<T>(this List<T> list, int amount, T value)
    {
        list.Clear();
        for (int i = 0; i < amount; i++)
            list.Add(value);
    }

    /// <summary>
    /// Fill list with value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name=""></param>
    public static void Fill<T>(this List<T> list, int amount, Func<T> creator)
    {
        list.Clear();
        for (int i = 0; i < amount; i++)
            list.Add(creator());
    }

    /// <summary>
    /// Get random an element in array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Random<T>(this IEnumerable<T> sequence, params T[] excepts)
    {
        int count = sequence.Count();
        if (count == 0) return default;
        if (count == 1 && (excepts.Length == 0 || excepts[0] == null)) return sequence.First();
        if (count == 1 && sequence.First().Equals(excepts[0])) return default;
        if (excepts.Length >= count) return default;

        T t;
        List<T> _excepts = excepts.ToList();
        do { t = sequence.ElementAt(ERandom.Range(0, count)); }
        while (_excepts.Contains(t));
        return t;
    }

    /// <summary>
    /// Get random an element in array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Random<T>(this IEnumerable<T> sequence, List<T> excepts)
    {
        int count = sequence.Count();
        if (count == 0) return default;
        if (count == 1 && (excepts.Count == 0 || excepts[0] == null)) return sequence.First();
        if (count == 1 && sequence.First().Equals(excepts[0])) return default;
        if (excepts.Count >= count) return default;

        T t;
        List<T> _excepts = excepts.ToList();
        do { t = sequence.ElementAt(ERandom.Range(0, count)); }
        while (_excepts.Contains(t));
        return t;
    }

    /// <summary>
    /// Get random elements in array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Random<T>(this IEnumerable<T> sequence, int amount, params T[] excepts)
    {
        List<T> ret = new List<T>();

        if (sequence.Count() == 0) return ret;

        List<T> _list;
        if (excepts.Length > 0)
        {
            List<T> _excepts = excepts.ToList();
            _list = sequence.Where(t => !_excepts.Contains(t)).ToList();
        }
        else
        {
            _list = sequence.ToList();
        }

        if (_list.Count <= amount) return _list;

        for (int i = 0; i < amount; i++)
        {
            T t = _list[ERandom.Range(0, _list.Count)];
            _list.Remove(t);
            ret.Add(t);
        }
        return ret;
    }

    /// <summary>
    /// Get and remove random element from list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T PopRandom<T>(this List<T> list)
    {
        if (list.Count == 0) throw new IndexOutOfRangeException("List is empty!");

        if (list.Count == 1)
            return list.Pop();

        T t = list[ERandom.Range(0, list.Count)];
        list.Remove(t);
        return t;
    }

    /// <summary>
    /// Get and remove last element from list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Pop<T>(this List<T> list)
    {
        if (list.Count == 0) throw new IndexOutOfRangeException("List is empty!");

        T t = list[list.Count - 1]; list.Remove(t); return t;
    }

    /// <summary>
    /// Remove all elements which satifies condition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<T> RemoveAll<T>(this List<T> list, Predicate<T> predicate)
    {
        List<T> ret = list.FindAll(predicate);
        list.RemoveAll(ret);
        return ret;
    }

    /// <summary>
    /// Remove a first element which satifies condition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T Remove<T>(this List<T> list, Predicate<T> predicate)
    {
        T ret = list.Find(predicate);
        list.Remove(ret);
        return ret;
    }

    /// <summary>
    /// Random shuffle element of list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i)
        {
            int r = ERandom.Range(i, count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    /// <summary>
    /// Removes the first element from list and returns that removed element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Shift<T>(this List<T> list)
    {
        if (list.Count == 0) throw new IndexOutOfRangeException("List is empty!");
        T t = list[0]; list.Remove(t); return t;
    }

    /// <summary>
    /// Add element to first of list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> Unshift<T>(this List<T> list, T t)
    {
        list.Insert(0, t);
        return list;
    }

    public static int RemoveAll<T>(this List<T> list, List<T> subList) => list.RemoveAll(ele => subList.Contains(ele));

    public static T RemoveLast<T>(this List<T> list)
    {
        if (list.Count == 0) return default;

        var last = list.Last();
        list.Remove(last);
        return last;
    }

    public static int IndexOf<T>(this List<T> list, Predicate<T> predicate)
    {
        T t = list.Find(predicate);
        return t != null ? list.IndexOf(t) : -1;
    }

    /// <summary>
    /// Get next element from input element in list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="from"></param>
    /// <param name="loop">
    ///     True - Return first element when input element is last
    ///     False - Return null when input element is last
    /// </param>
    /// <returns>
    ///     A next element
    /// </returns>
    public static T Next<T>(this List<T> list, T from, bool loop = false)
    {
        int index = list.IndexOf(from);
        return loop
            ? list[(index + 1) % list.Count]
            : index + 1 < list.Count ? list[index + 1] : default;
    }

    /// <summary>
    /// Get prev element from input element in list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="from"></param>
    /// <param name="loop">
    ///     True - Return last element when input element is first
    ///     False - Return null when input element is first
    /// </param>
    /// <returns>
    ///     A previous element
    /// </returns>
    public static T Prev<T>(this List<T> list, T from, bool loop = false)
    {
        int index = list.IndexOf(from) - 1;
        if (index == -1)
        {
            if (loop) index = list.Count - 1;
            else return default;
        }
        return list[index];
    }

    /// <summary>
    /// Get next TValue from input TValue in list of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static TValue Next<T, TValue>(this List<T> list, TValue from, Func<T, TValue> getter, bool loop = true)
    {
        T t = list.Find(ele => getter.Invoke(ele).Equals(from));
        int index = list.IndexOf(t);
        if (loop)
            t = list[(index + 1) % list.Count];
        else
            t = list[Mathf.Clamp(index + 1, 0, list.Count - 1)];

        return getter.Invoke(t);
    }

    /// <summary>
    /// Remove element at index and add new element to that index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    public static void Replace<T>(this List<T> list, T t, int index)
    {
        if (list.Count <= index || index < 0) return;
        list.RemoveAt(index);
        list.Insert(index, t);
    }

    /// <summary>
    /// Find min element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="initValue"></param>
    /// <returns></returns>
    public static T MinBy<T>(this IEnumerable<T> sequence, Func<T, float> valueGetter, float minValue = float.MaxValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            float result = valueGetter.Invoke(_t);
            if (result < minValue)
            {
                t = _t;
                minValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find min element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="initValue"></param>
    /// <returns></returns>
    public static T MinBy<T>(this IEnumerable<T> sequence, Func<T, long> valueGetter, long minValue = long.MaxValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            long result = valueGetter.Invoke(_t);
            if (result < minValue)
            {
                t = _t;
                minValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find min element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="initValue"></param>
    /// <returns></returns>
    public static T MinBy<T>(this IEnumerable<T> sequence, Func<T, int> valueGetter, int minValue = int.MaxValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            int result = valueGetter.Invoke(_t);
            if (result < minValue)
            {
                t = _t;
                minValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find element by with min value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="initValue"></param>
    /// <returns></returns>
    public static T Min<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> valueGetter, TResult initValue) where TResult : IComparable
    {
        T t = default;
        TResult minValue = initValue;
        foreach (T _t in sequence)
        {
            TResult result = valueGetter.Invoke(_t);
            if (result.CompareTo(minValue) < 0)
            {
                t = _t;
                minValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find max element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static T MaxBy<T>(this IEnumerable<T> sequence, Func<T, float> valueGetter, float maxValue = float.MinValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            float result = valueGetter.Invoke(_t);
            if (result > maxValue)
            {
                t = _t;
                maxValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find max element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static T MaxBy<T>(this IEnumerable<T> sequence, Func<T, long> valueGetter, long maxValue = long.MinValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            long result = valueGetter.Invoke(_t);
            if (result > maxValue)
            {
                t = _t;
                maxValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find max element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static T MaxBy<T>(this IEnumerable<T> sequence, Func<T, double> valueGetter, double maxValue = double.MinValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            double result = valueGetter.Invoke(_t);
            if (result > maxValue)
            {
                t = _t;
                maxValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find max element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static T MaxBy<T>(this IEnumerable<T> sequence, Func<T, int> valueGetter, int maxValue = int.MinValue)
    {
        T t = default;
        foreach (T _t in sequence)
        {
            int result = valueGetter.Invoke(_t);
            if (result > maxValue)
            {
                t = _t;
                maxValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Find element by with max value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="valueGetter"></param>
    /// <param name="initValue"></param>
    /// <returns></returns>
    public static T Max<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> valueGetter, TResult initValue) where TResult : IComparable
    {
        T t = default;
        TResult maxValue = initValue;
        foreach (T _t in sequence)
        {
            TResult result = valueGetter.Invoke(_t);
            if (result.CompareTo(maxValue) > 0)
            {
                t = _t;
                maxValue = result;
            }
        }
        return t;
    }

    /// <summary>
    /// Get TResult list according to T list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="getter"></param>
    /// <returns></returns>
    public static List<TResult> Map<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> getter)
    {
        List<TResult> results = new List<TResult>();
        sequence.ForEach(t => results.Add(getter.Invoke(t)));
        return results;
    }

    /// <summary>
    /// Get TResult list according to T list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="getter"></param>
    /// <returns></returns>
    public static List<TResult> Map<T, TResult>(this IEnumerable<T> sequence, Func<T, int, TResult> getter)
    {
        List<TResult> results = new List<TResult>();
        sequence.ForEach((t, i) => results.Add(getter.Invoke(t, i)));
        return results;
    }

    public static Dictionary<TKey, T> ToMap<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyGetter)
    {
        Dictionary<TKey, T> dict = new Dictionary<TKey, T>();

        foreach (T _t in sequence)
        {
            TKey key = keyGetter.Invoke(_t);
            dict[key] = _t;
        }

        return dict;
    }

    public static Dictionary<TKey, TValue> ToMap<T, TKey, TValue>(this IEnumerable<T> sequence, Func<T, TKey> keyGetter, Func<T, TValue> valueGetter)
    {
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

        foreach (T _t in sequence)
        {
            TKey key = keyGetter.Invoke(_t);
            dict[key] = valueGetter.Invoke(_t);
        }

        return dict;
    }

    public static Dictionary<TKey, List<T>> ToListMap<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyGetter)
    {
        Dictionary<TKey, List<T>> dict = new Dictionary<TKey, List<T>>();

        foreach (T _t in sequence)
        {
            TKey key = keyGetter.Invoke(_t);
            List<T> list = dict.GetOrCreate(key, k => new List<T>());
            list.Add(_t);
        }

        return dict;
    }

    public static Dictionary<TKey, List<TValue>> ToListMap<T, TKey, TValue>(this IEnumerable<T> sequence, Func<T, TKey> keyGetter, Func<T, TValue> valueGetter)
    {
        Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();

        foreach (T _t in sequence)
        {
            TKey key = keyGetter.Invoke(_t);
            List<TValue> list = dict.GetOrCreate(key, k => new List<TValue>());
            list.Add(valueGetter.Invoke(_t));
        }

        return dict;
    }

    public static Dictionary<TKey, int> ToMapCount<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyGetter)
    {
        Dictionary<TKey, int> dict = new Dictionary<TKey, int>();

        foreach (T _t in sequence)
        {
            TKey key = keyGetter.Invoke(_t);
            int count = dict.GetOrCreate(key, () => 0);
            count++;
            dict[key] = count;
        }

        return dict;
    }

    /// <summary>
    /// Sort list by ascending
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="getter"></param>
    public static void SortAsc<T>(this List<T> sequence, Func<T, int> getter) => sequence.Sort((a, b) => getter(a) - getter(b));

    public static void SortAsc<T>(this List<T> sequence, Func<T, float> getter) => sequence.Sort((a, b) => (int)Mathf.Sign(getter(a) - getter(b)));

    /// <summary>
    /// Sort list by descending
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="getter"></param>
    public static void SortDesc<T>(this List<T> sequence, Func<T, int> getter) => sequence.Sort((a, b) => getter(b) - getter(a));
    public static void SortDesc<T>(this List<T> sequence, Func<T, float> getter) => sequence.Sort((a, b) => (int)Mathf.Sign(getter(b) - getter(a)));

    public static List<T> SubList<T>(this List<T> list, int fromIdx) => SubList(list, fromIdx, list.Count - fromIdx, false);

    public static List<T> SubList<T>(this List<T> list, int fromIdx, int amount, bool allowLoop = false)
    {
        List<T> newList = new List<T>();
        int _size = list.Count;
        for (int i = 0, j = fromIdx; i < amount && j < _size; i++)
        {
            newList.Add(list[j]);
            if (allowLoop) j = (j + 1) % _size;
            else j++;

            if (j == fromIdx) break;
        }
        return newList;
    }

    public static T Find<T>(this IEnumerable<T> sequence, Predicate<T> predict)
    {
        foreach (var s in sequence)
        {
            if (predict.Invoke(s))
                return s;
        }
        return default;
    }

    public static List<T> FindAll<T>(this IEnumerable<T> sequence, Predicate<T> predict, int maxAmount = int.MaxValue)
    {
        List<T> list = new List<T>();
        foreach (var s in sequence)
        {
            if (predict.Invoke(s)) list.Add(s);
            if (list.Count == maxAmount) break;
        }
        return list;
    }

    public static bool HaveAny<T>(this List<T> list, Predicate<T> predict)
    {
        foreach (var s in list)
            if (predict.Invoke(s))
                return true;

        return false;
    }

    public static TResult[] ToArray<T, TResult>(this List<T> list, Func<T, TResult> getter)
    {
        TResult[] res = new TResult[list.Count];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = getter.Invoke(list[i]);
        }

        return res;

    }
}