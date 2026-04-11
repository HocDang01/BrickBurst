using UnityEngine;
using UnityEngine.UI;

public class OutlineScoreEffect : MonoBehaviour
{
    private Image _outlineImg;
    private Color _cachedColor;

    private void Awake()
    {
        _outlineImg = GetComponent<Image>();
        _cachedColor = _outlineImg.color;
    }

    private void OnEnable()
    {
        ApplyAlpha(); // sync ngay khi bật
    }

    private void Update()
    {
        ApplyAlpha(); // follow master clock
    }

    private void ApplyAlpha()
    {
        if (OutlineFadeController.Ins == null) return;

        _cachedColor.a = OutlineFadeController.Ins.CurrentAlpha;
        _outlineImg.color = _cachedColor;
    }

    private void OnDisable()
    {
        // optional: reset về min alpha
        _cachedColor.a = 0f;
        _outlineImg.color = _cachedColor;
    }
}
