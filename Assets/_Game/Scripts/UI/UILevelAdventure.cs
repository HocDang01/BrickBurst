using TMPro;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class UILevelAdventure : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private GameObject _indicator;
    public void SetData(int level, Color color, bool isOvercome, bool isCurrentlevel)
    {
        if (isOvercome)
        {
            _levelText.gameObject.SetActive(false);
            _indicator.SetActive(false);
            return;
        }
        _levelText.gameObject.SetActive(true);
        _levelText.text = level.ToString();
        _levelText.color = color;
        _indicator.SetActive(isCurrentlevel);
    }

    public void SetSprite(Sprite sprite)
    {
        _img.sprite = sprite;
    }

}
