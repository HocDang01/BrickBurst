using UnityEngine;

public static class RectTransormExt
{
    public static void TopLeft(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.up;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void TopCenter(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void TopRight(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.one;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void MidLeft(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.up * 0.5f;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void MidCenter(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.one * 0.5f;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void MidRight(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.right + Vector2.up * 0.5f;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void BottomLeft(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void BottomCenter(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.right * 0.5f;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void BottomRight(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.right * 1f;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void FitParent(this RectTransform rectTransform, Vector2 size = default, Vector2 anchorPosition = default)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = size == default ? Vector2.zero : size;
    }

    public static void MidStretch(this RectTransform rectTransform, float height = 10, float posY = 0)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.up * 0.5f;
        rectTransform.anchorMax = new Vector2(1f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0, posY);
        rectTransform.sizeDelta = new Vector2(0, height);
    }

    public static void TopStretch(this RectTransform rectTransform, float height = 10, float posY = 0)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.up;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0, posY);
        rectTransform.sizeDelta = new Vector2(0, height);
    }

    public static void BottomStretch(this RectTransform rectTransform, float height = 10, float posY = 0)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.right;
        rectTransform.pivot = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, posY);
        rectTransform.sizeDelta = new Vector2(0, height);
    }

    public static void RightStretch(this RectTransform rectTransform, float width = 30, float posX = 0)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.up;
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = new Vector2(posX, 0);
        rectTransform.sizeDelta = new Vector2(width, 0);
    }

    public static Vector2 ConvertToPositionOfTarget(this RectTransform rectTransform, Camera camera, RectTransform target, Camera targetCamera)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(target, position, targetCamera, out position);
        return position;
    }
}