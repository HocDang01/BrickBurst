using System;
using System.Collections;
using DangExtension;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : BaseUI<MainMenu>
{
    [Header("Object")]
    [SerializeField] private GameObject _mainMenuObject;
    [SerializeField] private GameObject _adventureMapObject;
    [Header("MainMenu Ref")]
    [SerializeField] private SkeletonGraphic _titleAnim;
    [SerializeField] private Button _classicBtn;
    [SerializeField] private Button _adventureBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private ParticleSystem _particle;

    private const string TITLE_ANIM = "animation";
    private Coroutine _titleAnimRoutine;

    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
    }

    void Start()
    {
        _settingBtn.onClick.AddListener(OnClickSetting);
        _classicBtn.onClick.AddListener(ClassicMode);
        _adventureBtn.onClick.AddListener(AdventureMode);

        GameEvents.OnBackMainMenu += OnBackMainMenu;
    }
    protected override void OnShow()
    {
        // SoundyManager.StopAllSounds();
        SoundManager.Ins.PlayMainMenuMusic();
        _titleAnimRoutine = StartCoroutine(PlayAnimTitleRoutine());
        // GameManager.Ins.bgPlayer.StopBGMusic();
    }
    protected override void OnHide()
    {
        if (_titleAnimRoutine != null)
        {
            StopCoroutine(_titleAnimRoutine);
            _titleAnimRoutine = null;
        }
    }
    void OnDestroy()
    {
        GameEvents.OnBackMainMenu -= OnBackMainMenu;
        _classicBtn.onClick.RemoveListener(ClassicMode);
        _adventureBtn.onClick.RemoveListener(AdventureMode);
    }
    private IEnumerator PlayAnimTitleRoutine()
    {
        while (true)
        {
            PlayAnimTitle();
            yield return WaitTimeCache.Wait3;
        }
    }
    private void PlayAnimTitle()
    {
        _particle.gameObject.SetActive(true);
        _particle.Play();
        _titleAnim.AnimationState.SetAnimation(0, TITLE_ANIM, false);

    }
    private void ClassicMode()
    {
        GameplayManager.Ins.PlayMode = PlayMode.Classic;
        GameplayManager.Ins.CanWatchAds = true;
        GameEvents.OnEnterGameplay?.Invoke();
        _particle.Stop();
        _particle.gameObject.SetActive(false);
        GameplayView.Show();
        Hide();
    }
    private void AdventureMode()
    {
        GameplayManager.Ins.PlayMode = PlayMode.Adventure;
        _particle.Stop();
        _particle.gameObject.SetActive(false);
        _adventureMapObject.gameObject.SetActive(true);
        _mainMenuObject.gameObject.SetActive(false);
    }
    private void OnBackMainMenu(bool isMainMenu)
    {
        _mainMenuObject.SetActive(isMainMenu);
        _adventureMapObject.gameObject.SetActive(!isMainMenu);
    }

    private void OnClickSetting()
    {
        PopupSetting.Show();
        var setting = PopupSetting.Ins;
        setting.SetInMainMenu(true);
    }
}
