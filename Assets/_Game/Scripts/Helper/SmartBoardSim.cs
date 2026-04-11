using System.Collections.Generic;
using UnityEngine;

public static class SmartBoardSim
{
    // 1. Kiểm tra xem 3 khối có thể đặt được theo thứ tự bất kỳ không
    public static bool IsSequencePossible(int[,] board, List<ShapeData> shapes)
    {
        // Tìm tất cả hoán vị của 3 khối (ABC, ACB, BAC, BCA, CAB, CBA)
        var permutations = GetPermutations(shapes);

        foreach (var seq in permutations)
        {
            if (CanFitRecursive(board, seq, 0)) return true;
        }
        return false;
    }

    public static bool IsSequencePossible(int[,] board, ShapeData[] shapeDatas)
    {
        if (shapeDatas == null) return false;
        List<ShapeData> shapes = new();
        for (int i = 0; i < shapeDatas.Length; i++)
        {
            shapes.Add(shapeDatas[i]);
        }
        // Tìm tất cả hoán vị của 3 khối (ABC, ACB, BAC, BCA, CAB, CBA)
        var permutations = GetPermutations(shapes);

        foreach (var seq in permutations)
        {
            if (CanFitRecursive(board, seq, 0)) return true;
        }
        return false;
    }

    // Đệ quy thử đặt từng khối
    private static bool CanFitRecursive(int[,] currentBoard, List<ShapeData> shapes, int index)
    {
        if (index >= shapes.Count) return true; // Đã đặt được hết 3 khối

        ShapeData shape = shapes[index];
        int rCount = currentBoard.GetLength(0);
        int cCount = currentBoard.GetLength(1);

        // Thử mọi vị trí trên board
        for (int r = 0; r < rCount; r++)
        {
            for (int c = 0; c < cCount; c++)
            {
                if (CanPlace(currentBoard, shape, r, c))
                {
                    // Nếu đặt được, tạo board mới từ giả lập
                    int[,] nextBoard = CloneAndPlace(currentBoard, shape, r, c);

                    // QUAN TRỌNG: Giả lập nổ hàng để giải phóng không gian cho khối sau
                    SimulateClearLines(nextBoard);

                    if (CanFitRecursive(nextBoard, shapes, index + 1)) return true;
                }
            }
        }
        return false;
    }

    private static bool CanPlace(int[,] board, ShapeData shape, int row, int col)
    {
        for (int r = 0; r < shape.rows; r++)
        {
            for (int c = 0; c < shape.columns; c++)
            {
                if (shape[r, c])
                {
                    int targetR = row + r;
                    int targetC = col + c;

                    if (targetR >= board.GetLength(0) || targetC >= board.GetLength(1) || board[targetR, targetC] == 1)
                        return false;
                }
            }
        }
        return true;
    }

    private static int[,] CloneAndPlace(int[,] board, ShapeData shape, int row, int col)
    {
        int[,] newBoard = (int[,])board.Clone();
        for (int r = 0; r < shape.rows; r++)
        {
            for (int c = 0; c < shape.columns; c++)
            {
                if (shape[r, c]) newBoard[row + r, col + c] = 1;
            }
        }
        return newBoard;
    }

    private static void SimulateClearLines(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        List<int> fullRows = new();
        List<int> fullCols = new();

        for (int r = 0; r < rows; r++)
        {
            bool full = true;
            for (int c = 0; c < cols; c++) if (board[r, c] == 0) { full = false; break; }
            if (full) fullRows.Add(r);
        }

        for (int c = 0; c < cols; c++)
        {
            bool full = true;
            for (int r = 0; r < rows; r++) if (board[r, c] == 0) { full = false; break; }
            if (full) fullCols.Add(c);
        }

        foreach (int r in fullRows) for (int c = 0; c < cols; c++) board[r, c] = 0;
        foreach (int c in fullCols) for (int r = 0; r < rows; r++) board[r, c] = 0;
    }

    // Helper để tráo đổi vị trí 3 khối
    private static List<List<ShapeData>> GetPermutations(List<ShapeData> list)
    {
        var result = new List<List<ShapeData>>();
        if (list.Count == 1) { result.Add(new List<ShapeData>(list)); return result; }
        for (int i = 0; i < list.Count; i++)
        {
            var local = new List<ShapeData>(list);
            var element = local[i];
            local.RemoveAt(i);
            foreach (var p in GetPermutations(local))
            {
                p.Insert(0, element);
                result.Add(p);
            }
        }
        return result;
    }
}
