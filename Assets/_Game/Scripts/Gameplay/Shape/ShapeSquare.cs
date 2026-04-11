using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
    {
        [SerializeField] private Image _occupiedImage;
        private Sprite _sprite;
        public int RowIndex;
        public int ColumnIndex;
        public GoalItemType GoalItemType;
        public bool HasMoney;
        void Awake()
        {
            RowIndex = -1;
            ColumnIndex = -1;
            // GoalItemType = GoalItemType.None;
        }
        public void SetSprite(Sprite sprite)
        {
            _occupiedImage.sprite = sprite;
            _sprite = sprite;
        }
        public void SetGoalItem(GoalItemType goalItemType)
        {
            GoalItemType = goalItemType;
            if (goalItemType == GoalItemType.None) return;
            HasMoney = false;
            var a = GameConfig.Ins.TileColorConfig.TileGoalItems.Find(e => e.GoalItemType == goalItemType);
            if (a != null)
            {
                _occupiedImage.sprite = a.Tile;
                _sprite = a.Tile;
            }
        }
        public void SetMoney()
        {
            GoalItemType = GoalItemType.None;
            HasMoney = true;
            _sprite = GameConfig.Ins.TileColorConfig.MoneyTile;
            _occupiedImage.sprite = _sprite;
        }
        public void SetVirtual(RainBowColor rainBowColor)
        {
            _occupiedImage.sprite = rainBowColor.Tile;
        }
        public void SetVirtual(Sprite rainBowColor)
        {
            _occupiedImage.sprite = rainBowColor;
        }
        public void SetUnVirtual()
        {
            if (_sprite == null) return;
            _occupiedImage.sprite = _sprite;
        }
        public Sprite GetCurrentSprite() => _sprite;
    }