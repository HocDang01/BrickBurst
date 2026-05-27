using System.Collections;
using System.Collections.Generic;
using DangExtension;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoardBuilder))]
[RequireComponent(typeof(BoardOutlineRenderer))]
public class BoardManager : MonoBehaviour
{
    [Header("Ref Shapes")]
    [SerializeField] protected Transform _noSpaceLeft;
    [SerializeField] protected GameObject _shapeContainer;
    [SerializeField] protected List<Shape> _shapes;
    [Header("Setting Button")]
    [SerializeField] protected Button _settingBtn;

    protected int _rowsCount = 8;
    protected int _columnsCount = 8;
    public List<Shape> Shapes => _shapes;

    public BoardContext BoardContext;

    public BoardBuilder BoardBuilder;     // Create board
                                          // Calculate tile scale
                                          // Spawn grid
                                          // Setup rows/columns
                                          // Position tiles
    public BoardPlacementService BoardPlacementService;   // CanPlace()
                                                          // CanPlaceInGrid()
                                                          // CanPlaceByShapeData()
                                                          // GetIndex()
                                                          // GetTilesAddressDic()
    public BoardScoreService BoardScoreService;           // GetScoredList()
                                                          // GetAllScoredList()
                                                          // GetRowCenters()
                                                          // GetColumnCenters()
                                                          // TryPlayScoreEffect()
                                                          // ClearList()

    public BoardOutlineRenderer BoardOutlineRenderer;     // SpawnRowOutlines()
                                                          // SpawnColumnOutlines()
                                                          // HighlightOutlineByScore()
                                                          // ShowRowOutline()
                                                          // ShowColumnOutline()
                                                          // HideAllOutlines()

    public BoardPreviewService BoardPreviewService;       // CheckHover()
                                                          // CheckVirtual()
                                                          // SetVirtualScore()
                                                          // SetVirtualScoreNormal()
                                                          // SetVirtualScoreRainBow()
    public BoardLevelLoader BoardLevelLoader;             // LoadLevelByIndex()
                                                          // LoadBoardBaseOnLevel()
    public BoardProgressService BoardProgressService;     // LoadBoardSaved()
                                                          // LoadBoardFromSave()
    #region PUBLIC METHOD
    public int RowCount => _rowsCount;
    public int ColumnCount => _columnsCount;
    public Vector2 SquareScale => BoardBuilder._squareScale;
    public Tile[,] Board => BoardContext.Board;
    public List<Tile> rowsCanScore => BoardScoreService.rowsCanScore;
    public List<Tile> columnsCanScore => BoardScoreService.columnsCanScore;
    public List<Tile> allCanScore => BoardScoreService.allCanScore;
    public bool CanPlaceByShapeData(ShapeData shapeData) => BoardPlacementService.CanPlaceByShapeData(shapeData);
    public (int, int) GetIndexRowColByTransform(Transform transform) => BoardPlacementService.GetIndexRowColByTransform(transform);
    public void CheatLose()
    {
        if (!GameplayManager.Ins.IsInGame) return;
        GameplayManager.Ins.OnLoseGame(RestartLevel, NextLevel);
        NoSpaceLeft();
        DOVirtual.DelayedCall(0.1f, () =>
        {
            StartCoroutine(PlayEndTileWave());
        });
    }
    #endregion


    static BoardManager _instance = null;
    public static BoardManager Ins
    {
        get
        {
            return _instance;
        }
    }

    #region MonoBehavior
    protected virtual void Awake()
    {
        _instance = this;
        BoardBuilder = GetComponent<BoardBuilder>();
        BoardOutlineRenderer = GetComponent<BoardOutlineRenderer>();
        BoardContext = new(_rowsCount, _columnsCount);
        BoardBuilder.Init(BoardContext);      // mono
        BoardScoreService = new(BoardContext);
        BoardOutlineRenderer.Init(BoardContext, BoardScoreService);  // mono
        BoardPlacementService = new(BoardContext, this);
        BoardPreviewService = new(BoardContext, BoardScoreService, BoardOutlineRenderer, BoardPlacementService);
        BoardLevelLoader = new(BoardContext);
        BoardProgressService = new(BoardContext);
    }

