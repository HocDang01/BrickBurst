using CoreDang;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        UserProperty.Load();
    }
    protected override void Start()
    {
        // View
        MainMenu.Show();
        GameplayView.Hide();

        // Popup
        PopupEndAdventure.Hide();
        PopupEndClassic.Hide();
        PopupSetting.Hide();
        PopupWatchAds.Hide();
    }
}