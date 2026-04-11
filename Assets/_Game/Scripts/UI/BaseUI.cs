using UnityEngine;

public abstract class BaseUI<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Ins => _instance;

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    public static void Show()
    {
        if (Ins == null) return;
        Ins.gameObject.SetActive(true);
        (Ins as BaseUI<T>)?.OnShow();
    }

    public static void Hide()
    {
        if (Ins == null) return;
        (Ins as BaseUI<T>)?.OnHide();
        Ins.gameObject.SetActive(false);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}