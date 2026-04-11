using System;
using ERandom = UnityEngine.Random;
public static class ArrayExt
{
    /// <summary>
    /// Get Random an element in array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static T Random<T>(this T[] array, T except = default)
    {
        if (array.Length == 0) return default;
        if (array.Length == 1) return array[0];

        T t;
        do
        {
            t = array[ERandom.Range(0, array.Length)];
        } while (t.Equals(except));
        return t;
    }

    public static bool Contains<T>(this T[] array, T t) => Array.Exists(array, _ => _.Equals(t));

    public static int IndexOf<T>(this T[] array, T t) => Array.IndexOf(array, t);

    public static T Find<T>(this T[] array, Predicate<T> predict) => Array.Find(array, predict);

    public static T[] FindAll<T>(this T[] array, Predicate<T> predict) => Array.FindAll(array, predict);

    /// <summary>
    /// Find min element by value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="valueGetter"></param>
    /// <returns></returns>
    public static T MinBy<T>(this T[] array, Func<T, float> valueGetter)
    {
        float minValue = float.MaxValue;
        T t = default(T);
        foreach (T _t in array)
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

    public static void Fill<T>(this T[] array, T t)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = t;
        }
    }
}