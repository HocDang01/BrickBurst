using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class LevelJsonSystem
{
#if UNITY_EDITOR
    public static void SaveLevelToJson(int levelIndex, TargetType targetType, int scoreTarget,
        Dictionary<GoalItemType, int> goalTargets, TileEditor[,] Board)
    {
        foreach (var a in goalTargets)
        {
            Debug.Log(a.Key + " : " + a.Value);
        }
        LevelJsonData levelData = new LevelJsonData
        {
            level = levelIndex,
            target = new TargetData
            {
                TargetType = (int)targetType,
                score = targetType == TargetType.GoalIem ? 0 : scoreTarget,
                goalItems = Utility.GoalItemDictToList(goalTargets)
            },
            tiles = new List<TileJsonData>(BoardManager.Ins.RowCount * BoardManager.Ins.ColumnCount)
        };

        // Tiles (0,0 → 0,1 → ... → 1,0 ...)
        for (int row = 0; row < BoardManager.Ins.RowCount; row++)
        {
            for (int col = 0; col < BoardManager.Ins.ColumnCount; col++)
            {
                TileEditor tile = Board[row, col];

                levelData.tiles.Add(new TileJsonData
                {
                    rowIndex = row,
                    columnIndex = col,
                    occupied = tile.SquareOccupied,
                    goalItemType = (int)tile.GoalItemType,
                    colorIndex = tile.SquareOccupied
                        ? tile.GetSpriteActive() != null
                            ? GameConfig.Ins.TileColorConfig.ColorSprites.FindIndex(e => e.Tile == tile.GetSpriteActive())
                            : -1
                        : -1
                });
            }
        }

        string folderPath = GameConfig.Ins.LevelEditorConfig.FolderLevelPath;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string jsonPath = $"{folderPath}/{GameConfig.Ins.LevelEditorConfig.LevelName}{levelIndex}.json";
        string json = JsonUtility.ToJson(levelData, true);

        File.WriteAllText(jsonPath, json);
        AssetDatabase.Refresh();

        Debug.Log($"✅ Saved level JSON: {jsonPath}");
    }
#endif
    public static LevelJsonData LoadLevelJson(int levelIndex)
    {
        levelIndex = levelIndex % GameConfig.Ins.GameplayConfig.LevelToLoop;
        levelIndex = levelIndex <= 0 ? GameConfig.Ins.GameplayConfig.LevelToLoop : levelIndex;
        TextAsset jsonFile =
            Resources.Load<TextAsset>($"{GameConfig.Ins.LevelEditorConfig.FolderName}/{GameConfig.Ins.LevelEditorConfig.LevelName}{levelIndex}");

        if (jsonFile == null)
        {
            Debug.LogError($"❌ {GameConfig.Ins.LevelEditorConfig.LevelName}{levelIndex}.json not found");
            return null;
        }

        return JsonUtility.FromJson<LevelJsonData>(jsonFile.text);
    }

}
