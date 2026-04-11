using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterEraseShape : Booster
{
    [SerializeField] private Button _uIButton;
    [SerializeField] private Button _overlayButton;

    [SerializeField] private Transform _initParent;
    [SerializeField] private Transform _frontOverlay;
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        _uIButton.onClick.AddListener(OnClickUseBooster);
        _overlayButton.onClick.AddListener(OnClickUseBooster);
    }
    private void OnClickUseBooster()
    {
        if (BoosterManager.Ins.BoosterTypeUsing != BoosterType)
        {
            if (Amount <= 0) return;
            OnUsing();
            transform.SetParent(_frontOverlay, false);
            // transform.parent = _frontOverlay;
            BoosterManager.Ins.BoosterTypeUsing = BoosterType;
            BoosterManager.Ins.EnableEraseShape(true);
        }
        else
        {
            OnEndUsing();
            transform.SetParent(_initParent, false);
            // transform.parent = _initParent;
            BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
            BoosterManager.Ins.EnableEraseShape(false);
        }
    }
    public void BackToInitScale()
    {
        transform.SetParent(_initParent, false);
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
        BoosterManager.Ins.EnableEraseShape(false);
        OnEndUsing();
    }
}
