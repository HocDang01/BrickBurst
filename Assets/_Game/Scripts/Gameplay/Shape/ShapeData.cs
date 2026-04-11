using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData")]
[Serializable]
public class ShapeData : ScriptableObject
{
    [Serializable]
    public class Row
    {
        public bool[] column;
        private int _size = 0;
        public Row() { }
        public Row(int size)
        {
            CreateRow(size);
        }
        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int columns = 0;
    public int rows = 0;
    public Row[] board;

    public bool this[int row, int col]
    {
        get => board[row].column[col];
        set => board[row].column[col] = value;
    }
    private bool[,] _cachedMatrix;

    public bool[,] CachedMatrix
    {
        get
        {
            if (_cachedMatrix == null)
            {
                _cachedMatrix = new bool[rows, columns];

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < columns; c++)
                        _cachedMatrix[r, c] = this[r, c];
            }

            return _cachedMatrix;
        }
    }

    public int GetCountNode()
    {
        int totalNode = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i].column[j] == true)
                {
                    totalNode++;
                }
            }
        }
        return totalNode;
    }

    public void Clear()
    {
        for (int i = 0; i < rows; i++)
        {
            board[i].ClearRow();
        }
    }
    public void CreateNewBoard()
    {
        board = new Row[rows];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
    }

}
