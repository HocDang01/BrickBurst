using System.Collections.Generic;
using DangExtension;
using UnityEngine;
public class ShapeSpawner : MonoBehaviour
{
    [SerializeField] private List<ShapeData> _shapeDatas;
    [SerializeField] private List<Shape> shapeList;
    [SerializeField] private List<ShapeData> _shapeRescues;
    [SerializeField] private List<ShapeData> _shapeWatchAds;
    private List<TileColor> _tileColors;
    private ShapeSpawnerConfig _shapeSpawnerConfig;
    private ShapeData[] _shapesUpcoming;

    public static ShapeSpawner Ins;
    void Awake()
    {
        Ins = this;
        _shapesUpcoming = new ShapeData[3];
        _tileColors = GameConfig.Ins.TileColorConfig.ColorSprites;
        _shapeSpawnerConfig = GameConfig.Ins.ShapeSpawnerConfig.CloneData();
        GameEvents.OnNeedCreateShapes += OnCreateShapes;
        // Checker();
    }
    private void Checker()
    {
        foreach (var shape1 in _shapeDatas)
        {
            if (shape1 == null) continue;
            foreach (var shape2 in _shapeDatas)
            {
                if (shape2 == null) continue;
                if (shape2 == shape1) continue;
                bool isDup = true;
                if (shape1.rows == shape2.rows && shape1.columns == shape2.columns)
                {
                    for (int i = 0; i < shape1.rows; i++)
                    {
                        for (int j = 0; j < shape1.columns; j++)
                        {
                            if (shape1[i, j] != shape2[i, j])
                            {
                                isDup = false;
                                break;
                            }
                            if (isDup == false)
                                break;
                        }
                        if (isDup == false)
                            break;
                    }
                    if (isDup == false)
                        break;
                    Debug.Log(shape1.name + " same with " + shape2.name);
                }
            }
        }
    }

    void OnEnable()
    {
        // GameEvents.OnStartGame += CalculateWeightBasedOnLevel;
    }
    void OnDisable()
    {
        // GameEvents.OnStartGame -= CalculateWeightBasedOnLevel;
    }
    void OnDestroy()
    {
        GameEvents.OnNeedCreateShapes -= OnCreateShapes;
    }

    #region Create Shapes
    private void OnCreateShapes(ShapeSpawnType shapeSpawnType)
    {
        CreateSmartShape();
        ReplaceFitShapes2(2);
        switch (shapeSpawnType)
        {
            case ShapeSpawnType.Normal:
                break;
            case ShapeSpawnType.OneBeauty:
                ReplaceFitShapes(1);
                break;
            case ShapeSpawnType.TwoBeauty:
                ReplaceFitShapes(2);
                break;
            case ShapeSpawnType.ThreeBeauty:
                ReplaceFitShapes(3);
                break;
            case ShapeSpawnType.Perfect:
                break;
        }
        ReallySpawn();

        if (GameplayManager.Ins.PlayMode == PlayMode.Classic)
            BoardSaveData.Ins.SaveClassic(BoardManager.Ins);
        else if (GameplayManager.Ins.PlayMode == PlayMode.Adventure)
            BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
    }
    public void ReSpawnFollowData(BoardData boardData)
    {
        if (boardData == null || boardData.tiles == null || boardData.tiles.Count <= 0) return;
        for (int i = 0; i < 3; i++)
        {
            shapeList[i].DestroyShape();
            var shapeSaved = boardData.shapesSaveData[i];
            if (shapeSaved.shapeType != ShapeType.None && shapeSaved.shapeIndex >= 0)
            {
                var a = GameConfig.Ins.ShapeSpawnerConfig.PoolShapeConfigs.Find(e => e.ShapeType == shapeSaved.shapeType);
                if (a != null)
                {
                    int colorIndex = Random.Range(0, _tileColors.Count);
                    if (shapeSaved.goalItemType != GoalItemType.None && shapeSaved.itemIndexList != null && shapeSaved.itemIndexList.Count > 0)
                    {
                        shapeList[i].RequestNewShape(a.ShapeRatios[shapeSaved.shapeIndex].ShapeData, _tileColors[colorIndex], shapeSaved.goalItemType, shapeSaved.itemIndexList);
                    }
                    else
                    {
                        shapeList[i].RequestNewShape(a.ShapeRatios[shapeSaved.shapeIndex].ShapeData, _tileColors[colorIndex]);
                    }
                }
            }
        }

        if (GameplayManager.Ins.PlayMode == PlayMode.Classic)
            BoardSaveData.Ins.SaveClassic(BoardManager.Ins);
        else if (GameplayManager.Ins.PlayMode == PlayMode.Adventure)
            BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
    }

