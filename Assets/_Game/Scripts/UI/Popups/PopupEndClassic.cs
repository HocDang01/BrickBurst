using System;
using TMPro;
using UnityEngine;

public class PopupEndClassic : PopupEndGame<PopupEndClassic>
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _maxScoreText;

    private Action _onHide;

    protected override void Awake()
    {
        base.Awake();
        _replayBtn.onClick.AddListener(Replay);
    }
    void OnEnable()
    {
        _maxScoreText.text = Utility.FormatNumber(BBSaveData.Ins.BestScore);
    }

    protected override void Replay()
    {
        base.Replay();
        Hide();
    }

    protected override void OnHide()
    {
        base.OnHide();
        if(_onHide != null)
        {
            _onHide?.Invoke();
        }
    }

    public void SetScore(int score)
    {
        _scoreText.text = Utility.FormatNumber(score);
    }
    public void SetTitle(bool isNewScore)
    {
        _titleImg.sprite = isNewScore ? _winSprite : _loseSprite;
        StartAnimation(isNewScore);
    }

    public void SetActionOnHide(Action action)
    {
        _onHide = action;
    }
}
