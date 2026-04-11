using System.Collections.Generic;
using UnityEngine;

namespace DangExtension
{
    /// <summary>
    /// Static utility class to reuse WaitForSeconds instances to reduce GC allocations.
    /// Use this instead of creating new WaitForSeconds repeatedly.
    /// </summary>
    public static class WaitTimeCache
    {
        // Cache các instance WaitForSeconds đã tạo
        private static readonly Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();

        // Các instance phổ biến sẵn dùng
        public static readonly WaitForSeconds Wait0_1 = new WaitForSeconds(0.1f);
        public static readonly WaitForSeconds Wait0_2 = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds Wait0_25 = new WaitForSeconds(0.25f);
        public static readonly WaitForSeconds Wait0_5 = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds Wait1 = new WaitForSeconds(1f);
        public static readonly WaitForSeconds Wait2 = new WaitForSeconds(2f);
        public static readonly WaitForSeconds Wait3 = new WaitForSeconds(3f);
        public static readonly WaitForSeconds Wait5 = new WaitForSeconds(5f);

        /// <summary>
        /// Get or create a cached WaitForSeconds for any duration.
        /// </summary>
        /// <param name="seconds">Duration in seconds</param>
        /// <returns>Reusable WaitForSeconds instance</returns>
        public static WaitForSeconds Get(float seconds)
        {
            if (cache.TryGetValue(seconds, out var wait))
                return wait;

            wait = new WaitForSeconds(seconds);
            cache[seconds] = wait;
            return wait;
        }
    }
}