    #endregion

    #region Extent Smart
    private void ReplaceFitShapes(int count)
    {
        var suggested = BoardAnalyzer.AnalyzeBoardAndSuggestShapes(
            BoardManager.Ins.Board,
            _shapeDatas
        );
        if (suggested.Count == 0 || count <= 0)
            return;
        Debug.Log("Suggest count: " + suggested.Count);
        for (int i = 0; i < suggested.Count; i++)
        {
            Debug.Log("Suggest shape " + i + ": " + suggested[i].name);

        }

        count = Mathf.Min(count, _shapesUpcoming.Length, suggested.Count);
        int[,] currentBoardMatrix = GetCurrentBoardMatrix();
        // 1. Tạo list index slot (0,1,2...)
        List<int> slotIndices = new List<int>();
        for (int i = 0; i < _shapesUpcoming.Length; i++)
            slotIndices.Add(i);

        // 2. Replace random `count` slot
        for (int i = 0; i < count; i++)
        {
            // random slot
            int slotIdx = Random.Range(0, slotIndices.Count);
            int slot = slotIndices[slotIdx];
            slotIndices.RemoveAt(slotIdx);

            // random suggested shape
            int shapeIdx = 0;
            for (int j = 0; j < 10; j++)
            {
                shapeIdx = Random.Range(0, suggested.Count);
                _shapesUpcoming[slot] = suggested[shapeIdx];
                if (SmartBoardSim.IsSequencePossible(currentBoardMatrix, _shapesUpcoming))
                    break;
            }
            suggested.RemoveAt(shapeIdx);
        }
    }
    private void CreateSmartShape()
    {
        CalculateWeightBasedOnScore();
        int maxAttempts = 6; // Tránh treo máy nếu không tìm được bộ nào (hiếm khi xảy ra)

        // Lấy ma trận Board hiện tại từ TileManager
        int[,] currentBoardMatrix = GetCurrentBoardMatrix();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            for (int i = 0; i < 3; i++)
            {
                ShapeData shapeDataUpComing = GetRandomShapeByWeight(); // Get Random Shape
                                                                        // If Shape == null -> GetRandom
                if (shapeDataUpComing == null)
                {
                    int shapeIndex = Random.Range(0, _shapeDatas.Count);
                    shapeDataUpComing = _shapeDatas[shapeIndex];
                }
                _shapesUpcoming[i] = shapeDataUpComing;
            }

            // Bước "Siêu thông minh": Kiểm tra xem bộ 3 này có cửa thắng không
            if (SmartBoardSim.IsSequencePossible(currentBoardMatrix, _shapesUpcoming))
            {
                Debug.Log($"Found valid set after {attempt} attempts");
                break;
            }

            // Nếu đến lần thử cuối vẫn không được, AI sẽ tự chèn 1 khối 1x1 để cứu người chơi
            if (attempt >= maxAttempts - 1)
            {
                _shapesUpcoming[2] = _shapeRescues[Random.Range(0, _shapeRescues.Count)];
            }
        }
    }

    private void ReplaceFitShapes2(int count)
    {
        var suggested = BoardAnalyzerBeauty.AnalyzeBoardAndSuggestBeautifulShapes(
            BoardManager.Ins.Board,
            _shapeDatas
        );
        if (suggested.Count == 0 || count <= 0)
            return;

        count = Mathf.Min(count, _shapesUpcoming.Length, suggested.Count);
        int[,] currentBoardMatrix = GetCurrentBoardMatrix();
        // 1. Tạo list index slot (0,1,2...)
        List<int> slotIndices = new List<int>();
        for (int i = 0; i < _shapesUpcoming.Length; i++)
            slotIndices.Add(i);

        // 2. Replace random `count` slot
        for (int i = 0; i < count; i++)
        {
            // random slot
            int slotIdx = Random.Range(0, slotIndices.Count);
            int slot = slotIndices[slotIdx];
            slotIndices.RemoveAt(slotIdx);

            // random suggested shape
            int shapeIdx = 0;
            for (int j = 0; j < 5; j++)
            {
                shapeIdx = Random.Range(0, suggested.Count);
                _shapesUpcoming[slot] = suggested[shapeIdx];
                if (SmartBoardSim.IsSequencePossible(currentBoardMatrix, _shapesUpcoming))
                    break;
            }
            suggested.RemoveAt(shapeIdx);
        }
    }
    // Thực hiện Spawn thật lên màn hình
    private void ReallySpawn()
    {
        GameplayManager.Ins.IsHardTurn = false;
        var goalItems = ScoreManager.Ins.GoalItemTargets;
        for (int i = 0; i < 3; i++)
        {
            shapeList[i].DestroyShape();
            int colorIndex = Random.Range(0, _tileColors.Count);
            if (GameplayManager.Ins.PlayMode == PlayMode.Adventure && goalItems != null && goalItems.Count > 0)
            {
                var currGoalItems = goalItems.FindAll(e => e.Value > 0);
                if (currGoalItems != null && currGoalItems.Count > 0 && Random.Range(0, 3) != 0)
                {
                    var dicGoalItems = currGoalItems[Random.Range(0, currGoalItems.Count)];
                    shapeList[i].RequestNewShape(_shapesUpcoming[i], _tileColors[colorIndex], dicGoalItems.Key);
                }
                else
                {
                    shapeList[i].RequestNewShape(_shapesUpcoming[i], _tileColors[colorIndex]);
                }
            }
            else
            {
                shapeList[i].RequestNewShape(_shapesUpcoming[i], _tileColors[colorIndex]);
            }
            // if have at least 1 shape cannot place on board -> IsHardTurn.
            if (!BoardManager.Ins.CanPlaceByShapeData(_shapesUpcoming[i]))
            {
                GameplayManager.Ins.IsHardTurn = true;
            }
        }
        GameplayManager.Ins.CurrentShapeCount = 3;
        // SoundManager.Ins.pickupTileFx.Play();
    }

    private int[,] GetCurrentBoardMatrix()
    {
        var board = BoardManager.Ins.Board;
        int r = board.GetLength(0);
        int c = board.GetLength(1);
        int[,] matrix = new int[r, c];
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                matrix[i, j] = board[i, j].SquareOccupied ? 1 : 0;
        return matrix;
    }
    #endregion

    #region CalculateWeight
    private void CalculateWeightBasedOnLevel()
    {
        if (GameplayManager.Ins.PlayMode != PlayMode.Adventure) return;
        int level = 0;
        if (BBManager.EnableCheat)
            level = GameConfig.Ins.GameplayConfig.Level;
        else
            level = UserProperty.BrickBurstLevel;

        foreach (var pool in _shapeSpawnerConfig.PoolShapeConfigs)
        {
            pool.UpdateWeightFollowLevel(level);
            Debug.Log(pool.ShapeType + ": " + pool.Weight);
        }
        Debug.Log("Total weight: " + _shapeSpawnerConfig.TotalWeight);
    }
    private void CalculateWeightBasedOnScore()
    {
        // if (GameplayManager.Ins.PlayMode != PlayMode.Classic) return;
        int score = ScoreManager.Ins.TotalScore;
        foreach (var pool in _shapeSpawnerConfig.PoolShapeConfigs)
        {
            pool.UpdateWeightFollowEndless(score);
            Debug.Log(pool.ShapeType + ": " + pool.Weight);
        }
        Debug.Log("Total weight: " + _shapeSpawnerConfig.TotalWeight);
    }
    #endregion

    #region GetRandomShape
    private ShapeData GetRandomShapeByWeight()   // Use this shape
    {
        return _shapeSpawnerConfig.GetRandomShapeByWeight();
    }
    #endregion

    #region WatchAds
    public void SpawnShapeWatchAds(List<ShapeData> selectedShapes)
    {
        GameplayManager.Ins.IsHardTurn = false;
        var goalItems = ScoreManager.Ins.GoalItemTargets;
        // Thực hiện Spawn thật lên màn hình
        for (int i = 0; i < 3; i++)
        {
            shapeList[i].DestroyShape();
            int colorIndex = Random.Range(0, _tileColors.Count);
            if (GameplayManager.Ins.PlayMode == PlayMode.Adventure && goalItems != null && goalItems.Count > 0)
            {
                var currGoalItems = goalItems.FindAll(e => e.Value > 0);
                if (currGoalItems != null && currGoalItems.Count > 0 && Random.Range(0, 3) != 0)
                {
                    var dicGoalItems = currGoalItems[Random.Range(0, currGoalItems.Count)];
                    shapeList[i].RequestNewShape(selectedShapes[i], _tileColors[colorIndex], dicGoalItems.Key);
                }
                else
                {
                    shapeList[i].RequestNewShape(selectedShapes[i], _tileColors[colorIndex]);
                }
            }
            else
            {
                shapeList[i].RequestNewShape(selectedShapes[i], _tileColors[colorIndex]);
            }
        }
        GameplayManager.Ins.CurrentShapeCount = 3;
    }
    public List<ShapeData> GetShapeWatchAds()
    {
        int maxAttempts = 15; // Tránh treo máy nếu không tìm được bộ nào (hiếm khi xảy ra)
        List<ShapeData> selectedShapes = new List<ShapeData>();

        // Lấy ma trận Board hiện tại từ TileManager
        int[,] currentBoardMatrix = GetCurrentBoardMatrix();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            selectedShapes.Clear();
            for (int i = 0; i < 3; i++)
            {
                ShapeData shapeDataUpComing = _shapeWatchAds[Random.Range(0, _shapeWatchAds.Count)];
                if (shapeDataUpComing == null)
                {
                    int shapeIndex = Random.Range(0, _shapeDatas.Count);
                    shapeDataUpComing = _shapeDatas[shapeIndex];
                }
                selectedShapes.Add(shapeDataUpComing);
            }

            // Bước "Siêu thông minh": Kiểm tra xem bộ 3 này có cửa thắng không
            if (SmartBoardSim.IsSequencePossible(currentBoardMatrix, selectedShapes))
            {
                Debug.Log($"Found valid set after {attempt} attempts");
                break;
            }

            if (attempt >= maxAttempts - 1)
            {
                selectedShapes[2] = _shapeWatchAds[Random.Range(0, _shapeWatchAds.Count)];
            }
        }
        return selectedShapes;
    }

    #endregion

    
}


// private void Create3Shapes()
// {
//     CalculateWeightBasedOnScore();
//     int idx = 0;
//     foreach (var shape in shapeList)
//     {
//         ShapeData shapeDataUpComing = GetRandomShapeByWeight(); // Get Random Shape
//         // If Shape == null -> GetRandom
//         if (shapeDataUpComing == null)
//         {
//             int shapeIndex = Random.Range(0, _shapeDatas.Count);
//             shapeDataUpComing = _shapeDatas[shapeIndex];
//         }
//         _shapesUpcoming[idx++] = shapeDataUpComing;
//     }
//     if (CalculateOccupiedOfBoard() > 0.3f)
//     {
//         SmartShape(3);
//     }
//     for (int i = 0; i < 3; i++)
//     {
//         shapeList[i].DestroyShape();
//         int colorIndex = Random.Range(0, _tileColors.Count);
//         shapeList[i].RequestNewShape(_shapesUpcoming[i], _tileColors[colorIndex]);
//         GameplayManager.Ins.CurrentShapeCount++;
//     }
// }