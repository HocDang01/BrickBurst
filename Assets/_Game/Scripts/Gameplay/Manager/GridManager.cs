// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor.Analytics;
// using UnityEngine;
// using UnityEngine.UI;

// public class GridManager : MonoBehaviour
// {
//     [Header("Setup Board")]
//     [SerializeField] private int _columnsCount = 8;
//     [SerializeField] private int _rowsCount = 8;
//     [SerializeField] private float _squaresGap = 0.1f;
//     [SerializeField] private Tile _gridSquare;
//     [SerializeField] private Vector2 _startPosition = new Vector2(0.0f, 0.0f);
//     [SerializeField] private float _squareScale = 0.5f;
//     [SerializeField] private float _everySquareOffset = 0.05f;

//     [Header("Ref Shapes")]
//     [SerializeField] private List<Shape> _shapes;

//     [Header("Score")]
//     public List<Tile> rowsCanScore = new();
//     public List<Tile> columnsCanScore = new();
//     public List<Tile> allCanScore = new();

//     private Dictionary<int, List<Tile>> _rows = new();
//     private Dictionary<int, List<Tile>> _columns = new();


//     [Header("Dev")]
//     [SerializeField] private List<RectTransform> _lineColumn;
//     [SerializeField] private List<RectTransform> _lineRow;
//     [SerializeField] private float[] _lineColumnPositions;
//     [SerializeField] private float[] _lineRowPosition;
//     [SerializeField] private Button _restartBtn;
//     private List<Tile> _gridSquares = new List<Tile>();
//     public Tile[,] Board;

//     private bool _canPlace;
//     void Awake()
//     {
//         Board = new Tile[_rowsCount, _columnsCount];
//         _lineColumnPositions = new float[_columnsCount + 1];
//         _lineRowPosition = new float[_rowsCount + 1];
//         _lineColumn = new();
//         _lineRow = new();
//         _rows = new();
//         _columns = new();
//         GameplayManager.Ins.GridManager = this;
//         _restartBtn.onClick.AddListener(RestartGame);
//     }
//     void Start()
//     {
//         CreateGrid();
//         SetPosForRowAndColumn();
//         _restartBtn.gameObject.SetActive(false);
//     }
//     void OnEnable()
//     {
//         GameEvents.CheckIfShapeCanPlaced += OnPlaceOnGrid;
//     }
//     void OnDisable()
//     {
//         GameEvents.CheckIfShapeCanPlaced -= OnPlaceOnGrid;
//     }
//     void Update()
//     {
//         CheckHover();
//         CheckVirtual();
//     }
//     #region Create board
//     private void RestartGame()
//     {
//         ClearList();
//         foreach (var tile in Board)
//         {
//             tile.ResetSquare();
//         }
//         foreach (Shape shape in _shapes)
//         {
//             shape.ResetShape();
//         }
//         _restartBtn.gameObject.SetActive(false);
//         GameplayManager.Ins.CurrentShapeCount = 0;
//         GameplayManager.Ins.DraggingShape = null;
//         // GameEvents.OnNeedCreateShapes();
//     }

//     private void CreateGrid()
//     {
//         SpawnGridSquare();
//         SetupRowColumn();
//         SetGridSquaresPositions();
//     }

//     private void SpawnGridSquare()
//     {
//         // 0,1,2,3,4
//         // 5,6,7,8,9

//         int square_index = 0;
//         for (var row = _rowsCount - 1; row >= 0; row--)
//         {
//             for (var column = 0; column < _columnsCount; column++)
//             {
//                 var grid = Instantiate(_gridSquare);
//                 _gridSquares.Add(grid);
//                 Board[row, column] = grid;
//                 grid.transform.SetParent(this.transform);
//                 grid.gameObject.name = "Tile_" + row + "_" + column;
//                 grid.transform.localScale = new Vector3(_squareScale, _squareScale, _squareScale);
//                 grid.GetComponent<Tile>().SetImage(square_index % 2 == 0);
//                 square_index++;

