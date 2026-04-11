
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupEndGame<T> : BaseUI<T> where T : PopupEndGame<T>
{
    [SerializeField] protected Button _replayBtn;
    [SerializeField] protected Image _titleImg;
    [SerializeField] protected Sprite _loseSprite;
    [SerializeField] protected Sprite _winSprite;

    [SerializeField] protected RectTransform _titleRect;
    [SerializeField] protected RectTransform _centerPoint;
    [SerializeField] protected Transform _content;
    [SerializeField] protected GameObject _nativeAds;
    protected Vector3 _originPosTitle;
    protected override void Awake()
    {
        base.Awake();
        _originPosTitle = _titleRect.localPosition;
        transform.position = Vector3.zero;
    }
    protected virtual void Start()
    {
    }
    protected override void OnShow()
    {
        base.OnShow();
        _nativeAds.SetActive(true);
    }
    protected override void OnHide()
    {
        _nativeAds.SetActive(false);
        base.OnHide();
    }
    protected virtual void Replay() { }
    #region StartAnim
    protected void StartAnimation(bool isWin)
    {
        // Reset states
        _titleRect.position = _centerPoint.position;
        // Reset scale
        _titleRect.localScale = Vector3.zero;
        _content.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence
            .SetUpdate(true)
            .Append(_titleRect.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack))
            .Append(_titleRect.DOScale(1f, 0.3f))
            .Append(_titleRect.DOLocalMove(_originPosTitle, 0.8f).SetEase(Ease.OutQuad))
            .AppendCallback(() =>
            {
                if (isWin)
                {
                    //@TODO: Spawn VFX Win
                }
            })
            .Append(_content.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

    }
    #endregion
}
