using System.Collections.Generic;
using UnityEngine;
public static class BoardAnalyzerBeauty
{
    public static List<ShapeData> AnalyzeBoardAndSuggestBeautifulShapes(
        Tile[,] board,
        List<ShapeData> allShapes)
    {
        int shapeCount = allShapes.Count;

        // Preallocate list to avoid resize
        List<ShapeScore> scoredShapes = new List<ShapeScore>(shapeCount);

        for (int i = 0; i < shapeCount; i++)
        {
            var shape = allShapes[i];
            if (shape == null) continue;

            float bestScore = GetBestBeautyScore(board, shape);

            if (bestScore > 0)
                scoredShapes.Add(new ShapeScore(shape, bestScore));
        }

        // Manual sort (faster than LINQ, no GC)
        scoredShapes.Sort((a, b) => b.score.CompareTo(a.score));

        int takeCount = Mathf.Min(10, scoredShapes.Count);

        List<ShapeData> result = new List<ShapeData>(takeCount);

        for (int i = 0; i < takeCount; i++)
            result.Add(scoredShapes[i].shape);

        return result;
    }

    private static float GetBestBeautyScore(Tile[,] board, ShapeData shape)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        int[,] shapeMatrix = BoardAnalyzer.ConvertShapeToMatrix(shape);

        int sH = shapeMatrix.GetLength(0);
        int sW = shapeMatrix.GetLength(1);

        float bestScore = 0f;

        for (int r = 0; r <= rows - sH; r++)
        {
            for (int c = 0; c <= cols - sW; c++)
            {
                if (!CanPlace(board, shapeMatrix, r, c))
                    continue;

                float score = EvaluatePlacementVirtual(board, shapeMatrix, r, c, rows, cols);

                if (score > bestScore)
                    bestScore = score;
            }
        }

        return bestScore;
    }

    private static float EvaluatePlacementVirtual(
        Tile[,] board,
        int[,] shape,
        int startR,
        int startC,
        int rows,
        int cols)
    {
        int linesCleared = 0;
        int holes = 0;
        int filled = 0;

        // Count filled + check rows
        for (int r = 0; r < rows; r++)
        {
            bool fullRow = true;

            for (int c = 0; c < cols; c++)
            {
                bool occupied = IsOccupied(board, shape, startR, startC, r, c);

                if (occupied) filled++;
                else fullRow = false;
            }

            if (fullRow) linesCleared++;
        }

        // Check columns
        for (int c = 0; c < cols; c++)
        {
            bool fullCol = true;

            for (int r = 0; r < rows; r++)
            {
                if (!IsOccupied(board, shape, startR, startC, r, c))
                {
                    fullCol = false;
                    break;
                }
            }

            if (fullCol) linesCleared++;
        }

        // Count holes
        for (int r = 1; r < rows - 1; r++)
        {
            for (int c = 1; c < cols - 1; c++)
            {
                if (!IsOccupied(board, shape, startR, startC, r, c))
                {
                    int neighbors = 0;

                    if (IsOccupied(board, shape, startR, startC, r + 1, c)) neighbors++;
                    if (IsOccupied(board, shape, startR, startC, r - 1, c)) neighbors++;
                    if (IsOccupied(board, shape, startR, startC, r, c + 1)) neighbors++;
                    if (IsOccupied(board, shape, startR, startC, r, c - 1)) neighbors++;

                    if (neighbors >= 3)
                        holes++;
                }
            }
        }

        float density = (float)filled / (rows * cols);

        return linesCleared * 100f
               - holes * 5f
               + density * 10f;
    }

    private static bool IsOccupied(
        Tile[,] board,
        int[,] shape,
        int startR,
        int startC,
        int r,
        int c)
    {
        // Check if cell belongs to placed shape
        int localR = r - startR;
        int localC = c - startC;

        if (localR >= 0 && localC >= 0 &&
            localR < shape.GetLength(0) &&
            localC < shape.GetLength(1) &&
            shape[localR, localC] == 1)
            return true;

        return board[r, c].SquareOccupied;
    }
    private static bool CanPlace(Tile[,] board, int[,] shape, int startR, int startC)
    {
        int sH = shape.GetLength(0);
        int sW = shape.GetLength(1);

        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                if (shape[r, c] == 1)
                {
                    if (board[startR + r, startC + c].SquareOccupied) // true = occupied
                        return false;
                }
            }
        }

        return true;
    }


    private struct ShapeScore
    {
        public ShapeData shape;
        public float score;

        public ShapeScore(ShapeData s, float sc)
        {
            shape = s;
            score = sc;
        }
    }
}