//                 if (column == 0)
//                 {
//                     _lineRow.Add(grid.GetComponent<RectTransform>());
//                 }
//                 if (row == 0)
//                 {
//                     _lineColumn.Add(grid.GetComponent<RectTransform>());
//                 }
//             }
//         }
//         _lineRow.Reverse();
//     }
//     private void SetupRowColumn()
//     {
//         for (int i = 0; i < _rowsCount; i++)
//         {
//             List<Tile> rows = new();
//             for (int j = 0; j < _columnsCount; j++)
//             {
//                 rows.Add(Board[i, j]);
//             }
//             _rows.Add(i, rows);
//         }

//         for (int j = 0; j < _columnsCount; j++)
//         {
//             List<Tile> columns = new();
//             for (int i = 0; i < _rowsCount; i++)
//             {
//                 columns.Add(Board[i, j]);
//             }
//             _columns.Add(j, columns);
//         }
//     }


//     private void SetGridSquaresPositions()
//     {
//         int column_number = 0;
//         int row_number = 0;
//         Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
//         bool row_moved = false;
//         var square_rect = _gridSquares[0].GetComponent<RectTransform>();

//         Vector2 offset = new Vector2(0.0f, 0.0f);
//         offset.x = square_rect.rect.width * square_rect.transform.localScale.x + _everySquareOffset;
//         offset.y = square_rect.rect.height * square_rect.transform.localScale.y + _everySquareOffset;

//         foreach (Tile square in _gridSquares)
//         {
//             if (column_number + 1 > _columnsCount)
//             {
//                 square_gap_number.x = 0;
//                 // go to next column
//                 column_number = 0;
//                 row_number++;
//                 row_moved = true;
//             }

//             var pos_x_offset = offset.x * column_number + (square_gap_number.x * _squaresGap);
//             var pos_y_offset = offset.y * row_number + (square_gap_number.y * _squaresGap);

//             if (column_number > 0 && column_number % 3 == 0)
//             {
//                 square_gap_number.x++;
//                 pos_x_offset += _squaresGap;
//             }

//             if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
//             {
//                 row_moved = true;
//                 square_gap_number.y++;
//                 pos_y_offset += _squaresGap;
//             }

//             square.GetComponent<RectTransform>().anchoredPosition = new Vector2(_startPosition.x + pos_x_offset,
//                                                                             _startPosition.y + pos_y_offset);
//             square.GetComponent<RectTransform>().localPosition = new Vector3(_startPosition.x + pos_x_offset,
//                                                                             _startPosition.y + pos_y_offset, 0.0f);
//             column_number++;
//         }

//     }

//     // private void SetPosForRowAndColumn()
//     // {
//     //     if (_lineColumn == null || _lineColumn.Count <= 0 || _lineRow == null || _lineRow.Count <= 0) return;
//     //     _lineColumnPositions[0] = _lineColumn[0].transform.position.x - _lineColumn[0].rect.width * _squareScale * 0.5f;
//     //     _lineRowPosition[0] = _lineRow[0].transform.position.y - _lineRow[0].rect.height * _lineRow[0].transform.localScale.y * 0.5f;
//     //     for (int i = 1; i <= _lineColumn.Count; i++)
//     //     {
//     //         _lineColumnPositions[i] = _lineColumnPositions[i-1] + _lineColumn[i-1].rect.width * _squareScale ;
//     //     }
//     //     for (int i = 1; i <= _lineRow.Count; i++)
//     //     {
//     //         _lineRowPosition[i] = _lineRowPosition[i-1] + _lineRow[i-1].rect.height * _squareScale;
//     //     }
//     // }
//     private void SetPosForRowAndColumn()
//     {
//         if (_lineColumn == null || _lineColumn.Count <= 0 ||
//             _lineRow == null || _lineRow.Count <= 0) return;

//         // ========== COLUMN ==========
//         Vector3 colWorld = _lineColumn[0].position;
//         Vector3 colLocal = transform.InverseTransformPoint(colWorld);