    public void OnFirstEnable()
    {
        GameEvents.OnEnterGameplay += RestartLevel;
        StartGame();
    }
    protected virtual void OnEnable()
    {
        _instance = this;
        GameEvents.CheckIfShapeCanPlaced += OnPlaceOnGrid;

        _settingBtn.onClick.AddListener(OnClickSetting);
    }

    protected virtual void OnDisable()
    {
        GameEvents.CheckIfShapeCanPlaced -= OnPlaceOnGrid;
        _settingBtn.onClick.RemoveListener(OnClickSetting);
    }
    protected virtual void OnDestroy()
    {
        GameEvents.OnEnterGameplay -= RestartLevel;
    }
    protected virtual void Update()
    {
        BoardPreviewService.CheckHover();
        BoardPreviewService.CheckVirtual();
        BoardPreviewService.SetVirtualScore();
    }
    #endregion

    #region Setting
    protected virtual void OnClickSetting()
    {
        if (!GameplayManager.Ins.IsInGame) return;
        PopupSetting.Show();
        var setting = PopupSetting.Ins;
        setting.SetInMainMenu(false);
        setting.SetActionReplay(OnClickReplay);
        setting.SetActionHome(OnActionHome);
    }
    private void OnActionHome()
    {
        GameplayView.Hide();
        MainMenu.Show();
        GameEvents.OnBackMainMenu?.Invoke(true);
    }
    #endregion


