using System.Collections;
using DangExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopupFakeLoading : BaseUI<PopupFakeLoading>
{
    [Header("UI")]
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _percentText;

    private void Start()
    {
        ResetUI();
        StartCoroutine(LoadingFlow());
    }

    private void ResetUI()
    {
        SetProgress(0f);
    }

    private IEnumerator LoadingFlow()
    {
        yield return WaitTimeCache.Wait1;                                  // 0.3s

        yield return AnimateTo(0.6f, 1.2f);   // nhanh đầu,     0.6s
        yield return WaitTimeCache.Wait0_5;                                 // 0.2s    

        float duration = Random.Range(1f, 2.4f);
        yield return AnimateTo(0.95f, duration * 0.95f);
        yield return AnimateTo(1f, duration * 0.05f);     // chậm cuối  // 1.2s
                                                          // Total: 2.3s
        Hide();
    }

    #region Core Animation

    private IEnumerator AnimateTo(float target, float duration)
    {
        float start = _slider.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = EaseOut(t); // ⭐ quan trọng

            float value = Mathf.Lerp(start, target, t);
            SetProgress(value);

            yield return null;
        }

        SetProgress(target);
    }

    private float EaseOut(float t)
    {
        // SmoothStep = ease out tự nhiên
        return Mathf.SmoothStep(0f, 1f, t);
    }

    #endregion

    #region Helpers

    private void SetProgress(float value)
    {
        _slider.value = value;
        _percentText.text = Mathf.RoundToInt(value * 100f).ToString("00") + "%";
    }

    #endregion
}
