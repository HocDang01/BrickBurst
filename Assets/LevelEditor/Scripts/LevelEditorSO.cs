using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Game/Level")]
public class LevelEditorSO : ScriptableObject
{
    public int RowsCount = 8;
    public int ColumnsCount = 8;
    public int ScoreTarget = 100;

    public List<CellData> Board = new();

    public void CreateBoard()
    {
        Board = new List<CellData>(RowsCount * ColumnsCount);

        for (int i = 0; i < RowsCount * ColumnsCount; i++)
        {
            Board.Add(new CellData());
        }
    }
    public void Clear()
    {
        RowsCount = 0;
        ColumnsCount = 0;
        ScoreTarget = 0;
        Board.Clear();
    }
    public void SetSize(int row, int column)
    {
        RowsCount = row;
        ColumnsCount = column;
        Board.Clear();
        CreateBoard();
    }
    public CellData this[int row, int col]
    {
        get
        {
            if (!IsValidIndex(row, col))
            {
                Debug.LogError($"Invalid index ({row},{col})");
                return null;
            }

            return Board[row * ColumnsCount + col];
        }
    }
    public bool IsValidIndex(int row, int col)
    {
        return row >= 0 && row < RowsCount &&
               col >= 0 && col < ColumnsCount &&
               Board != null &&
               Board.Count == RowsCount * ColumnsCount;
    }


    public CellData GetCell(int row, int col)
    {
        return Board[row * ColumnsCount + col];
    }
}

[System.Serializable]
public class CellData
{
    public bool Occupied;
    public Sprite Sprite;
}
