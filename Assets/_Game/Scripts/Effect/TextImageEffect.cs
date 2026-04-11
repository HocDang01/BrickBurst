using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TextImageEffect : MonoBehaviour
{
    [SerializeField] private TextImageType _textImageType;
    [SerializeField] private Transform _childMove;
    [SerializeField] private List<Transform> _lights;
    [SerializeField] private Vector2 _lightsPos = new Vector2(1000f, 1700f);
    [SerializeField] private float _delayAfterHide = 0f;

    [Header("Scale")]
    [SerializeField] private float _scaleUp = 1.2f;
    [SerializeField] private float _scaleUpDuration = 0.5f;
    [SerializeField] private float _scaleDownDuration = 0.15f;

    [Header("Move")]
    [SerializeField] private float _moveUpDistance = 100f;
    [SerializeField] private float _moveDuration = 0.5f;

    private Vector3 _startPos;
    private Vector3 _startScale;

    private Sequence _sequence;

    void Awake()
    {
        _startPos = _childMove.localPosition;
        _startScale = Vector3.zero;
    }
    void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }

    public void Play()
    {
        // 🔥 Kill sequence cũ nếu có
        _sequence?.Kill();
        _sequence = null;
        switch (_textImageType)
        {
            case TextImageType.Big:
                PlayBig();
                break;
            case TextImageType.Small:
                PlaySmall();
                break;
            default:
                PlayBig();
                break;
        }
        Debug.Log("Play");
        // RandomizeLightPositions();
    }

    // Waste 1.065
    private void PlayBig()
    {
        gameObject.SetActive(true);

        _childMove.localPosition = _startPos;
        transform.localScale = Vector3.zero;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        // 1️⃣ POP mạnh (overshoot lớn)
        _sequence.Append(
            transform.DOScale(_scaleUp * 1.2f, _scaleUpDuration * 0.8f)
                .SetEase(Ease.OutBack)
        );

        // 2️⃣ HIT lần 2 + giật
        _sequence.Append(
            transform.DOPunchScale(Vector3.one * 0.25f, 0.15f, 10, 1f)
        );

        // 3️⃣ Nhảy mạnh lên + rung
        // _sequence.Join(
        //     _childMove.DOLocalMoveY(
        //         _startPos.y + _moveUpDistance * 1.3f,
        //         0.12f
        //     )
        //     .SetLoops(2, LoopType.Yoyo)
        //     .SetEase(Ease.OutFlash)
        // );

        _sequence.Join(
            transform.DOShakePosition(0.15f, 15f, 20)
        );

        // 4️⃣ Hạ về size chuẩn (cảm giác nặng)
        _sequence.Append(
            transform.DOScale(1f, _scaleDownDuration * 0.7f)
                .SetEase(Ease.InOutSine)
        );

        // 5️⃣ Hold cho người nhìn
        _sequence.AppendInterval(_delayAfterHide * 0.8f);

        // 6️⃣ Tắt gắt – dứt khoát
        _sequence.Append(
            transform.DOScale(0f, 0.15f)
                .SetEase(Ease.InBack)
        );

        _sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            _sequence = null;
        });
    }

    // Waste 0.6s
    private void PlaySmall()
    {
        gameObject.SetActive(true);

        _childMove.localPosition = _startPos;
        transform.localScale = Vector3.zero;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        // POP nhanh
        _sequence.Append(
            transform.DOScale(_scaleUp * 1.1f, _scaleUpDuration * 0.5f)
                .SetEase(Ease.OutBack)
        );

        // Giật nhẹ
        _sequence.Append(
            transform.DOPunchScale(Vector3.one * 0.15f, 0.12f, 8, 0.8f)
        );

        _sequence.AppendInterval(_delayAfterHide * 0.5f);

        // Tắt nhanh
        _sequence.Append(
            transform.DOScale(0f, 0.12f)
                .SetEase(Ease.InBack)
        );

        _sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            _sequence = null;
        });
    }



    private void RandomizeLightPositions()
    {
        if (_lights == null || _lights.Count <= 0) return;
        float angleStep = 360f / _lights.Count;
        float maxRadius = Mathf.Min(_lightsPos.x, _lightsPos.y);

        for (int i = 0; i < _lights.Count; i++)
        {
            float angle = i * angleStep;
            // Use square root to distribute more evenly across radius
            float radius = maxRadius * Mathf.Sqrt(Random.value);

            float posX = transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float posY = transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            Vector3 spawnPosition = new Vector3(posX, posY, _lights[i].position.z);
            _lights[i].position = spawnPosition;
        }
    }

    private enum TextImageType
    {
        Big,
        Small,
    }
}
