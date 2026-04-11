using System;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

namespace DangExtension
{
    public static class Utility
    {
        /// <summary>
        /// Using: TryParseEnum<enum>(entry.key, out var type)
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnum<TEnum>(string key, out TEnum result) where TEnum : struct, Enum
        {
            return Enum.TryParse(key, out result);
        }

        #region FormatHelper
        public static string FormatNumber(int num)
        {
            if (num >= 1_000_000_000)
                return (num / 1_000_000_000f).ToString("0.#") + "B"; // hiển thị 1.2B, 2.5B
            else if (num >= 1_000_000)
                return (num / 1_000_000f).ToString("0.#") + "M";     // hiển thị 1.2M, 3M
            else if (num >= 1_000)
                return num.ToString("N0", CultureInfo.InvariantCulture); // ví dụ 12,345
            else
                return num.ToString("0", CultureInfo.InvariantCulture); // ví dụ 999
        }
        public static string FormatNumber(int num, char thousandSeparator)
        {
            if (num >= 1_000_000_000)
                return (num / 1_000_000_000f).ToString("0.#", CultureInfo.InvariantCulture) + "B";
            else if (num >= 1_000_000)
                return (num / 1_000_000f).ToString("0.#", CultureInfo.InvariantCulture) + "M";
            else if (num >= 1_000)
            {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = thousandSeparator.ToString();
                return num.ToString("N0", culture);
            }
            else
                return num.ToString();
        }
        public static string FormatNumberWithoutPostFix<T>(T number, char separator)
        {
            if (number == null)
                return "0";

            // Clone culture để custom dấu phân cách
            CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = separator.ToString();

            // N0 = có dấu phân cách, không có decimal
            return string.Format(culture, "{0:N0}", number);
        }
        /// <summary>
        /// Convert seconds → MM:SS format (e.g. 02:07)
        /// </summary>
        public static string ToMinuteSecond(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Convert seconds → HH:MM:SS format (e.g. 01:05:09)
        /// </summary>
        public static string ToHourMinuteSecond(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Convert seconds → 02h30m30s format
        /// </summary>
        public static string ToHourMinuteSecondText(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            return $"{hours:00}h{minutes:00}m{seconds:00}s";
        }

        /// <summary>
        /// Convert seconds → 00m02s format (ignore hours)
        /// </summary>
        public static string ToMinuteSecondText(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}m{seconds:00}s";
        }

        #endregion
    }
}
