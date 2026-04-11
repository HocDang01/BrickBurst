using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonsLevelEditor : MonoBehaviour
{
    [Header("Ref Manager")]
    [SerializeField] private LevelEditorManager _levelEditorManager;
    [SerializeField] private TileEditorManager _tileEditorManager;

    [Header("Ref UI")]
    [SerializeField] private TMP_InputField _levelInputField;
    [SerializeField] private TMP_InputField _targetScoreInput;
    [SerializeField] private Button _genBtn;
    [SerializeField] private Button _loadBtn;
    [SerializeField] private Button _resetBtn;
    [SerializeField] private Button _newBoardBtn;
    [SerializeField] private Button _playTestBtn;
    [SerializeField] private Button _backToEditorBtn;

    [Header("Target Type")]
    [SerializeField] private TextMeshProUGUI _targetTypeText;
    [SerializeField] private GameObject _listTargetTypeObject;
    [SerializeField] private Button _targetTypeBtn;
    [SerializeField] private Button _targetScoreTypeBtn;
    [SerializeField] private Button _targetGoalItemTypeBtn;
    [SerializeField] private Button _targetBothTypeBtn;
    [SerializeField] private GameObject _goalItemTargetObject;
    [SerializeField] private GameObject _scoreObject;
    [SerializeField] private GameObject _goalItemTileObject;
    [SerializeField] private List<TargetItemLvEditor> _targetItemLvEditors;

    [Header("Ref Button GoalItem")]
    [SerializeField] private Button _gemBlueBtn;
    [SerializeField] private Button _gemGreenBtn;
    [SerializeField] private Button _gemPinkBtn;
    [SerializeField] private Button _gemRedBtn;
    [SerializeField] private Button _gemYellowBtn;

    [Header("Ref Play Test")]
    [SerializeField] private GameObject _playView;
    [SerializeField] private BoardManager _tileManager;
    private int _level;
    private int _targetScore;
    private TargetType _targetType;
    private bool _isOpenTargetTypeObject;


    void Start()
    {
        _genBtn.onClick.AddListener(GenerateLevel);
        _loadBtn.onClick.AddListener(LoadLevel);
        _resetBtn.onClick.AddListener(ResetBoard);
        _newBoardBtn.onClick.AddListener(NewBoard);
        _playTestBtn.onClick.AddListener(PlayTest);
        _backToEditorBtn.onClick.AddListener(BackToEditor);

        _targetTypeBtn.onClick.AddListener(OnClickTargetType);
        _targetScoreTypeBtn.onClick.AddListener(OnClickTargetScore);
        _targetGoalItemTypeBtn.onClick.AddListener(OnClickTargetGoalItem);
        _targetBothTypeBtn.onClick.AddListener(OnClickTargetBoth);

        _gemBlueBtn.onClick.AddListener(() => { OnClickGoalItemShape(GoalItemType.GemBlue); });
        _gemGreenBtn.onClick.AddListener(() => { OnClickGoalItemShape(GoalItemType.GemGreen); });
        _gemPinkBtn.onClick.AddListener(() => { OnClickGoalItemShape(GoalItemType.GemPink); });
        _gemRedBtn.onClick.AddListener(() => { OnClickGoalItemShape(GoalItemType.GemRed); });
        _gemYellowBtn.onClick.AddListener(() => { OnClickGoalItemShape(GoalItemType.GemYellow); });

        InitialScene();

        DOVirtual.DelayedCall(0.001f, () =>
        {
            _playView.gameObject.SetActive(false);
        });
    }
    private void InitialScene()
    {
        _level = 0;
        _targetScore = 0;
        _targetScoreInput.text = _targetScore.ToString();
        _levelInputField.text = _level.ToString();

        OnClickTargetScore();
    }
    private void GenerateLevel()
    {
#if UNITY_EDITOR
        if (!int.TryParse(_levelInputField.text, out _level))
        {
            Debug.LogWarning("Invalid level number");
            return;
        }

        if (!int.TryParse(_targetScoreInput.text, out _targetScore))
        {
            Debug.LogWarning("Invalid score number");
            return;
        }
        string jsonPath = GameConfig.Ins.LevelEditorConfig.FolderLevelPath + "/" + GameConfig.Ins.LevelEditorConfig.LevelName + _level + ".json";
        // string jsonPath = $"Assets/_Game/Resources/Levels/Level_{_level}.json";

        // 🔴 Nếu tồn tại → confirm overwrite
        if (File.Exists(jsonPath))
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Overwrite Level?",
                $"{GameConfig.Ins.LevelEditorConfig.LevelName}{_level}.json already exists.\nDo you want to overwrite it?",
                "Overwrite",
                "Cancel"
            );

            if (!confirm)
            {
                Debug.Log("Generate cancelled");
                return;
            }
        }

        // 👉 TODO: lấy target goalItems (nếu có UI thì map ở đây)
        Dictionary<GoalItemType, int> goalTargets = new();
        foreach (var goalItem in _targetItemLvEditors)
        {
            if (goalItem && goalItem.GoalItemType != GoalItemType.None)
            {
                goalTargets.Add(goalItem.GoalItemType, goalItem.GetCount());
            }
        }


        LevelJsonSystem.SaveLevelToJson(
            _level,
            _targetType,
            _targetScore,
            goalTargets,
            _tileEditorManager.Board
        );

        Debug.Log($"✅ {GameConfig.Ins.LevelEditorConfig.LevelName}{_level} saved as JSON");
