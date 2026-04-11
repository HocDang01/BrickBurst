// using I2.Loc;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringExt
{
    const string theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                               + "@"
                               + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";

    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        return Regex.IsMatch(email, theEmailPattern);
    }

    public static string ToTitleCase(this string input)
    {
        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
        string[] words = input.Split(' ');
        string text = myTI.ToTitleCase(words[0]);
        for (int i = 1; i < words.Length; i++)
        {
            text += " " + myTI.ToTitleCase(words[i]);
        }

        return text;
        //return Regex.Replace(str, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))", "$1 ");
    }

    public static string SplitTitleCase(this string input)
    {
        return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
    }

    // public static string ToLocalized(this string keyStr, bool fixForRTL = true)
    // {
    //     if (!keyStr.StartsWith("TXT_"))
    //         return keyStr;

    //     var str = LocalizationManager.GetTranslation(keyStr, fixForRTL);
    //     return string.IsNullOrEmpty(str) ? keyStr : str;
    // }

    // public static string ToLocalized(this string keyStr, bool fixForRTL, string overrideLanguage = null)
    // {
    //     if (!keyStr.StartsWith("TXT_"))
    //         return keyStr;

    //     var str = LocalizationManager.GetTranslation(keyStr, fixForRTL, 0, true, false, null, overrideLanguage);
    //     return string.IsNullOrEmpty(str) ? keyStr : str;
    // }

    public static string Format(this string str, params object[] objects) => string.Format(str, objects);

    public static string RemoveSpace(this string str) => str.Replace(" ", "");

    static readonly Random random = new Random();

    const string fullChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    const string textChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string RandomAlphabetWithNumber(int length)
    {
        return new string(Enumerable.Repeat(fullChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string RandomAlphabet(int length)
    {
        return new string(Enumerable.Repeat(textChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}