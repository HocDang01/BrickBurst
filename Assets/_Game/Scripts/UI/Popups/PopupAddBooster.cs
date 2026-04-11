using System;
using UnityEngine;
using UnityEngine.UI;
public class PopupAddBooster : BaseUI<PopupAddBooster>
{

    // public static PopupAddBooster Show() => Show<PopupAddBooster>(PopupConsts.POPUPADDBOOSTER);

    // [SerializeField] private UIRewardAdsButton _uIRewardAdsButton;
    [SerializeField] private Button _uIRewardAdsButton;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _bombSprite;
    [SerializeField] private Sprite _eraseShapeSprite;
    [SerializeField] private Sprite _oneTileSprite;
    [SerializeField] private Sprite _rerollSprite;

    private BoosterType _boosterType;
    protected override void Awake()
    {
        base.Awake();
        _uIRewardAdsButton.onClick.AddListener(OnGetReward);
        _closeBtn.onClick.AddListener(Hide);
        transform.position = Vector3.zero;
    }

    public void Init(BoosterType boosterType)
    {
        _boosterType = boosterType;
        Sprite sprite = GetSprite(boosterType);
        _icon.sprite = sprite;
        // _uIRewardAdsButton.adsPlacementName = GetAdPlacement(boosterType);
        // _uIRewardAdsButton.action = GetContextAction(boosterType);

    }
    private void OnGetReward()
    {
        BoosterManager.Ins.AddBooster(_boosterType);
        Hide();
    }
    private string GetAdPlacement(BoosterType boosterType)
    {
        switch (boosterType)
        {
            case BoosterType.Bomb:
                return "brickburst_rw_booster_bomb";
            case BoosterType.EraseShape:
                return "brickburst_rw_booster_erase";
            case BoosterType.OneTile:
                return "brickburst_rw_booster_oneblock";
            case BoosterType.Reroll:
                return "brickburst_rw_booster_reroll";
        }
        return default;
    }
    private string GetContextAction(BoosterType boosterType)
    {
        switch (boosterType)
        {
            case BoosterType.Bomb:
                return "rw_booster_bomb";
            case BoosterType.EraseShape:
                return "rw_booster_erase";
            case BoosterType.OneTile:
                return "rw_booster_oneblock";
            case BoosterType.Reroll:
                return "rw_booster_reroll";
        }
        return default;
    }
    private Sprite GetSprite(BoosterType boosterType)
    {
        switch (boosterType)
        {
            case BoosterType.Bomb:
                return _bombSprite;
            case BoosterType.EraseShape:
                return _eraseShapeSprite;
            case BoosterType.OneTile:
                return _oneTileSprite;
            case BoosterType.Reroll:
                return _rerollSprite;
        }
        return null;
    }

}
