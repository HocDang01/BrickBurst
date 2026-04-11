using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

public static class NumberExt
{
    public static double RandomValue()
    {
        double a = Random.Range(0, int.MaxValue);
        double b = Random.Range(0, int.MaxValue);
        return a * 100000000 + b % 100000000;
    }

    public static string ToShortString(this int @this, int digit = 3)
    {
        if(@this < 1000) return ToShortString(@this.ToString(), digit, 0);
        else return ToShortString(@this.ToString(), digit, 0);
    }

    public static string ToShortString(this double @this, int digit = 3, int precision = 2)
    {
        if(@this < 1000) return ToShortString(@this.ToString("F0"), digit, 0);
        else return ToShortString(@this.ToString("F0"), digit, precision);
    }

    public static string ToShortString(this long @this, int digit = 3, int precision = 2)
    {
        if(@this < 1000) return ToShortString(@this.ToString("F0"), digit, 0);
        else return ToShortString(@this.ToString("F0"), digit, precision);
    }

    public static string ToShortString(this float @this, int digit = 3, int precision = 2)
    {
        if(@this < 1000) return ToShortString(@this.ToString("F0"), digit, 0);
        else return ToShortString(@this.ToString("F0"), digit, precision);
    }

    private static List<string> exponentCodes = new List<string> { "", "K", "M", "B", "T" };
    private static readonly string alphabe = "abcdefghijklmnopqrstuvwxyz";
    private static bool init = false;
    private static readonly int maxCharDigit = 2;

    // disclaimed:
    // + WORK WITH long string format. eg: "234234234242343545646948583.585938583"
    // - NOT WORK WITH science format yet!. eg: "23.33E+34"
    // - NOT WORK WITH digit <= 2
    public static string ToShortString(string str, int digit = 4, int precision = 0)
    {
        if(digit <= 2) return str;
        
        bool isFloat = str.Contains(".");
        int exponentIdx = str.Contains("E") ? str.IndexOf("E") : -1;

        // get exponent value
        int exponent = exponentIdx == -1 ? 0 :
            str[exponentIdx + 1] == '+' ? int.Parse(str.Substring(exponentIdx + 2, str.Length - exponentIdx - 2)) :
            -int.Parse(str.Substring(exponentIdx + 2, str.Length - exponentIdx - 2));

        int digitLen = isFloat ? str.IndexOf(".") : exponentIdx == -1 ? str.Length : exponentIdx;
        int a = Mathf.FloorToInt(digitLen / 3);

        while(a > 0 && digitLen - (a - 1) * 3 <= digit) a--;

        int b = a * 3;
        int endIdx = digitLen - b;
        string newStr = str.Substring(0, endIdx);

        // insert ","
        string _newStr = newStr;
        for(int i = _newStr.Length - 3; i >= 1; i -= 3)
            newStr = newStr.Insert(i, ",");

        // insert number after float point
        if(precision > 0)
        {
            int from = isFloat && a == 0 ? endIdx + 1 : endIdx;
            int len = Mathf.Min(precision, exponentIdx != -1 ? exponentIdx - from : str.Length - from);
            if(len > 0)
            {
                newStr += "." + str.Substring(from, len);
                int diff = precision - len;
                if(diff > 0) newStr += "".PadRight(diff, '0');
            }
            else
                newStr += "." + "".PadRight(precision, '0');
        }

        exponentIdx = a + exponent;
        if(!init && exponentIdx >= exponentCodes.Count) InitNumberCodes(maxCharDigit);
        
        newStr += exponentCodes[Mathf.Min(exponentIdx, exponentCodes.Count - 1)];
        return newStr;
    }

    private static void InitNumberCodes(int charLen)
    {
        Iter("", charLen, 0);
        init = true;
    }

    private static int IterChar(string code, int step, int i)
    {
        code += alphabe[i];
        if(step > 0) return Iter(code, step, i);

        exponentCodes.Add(code);
        return i + 1;
    }

    private static int Iter(string code, int step, int i)
    {
        int k = 0;
        while(k < alphabe.Length) k = IterChar(code, step - 1, k);
        return i + 1;
    }

    public static bool IsNumeric<T>(T expression)
    {
        if(expression == null)
            return false;

        return double.TryParse(Convert.ToString(expression
            , CultureInfo.InvariantCulture)
            , System.Globalization.NumberStyles.Any
            , NumberFormatInfo.InvariantInfo
            , out double number);
    }
}