//         _lineColumnPositions[0] =
//             colLocal.x - (_lineColumn[0].rect.width * _squareScale * 0.5f);

//         for (int i = 1; i <= _lineColumn.Count; i++)
//         {
//             _lineColumnPositions[i] =
//                 _lineColumnPositions[i - 1] +
//                 (_lineColumn[i - 1].rect.width * _squareScale);
//         }

//         // ========== ROW ==========
//         Vector3 rowWorld = _lineRow[0].position;
//         Vector3 rowLocal = transform.InverseTransformPoint(rowWorld);

//         _lineRowPosition[0] =
//             rowLocal.y + (_lineRow[0].rect.height * _squareScale * 0.5f);

//         for (int i = 1; i <= _lineRow.Count; i++)
//         {
//             _lineRowPosition[i] =
//                 _lineRowPosition[i - 1] -
//                 (_lineRow[i - 1].rect.height * _squareScale);
//         }
//     }

//     #endregion

//     #region CheckCanPlace
//     private List<TileAddress> GetTilesAddressDic(int[,] tilesMatrix)
//     {
//         List<TileAddress> temp = new();
//         for (int i = 0; i < tilesMatrix.GetLength(0); i++)
//         {
//             for (int j = 0; j < tilesMatrix.GetLength(1); j++)
//             {
//                 if (tilesMatrix[i, j] == 1)
//                 {
//                     temp.Add(new TileAddress(i, j));
//                 }
//             }
//         }
//         return temp;
//     }
//     public int GetIndex(ShapeSquare tile, string option)
//     {
//         RectTransform tileRect = tile.GetComponent<RectTransform>();
//         if (tileRect == null) return -1;
//         Vector3 tileWorld = tileRect.position;
//         Vector3 tileLocal = transform.InverseTransformPoint(tileWorld);
//         switch (option)
//         {
//             case "Column":
//                 float tileX = tileLocal.x;

//                 if (tileX < _lineColumnPositions[0] ||
//                     tileX > _lineColumnPositions[^1]) return -1;

//                 for (int i = 0; i < _lineColumnPositions.Length - 1; i++)
//                 {
//                     if (tileX >= _lineColumnPositions[i] &&
//                         tileX < _lineColumnPositions[i + 1])
//                     {
//                         return i;
//                     }
//                 }
//                 return -1;

//             case "Row":
//                 float tileY = tileLocal.y;

//                 if (tileY > _lineRowPosition[0] ||
//                     tileY < _lineRowPosition[^1]) return -1;

//                 for (int j = 0; j < _lineRowPosition.Length - 1; j++)
//                 {
//                     if (tileY <= _lineRowPosition[j] &&
//                         tileY > _lineRowPosition[j + 1])
//                     {
//                         return j;
//                     }
//                 }
//                 return -1;

//             default:
//                 Debug.Log("Wrong type option!");
//                 return -1;
//         }
//     }

