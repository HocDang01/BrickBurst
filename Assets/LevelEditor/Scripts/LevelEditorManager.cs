using System;
using UnityEngine;
public class LevelEditorManager : MonoBehaviour
{
    public TileEditorManager TileManager;
    private ShapeLevelEditor _draggingShape;
    public ShapeLevelEditor shapeLevelEditor;
    public ShapeLevelEditor DraggingShape
    {
        get
        {
            return _draggingShape;
        }
        set
        {
            _draggingShape = value;
            shapeLevelEditor = _draggingShape;
        }
    }

    public Vector2 ScaleSquare;       // scale of each TileSquare in TileManager
    public int CurrentRowCount;
    public int CurrentColumnCount;
    #region Events
    public static Action<ShapeData> OnClickBaseShape;
    public static Action<GoalItemType> OnClickGoalItem;
    public static Action<TileColor> OnClickColor;
    #endregion
    #region PublicMethod
    public void OnPlaceShapeSuccess()
    {
        DraggingShape.DestroyShape();
        DraggingShape.BackToPos();
        DraggingShape = null;
    }
    public void RestartGame()
    {
        if (DraggingShape)
        {
            DraggingShape.DestroyShape();
            DraggingShape.BackToPos();
            DraggingShape = null;
        }
    }
    #endregion

}
