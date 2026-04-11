using System;
using System.Collections.Generic;
using ERandom = UnityEngine.Random;

public static class DictionaryExt
{
    /// <summary>
    /// Get Or Create new value with key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static V GetOrCreate<K, V>(this Dictionary<K, V> map, K k, Func<K, V> creator = null)
    {
        if (k == null) return default;
        if (!map.ContainsKey(k))
        {
            if (creator != null) map[k] = creator(k);
            else map[k] = (V)Activator.CreateInstance(typeof(V));
        }
        return map[k];
    }

    /// <summary>
    /// Get Or Create new value with key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static V GetOrCreate<K, V>(this Dictionary<K, V> map, K k, Func<V> creator = null)
    {
        if (k == null) return default;
        if (!map.ContainsKey(k))
        {
            if (creator != null) map[k] = creator();
            else map[k] = (V)Activator.CreateInstance(typeof(V));
        }
        return map[k];
    }

    /// <summary>
    ///
    /// </summary>
    public static Dictionary<K, V> Filter<K, V>(this Dictionary<K, V> map, Predicate<K> selector)
    {
        Dictionary<K, V> newMap = new Dictionary<K, V>();
        foreach (var pair in map)
            if (selector.Invoke(pair.Key)) newMap[pair.Key] = pair.Value;
        return newMap;
    }

    /// <summary>
    ///
    /// </summary>
    public static Dictionary<K, T> ConvertValue<K, V, T>(this Dictionary<K, V> map, Func<V, T> converter)
    {
        Dictionary<K, T> newMap = new Dictionary<K, T>();
        foreach (var pair in map)
            newMap[pair.Key] = converter.Invoke(pair.Value);
        return newMap;
    }

    /// <summary>
    ///
    /// </summary>
    public static Dictionary<E, T> Convert<K, E, V, T>(this Dictionary<K, V> map,
        Func<K, E> keyConverter,
        Func<V, T> valueConverter)
    {
        Dictionary<E, T> newMap = new Dictionary<E, T>();
        foreach (var pair in map)
            newMap[keyConverter(pair.Key)] = valueConverter.Invoke(pair.Value);
        return newMap;
    }
}