//     // public bool CanPlaceInGrid(int[,] tilesMatrix)
//     // {
//     //     Debug.Log("Check CanPlace");
//     //     if (tilesMatrix == null) return false;
//     //     var listAddress = GetTilesAddressDic(tilesMatrix);
//     //     int emptyRowLength = Board.GetLength(0);
//     //     int emptyColumnLength = Board.GetLength(1);
//     //     for (int i = 0; i < emptyRowLength; i++)
//     //     {
//     //         for (int j = 0; j < emptyColumnLength; j++)
//     //         {
//     //             int equalCount = 0;
//     //             for (int k = 0; k < listAddress.Count; k++)
//     //             {
//     //                 var tileAddress = listAddress[k];
//     //                 int visitRowIndex = i + tileAddress.rowIndex;
//     //                 int visitColumnIndex = j + tileAddress.columnIndex;
//     //                 // if (visitRowIndex < 0 || visitRowIndex >= emptyRowLength || visitColumnIndex < 0 || visitColumnIndex >= emptyColumnLength)
//     //                 // {
//     //                 //     if (tilesMatrix[tileAddress.rowIndex, tileAddress.columnIndex] == 1)
//     //                 //         break;
//     //                 // }
//     //                 if (visitRowIndex < emptyRowLength && visitColumnIndex < emptyColumnLength)
//     //                 {
//     //                     int cell = (!Board[visitRowIndex, visitColumnIndex].SquareOccupied) ? 1 : 0;
//     //                     int tile = tilesMatrix[tileAddress.rowIndex, tileAddress.columnIndex];
//     //                     if (cell == tile)
//     //                     {
//     //                         equalCount++;
//     //                     }
//     //                 }
//     //             }
//     //             if (equalCount == listAddress.Count) return true;
//     //         }
//     //     }
//     //     return false;
//     // }
//     public bool CanPlaceInGrid(int[,] tilesMatrix)
//     {
//         Debug.Log("Check CanPlace");
//         if (tilesMatrix == null) return false;

//         var listAddress = GetTilesAddressDic(tilesMatrix);

//         int boardRows = Board.GetLength(0);
//         int boardCols = Board.GetLength(1);

//         for (int baseRow = 0; baseRow < boardRows; baseRow++)
//         {
//             for (int baseCol = 0; baseCol < boardCols; baseCol++)
//             {
//                 bool canPlaceHere = true;

//                 for (int k = 0; k < listAddress.Count; k++)
//                 {
//                     int r = baseRow + listAddress[k].rowIndex;
//                     int c = baseCol + listAddress[k].columnIndex;

//                     // ❌ VƯỢT BIÊN → FAIL NGAY
//                     if (r < 0 || r >= boardRows || c < 0 || c >= boardCols)
//                     {
//                         canPlaceHere = false;
//                         break;
//                     }

//                     // ❌ ĐÈ Ô ĐÃ CHIẾM → FAIL
//                     if (Board[r, c].SquareOccupied)
//                     {
//                         canPlaceHere = false;
//                         break;
//                     }
//                 }

//                 // ✅ TÌM ĐƯỢC 1 VỊ TRÍ HỢP LỆ
//                 if (canPlaceHere)
//                     return true;
//             }
//         }

//         return false;
//     }

//     public bool CanPlaceByShapeData(ShapeData shapeData)
//     {
//         int[,] matrix = new int[shapeData.rows, shapeData.columns];

//         for (int r = 0; r < shapeData.rows; r++)
//         {
//             for (int c = 0; c < shapeData.columns; c++)
//             {
//                 matrix[r, c] = shapeData[r, c] ? 1 : 0;
//                 Debug.Log(r + "," + c + ":" + matrix[r, c]);
//             }
//         }
//         return CanPlaceInGrid(matrix);
//     }

//     public bool CanPlace(List<ShapeSquare> listTile)
//     {
//         if (listTile.Count == 0) return false;
//         int count = 0;
//         var listIndex = new List<TileAddress>();
//         foreach (var tile in listTile)
//         {
//             int indexRow = GetIndex(tile, "Row");
//             int indexColumn = GetIndex(tile, "Column");
//             int duplicateCount = 0;
//             foreach (var index in listIndex)
//             {
//                 if (index.rowIndex == indexRow && index.columnIndex == indexColumn) duplicateCount++;

//             }
//             if (duplicateCount == 0)
//             {
//                 listIndex.Add((new TileAddress(indexRow, indexColumn)));
//                 if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount) if (!Board[indexRow, indexColumn].SquareOccupied) count++;
//             }
//         }
//         return (count == listTile.Count);
//     }
//     #endregion

