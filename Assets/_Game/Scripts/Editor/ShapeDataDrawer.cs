using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    private ShapeData ShapedataInstance => target as ShapeData;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumnsInputField();
        EditorGUILayout.Space();

        if (ShapedataInstance.board != null && ShapedataInstance.columns > 0 && ShapedataInstance.rows > 0)
        {
            DrawBoardTable();
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapedataInstance);
        }
    }
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            ShapedataInstance.Clear();
        }
    }
    private void DrawColumnsInputField()
    {
        var columnsTemp = ShapedataInstance.columns;
        var rowsTemp = ShapedataInstance.rows;

        ShapedataInstance.columns = EditorGUILayout.IntField("Columns", ShapedataInstance.columns);
        ShapedataInstance.rows = EditorGUILayout.IntField("Rows", ShapedataInstance.rows);

        if ((ShapedataInstance.columns != columnsTemp || ShapedataInstance.rows != rowsTemp) &&
            ShapedataInstance.columns > 0 && ShapedataInstance.rows > 0)
        {
            ShapedataInstance.CreateNewBoard();
        }
    }
    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for (var row = 0; row < ShapedataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);

            for (var column = 0; column < ShapedataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                // var data = EditorGUILayout.Toggle(ShapedataInstance.board[row].column[column], dataFieldStyle);
                // ShapedataInstance.board[row].column[column] = data;
                var data = EditorGUILayout.Toggle(ShapedataInstance[row, column], dataFieldStyle);
                ShapedataInstance[row, column] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }


    }
}
