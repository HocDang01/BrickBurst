using System.Collections.Generic;
using CoreDang;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    [Header("Music")]
    public AudioClip mainMenuMusic;
    public List<AudioClip> ingameTracks;

    [Header("SFX")]
    public AudioClip newBestScoreInGameFx;
    public AudioClip newBestScoreUpRankEndGameFx;
    public AudioClip newRecordPopupShowFx;
    public AudioClip normalScoredFx;
    public AudioClip putTilesFx;
    public AudioClip pickupTileFx;
    public AudioClip breakTile;
    public AudioClip settingsPopupShowFx;

    public AudioClip amazingFX;
    public AudioClip excellentFX;
    public AudioClip failPlayFX;
    public AudioClip goodFX;
    public AudioClip greatFX;
    public AudioClip perfectFX;
    public AudioClip unbelieveableFX;
    public AudioClip winFX;
    public AudioClip columns23;
    public AudioClip columns4;
    public AudioClip columns5;

    public List<AudioClip> comboScoreFXs;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    protected override void Awake()
    {
        base.Awake();

        // tạo 2 AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        Subscribe();

    }
    protected override void Start()
    {
        base.Start();
        SetMusicEnabled(UserProperty.MusicOn);
        SetSfxEnabled(UserProperty.SfxOn);
    }

    protected override void OnDestroy()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        GameEvents.OnStartGame += PlayIngameMusic;
    }

    void Unsubscribe()
    {
        GameEvents.OnStartGame -= PlayIngameMusic;
    }

    // ================= MUSIC =================

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        musicSource.volume = enabled ? 1f : 0f;
    }

    public void PlayMainMenuMusic()
    {
        if (!musicEnabled) return;

        musicSource.Stop();
        musicSource.clip = mainMenuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayIngameMusic()
    {
        if (!musicEnabled || ingameTracks.Count == 0) return;

        musicSource.Stop();

        int index = Random.Range(0, ingameTracks.Count);
        musicSource.clip = ingameTracks[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopInGameMusic()
    {
        musicSource.Stop();
    }

    // ================= SFX =================

    public void SetSfxEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        sfxSource.volume = enabled ? 1f : 0f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}