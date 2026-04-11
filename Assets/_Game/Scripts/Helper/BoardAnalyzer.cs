using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoardAnalyzer
{
    // ⭐ Cache AI hint theo region signature
    private static readonly Dictionary<string, List<ShapeData>>
        _regionHintCache = new();
    private static string GenerateRegionSignature(bool[,] region)
    {
        int h = region.GetLength(0);
        int w = region.GetLength(1);

        System.Text.StringBuilder sb = new();
        sb.Append(w).Append("x").Append(h).Append("|");

        for (int r = 0; r < h; r++)
        {
            for (int c = 0; c < w; c++)
            {
                sb.Append(region[r, c]);
            }
        }

        return sb.ToString();
    }

    #region ENTRY POINT

    // public static List<ShapeData> AnalyzeBoardAndSuggestShapes(
    //     Tile[,] board,
    //     List<ShapeData> allShapes)
    // {
    //     int[,] freeMap = GetFreeMap(board);
    //     var regions = GetFreeRegions(freeMap);

    //     List<ShapeData> suggestedShapes = new();

    //     foreach (var region in regions)
    //     {
    //         // Bỏ qua khe quá nhỏ
    //         if (region.Count <= 1 || region.Count >= 10)
    //             continue;

    //         int[,] regionMatrix = NormalizeRegion(region);
    //         string signature = GenerateRegionSignature(regionMatrix);

    //         // ⭐ CACHE HIT → dùng lại kết quả cũ
    //         if (_regionHintCache.TryGetValue(signature, out var cachedShapes))
    //         {
    //             suggestedShapes.AddRange(cachedShapes);
    //             continue;
    //         }

    //         // ⭐ CACHE MISS → tính như logic cũ
    //         List<ShapeData> matchedShapes = new();

    //         foreach (var shape in allShapes)
    //         {
    //             if (shape == null) continue;
    //             int[,] shapeMatrix = ConvertShapeToMatrix(shape);

    //             if (region.Count <= 4 && region.Count > 1)
    //             {
    //                 if (IsSameShape(regionMatrix, shapeMatrix))
    //                 {
    //                     matchedShapes.Add(shape);
    //                 }
    //             }
    //             else
    //             {
    //                 if (IsGoodFit(regionMatrix, shapeMatrix))
    //                 {
    //                     matchedShapes.Add(shape);
    //                 }
    //                 // float score = GetBestFitScore(regionMatrix, region.Count, shapeMatrix);

    //                 // // Ngưỡng 0.5f nghĩa là Shape phải lấp đầy được ít nhất 50% cái hố mới lấy
    //                 // if (score >= 0.5f)
    //                 // {
    //                 //     // Có thể lưu kèm điểm số để sort nếu muốn, hoặc add thẳng như cũ
    //                 //     matchedShapes.Add(shape);
    //                 //     Debug.Log("Shape good: " + shape.name);
    //                 // }
    //             }
    //         }
    //         // ⭐ LƯU CACHE + ADD RESULT
    //         _regionHintCache[signature] = matchedShapes;
    //         suggestedShapes.AddRange(matchedShapes);
    //     }

    //     return suggestedShapes;
    // }

    // #endregion

    // #region FREE MAP

    // private static int[,] GetFreeMap(Tile[,] board)
    // {
    //     int rows = board.GetLength(0);
    //     int cols = board.GetLength(1);

    //     int[,] map = new int[rows, cols];

    //     for (int r = 0; r < rows; r++)
    //     {
    //         for (int c = 0; c < cols; c++)
    //         {
    //             map[r, c] = board[r, c].SquareOccupied ? 0 : 1;
    //         }
    //     }
    //     return map;
    // }

    // #endregion

    // #region REGION DETECTION (FLOOD FILL)

    // private static List<List<Vector2Int>> GetFreeRegions(int[,] freeMap)
    // {
    //     int rows = freeMap.GetLength(0);
    //     int cols = freeMap.GetLength(1);

    //     bool[,] visited = new bool[rows, cols];
    //     List<List<Vector2Int>> regions = new();

    //     int[] dr = { 1, -1, 0, 0 };
    //     int[] dc = { 0, 0, 1, -1 };

    //     for (int r = 0; r < rows; r++)
    //     {
    //         for (int c = 0; c < cols; c++)
    //         {
    //             if (freeMap[r, c] == 1 && !visited[r, c])
    //             {
    //                 List<Vector2Int> region = new();
    //                 Queue<Vector2Int> queue = new();

    //                 queue.Enqueue(new Vector2Int(r, c));
    //                 visited[r, c] = true;

    //                 while (queue.Count > 0)
    //                 {
    //                     var cur = queue.Dequeue();
    //                     region.Add(cur);

    //                     for (int i = 0; i < 4; i++)
    //                     {
    //                         int nr = cur.x + dr[i];
    //                         int nc = cur.y + dc[i];

    //                         if (nr >= 0 && nr < rows &&
    //                             nc >= 0 && nc < cols &&
    //                             freeMap[nr, nc] == 1 &&
    //                             !visited[nr, nc])
    //                         {
    //                             visited[nr, nc] = true;
    //                             queue.Enqueue(new Vector2Int(nr, nc));
    //                         }
    //                     }
    //                 }

    //                 regions.Add(region);
    //             }
    //         }
    //     }

    //     return regions;
    // }

    // #endregion

    // #region NORMALIZE REGION → MATRIX

    // private static int[,] NormalizeRegion(List<Vector2Int> region)
    // {
    //     int minR = region.Min(p => p.x);
    //     int minC = region.Min(p => p.y);

    //     var normalized = region
    //         .Select(p => new Vector2Int(p.x - minR, p.y - minC))
    //         .ToList();

    //     int maxR = normalized.Max(p => p.x);
    //     int maxC = normalized.Max(p => p.y);

    //     int[,] matrix = new int[maxR + 1, maxC + 1];

    //     foreach (var p in normalized)
    //     {
    //         matrix[p.x, p.y] = 1;
    //     }

    //     return matrix;
    // }

    // #endregion

    // #region SHAPE DATA → MATRIX

    // #endregion

    // #region MATRIX COMPARE

    // private static bool IsSameShape(int[,] a, int[,] b)
    // {
    //     if (a.GetLength(0) != b.GetLength(0) ||
    //         a.GetLength(1) != b.GetLength(1))
    //         return false;

    //     for (int r = 0; r < a.GetLength(0); r++)
    //     {
    //         for (int c = 0; c < a.GetLength(1); c++)
    //         {
    //             if (a[r, c] != b[r, c])
    //                 return false;
    //         }
    //     }
    //     return true;
    // }
    // private static bool IsGoodFit(int[,] region, int[,] shape)
    // {
    //     int rH = region.GetLength(0);
    //     int rW = region.GetLength(1);

    //     int sH = shape.GetLength(0);
    //     int sW = shape.GetLength(1);

    //     for (int offR = 0; offR <= rH - sH; offR++)
    //     {
    //         for (int offC = 0; offC <= rW - sW; offC++)
    //         {
    //             int overlap = 0;
    //             int shapeCells = 0;
    //             bool fit = true;

    //             for (int r = 0; r < sH && fit; r++)
    //             {
    //                 for (int c = 0; c < sW; c++)
    //                 {
    //                     if (shape[r, c] == 1)
    //                     {
    //                         shapeCells++;

    //                         if (region[offR + r, offC + c] == 1)
    //                             overlap++;
    //                         else
    //                         {
    //                             fit = false;
    //                             break;
    //                         }
    //                     }
    //                 }
    //             }

    //             // ✅ shape khớp
    //             // ✅ ăn được phần lớn khe (>=70%)
    //             if (fit && overlap >= shapeCells * 0.7f)
    //                 return true;
    //         }
    //     }

    //     return false;
    // }



    #endregion

    #region ENTRYPOINT2
    public static List<ShapeData> AnalyzeBoardAndSuggestShapes(
Tile[,] board,
List<ShapeData> allShapes)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        bool[,] freeMap = new bool[rows, cols];

        // Build free map (bool instead of int)
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                freeMap[r, c] = !board[r, c].SquareOccupied;

        List<List<Vector2Int>> regions = GetFreeRegions(freeMap);

        List<ShapeData> suggestedShapes = new List<ShapeData>(16);

        foreach (var region in regions)
        {
            int regionSize = region.Count;

            if (regionSize <= 1 || regionSize >= 10)
                continue;
            Debug.Log($"Region size: {regionSize}");
            bool[,] regionMatrix = NormalizeRegion(region);

            string signature = GenerateRegionSignature(regionMatrix);

            if (_regionHintCache.TryGetValue(signature, out var cachedShapes))
            {
                suggestedShapes.AddRange(cachedShapes);
                continue;
            }

            List<ShapeData> matchedShapes = new List<ShapeData>(8);

            foreach (var shape in allShapes)
            {
                if (shape == null) continue;

                // 🔥 IMPORTANT: Cache matrix inside ShapeData once
                bool[,] shapeMatrix = shape.CachedMatrix;
                if (regionSize <= 6)
                {
                    if (IsSameShape(regionMatrix, shapeMatrix))
                        matchedShapes.Add(shape);
                }
                else
                {
                    if (IsGoodFit(regionMatrix, shapeMatrix))
                        matchedShapes.Add(shape);
                }
            }

            _regionHintCache[signature] = matchedShapes;
            suggestedShapes.AddRange(matchedShapes);
        }

        return suggestedShapes;
    }

    private static List<List<Vector2Int>> GetFreeRegions(bool[,] freeMap)
    {
        int rows = freeMap.GetLength(0);
        int cols = freeMap.GetLength(1);

        bool[,] visited = new bool[rows, cols];

        List<List<Vector2Int>> regions = new List<List<Vector2Int>>(8);

        int[] dr = { 1, -1, 0, 0 };
        int[] dc = { 0, 0, 1, -1 };

        Queue<Vector2Int> queue = new Queue<Vector2Int>(32);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (!freeMap[r, c] || visited[r, c])
                    continue;

                List<Vector2Int> region = new List<Vector2Int>(8);

                queue.Clear();
                queue.Enqueue(new Vector2Int(r, c));
                visited[r, c] = true;

                while (queue.Count > 0)
                {
                    Vector2Int cur = queue.Dequeue();
                    region.Add(cur);

                    for (int i = 0; i < 4; i++)
                    {
                        int nr = cur.x + dr[i];
                        int nc = cur.y + dc[i];

                        if (nr >= 0 && nr < rows &&
                            nc >= 0 && nc < cols &&
                            freeMap[nr, nc] &&
                            !visited[nr, nc])
                        {
                            visited[nr, nc] = true;
                            queue.Enqueue(new Vector2Int(nr, nc));
                        }
                    }
                }

                regions.Add(region);
            }
        }

        return regions;
    }

    private static bool[,] NormalizeRegion(List<Vector2Int> region)
    {
        int count = region.Count;

        int minR = int.MaxValue;
        int minC = int.MaxValue;
        int maxR = int.MinValue;
        int maxC = int.MinValue;

        // Single pass min/max
        for (int i = 0; i < count; i++)
        {
            var p = region[i];

            if (p.x < minR) minR = p.x;
            if (p.y < minC) minC = p.y;
            if (p.x > maxR) maxR = p.x;
            if (p.y > maxC) maxC = p.y;
        }

        int height = maxR - minR + 1;
        int width = maxC - minC + 1;

        bool[,] matrix = new bool[height, width];

        for (int i = 0; i < count; i++)
        {
            var p = region[i];
            matrix[p.x - minR, p.y - minC] = true;
        }

        return matrix;
    }

    private static bool IsSameShape(bool[,] a, bool[,] b)
    {
        int h = a.GetLength(0);
        int w = a.GetLength(1);

        if (h != b.GetLength(0) || w != b.GetLength(1))
            return false;

        for (int r = 0; r < h; r++)
            for (int c = 0; c < w; c++)
                if (a[r, c] != b[r, c])
                    return false;

        return true;
    }

    // private static bool IsGoodFit(bool[,] region, bool[,] shape)
    // {
    //     int rH = region.GetLength(0);
    //     int rW = region.GetLength(1);

    //     int sH = shape.GetLength(0);
    //     int sW = shape.GetLength(1);

    //     for (int offR = 0; offR <= rH - sH; offR++)
    //     {
    //         for (int offC = 0; offC <= rW - sW; offC++)
    //         {
    //             int overlap = 0;
    //             int shapeCells = 0;
    //             bool fit = true;

    //             for (int r = 0; r < sH && fit; r++)
    //             {
    //                 for (int c = 0; c < sW; c++)
    //                 {
    //                     if (!shape[r, c]) continue;

    //                     shapeCells++;

    //                     if (region[offR + r, offC + c])
    //                         overlap++;
    //                     else
    //                     {
    //                         fit = false;
    //                         break;
    //                     }
    //                 }
    //             }

    //             if (fit && overlap >= shapeCells * 0.8f)
    //                 return true;
    //         }
    //     }

    //     return false;
    // }
    private static bool IsGoodFit(bool[,] region, bool[,] shape)
    {
        int rH = region.GetLength(0);
        int rW = region.GetLength(1);

        int sH = shape.GetLength(0);
        int sW = shape.GetLength(1);

        // 🔥 Đếm tổng số ô của region (hố)
        int regionCells = 0;
        for (int r = 0; r < rH; r++)
            for (int c = 0; c < rW; c++)
                if (region[r, c]) regionCells++;

        if (CountShapeCells(shape) > regionCells)
            return false;

        for (int offR = 0; offR <= rH - sH; offR++)
        {
            for (int offC = 0; offC <= rW - sW; offC++)
            {
                int overlap = 0;
                bool fit = true;

                for (int r = 0; r < sH && fit; r++)
                {
                    for (int c = 0; c < sW; c++)
                    {
                        if (!shape[r, c]) continue;

                        if (region[offR + r, offC + c])
                            overlap++;
                        else
                        {
                            fit = false;
                            break;
                        }
                    }
                }

                // 🔥 check theo % của REGION (không phải shape)
                if (fit && (float)overlap / regionCells >= 0.65f)
                    return true;
            }
        }

        return false;
    }
    private static int CountShapeCells(bool[,] shape)
    {
        int count = 0;

        for (int r = 0, h = shape.GetLength(0); r < h; r++)
        {
            for (int c = 0, w = shape.GetLength(1); c < w; c++)
            {
                if (shape[r, c]) count++;
            }
        }

        return count;
    }


    #endregion

    #region Helper
    public static int[,] ConvertShapeToMatrix(ShapeData shape)
    {
        int[,] matrix = new int[shape.rows, shape.columns];

        for (int r = 0; r < shape.rows; r++)
        {
            for (int c = 0; c < shape.columns; c++)
            {
                matrix[r, c] = shape[r, c] ? 1 : 0;
            }
        }
        return matrix;
    }


    // Hàm tính điểm: Trả về từ 0.0 đến 1.0 (1.0 là vừa khít hoàn hảo)
    private static float GetBestFitScore(int[,] region, int regionTotalCells, int[,] shape)
    {
        int rH = region.GetLength(0);
        int rW = region.GetLength(1);
        int sH = shape.GetLength(0);
        int sW = shape.GetLength(1);

        // Đếm số ô đặc của Shape
        int shapeCells = 0;
        for (int r = 0; r < sH; r++)
            for (int c = 0; c < sW; c++)
                if (shape[r, c] == 1) shapeCells++;

        // Nếu shape lớn hơn cái hố thì chắc chắn 0 điểm
        if (shapeCells > regionTotalCells) return 0f;

        float maxScore = 0f;

        // Duyệt qua mọi vị trí offset có thể đặt
        for (int offR = 0; offR <= rH - sH; offR++)
        {
            for (int offC = 0; offC <= rW - sW; offC++)
            {
                // Kiểm tra xem đặt ở vị trí (offR, offC) có bị cấn không
                if (CheckFitAtPosition(region, shape, offR, offC))
                {
                    // Công thức: Tỷ lệ lấp đầy = (Diện tích Shape) / (Diện tích Hố)
                    float currentScore = (float)shapeCells / regionTotalCells;

                    if (currentScore > maxScore) maxScore = currentScore;

                    // Nếu đã tìm được vị trí Perfect Match (1.0), return luôn cho lẹ
                    if (maxScore >= 0.99f) return 1.0f;
                }
            }
        }

        return maxScore;
    }

    // Hàm bổ trợ: Kiểm tra xem shape có đặt vừa vào vị trí offR, offC không
    private static bool CheckFitAtPosition(int[,] region, int[,] shape, int offR, int offC)
    {
        int sH = shape.GetLength(0);
        int sW = shape.GetLength(1);

        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                // Nếu Shape có block (1)
                if (shape[r, c] == 1)
                {
                    // Kiểm tra xem Region tại đó có phải là ô trống (1) không
                    // Lưu ý: Region đã normalize thì 1 là ô trống thuộc vùng đó
                    if (region[offR + r, offC + c] != 1)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    #endregion
}

