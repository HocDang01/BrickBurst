using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class PopupWatchAds : BaseUI<PopupWatchAds>
{
    // [SerializeField] private UIRewardAdsButton _watchRewardAdsBtn;
    [SerializeField] private Button _watchRewardAdsBtn;
    [SerializeField] private Button _watchAdsButton;
    [SerializeField] private Button _watchAdsButtonTest;
    [SerializeField] private Button _noThanksBtn;
    // [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

    [SerializeField] private List<ShapeRevive> _shapeRevives;

    // private bool _isWatch = false;
    private List<TileColor> _tileColors;

    private Action _onContinue;
    private Action _onLoseGame;

    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
        _watchRewardAdsBtn.onClick.AddListener(OnWatchAdsReward);
        _noThanksBtn.onClick.AddListener(OnDontWatchAds);
        _watchAdsButtonTest.onClick.AddListener(OnWatchAdsReward);
    }
    public void SpawnShape(List<ShapeData> selectedShapes)
    {
        _tileColors = GameConfig.Ins.TileColorConfig.ColorSprites;
        GameplayManager.Ins.IsHardTurn = false;
        // Thực hiện Spawn thật lên màn hình
        for (int i = 0; i < 3; i++)
        {
            _shapeRevives[i].DestroyShape();
            int colorIndex = UnityEngine.Random.Range(0, _tileColors.Count);
            _shapeRevives[i].RequestNewShape(selectedShapes[i], _tileColors[colorIndex]);
            Debug.Log("Shape " + i + ": " + selectedShapes[i].name);
        }

    }
    public void SetAction(Action continueAction = null, Action loseGameAction = null)
    {
        _onContinue = continueAction;
        _onLoseGame = loseGameAction;
    }

    private void OnWatchAdsReward()
    {
        Debug.Log("Watch Ads");
        if (_onContinue != null)
            _onContinue?.Invoke();
        Hide();
    }

    private void OnDontWatchAds()
    {
        Debug.Log("No thanks");
        if (_onLoseGame != null)
            _onLoseGame?.Invoke();
        Hide();
    }
}