//     #region Update
//     public void CheckHover()
//     {
//         if (GameplayManager.Ins.DraggingShape == null || GameplayManager.Ins.DraggingShape.CurrentShape == null) return;
//         var listTile = GameplayManager.Ins.DraggingShape.CurrentShape;
//         _canPlace = CanPlace(listTile);
//         if (_canPlace)
//         {
//             var listNowShadow = new List<Tile>();
//             foreach (var tile in listTile)
//             {
//                 int indexRow = GetIndex(tile, "Row");
//                 int indexColumn = GetIndex(tile, "Column");
//                 if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount)
//                 {
//                     Board[indexRow, indexColumn].SetHover(true);
//                     listNowShadow.Add(Board[indexRow, indexColumn]);
//                 }
//             }
//             foreach (var cell in Board) if (!listNowShadow.Exists(x => x == cell)) cell.SetHover(false);
//         }
//         else foreach (var cell in Board) cell.SetHover(false);
//     }
//     public void CheckVirtual()
//     {
//         ClearList();
//         rowsCanScore = GetScoredList(_rows);
//         columnsCanScore = GetScoredList(_columns);
//         if (rowsCanScore.Count > 0 || columnsCanScore.Count > 0)
//         {
//             allCanScore = GetAllScoredList();
//         }
//     }
//     #endregion
//     #region OnPlaceOnGrid
//     public void OnPlaceOnGrid()
//     {
//         if (GameplayManager.Ins.DraggingShape == null)
//         {
//             Debug.Log("Null??");
//             return;
//         }
//         var listTile = GameplayManager.Ins.DraggingShape.CurrentShape;
//         if (_canPlace)
//         {
//             foreach (var tile in listTile)
//             {
//                 int indexRow = GetIndex(tile, "Row");
//                 int indexColumn = GetIndex(tile, "Column");
//                 if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount)
//                 {
//                     // Debug.Log("Hover true at: " + Board[indexRow, indexColumn].gameObject.name);
//                     Board[indexRow, indexColumn].ActivateSquare();
//                 }
//             }
//             Debug.Log("Complete Place");
//             GameplayManager.Ins.OnPlaceShapeSuccess();
//             CheckScore();
//             CheckEndGame();
//         }
//         else
//         {
//             GameplayManager.Ins.DraggingShape.BackToPos();
//         }
//     }
//     #endregion
//     #region CheckScore
//     private void CheckScore()
//     {
//         if (allCanScore == null || allCanScore.Count <= 0) return;
//         foreach (Tile tile in allCanScore)
//         {
//             tile.ResetSquare();
//         }
//     }
//     private List<Tile> GetScoredList(Dictionary<int, List<Tile>> dic)
//     {
//         var returnList = new List<Tile>();
//         for (int i = 0; i < dic.Count; i++)
//         {
//             dic.TryGetValue(i, out var temp);
//             int count = temp.Count(cell => cell.SquareOccupied || cell.IsPlacedVirtual);
//             if (count == _rowsCount) returnList.AddRange(temp);
//         }
//         return returnList;
//     }
//     private void ClearList()
//     {
//         rowsCanScore.Clear();
//         columnsCanScore.Clear();
//         allCanScore.Clear();
//     }
//     private List<Tile> GetAllScoredList()
//     {
//         var temp = new List<Tile>();
//         foreach (var row in rowsCanScore) if (!temp.Contains(row)) temp.Add(row);
//         foreach (var column in columnsCanScore) if (!temp.Contains(column)) temp.Add(column);
//         return temp;
//     }
//     #endregion
//     #region CheckEndGame
//     public void CheckEndGame()
//     {
//         bool isEndGame = true;
//         foreach (Shape shape in _shapes)
//         {
//             if (shape.CurrentShape == null || shape.CurrentShape.Count <= 0 || shape.CurrentShapeData == null)
//                 continue;
//             if (CanPlaceByShapeData(shape.CurrentShapeData))
//             {
//                 isEndGame = false;
//                 Debug.Log("Continue Game");
//                 break;
//             }
//         }
//         if (isEndGame)
//         {
//             _restartBtn.gameObject.SetActive(true);
//             Debug.Log("End Game");
//         }
//     }
//     #endregion

// }


