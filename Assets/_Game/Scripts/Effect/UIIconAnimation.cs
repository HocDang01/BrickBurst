using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIIconAnimation : MonoBehaviour
{
    /// <summary>
    /// This script will be assigned into effects have infinity animation like light, ...
    /// </summary>
    [SerializeField] private AnimType _animType;
    [SerializeField] private float _duration = 0.5f;

    [Header("Scale")]
    [SerializeField] private float _scale = 1.2f;

    [Header("Ring")]
    [SerializeField] private Image _ringImage;
    private Vector3 _initScale;

    void Awake()
    {
        _initScale = transform.localScale;
    }
    void OnEnable()
    {
        PlayAnim();
    }
    void OnDisable()
    {
        transform.DOKill();
        _ringImage?.DOKill();
    }

    private void PlayAnim()
    {
        switch (_animType)
        {
            case AnimType.Rotate:
                RotateAnim();
                break;
            case AnimType.Scale:
                ScaleAnim();
                break;
            case AnimType.RingImage:
                RingImageAnim();
                break;
            default:
                break;
        }
    }

    private void RotateAnim()
    {
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DORotate(
            new Vector3(0, 0, 360f),
            _duration,
            RotateMode.FastBeyond360
        )
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);
    }

    private void ScaleAnim()
    {
        transform.localScale = _initScale;
        transform.DOKill();
        transform.DOScale(_initScale * _scale, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
    }

    private void RingImageAnim()
    {
        if (_ringImage == null) return;
        transform.localScale = _initScale;
        transform.DOKill();
        _ringImage.DOKill();
        _ringImage.DOFade(0f, 0f);
        transform.DOScale(_initScale * _scale, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);

        _ringImage.DOFade(0.8f, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
    }


}

public enum AnimType
{
    None,
    Scale,
    Rotate,
    RingImage,
}
