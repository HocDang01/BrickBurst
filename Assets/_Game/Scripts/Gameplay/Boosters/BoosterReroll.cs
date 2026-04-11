using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class BoosterReroll : Booster
{
    [SerializeField] private Button _uIButton;

    protected override void Awake()
    {
        base.Awake();
        _uIButton.onClick.AddListener(OnClickUse);
    }
    private void OnClickUse()
    {
        if (GameplayManager.Ins.IsInGame == false || Amount <= 0)
        {
            return;
        }
        GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.TwoBeauty);
        BoosterManager.Ins.UseBooster(BoosterType.Reroll);
    }
}
