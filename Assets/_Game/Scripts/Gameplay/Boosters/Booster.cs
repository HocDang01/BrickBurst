using System;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Booster : MonoBehaviour
{
    public BoosterType BoosterType;
    [SerializeField] protected Transform _scaleableTransform;
    [SerializeField] protected Button _addBtn;
    [SerializeField] protected GameObject _amountObject;
    [SerializeField] protected TextMeshProUGUI _amountText;
    public GameObject _highLightObject;

    private float usingScale = 1.25f;
    protected Vector3 _initScale;
    public int Amount;

    public float UsingScale { get => usingScale; set => usingScale = value; }

    protected virtual void Awake()
    {
        _initScale = _scaleableTransform.localScale;
        _addBtn.onClick.AddListener(OnClickAddBtn);
        _highLightObject.SetActive(false);
    }

    protected virtual void OnClickAddBtn()
    {
        Debug.Log("OnClickAddBtn");
        // var popup = PopupAddBooster.Show();
        // popup.Init(BoosterType);
    }

    public void OnUsing()
    {
        _highLightObject.SetActive(true);
        _scaleableTransform.localScale = _initScale * UsingScale;

    }
    public void OnEndUsing()
    {
        _highLightObject.SetActive(false);
        _scaleableTransform.localScale = _initScale;
    }

    public void UpdateAmount(int amount)
    {
        Amount = amount;
        _amountObject.SetActive(amount > 0);
        _addBtn.gameObject.SetActive(amount <= 0);
        _amountText.text = amount.ToString();
    }
}

public enum BoosterType
{
    None,
    Bomb,
    EraseShape,
    Reroll,
    OneTile,
}

