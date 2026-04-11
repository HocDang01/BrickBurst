using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelEditorSO))]
public class LevelEditorSOEditor : Editor
{
    private LevelEditorSO level;

    private void OnEnable()
    {
        level = (LevelEditorSO)target;

        if (level.Board == null || level.Board.Count == 0)
        {
            level.CreateBoard();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("TARGET EDITOR", EditorStyles.boldLabel);
        level.ScoreTarget = EditorGUILayout.IntField("Score Target", level.ScoreTarget);

        EditorGUILayout.LabelField("BOARD EDITOR", EditorStyles.boldLabel);
        level.RowsCount = EditorGUILayout.IntField("Rows", level.RowsCount);
        level.ColumnsCount = EditorGUILayout.IntField("Columns", level.ColumnsCount);

        if (GUILayout.Button("Generate Grid"))
        {
            level.CreateBoard();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Board Preview", EditorStyles.boldLabel);

        DrawBoard();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(level);
        }
    }

    private void DrawBoard()
    {
        if (level.Board == null || level.Board.Count == 0)
            return;

        for (int r = 0; r < level.RowsCount; r++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int c = 0; c < level.ColumnsCount; c++)
            {
                var cell = level.GetCell(r, c);

                EditorGUILayout.BeginVertical(GUILayout.Width(80));

                // Toggle Occupied
                cell.Occupied = EditorGUILayout.Toggle(cell.Occupied);

                // Sprite field
                cell.Sprite = (Sprite)EditorGUILayout.ObjectField(
                    cell.Sprite, typeof(Sprite), false,
                    GUILayout.Width(70), GUILayout.Height(70));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif