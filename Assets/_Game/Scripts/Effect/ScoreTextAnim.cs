using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreTextAnim : MonoBehaviour
{
    [SerializeField] private bool _isBestScore;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private string _preFix;

    [Header("Speed")]
    [SerializeField] private float _pointsPerSecond = 80f;
    [SerializeField] private float _minDuration = 0.3f;
    [SerializeField] private float _maxDuration = 0.8f;

    [Header("Step Count")]
    [SerializeField] private int _smallStep = 1;
    [SerializeField] private int _mediumStep = 5;
    [SerializeField] private int _largeStep = 10;

    [SerializeField] private int _currentScore = 0;
    private Tween _scoreTween;
    // private float _lastTickTime;
    void Awake()
    {
        _currentScore = 0;
    }
    void OnEnable()
    {
        if (!_isBestScore)
        {
            RefreshScore();
        }
    }

    public void RefreshScore()
    {
        _scoreTween?.Kill();
        _currentScore = 0;
        _scoreText.text = $"{_preFix}0";
    }

    public void SetScore(int newScore)
    {
        _scoreTween?.Kill();

        int startValue = _currentScore;
        int delta = Mathf.Abs(newScore - startValue);
        if (delta == 0)
        {
            _scoreText.text = $"{_preFix}{Utility.FormatNumber(newScore)}";
            return;
        }
        // 1️⃣ duration thông minh
        float duration = delta / _pointsPerSecond;
        duration = Mathf.Clamp(duration, _minDuration, _maxDuration);

        // 2️⃣ chọn step theo delta
        int step = GetStep(delta);

        int displayValue = startValue;

        _scoreTween = DOTween.To(
            () => displayValue,
            x =>
            {
                // snap theo step
                x = Snap(x, step, newScore);

                if (x == _currentScore) return;

                displayValue = x;
                _currentScore = x;
                _scoreText.text = $"{_preFix}{Utility.FormatNumber(x)}";

                // TryPlayTick();
            },
            newScore,
            duration
        )
        .SetEase(Ease.OutQuad);
    }

    public void SetCashForAdventure(int newScore, bool isAnchor)
    {
        _scoreTween?.Kill();

        int startValue = _currentScore;
        int delta = Mathf.Abs(newScore - startValue);
        if (delta == 0)
        {
            if (isAnchor)
            {
                _scoreText.text = $"<color=#F6E13B>{_preFix}{Utility.FormatNumberWithoutPostFix(newScore, '.')}</color>";
            }
            else
            {
                _scoreText.text = $"{_preFix}{Utility.FormatNumberWithoutPostFix(newScore, '.')}";
            }
            return;
        }
        // 1️⃣ duration thông minh
        float duration = delta / _pointsPerSecond;
        duration = Mathf.Clamp(duration, _minDuration, _maxDuration);

        // 2️⃣ chọn step theo delta
        int step = GetStep(delta);

        int displayValue = startValue;

        _scoreTween = DOTween.To(
            () => displayValue,
            x =>
            {
                // snap theo step
                x = Snap(x, step, newScore);

                if (x == _currentScore) return;

                displayValue = x;
                _currentScore = x;
                if (isAnchor)
                {
                    _scoreText.text = $"<color=#FFC012>{_preFix}{Utility.FormatNumberWithoutPostFix(x, '.')}</color>";
                }
                else
                {
                    _scoreText.text = $"{_preFix}{Utility.FormatNumberWithoutPostFix(x, '.')}";
                }

                // TryPlayTick();
            },
            newScore,
            duration
        )
        .SetEase(Ease.OutQuad);
    }

    private int GetStep(int delta)
    {
        if (delta < 30) return _smallStep;
        if (delta < 150) return _mediumStep;
        return _largeStep;
    }

    private int Snap(int value, int step, int target)
    {
        if (step <= 1) return value;

        int snapped = (value / step) * step;

        // đảm bảo chạm đúng target
        if (Mathf.Abs(target - snapped) < step)
            snapped = target;

        return snapped;
    }

    // private void TryPlayTick()
    // {
    //     if (_audioSource == null || _tickSound == null) return;

    //     if (Time.time - _lastTickTime < _tickCooldown) return;

    //     _audioSource.PlayOneShot(_tickSound);
    //     _lastTickTime = Time.time;
    // }

    public void SetScoreImmediate(int score)
    {
        _scoreTween?.Kill();
        _currentScore = score;
        _scoreText.text = $"{_preFix}{Utility.FormatNumber(score)}";
    }
}