    #region Start Level
    protected void StartGame()
    {
        _noSpaceLeft.gameObject.SetActive(false);
        TargetData targetScoreData = null;
        if (GameplayManager.Ins.PlayMode == PlayMode.Adventure)
        {
            if (BBManager.EnableCheat)
            {
                targetScoreData = LoadLevelByIndex(GameConfig.Ins.GameplayConfig.Level);
            }
            else
            {
                targetScoreData = LoadLevelByIndex(BBSaveData.Ins.Level);
            }
        }
        else if (GameplayManager.Ins.PlayMode == PlayMode.Classic)
        {
            BoardLevelLoader.CurrentLevelData = null;
        }
        GameplayManager.Ins.RestartGame();
        ScoreManager.Ins.SetTarget(targetScoreData);
        // Call after set target to get goalItem
        BoardLevelLoader.LoadBoardBaseOnLevel();
        BoardProgressService.LoadBoardSaved();

        if (!GameplayManager.Ins._levelEditor)
        {
            _shapeContainer.transform.localScale = Vector3.zero;
            PlayAnimOfAdventure();


            BoardSaveData.Ins.HasProgress = true;
            BoardSaveData.Ins.dirty = true;
        }
        else
        {
            EffectManager.Ins.PlayAnimStartTarget(BoardLevelLoader.CurrentLevelData != null ? BoardLevelLoader.CurrentLevelData.target : null);
        }
    }
    public TargetData LoadLevelByIndex(int level)
    {
        level = level % GameConfig.Ins.GameplayConfig.LevelToLoop;
        level = level <= 0 ? GameConfig.Ins.GameplayConfig.LevelToLoop : level;
        BoardLevelLoader.CurrentLevelData = LevelJsonSystem.LoadLevelJson(level);

        if (BoardLevelLoader.CurrentLevelData == null)
        {
            Debug.LogError($"❌ Cannot load Level_{level}.json");
            return null;
        }

        return BoardLevelLoader.CurrentLevelData.target;
    }
    protected void RestartGame(TargetData targetScoreData)
    {
        _instance = this;
        _noSpaceLeft.gameObject.SetActive(false);
        BoardScoreService.ClearList();
        BoardOutlineRenderer.HideAllOutlines();
        // Reset Board
        for (int i = 0; i < BoardContext.RowsCount; i++)
        {
            for (int j = 0; j < BoardContext.ColumnsCount; j++)
            {
                BoardContext.Board[i, j].ResetSquare();
            }
        }
        BoardLevelLoader.LoadBoardBaseOnLevel();
        GameplayManager.Ins.RestartGame();
        ScoreManager.Ins.SetTarget(targetScoreData);

        // Call after set target to get goalItem
        BoardProgressService.LoadBoardSaved();
        _shapeContainer.transform.localScale = Vector3.zero;

        PlayAnimOfAdventure();


        BoardSaveData.Ins.HasProgress = true;
        BoardSaveData.Ins.dirty = true;

    }
    private void PlayAnimOfAdventure()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            BBManager.Ins.InMenuToGameplay = false;
            GameplayManager.Ins.StartCoroutine(PlayIntroTileWave());
            EffectManager.Ins.PlayAnimStartTarget(BoardLevelLoader.CurrentLevelData != null ? BoardLevelLoader.CurrentLevelData.target : null);
        });
    }
    protected void OnClickReplay()
    {
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Adventure:
                BoardSaveData.Ins.ClearAdventure();

                TrackingHandler.OnAdventureEnd(BBSaveData.Ins.BBStartLevelCount, ScoreManager.Ins.TotalScore,
                                            GameplayManager.Ins.PlayTime, BBSaveData.Ins.BBContinueLevelCount, false);
                BBSaveData.Ins.BBContinueLevelCount = 0;
                BBSaveData.Ins.dirty = true;
                break;
            case PlayMode.Classic:
                BoardSaveData.Ins.ClearClassic();

                TrackingHandler.OnClassicEnd(BBSaveData.Ins.BBClassicPlayCount, ScoreManager.Ins.TotalScore,
                                             GameplayManager.Ins.PlayTime, BBSaveData.Ins.BBClassicContinue);
                BBSaveData.Ins.BBClassicContinue = 0;
                BBSaveData.Ins.dirty = true;
                break;
        }
        // Look like continue to don't move milestone in BBCanvasTop
        BBManager.Ins.ContinueProgress = true;

        RestartLevel();
    }

    public void RestartLevel()
    {
        TargetData targetScoreData = null;
        if (GameplayManager.Ins.PlayMode == PlayMode.Adventure)
        {
            if (BBManager.EnableCheat)
            {
                targetScoreData = LoadLevelByIndex(GameConfig.Ins.GameplayConfig.Level);
            }
            else
            {
                targetScoreData = LoadLevelByIndex(BBSaveData.Ins.Level);
            }
        }
        else if (GameplayManager.Ins.PlayMode == PlayMode.Classic)
        {
            BoardLevelLoader.CurrentLevelData = null;
        }

        RestartGame(targetScoreData);
    }

    public void NextLevel()
    {

        TargetData targetScoreData = null;

        if (BBManager.EnableCheat)
            targetScoreData = LoadLevelByIndex(GameConfig.Ins.GameplayConfig.Level);
        else
            targetScoreData = LoadLevelByIndex(BBSaveData.Ins.Level);

        GameplayManager.Ins.CanWatchAds = true;
        if (targetScoreData == null)
        {
            if (BBManager.EnableCheat)
            {
                GameConfig.Ins.GameplayConfig.Level--;
                targetScoreData = LoadLevelByIndex(GameConfig.Ins.GameplayConfig.Level);
            }
            else
            {
                BBSaveData.Ins.Level--;
                BBSaveData.Ins.dirty = true;
                targetScoreData = LoadLevelByIndex(BBSaveData.Ins.Level);
            }
        }

        RestartGame(targetScoreData);
    }
    #endregion

    #region PlayTileWave
    protected IEnumerator PlayIntroTileWave()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);
        _shapeContainer.transform.localScale = Vector3.zero;
        var sprites = GameConfig.Ins.TileColorConfig.ColorSprites;

        float columnDelay = GameConfig.Ins.EffectConfig.IntroColumnDelay; // delay giữa các tile trong 1 hàng
        float rowDelay = GameConfig.Ins.EffectConfig.IntroRowDelay;    // delay giữa các hàng (nhỏ để chồng lên)

        if (!gameObject.activeSelf) yield break;
        for (int row = 0; row < BoardContext.RowsCount; row++)
        {
            int currentRow = row;
            if (!gameObject.activeSelf) yield break;
            StartCoroutine(PlayRowIntro(currentRow, sprites, columnDelay));

            yield return WaitTimeCache.Get(rowDelay);
        }
    }

    protected IEnumerator PlayRowIntro(int row, List<TileColor> sprites, float columnDelay)
    {
        for (int col = 0; col < BoardContext.ColumnsCount; col++)
        {
            Tile tile = BoardContext.Board[row, col];
            tile.SetIntro(sprites[UnityEngine.Random.Range(0, sprites.Count)].Tile);

            yield return WaitTimeCache.Get(columnDelay);
        }
        yield return WaitTimeCache.Get(0.8f);
        _shapeContainer.transform.DOScale(1f, 0.4f);
    }

    protected IEnumerator PlayEndTileWave()
    {
        var sprites = GameConfig.Ins.TileColorConfig.ColorSprites;

        float columnDelay = GameConfig.Ins.EffectConfig.EndColumnDelay; // delay giữa các tile trong 1 hàng
        float rowDelay = GameConfig.Ins.EffectConfig.EndRowDelay;    // delay giữa các hàng (nhỏ để chồng lên)

        for (int row = 0; row < BoardContext.RowsCount; row++)
        {
            int currentRow = row;
            StartCoroutine(PlayRowEnd(currentRow, sprites, columnDelay));

            yield return WaitTimeCache.Get(rowDelay);
        }
    }

    protected IEnumerator PlayRowEnd(int row, List<TileColor> sprites, float columnDelay)
    {
        for (int col = 0; col < BoardContext.ColumnsCount; col++)
        {
            Tile tile = BoardContext.Board[row, col];
            tile.SetLose(sprites[UnityEngine.Random.Range(0, sprites.Count)].Tile);

            yield return WaitTimeCache.Get(columnDelay);
        }
    }

    #endregion

    #region OnPlaceOnGrid
    protected void OnPlaceOnGrid()
    {
        if (GameplayManager.Ins.DraggingShape == null)
        {
            Debug.Log("Null??");
            return;
        }
        var listTile = GameplayManager.Ins.DraggingShape.CurrentShape;
        if (BoardPlacementService.CanPlace(listTile))
        {
            foreach (ShapeSquare tile in listTile)
            {
                int indexRow = BoardPlacementService.GetIndex(tile, "Row");
                int indexColumn = BoardPlacementService.GetIndex(tile, "Column");
                if (indexRow >= 0 && indexRow < BoardContext.RowsCount && indexColumn >= 0 && indexColumn < BoardContext.ColumnsCount)
                {
                    if (tile.GoalItemType == GoalItemType.None)
                    {
                        if (tile.HasMoney)
                        {
                            BoardContext.Board[indexRow, indexColumn].ActivateSquareMoney();
                        }
                        else
                        {
                            BoardContext.Board[indexRow, indexColumn].ActivateSquare(GameplayManager.Ins.DraggingShape.CurrentTileColor, false);
                        }
                    }
                    else
                    {
                        BoardContext.Board[indexRow, indexColumn].ActivateSquareGoalItem(tile.GoalItemType);
                    }
                    // EffectManager.Ins.RequireLikeEffect(GameplayManager.Ins.DraggingShape.CurrentTileColor,  Board[indexRow, indexColumn].transform.position);
                }
            }

            BoardScoreService.rowsCanScore = BoardScoreService.GetScoredList(BoardContext.Rows);
            BoardScoreService.columnsCanScore = BoardScoreService.GetScoredList(BoardContext.Columns);
            if (BoardScoreService.rowsCanScore.Count > 0 || BoardScoreService.columnsCanScore.Count > 0)
            {
                BoardScoreService.allCanScore = BoardScoreService.GetAllScoredList();
            }
            // Invoke Score
            GameEvents.OnPlaceOnGrid?.Invoke();
            // Play Effect Score And ResetTile
            BoardScoreService.TryPlayScoreEffect();


            // Return Dragging to null and Check to Spawn Shape
            GameplayManager.Ins.OnPlaceShapeSuccess();

            // wait to spawn shape
            // DOVirtual.DelayedCall(0.03f, () =>
            // {
            //     CheckLoseGame();
            // });
        }
        else
        {
            GameplayManager.Ins.DraggingShape.BackToPos();
        }
    }
    #endregion



    #region CheckEndGame
    public void CheckLoseGame()
    {

        bool isEndGame = true;
        foreach (Shape shape in _shapes)
        {
            if (shape.CurrentShape == null || shape.CurrentShape.Count <= 0 || shape.CurrentShapeData == null)
                continue;
            if (BoardPlacementService.CanPlaceByShapeData(shape.CurrentShapeData))
            {
                isEndGame = false;
                // Debug.Log("Continue Game");
                break;
            }
        }
        if (isEndGame)
        {
            GameplayManager.Ins.OnLoseGame(RestartLevel, NextLevel);
            NoSpaceLeft();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                StartCoroutine(PlayEndTileWave());
            });
        }
    }
    protected void NoSpaceLeft()
    {
        _noSpaceLeft.gameObject.SetActive(true);
        _noSpaceLeft.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_noSpaceLeft.DOScale(1f, 0.2f));
        sequence.AppendInterval(1.5f);
        sequence.Append(_noSpaceLeft.DOScale(0f, 0.2f));
        sequence.OnComplete(() =>
        {
            _noSpaceLeft.gameObject.SetActive(false);
        });
    }
    #endregion
    #region PlayTest
    public void SetPlayTest(TileEditor[,] board, TargetData targetData)
    {

        DOVirtual.DelayedCall(0.1f, () =>
        {
            _instance = this;
            BoardScoreService.ClearList();
            BoardOutlineRenderer.HideAllOutlines();
            // Reset Board
            for (int i = 0; i < BoardContext.RowsCount; i++)
            {
                for (int j = 0; j < BoardContext.ColumnsCount; j++)
                {
                    if (board[i, j].SquareOccupied == false)
                    {
                        BoardContext.Board[i, j].ResetSquare();
                    }
                    else
                    {
                        if (board[i, j].GoalItemType == GoalItemType.None)
                        {
                            BoardContext.Board[i, j].ActivateSquare(GameConfig.Ins.TileColorConfig.ColorSprites.Find(e => e.Tile == board[i, j].GetSpriteActive()));
                        }
                        else
                        {
                            BoardContext.Board[i, j].ActivateSquareGoalItem(board[i, j].GoalItemType);
                        }
                    }
                }
            }

            // Call after set target to get goalItem
            Debug.Log("Respawn");
            GameplayManager.Ins.RestartGame();
            ScoreManager.Ins.SetTarget(targetData);
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
        });

    }
    #endregion

}

[System.Serializable]
public class BoardContext
{
    public Tile[,] Board { get; set; }

    public int RowsCount { get; set; }
    public int ColumnsCount { get; set; }



    public float[] LineColumnPositions { get; set; }
    public float[] LineRowPositions { get; set; }

    // public Dictionary<int, List<Tile>> Rows { get; set; }
    // public Dictionary<int, List<Tile>> Columns { get; set; }
    public List<Tile>[] Rows { get; set; }
    public List<Tile>[] Columns { get; set; }

    public Dictionary<int, Image> RowOutlines = new();
    public Dictionary<int, Image> ColumnOutlines = new();

    public BoardContext()
    {
    }
    public BoardContext(int row, int column)
    {
        RowsCount = row;
        ColumnsCount = column;
        Board = new Tile[row, column];
        LineColumnPositions = new float[column + 1];
        LineRowPositions = new float[row + 1];
        Rows = new List<Tile>[row];
        Columns = new List<Tile>[column];
        RowOutlines = new();
        ColumnOutlines = new();
    }
}
internal class TileAddress
{
    public TileAddress(int rowIndex, int columnIndex)
    {
        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;
    }
    public int rowIndex;
    public int columnIndex;
}
