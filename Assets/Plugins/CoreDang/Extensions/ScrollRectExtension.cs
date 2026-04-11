using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExtensions
{
    public static void SmoothScrollTo(this ScrollRect scrollRect, string childName, float duration = 0.5f)
    {
        Transform child = scrollRect.content.transform.Find(childName);
        if(!child) return;
        RectTransform target = child.GetComponent<RectTransform>();
        if(target) scrollRect.SmoothScrollTo(target, duration);
    }

    public static void SmoothScrollTo(this ScrollRect scrollRect, RectTransform target, float duration = 0.5f)
    {
        SmoothScrollTo(scrollRect, target, Vector2.zero, Vector2.zero, duration);
    }

    public static void SmoothScrollTo(this ScrollRect scrollRect, RectTransform target, Vector2 offset, Vector2 fixedAxis, float duration = 0.5f)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform viewport = scrollRect.viewport;
        RectTransform content = scrollRect.content;

        Vector2 deltaSize = content.rect.size - viewport.rect.size;
        Vector2 result =
            (Vector2)viewport.transform.InverseTransformPoint(content.position)
            - (Vector2)viewport.transform.InverseTransformPoint(target.position);
        
        HorizontalLayoutGroup horizontal = scrollRect.content.GetComponent<HorizontalLayoutGroup>();
        if(horizontal) result.x += horizontal.padding.left;

        VerticalLayoutGroup vertical = scrollRect.content.GetComponent<VerticalLayoutGroup>();
        if(vertical) result.y -= vertical.padding.top;

        Vector2 min = deltaSize * (content.pivot - Vector2.one);
        Vector2 max = deltaSize * content.pivot;
        result.x = Mathf.Clamp(result.x, min.x, max.x);
        result.y = Mathf.Clamp(result.y, min.y, max.y);

        if(duration == 0) scrollRect.content.localPosition = result;
        else scrollRect.content.DOLocalMove(result, duration, true);
    }
}