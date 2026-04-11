using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShapeColorLevelEditor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _image;

    private TileColor _currentTileColor;

    public void CreatColor(TileColor tileColor)
    {
        if (tileColor == null) return;
        _currentTileColor = tileColor;
        _image.sprite = tileColor.Tile;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentTileColor == null) return;
        LevelEditorManager.OnClickColor?.Invoke(_currentTileColor);
    }
}
