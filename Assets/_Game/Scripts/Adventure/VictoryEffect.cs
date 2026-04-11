using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryEffect : MonoBehaviour
{
    [SerializeField] private Image _victory;

    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = _victory.transform.localPosition;
    }

    public void Play()
    {
        gameObject.SetActive(true);
        PlayAnim();
    }

    private void PlayAnim()
    {
        _victory.transform.DOKill();
        _victory.DOFade(1f, 0f);

        // reset
        _victory.transform.localScale = Vector3.one * 3.5f;
        _victory.transform.localPosition = _startPos + Vector3.up * 600f;

        Sequence seq = DOTween.Sequence();

        seq.Append(_victory.transform.DOLocalMoveY(_startPos.y, 0.4f)   // ⬅️ 0.35 -> 0.7
            .SetEase(Ease.InQuad))

           .Join(_victory.transform.DOScale(1f, 0.4f)
            .SetEase(Ease.InQuad))

           // đập mạnh + nảy (cũng kéo dài hơn)
           .Append(_victory.transform.DOScale(1.25f, 0.18f).SetEase(Ease.OutQuad)) // 0.12 -> 0.18
           .Append(_victory.transform.DOScale(0.9f, 0.14f).SetEase(Ease.InQuad))   // 0.1 -> 0.14
           .Append(_victory.transform.DOScale(1f, 0.14f).SetEase(Ease.OutQuad))    // 0.1 -> 0.14
            .AppendCallback(() =>
            {
                // pulse scale
                _victory.transform.DOScale(1.05f, 0.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);

                // float lên xuống
                _victory.transform.DOLocalMoveY(_startPos.y + 20f, 0.8f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            })
            .AppendInterval(0.6f)
            .AppendCallback(() =>
            {
                _victory.transform.DOKill(); // stop pulse + float
            })
            .Append(_victory.transform.DOScale(0.3f, 0.4f).SetEase(Ease.InBack))
            .Join(_victory.DOFade(0f, 0.35f))
           .OnComplete(() =>
           {
               gameObject.SetActive(false);
           });
    }

    public float GetTime()
    {
        return 0.4f     // rơi + scale
             + 0.18f
             + 0.14f
             + 0.14f
            + 0.6f
            + 0.4f;
    }

}
