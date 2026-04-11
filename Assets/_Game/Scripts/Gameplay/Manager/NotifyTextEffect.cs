using UnityEngine;
public class NotifyTextEffect : MonoBehaviour
{
    [SerializeField] private TextImageEffect _excellent;
    [SerializeField] private TextImageEffect _allClear;
    [SerializeField] private TextImageEffect _newScore;
    [SerializeField] private TextImageEffect _good;
    [SerializeField] private TextImageEffect _great;
    [SerializeField] private TextImageEffect _perfect;
    [SerializeField] private TextImageEffect _amazing;

    [SerializeField] private BigComboEffect _bigComboEffect;

    private void OnEnable()
    {
        GameEvents.GoodEffect += ShowGood;
        GameEvents.GreatEffect += ShowGreat;
        GameEvents.PerfectEffect += ShowPerfect;
        GameEvents.AmazingEffect += ShowAmazing;
        GameEvents.NewScoreEffect += ShowNewScore;
        GameEvents.AllClearEffect += ShowAllClear;
        GameEvents.ExcellentEffect += ShowExcellent;

        GameEvents.ComboEfect += ShowComboEffect;

        GameEvents.OnEndGame += PreventEffect;

        PreventEffect();

    }

    private void OnDisable()
    {
        GameEvents.GoodEffect -= ShowGood;
        GameEvents.GreatEffect -= ShowGreat;
        GameEvents.PerfectEffect -= ShowPerfect;
        GameEvents.AmazingEffect -= ShowAmazing;
        GameEvents.NewScoreEffect -= ShowNewScore;
        GameEvents.AllClearEffect -= ShowAllClear;
        GameEvents.ExcellentEffect -= ShowExcellent;

        GameEvents.ComboEfect -= ShowComboEffect;

        GameEvents.OnEndGame -= PreventEffect;
    }
    private void PreventEffect()
    {
        if (_good)
            _good.gameObject.SetActive(false);
        if (_excellent)
            _excellent.gameObject.SetActive(false);
        if (_allClear)
            _allClear.gameObject.SetActive(false);
        if (_newScore)
            _newScore.gameObject.SetActive(false);
        if (_great)
            _great.gameObject.SetActive(false);
        if (_perfect)
            _perfect.gameObject.SetActive(false);
        if (_amazing)
            _amazing.gameObject.SetActive(false);
        if (_bigComboEffect)
            _bigComboEffect.gameObject.SetActive(false);
    }

    private void ShowGood()
    {
        if (_good == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.goodFX.Play();
        _good.Play();
    }

    private void ShowGreat()
    {
        if (_great == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.greatFX.Play();
        _great.Play();
    }

    private void ShowPerfect()
    {
        if (_perfect == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.perf.Play();
        // SoundManager.Ins.perfectFX.Play();
        _perfect.Play();
    }
    // Small: take 0.7s
    private void ShowAmazing()
    {
        if (_amazing == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.amazingFX.Play();
        _amazing.Play();
    }

    private void ShowNewScore()
    {
        if (_newScore == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.newBestScoreInGameFx.Play();
        _newScore.Play();
    }

    private void ShowAllClear()
    {
        if (_allClear == null || !GameplayManager.Ins.IsInGame) return;
        _allClear.Play();
    }

    private void ShowExcellent()
    {
        if (_excellent == null || !GameplayManager.Ins.IsInGame) return;
        // SoundManager.Ins.excellentFX.Play();
        _excellent.Play();
    }

    private void ShowComboEffect(int combo)
    {
        if (combo <= 0 || !GameplayManager.Ins.IsInGame) return;
        _bigComboEffect.Play(combo);
    }
}
