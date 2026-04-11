using System;
using UnityEngine;
using UnityEngine.UI;

public class Cheat : MonoBehaviour
{
    [SerializeField] private Button _cheat;
    [SerializeField] private GameObject _content;

    [Header("Buttons")]
    [SerializeField] private Button _win;
    [SerializeField] private Button _lose;
    [SerializeField] private Button _good;
    [SerializeField] private Button _great;
    [SerializeField] private Button _perfect;
    [SerializeField] private Button _amazing;
    [SerializeField] private Button _excellent;
    [SerializeField] private Button _newScore;
    [SerializeField] private Button _allClear;
    [SerializeField] private Button _addScore;

    private bool _isOpen;
    void Awake()
    {
        if (BBManager.EnableCheat)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void Start()
    {
        _isOpen = false;
        _content.SetActive(_isOpen);
        _cheat.onClick.AddListener(OnClickCheat);
        _win.onClick.AddListener(CheatWin);
        _lose.onClick.AddListener(CheatLose);
        _good.onClick.AddListener(CheatGood);
        _great.onClick.AddListener(CheatGreat);
        _perfect.onClick.AddListener(CheatPerfect);
        _amazing.onClick.AddListener(CheatAmazing);
        _excellent.onClick.AddListener(CheatExcellent);
        _newScore.onClick.AddListener(CheatNewScore);
        _allClear.onClick.AddListener(CheatAllClear);
        _addScore.onClick.AddListener(CheatAddScore);
    }

    private void CheatAddScore()
    {
        ScoreManager.Ins.CheatAddScore(300);
    }

    private void OnClickCheat()
    {
        _isOpen = !_isOpen;
        _content.SetActive(_isOpen);
    }
    private void CheatWin()
    {
        ScoreManager.Ins.CheatWin();
    }
    private void CheatLose()
    {
        BoardManager.Ins.CheatLose();
    }
    private void CheatGood()
    {
        GameEvents.GoodEffect?.Invoke();
    }
    private void CheatGreat()
    {
        GameEvents.GreatEffect?.Invoke();
    }
    private void CheatPerfect()
    {
        GameEvents.PerfectEffect?.Invoke();
    }
    private void CheatAmazing()
    {
        GameEvents.AmazingEffect?.Invoke();
    }
    private void CheatExcellent()
    {
        GameEvents.ExcellentEffect?.Invoke();
    }
    private void CheatNewScore()
    {
        GameEvents.NewScoreEffect?.Invoke();
    }
    private void CheatAllClear()
    {
        GameEvents.AllClearEffect?.Invoke();
    }

}
