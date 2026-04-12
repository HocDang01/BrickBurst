using System;
using UnityEngine;
using UnityEngine.UI;
public class PopupSetting : BaseUI<PopupSetting>
{
    [SerializeField] private ButtonSound _musicBtn;
    [SerializeField] private ButtonSound _soundBtn;
    [SerializeField] private ButtonSound _vibrationBtn;

    [SerializeField] private Button _homeBtn;
    [SerializeField] private Button _replayBtn;

    [SerializeField] private GameObject _nativeAds;

    private Action _replayAction;
    private Action _homeAction;
    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
    }
    void Start()
    {
        _musicBtn.uIButton.onClick.AddListener(ToggleMusic);
        _soundBtn.uIButton.onClick.AddListener(ToggleSound);
        _vibrationBtn.uIButton.onClick.AddListener(ToggleVibration);

        _musicBtn.SetState(UserProperty.MusicOn);
        _soundBtn.SetState(UserProperty.SfxOn);
        _vibrationBtn.SetState(UserProperty.HapticOn);


        _homeBtn.onClick.AddListener(OnClickHome);
        _replayBtn.onClick.AddListener(OnClickReplay);
    }

    public void SetActionReplay(Action replay)
    {
        _replayAction = replay;
    }
    public void SetActionHome(Action home)
    {
        _homeAction = home;
    }
    protected override void OnShow()
    {
        base.OnShow();
        _nativeAds.SetActive(true);
    }
    protected override void OnHide()
    {
        base.OnHide();
        _nativeAds.SetActive(false);
    }



    private void OnClickHome()
    {
        if (_homeAction != null)
            _homeAction?.Invoke();
        Hide();
    }
    private void OnClickReplay()
    {
        if (_replayAction != null)
            _replayAction?.Invoke();
        Hide();
    }

    public void SetInMainMenu(bool isMainMenu)
    {
        _replayBtn.gameObject.SetActive(!isMainMenu);
    }


    private void ToggleMusic()
    {
        bool music = !UserProperty.MusicOn;
        UserProperty.MusicOn = music;
        _musicBtn.Toggle(music);
        UserProperty.Save();

        SoundManager.Ins.SetMusicEnabled(music);
    }

    private void ToggleSound()
    {
        bool sound = !UserProperty.SfxOn;
        UserProperty.SfxOn = sound;
        _soundBtn.Toggle(sound);
        UserProperty.Save();

        SoundManager.Ins.SetSfxEnabled(sound);
    }

    private void ToggleVibration()
    {
        bool vibration = !UserProperty.HapticOn;
        UserProperty.HapticOn = vibration;
        _vibrationBtn.Toggle(vibration);
        UserProperty.Save();
    }
}