#else
    Debug.LogError("Generate Level only works in Editor");
#endif
    }

    private void ResetBoard()
    {
        _tileEditorManager.RestartLevel();
    }

    private void NewBoard()
    {
        _tileEditorManager.LevelEditorSO = null;
        _tileEditorManager.RestartLevel();
    }

    #region LoadLevel
    private void LoadLevel()
    {
#if UNITY_EDITOR
        string folderPath = GameConfig.Ins.LevelEditorConfig.FolderLevelPath;

        // Mở file picker
        string path = EditorUtility.OpenFilePanel(
            "Select Level JSON",
            folderPath,
            "json"
        );

        // Cancel
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Load level cancelled");
            return;
        }

        // Đảm bảo nằm trong Resources
        if (!path.Contains("/Resources/"))
        {
            Debug.LogError("❌ Level file must be inside Resources folder!");
            return;
        }

        // Convert absolute path → Resources path
        // VD: Assets/_Game/Resources/Levels/Level_5.json
        // => Levels/Level_5
        string resourcesPath = path
            .Substring(path.IndexOf("Resources/") + "Resources/".Length)
            .Replace(".json", "");

        TextAsset jsonFile = Resources.Load<TextAsset>(resourcesPath);

        if (jsonFile == null)
        {
            Debug.LogError("❌ Failed to load level JSON");
            return;
        }

        LevelJsonData data = JsonUtility.FromJson<LevelJsonData>(jsonFile.text);

        if (data == null)
        {
            Debug.LogError("❌ Invalid level JSON format");
            return;
        }

        // Set UI
        _level = data.level;
        _levelInputField.text = _level.ToString();
        _targetScoreInput.text = data.target.score.ToString();

        _targetType = (TargetType)data.target.TargetType;

        _targetTypeText.text = _targetType.ToString();
        switch (_targetType)
        {
            case TargetType.Score:
                OnClickTargetScore();
                break;
            case TargetType.GoalIem:
                OnClickTargetGoalItem();
                foreach (var goalItem in data.target.goalItems)
                {
                    var goalItemType = (GoalItemType)goalItem.goalItem;
                    TargetItemLvEditor a = _targetItemLvEditors.Find(e => e.GoalItemType == goalItemType);
                    if (a) a.SetCount(goalItem.count);
                }
                break;
            case TargetType.Both:
                OnClickTargetBoth();
                break;
        }

        // Reset & Apply
        _tileEditorManager.RestartLevel();
        _tileEditorManager.ApplyLevelFromJson(data);

        Debug.Log($"✅ Loaded level JSON: {resourcesPath}");
#else
    Debug.LogError("Load Level only works in Editor!");
#endif
    }
    #endregion

    #region TargetType
    // Click to select
    private void OnClickTargetType()
    {
        _isOpenTargetTypeObject = !_isOpenTargetTypeObject;
        _listTargetTypeObject.SetActive(_isOpenTargetTypeObject);
    }
    private void OnClickTargetScore()
    {
        _isOpenTargetTypeObject = false;
        _targetType = TargetType.Score;
        _targetTypeText.text = _targetType.ToString();
        _listTargetTypeObject.SetActive(_isOpenTargetTypeObject);

        _scoreObject.SetActive(true);
        _goalItemTargetObject.SetActive(false);
        _goalItemTileObject.SetActive(false);

    }
    private void OnClickTargetGoalItem()
    {
        _isOpenTargetTypeObject = false;
        _targetType = TargetType.GoalIem;
        _targetTypeText.text = _targetType.ToString();
        _listTargetTypeObject.SetActive(_isOpenTargetTypeObject);

        _scoreObject.SetActive(false);
        _goalItemTargetObject.SetActive(true);
        _goalItemTileObject.SetActive(true);
    }
    private void OnClickTargetBoth()
    {
        _isOpenTargetTypeObject = false;
        _targetType = TargetType.Both;
        _targetTypeText.text = _targetType.ToString();
        _listTargetTypeObject.SetActive(_isOpenTargetTypeObject);

        _scoreObject.SetActive(true);
        _goalItemTargetObject.SetActive(true);
        _goalItemTileObject.SetActive(true);
    }
    #endregion

    #region GoalItemShape
    private void OnClickGoalItemShape(GoalItemType goalItemType)
    {
        LevelEditorManager.OnClickGoalItem?.Invoke(goalItemType);
    }
    #endregion

    #region PlayTest
    private void PlayTest()
    {
        GameplayManager.Ins.PlayMode = PlayMode.Adventure;
        _playView.gameObject.SetActive(true);
        if (!int.TryParse(_targetScoreInput.text, out _targetScore))
        {
            Debug.LogWarning("Invalid score number");
            return;
        }
        Dictionary<GoalItemType, int> goalTargets = new();
        foreach (var goalItem in _targetItemLvEditors)
        {
            if (goalItem && goalItem.GoalItemType != GoalItemType.None)
            {
                goalTargets.Add(goalItem.GoalItemType, goalItem.GetCount());
            }
        }
        TargetData target = new TargetData
        {
            TargetType = (int)_targetType,
            score = _targetScore,
            goalItems = Utility.GoalItemDictToList(goalTargets),
        };
        _tileManager.SetPlayTest(_tileEditorManager.Board, target);
    }
    private void BackToEditor()
    {
        _playView.gameObject.SetActive(false);
    }
    #endregion

}

