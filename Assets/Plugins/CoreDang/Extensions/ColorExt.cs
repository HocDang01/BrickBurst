using UnityEngine;

public static class ColorExt
{
    public static Color Parse(string htmlColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(htmlColor, out color);
        return color;
    }
}

