using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopupMoreSettings : BaseUI<PopupMoreSettings>
{
    [SerializeField] private TextMeshProUGUI _versionText;

    [SerializeField] private Button _contactUs;
    [SerializeField] private Button _shareFriends;
    [SerializeField] private Button _termService;
    [SerializeField] private Button _privacyPolicy;
    [SerializeField] private Button _aboutUs;

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        _privacyPolicy.onClick.AddListener(() =>
        {
            Application.OpenURL("");
        });
    }
    void OnEnable()
    {
        _versionText.text = $"Version: {Application.version}";
    }
}
