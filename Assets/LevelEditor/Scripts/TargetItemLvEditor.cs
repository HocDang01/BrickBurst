using TMPro;
using UnityEngine;
using UnityEngine.UI;
    public class TargetItemLvEditor : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_InputField _countInput;
        public GoalItemType GoalItemType;
        void Start()
        {
            var a = GameConfig.Ins.TileColorConfig.TileGoalItems.Find(e => e.GoalItemType == GoalItemType);
            if (a != null && a.Icon != null)
            {
                _icon.sprite = a.Icon;
            }
        }
        public void Init()
        {
            _countInput.text = "0";
        }
        public int GetCount()
        {
            int count = 0;
            if (!int.TryParse(_countInput.text, out count))
            {
                Debug.LogWarning($"Invalid {GoalItemType} number");
                return count;
            }
            return count;
        }
        public void SetCount(int count)
        {
            _countInput.text = count.ToString();
        }
    }
