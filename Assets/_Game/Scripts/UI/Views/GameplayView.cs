using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameplayView : BaseUI<GameplayView>
{
    [SerializeField] private GameObject _bestScoreClassic;
    [SerializeField] private Button _backBtn;

    private bool _isFirstEnable = true;
    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
        _backBtn.onClick.AddListener(OnClickBack);
        _isFirstEnable = true;
    }

    private void OnClickBack()
    {
        MainMenu.Show();
        GameEvents.OnBackMainMenu?.Invoke(false);
    }

    protected override void OnShow()
    {
        if(_isFirstEnable)
        {
            BoardManager.Ins.OnFirstEnable();
        }
        _isFirstEnable = false;
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                _backBtn.gameObject.SetActive(false);
                _bestScoreClassic.gameObject.SetActive(true);
                break;
            case PlayMode.Adventure:
                _backBtn.gameObject.SetActive(true);
                _bestScoreClassic.gameObject.SetActive(false);
                break;
        }
    }
}
