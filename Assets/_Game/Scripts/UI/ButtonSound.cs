using UnityEngine;
using UnityEngine.UI;
public class ButtonSound : MonoBehaviour
{
    public Button uIButton;
    [SerializeField] private GameObject _muteObject;
    public void Toggle(bool enable)
    {
        _muteObject.SetActive(!enable);
    }

    public void SetState(bool enable)
    {
        _muteObject.SetActive(!enable);
    }

}

